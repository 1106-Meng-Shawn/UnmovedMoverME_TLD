Shader "Transition/Wave"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _SubTex  ("Sub Tex", 2D) = "black" {}
        
        _MainColor ("Main Color (Fallback/Tint)", Color) = (1,1,1,1)
        _SubColor  ("Sub Color (Fallback/Tint)", Color) = (1,1,1,1)
        
        [Enum(Horizontal,0, Vertical,1)] _Direction("Direction", Float) = 1

        _Progress("Progress", Range(0, 1)) = 0.5
        _Height("Wave Height", Float) = 2
        _Count("Count (Wave Multiplier)", Float) = 5
        
        _SoftEdge("Soft Edge", Range(0, 1)) = 0.1 
        _WaveSpeed("Wave Speed", Float) = 2
        _Rotation("Rotation", Range(0, 360)) = 0
        [ToggleUI]_IsSelfTime("Self Time (Auto-move)", Float) = 0
        _AnimationProgress("Animation Progress (0-10)", Range(0, 10)) = 0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _SubTex;
            float4 _MainTex_TexelSize;
            float4 _SubTex_TexelSize;
            float4 _MainColor;
            float4 _SubColor;
            
            float _Direction;
            float _Progress;
            float _Height;
            float _Count;
            float _WaveSpeed;
            float _Rotation;
            float _IsSelfTime;
            float _AnimationProgress;
            float _SoftEdge;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_FOG_COORDS(1)
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            //---------------------------------------------
            // 改进的纹理采样函数
            //---------------------------------------------
            float4 SampleTex(sampler2D tex, float4 fallbackColor, float2 uv, float4 texelSize)
            {
                // 检查纹理是否有效
                bool hasTexture = (texelSize.z > 1.0 && texelSize.w > 1.0);
                
                if (!hasTexture)
                {
                    // 没有纹理，返回纯色
                    return fallbackColor;
                }
                
                // 采样纹理
                float4 texColor = tex2D(tex, uv);
                
                // 如果纹理像素完全透明，返回透明
                if (texColor.a < 0.001)
                {
                    return float4(fallbackColor.rgb, 0.0);
                }
                
                // 有纹理且不透明：纹理颜色 × Color调制
                return float4(texColor.rgb * fallbackColor.rgb, texColor.a * fallbackColor.a);
            }

            //---------------------------------------------
            // UV 旋转函数
            //---------------------------------------------
            float2 RotateDegrees(float2 uv, float2 center, float rotation)
            {
                rotation = rotation * (UNITY_PI/180.0);
                uv -= center;
                float s = sin(rotation);
                float c = cos(rotation);
                float2x2 rMatrix = float2x2(c, -s, s, c);
                uv = mul(uv, rMatrix); 
                return uv + center;
            }

            //---------------------------------------------
            // Fragment Shader
            //---------------------------------------------
            float4 frag (v2f i) : SV_Target
            {
                float4 mainCol = SampleTex(_MainTex, _MainColor, i.uv, _MainTex_TexelSize);
                float4 subCol = SampleTex(_SubTex, _SubColor, i.uv, _SubTex_TexelSize);

                if (_Progress <= 0.0) return mainCol;
                if (_Progress >= 1.0) return subCol;
                
                float time_val = (_IsSelfTime > 0.5) ? _Time.y : _AnimationProgress;
                float time_calc = _WaveSpeed * time_val;

                float2 rotated_uv = RotateDegrees(i.uv, float2(0.5, 0.5), _Rotation);
                
                // Direction: 0 = Horizontal, 1 = Vertical
                float wave_drive_axis = (_Direction > 0.5) ? rotated_uv.x : rotated_uv.y;
                float transition_axis = (_Direction > 0.5) ? rotated_uv.y : rotated_uv.x;
                float transition_axis_inv = 1.0 - transition_axis;

                float transition_remap = -1.0 + (_Progress - 0.0) * (1.0 - (-1.0)) / (1.0 - 0.0);
                float transition_offset = _Height * transition_remap; 

                float wave_height_offset_1 = _Height * (transition_axis - 0.5);
                float offset_1 = wave_height_offset_1 - transition_offset;
                
                float wave_fn_1 = sin(_Count * (time_calc + wave_drive_axis * UNITY_PI));
                float wave_pos_1 = wave_fn_1 + offset_1;
                
                float mask_1 = smoothstep(0.0 - _SoftEdge, 0.0 + _SoftEdge, wave_pos_1);

                float wave_height_offset_2 = _Height * (transition_axis_inv - 0.5);
                float offset_2 = wave_height_offset_2 - transition_offset;
                
                float wave_fn_2 = cos(_Count * (time_calc + wave_drive_axis * UNITY_PI) + UNITY_PI * 0.5);
                float wave_pos_2 = wave_fn_2 + offset_2;
                
                float mask_2 = smoothstep(1.0 - _SoftEdge, 1.0 + _SoftEdge, wave_pos_2);

                float blend_mask = saturate(mask_1 + mask_2);

                float4 final_color = lerp(subCol, mainCol, blend_mask);
                
                UNITY_APPLY_FOG(i.fogCoord, final_color);
                
                return final_color;
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}