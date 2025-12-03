Shader "Transition/GridTime"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _SubTex  ("Sub Tex", 2D) = "black" {}
        
        _MainColor ("Main Color (Fallback/Tint)", Color) = (1,1,1,1)
        _SubColor  ("Sub Color (Fallback/Tint)", Color) = (1,1,1,1)
        
        _GridXY("Grid XY", Vector) = (16, 8, 0, 0)
        _Progress("Progress", Range(0, 1)) = 0
        [ToggleUI]_IsInvert("Invert (Middle Big)", Float) = 0
        [ToggleUI]_IsTimeDifference("Time Difference", Float) = 0
        _Rotation("Rotation", Range(0, 360)) = 45
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
            
            float2 _GridXY;
            float _Progress;
            float _IsInvert;
            float _IsTimeDifference;
            float _Rotation;
            
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
            float2 RotateUV(float2 uv, float2 center, float angle)
            {
                float rad = radians(angle);
                uv -= center;
                float s = sin(rad);
                float c = cos(rad);
                float2x2 rotMatrix = float2x2(c, -s, s, c);
                uv = mul(uv, rotMatrix);
                uv += center;
                return uv;
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
                
                // 时间差计算
                float timeDiff = 0;
                if (_IsTimeDifference > 0.5)
                {
                    float xOffset = abs(i.uv.x - 0.5);
                    float yOffset = abs(i.uv.y - 0.5);
                    
                    float xGrid = floor(xOffset * _GridXY.x) / _GridXY.x;
                    float yGrid = floor(yOffset * _GridXY.y) / _GridXY.y;
                    
                    timeDiff = xGrid + yGrid;
                }
                
                // 网格计算和旋转
                float2 gridUV = frac(i.uv * _GridXY);
                float2 rotatedUV = RotateUV(gridUV, float2(0.5, 0.5), _Rotation);
                
                float distX = abs(rotatedUV.x - 0.5);
                float distY = abs(rotatedUV.y - 0.5);
                float centerEffect = (_IsInvert > 0.5) ? (distX + distY) : (1 - (distX + distY));
                
                // 过渡计算
                float transition = timeDiff + centerEffect;
                
                float maxTransition = 1.414;
                if (_IsTimeDifference > 0.5)
                {
                    maxTransition += (_GridXY.x + _GridXY.y) * 0.5 / max(_GridXY.x, _GridXY.y);
                }
                transition = saturate(transition / maxTransition);
                float mask = step(transition, _Progress);
                
                // 在两个颜色之间插值
                fixed4 finalColor = lerp(mainCol, subCol, mask);
                
                return finalColor;
            }
            
            ENDCG
        }
    }
    
    FallBack "Unlit/Transparent"
}