Shader "Transition/Sawtooth"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _SubTex  ("Sub Tex", 2D) = "black" {}
        
        _MainColor ("Main Color (Fallback/Tint)", Color) = (1,1,1,1)
        _SubColor  ("Sub Color (Fallback/Tint)", Color) = (1,1,1,1)
        
        [Header(Transition)]
        _Progress ("Progress", Range(0, 1)) = 0.5
        [Enum(Vertical,0, Horizontal,1)] _Direction ("Direction", Float) = 1
        
        [Header(Wave Geometry)]
        _Height ("Wave Height", Float) = 1.0
        _Count ("Count (Wave Multiplier)", Float) = 2.0 
        [ToggleUI]_IsInvert("Invert Wave Shape (M/W)", Float) = 0
        
        [Header(Controls)]
        _Pivot ("Pivot", Vector) = (0.5, 0.5, 0, 0)
        _SoftEdge("Soft Edge", Range(0.0, 0.5)) = 0.05
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

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _SubTex;
            float4 _MainTex_TexelSize;
            float4 _SubTex_TexelSize;
            float4 _MainColor;
            float4 _SubColor;
            
            float _Progress;
            float _Direction;
            float _Height;
            float _Count;
            float4 _Pivot;
            float _IsInvert;
            float _SoftEdge;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
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
            // 羽化步进函数
            //---------------------------------------------
            float GetFeatheredStep(float blendValue, float feather)
            {
                float aaly_width = fwidth(blendValue) * 0.5;

                if (feather < 0.001)
                {
                    return smoothstep(0.5 - aaly_width, 0.5 + aaly_width, blendValue);
                }
                else
                {
                    float final_feather = max(feather, aaly_width);
                    return smoothstep(0.5 - final_feather, 0.5 + final_feather, blendValue);
                }
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

                // 1. 根据方向设置过渡轴和分割轴
                float t_axis, s_axis;
                float pivot_t, pivot_s;

                if (_Direction < 0.5) // Vertical
                {
                    t_axis = i.uv.y;
                    s_axis = i.uv.x;
                    pivot_t = _Pivot.y;
                    pivot_s = _Pivot.x;
                }
                else // Horizontal
                {
                    t_axis = i.uv.x;
                    s_axis = i.uv.y;
                    pivot_t = _Pivot.x;
                    pivot_s = _Pivot.y;
                }
                
                float t_adj = t_axis - pivot_t;
                float s_adj = s_axis - pivot_s;

                float waveRange = 0.5;
                float heightRange = _Height * 0.5;

                float minPossible = -heightRange;
                float maxPossible = waveRange + heightRange;

                float margin = 0.6;
                float transitionCenter = lerp(minPossible - margin, maxPossible + margin, _Progress);

                float saw_A = abs(frac(s_adj * _Count) - 0.5);
                float saw_B = abs(frac(s_adj * _Count + 0.5) - 0.5);

                float saw1, saw2;

                if (_IsInvert > 0.5) 
                {
                    saw1 = saw_B; 
                    saw2 = saw_A; 
                } 
                else 
                {
                    saw1 = saw_A; 
                    saw2 = saw_B; 
                }
                
                float tOffset1 = t_adj * _Height;
                float blendValue1 = saw1 + (tOffset1 - transitionCenter);

                float tOffset2 = (-t_adj) * _Height;
                float blendValue2 = saw2 + (tOffset2 - transitionCenter);
                
                float feathered_step1 = GetFeatheredStep(blendValue1, _SoftEdge);
                float feathered_step2 = GetFeatheredStep(blendValue2, _SoftEdge);

                float finalBlend = saturate(1.0 - (feathered_step1 + feathered_step2));

                return lerp(mainCol, subCol, finalBlend);
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}