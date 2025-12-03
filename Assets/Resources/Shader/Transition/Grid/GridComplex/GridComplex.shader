Shader "Transition/GridComplex"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _SubTex  ("Sub Tex", 2D) = "black" {}
        
        _MainColor ("Main Color (Fallback/Tint)", Color) = (1,1,1,1)
        _SubColor  ("Sub Color (Fallback/Tint)", Color) = (1,1,1,1)
        
        _Progress("Progress", Range(0, 1)) = 0
        
        [Enum(Rectangular,0, Doors,1, Radius,2)]
        _Shape("Shape", Float) = 0
        
        _GridXY("Grid XY", Vector) = (16, 8, 0, 0)
        _Rotation("Rotation", Range(0, 360)) = 45
        [ToggleUI]_IsInvert("Invert Grid Pattern", Float) = 0
        
        _ScaleXY("Scale XY", Vector) = (1, 1, 0, 0)
        _Pivot("Pivot", Vector) = (0.5, 0.5, 0, 0)
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        
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
            float _Shape;
            float2 _GridXY;
            float _Rotation;
            float _IsInvert;
            float2 _ScaleXY;
            float2 _Pivot;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
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
            // Shape Calculation
            //---------------------------------------------
            float CalculateShape(float2 uv)
            {
                float2 scaledUV = float2(uv.x * _ScaleXY.x, uv.y * _ScaleXY.y);
                float2 scaledPivot = float2(_Pivot.x * _ScaleXY.x, _Pivot.y * _ScaleXY.y);
                
                // 0 = Rectangular
                if (_Shape < 0.5)
                {
                    return distance(scaledUV, scaledPivot);
                }
                // 1 = Doors
                else if (_Shape < 1.5)
                {
                    return abs(uv.y - 0.5);
                }
                // 2 = Radius
                else
                {
                    return abs(uv.x - 0.5) + abs(uv.y - 0.5);
                }
            }
            
            //---------------------------------------------
            // Grid Pattern + Rotation
            //---------------------------------------------
            float CalculateGrid(float2 uv)
            {
                float2 gridUV = frac(uv * _GridXY);
                float2 center = float2(0.5, 0.5);
                float angle = radians(_Rotation);
                float2 rotated = gridUV - center;
                float2x2 rot = float2x2(cos(angle), -sin(angle), sin(angle), cos(angle));
                rotated = mul(rot, rotated) + center;
                float dist = abs(rotated.x - 0.5) + abs(rotated.y - 0.5);
                return (_IsInvert > 0.5) ? (1.0 - dist) : dist;
            }
            
            //---------------------------------------------
            // Fragment
            //---------------------------------------------
            fixed4 frag(v2f i) : SV_Target
            {
                float4 mainCol = SampleTex(_MainTex, _MainColor, i.uv, _MainTex_TexelSize);
                float4 subCol = SampleTex(_SubTex, _SubColor, i.uv, _SubTex_TexelSize);
                
                if (_Progress <= 0.0) return mainCol;
                if (_Progress >= 1.0) return subCol;
                
                float shape = CalculateShape(i.uv);
                float grid = CalculateGrid(i.uv);
                float normalized = saturate((shape + grid) / 2.0);
                float transition = step(_Progress, normalized);
                
                // ?????????
                float4 outCol = lerp(subCol, mainCol, transition);
                
                return outCol;
            }
            
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}