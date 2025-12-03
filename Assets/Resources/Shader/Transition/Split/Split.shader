Shader "Transition/Split"
{
    Properties
    {
        [Header(Transition)]
        _Progress("Progress", Range(0, 1)) = 0.5
        [KeywordEnum(Vertical, Horizontal)] _Direction("Transition Direction", Float) = 0
        [Toggle] _ReverseStartDir("Reverse Start Direction", Float) = 0
        
        [Header(Visual Effects)]
        _Feathering("Feathering Width", Range(0.001, 0.5)) = 0.05
        
        [Header(Textures)]
        [NoScaleOffset]_FormTex("From Tex", 2D) = "white" {}
        [NoScaleOffset]_ToTex("To Tex", 2D) = "white" {}
        
        [Header(Split Settings)]
        _SplitCount("Split Count", Float) = 4
        _SplitEndOffset("Transition End Offset", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha 
            ZTest LEqual
            ZWrite Off
            ColorMask RGB

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma shader_feature _DIRECTION_VERTICAL _DIRECTION_HORIZONTAL

            #include "UnityCG.cginc"

            sampler2D _FormTex;
            sampler2D _ToTex;
            float _Progress;
            float _Feathering;
            float _SplitCount;
            float _SplitEndOffset;
            float _ReverseStartDir;
            
            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 pos : SV_POSITION; };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 colFrom = tex2D(_FormTex, i.uv);

                float t_axis, s_axis;

                #if _DIRECTION_VERTICAL
                    t_axis = i.uv.y;
                    s_axis = i.uv.x;
                #else
                    t_axis = i.uv.x;
                    s_axis = i.uv.y;
                #endif

                float sine_mask = step(0, sin(s_axis * _SplitCount * 3.14159));
                float control_mask = lerp(sine_mask, 1.0 - sine_mask, _ReverseStartDir);

                float offset_dir = lerp(1.0, -1.0, sine_mask); 
                float t2_t = t_axis + (offset_dir * (_Progress - _SplitEndOffset));

                float2 t2_uv;
                #if _DIRECTION_VERTICAL
                    t2_uv = float2(i.uv.x, t2_t);
                #else
                    t2_uv = float2(t2_t, i.uv.y);
                #endif
                
                float4 colTo = tex2D(_ToTex, t2_uv);

                float transition_range = 1.0 + 2.0 * _Feathering;
                float transition_start = 0.0 - _Feathering;
                
                float transition_value = transition_start + _Progress * transition_range;
                
                float final_blend_factor = 0.0;
                
                float edge0_mask1 = transition_value - _Feathering;
                float edge1_mask1 = transition_value;
                
                float mask1_feather = smoothstep(edge0_mask1, edge1_mask1, t_axis);
                mask1_feather = 1.0 - mask1_feather; 
                final_blend_factor += mask1_feather * (1.0 - control_mask); 
                
                float edge0_mask2 = 1.0 - transition_value;
                float edge1_mask2 = 1.0 - transition_value + _Feathering;
                
                float mask2_feather = smoothstep(edge0_mask2, edge1_mask2, t_axis);
                final_blend_factor += mask2_feather * control_mask; 

                return lerp(colFrom, colTo, saturate(final_blend_factor));
            }
            ENDHLSL
        }
    }
}
