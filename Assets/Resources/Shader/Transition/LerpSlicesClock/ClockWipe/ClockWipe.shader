Shader "Transition/ClockWipe"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _SubTex  ("Sub Tex", 2D) = "black" {}
        
        _MainColor ("Main Color (Fallback/Tint)", Color) = (1,1,1,1)
        _SubColor  ("Sub Color (Fallback/Tint)", Color) = (1,1,1,1)
        
        _Progress ("Progress", Range(0, 1)) = 0
        _Pivot ("Pivot", Vector) = (0.5, 0.5, 0, 0)
        _Count ("Number of Blocks", Range(1, 24)) = 4
        _Clockwise ("Clockwise", Float) = 1
        _SoftEdge ("Soft Edge", Range(0, 0.05)) = 0.02
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
            float2 _Pivot;
            float _Count;
            float _Clockwise;
            float _SoftEdge;
            
            #define PI 3.14159265359
            #define TWO_PI 6.28318530718
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            //---------------------------------------------
            // ?????????
            //---------------------------------------------
            float4 SampleTex(sampler2D tex, float4 fallbackColor, float2 uv, float4 texelSize)
            {
                // ????????
                bool hasTexture = (texelSize.z > 1.0 && texelSize.w > 1.0);
                
                if (!hasTexture)
                {
                    // ?????????
                    return fallbackColor;
                }
                
                // ????
                float4 texColor = tex2D(tex, uv);
                
                // ???????????????
                if (texColor.a < 0.001)
                {
                    return float4(fallbackColor.rgb, 0.0);
                }
                
                // ???????????? × Color??
                return float4(texColor.rgb * fallbackColor.rgb, texColor.a * fallbackColor.a);
            }
            
            //---------------------------------------------
            // Fragment Shader
            //---------------------------------------------
            fixed4 frag(v2f i) : SV_Target
            {
                float4 mainCol = SampleTex(_MainTex, _MainColor, i.uv, _MainTex_TexelSize);
                float4 subCol = SampleTex(_SubTex, _SubColor, i.uv, _SubTex_TexelSize);
                
                if (_Progress <= 0.0) return mainCol;
                if (_Progress >= 1.0) return subCol;
                
                float2 uv = i.uv;
                float2 center = _Pivot;
                float2 dir = uv - center;
                
                // ???? 0~2PI?12?????0°
                float angle = atan2(dir.y, dir.x) + PI/2.0;
                if (angle < 0) angle += TWO_PI;
                
                if (_Clockwise < 0.5)
                    angle = TWO_PI - angle;
                
                // ---- ???? ----
                float segmentAngle = TWO_PI / _Count;
                float blockIndex = floor(angle / segmentAngle);
                float localAngle = angle - blockIndex * segmentAngle; // ????
                float normalizedLocal = localAngle / segmentAngle; // 0~1
                
                // ---- ?????? ----
                float blend = normalizedLocal - (1.0 - _Progress);
                
                if (_Progress <= 0.0)
                {
                    blend = 0.0; // ???????
                }
                else if (_Progress >= 1.0)
                {
                    blend = 1.0; // ???????
                }
                else
                {
                    // ???????
                    if (_SoftEdge > 0.0)
                    {
                        blend = smoothstep(0.0, _SoftEdge * 2.0, normalizedLocal - (1.0 - _Progress) + _SoftEdge);
                    }
                    else
                    {
                        blend = (normalizedLocal - (1.0 - _Progress) >= 0.0) ? 1.0 : 0.0;
                    }
                }
                
                blend = saturate(blend);
                
                // ?????????
                fixed4 finalColor = lerp(mainCol, subCol, blend);
                
                return finalColor;
            }
            
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}