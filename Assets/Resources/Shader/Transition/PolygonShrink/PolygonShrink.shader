Shader "Transition/PolygonShrink"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _SubTex  ("Sub Tex", 2D) = "black" {}
        
        _MainColor ("Main Color (Fallback/Tint)", Color) = (1,1,1,1)
        _SubColor  ("Sub Color (Fallback/Tint)", Color) = (1,1,1,1)
        
        _TransitionColor ("Transition Color", Color) = (0, 0, 0, 1)
        
        [Header(Transition Control)]
        _Progress ("Progress", Range(0, 1)) = 0
        _Pivot ("Pivot", Vector) = (0.5, 0.5, 0, 0)
        
        [Header(Polygon Shape)]
        _Sides ("Number of Sides", Range(3, 12)) = 6
        _Size ("Size (Polygon Size)", Range(0.1, 2.0)) = 1.5
        _SoftEdge ("Soft Edge", Range(0.0, 0.1)) = 0
        _Rotation ("Rotation", Range(0, 360)) = 0
        _AspectRatio ("Aspect Ratio", Float) = 1.777
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
            
            float4 _TransitionColor;
            float _Progress;
            float2 _Pivot;
            float _Sides;
            float _Size;
            float _SoftEdge;
            float _Rotation;
            float _AspectRatio;

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

            v2f vert (appdata v)
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
            // 旋转UV坐标
            //---------------------------------------------
            float2 RotateUV(float2 uv, float angleDeg)
            {
                float rad = radians(angleDeg);
                float c = cos(rad);
                float s = sin(rad);
                float2x2 rotMatrix = float2x2(c, -s, s, c);
                return mul(rotMatrix, uv);
            }

            //---------------------------------------------
            // 正多边形SDF（有向距离场）
            //---------------------------------------------
            float PolygonSDF(float2 p, float sides, float size)
            {
                float PI = 3.14159265359;
                float TAU = 6.28318530718;
                
                float angle = atan2(p.y, p.x);
                float radius = length(p);
                
                float sectorAngle = TAU / sides;
                angle = fmod(angle + PI, sectorAngle) - sectorAngle * 0.5;
                
                float edgeDistance = size * cos(sectorAngle * 0.5);
                float dist = cos(angle) * radius - edgeDistance;
                
                return dist;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 mainCol = SampleTex(_MainTex, _MainColor, i.uv, _MainTex_TexelSize);
                float4 subCol = SampleTex(_SubTex, _SubColor, i.uv, _SubTex_TexelSize);
                
                if (_Progress <= 0.0) return mainCol;
                if (_Progress >= 1.0) return subCol;
                
                float2 uv = i.uv - _Pivot;
                uv.x *= _AspectRatio;
                uv = RotateUV(uv, _Rotation);
                
                float sides = max(_Sides, 3.0);
                
                float4 finalColor;
                
                if (_Progress < 0.5)
                {
                    float shrinkProgress = 1.0 - (_Progress * 2.0);
                    float currentSize = _Size * shrinkProgress;
                    
                    float dist = PolygonSDF(uv, sides, currentSize);
                    float mask = smoothstep(_SoftEdge, -_SoftEdge, dist);
                    
                    finalColor = lerp(_TransitionColor, mainCol, mask);
                }
                else
                {
                    float expandProgress = (_Progress - 0.5) * 2.0;
                    float currentSize = _Size * expandProgress;
                    
                    float dist = PolygonSDF(uv, sides, currentSize);
                    float mask = smoothstep(_SoftEdge, -_SoftEdge, dist);
                    
                    finalColor = lerp(_TransitionColor, subCol, mask);
                }
                
                return finalColor;
            }
            ENDCG
        }
    }
    
    FallBack "Unlit/Transparent"
}