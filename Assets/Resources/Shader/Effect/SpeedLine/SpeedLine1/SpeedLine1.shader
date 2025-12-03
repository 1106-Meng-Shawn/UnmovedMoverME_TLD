Shader "Effect/SpeedLine1"
{
    Properties
    {
        [NoScaleOffset]_MainTex("Main Texture", 2D) = "white" {}
        _MainColor("Main Color", Color) = (1, 1, 1, 0)
        _Color("Speed Line Color", Color) = (0, 0, 0, 1)
        _Pivot("Pivot", Vector) = (0.5, 0.5, 0, 0)
        
        [Header(Speed Line Settings)]
        _Range("Visibility Range", Range(0, 2)) = 0.5
        _Speed("Animation Speed", Range(0, 20)) = 5
        _Power("Line Density", Range(5, 50)) = 5
        _Diff("Line Width", Range(0, 10)) = 1.5
        
        [Header(Animation Control)]
        [ToggleUI]_UseExternalTime("Use External Time", Float) = 0
        _ExternalTime("External Time Value", Range(0, 10)) = 0
        
        [Header(Visual Effects)]
        _ColorRange("Radial Fade", Range(0.1, 20)) = 0.1
        _Sediao("Color Steps (0=Smooth)", Range(0, 20)) = 10
        
        [Header(Options)]
        [ToggleUI]_Flip("Flip Visibility", Float) = 0
        [ToggleUI]_PosterizeMode("Frame Animation", Float) = 0
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
        Cull Back
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainColor;
            float _Flip;
            float _PosterizeMode;
            float _Range;
            float _ColorRange;
            float _Power;
            float _Speed;
            float2 _Pivot;
            float _Diff;
            float4 _Color;
            float _Sediao;
            float _UseExternalTime;
            float _ExternalTime;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float2 ToPolar(float2 uv, float2 center)
            {
                float2 delta = uv - center;
                float radius = length(delta) * 2;
                float angle = atan2(delta.x, delta.y) / 6.28318530718;
                return float2(radius, angle);
            }

            float2 Hash22(float2 p)
            {
                p = frac(p * float2(443.897, 441.423));
                p += dot(p, p.yx + 19.19);
                return frac((p.xx + p.yx) * p.xy);
            }

            float Voronoi(float2 uv, float angleOffset, float density)
            {
                float2 gridPos = floor(uv * density);
                float2 fracPos = frac(uv * density);
                float minDist = 8.0;
                
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 neighbor = float2(x, y);
                        float2 randomPoint = Hash22(gridPos + neighbor);
                        
                        randomPoint = float2(
                            sin(randomPoint.y * angleOffset),
                            cos(randomPoint.x * angleOffset)
                        ) * 0.5 + 0.5;
                        
                        float dist = length(neighbor + randomPoint - fracPos);
                        minDist = min(minDist, dist);
                    }
                }
                
                return minDist;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 mainTex = tex2D(_MainTex, i.uv);
                float4 baseColor = mainTex * _MainColor * i.color;
                
                float2 polarUV = ToPolar(i.uv, _Pivot);
                
                float2 speedUV = polarUV * float2(1.5, 0);
                float2 tiledUV = polarUV * float2(1, 200);
                
                float time;
                if (_UseExternalTime > 0.5)
                {
                    time = _ExternalTime * _Speed;
                }
                else
                {
                    time = _Time.y * _Speed;
                }
                
                if (_PosterizeMode > 0.5)
                {
                    time = floor(time);
                }
                
                float voronoi1 = Voronoi(tiledUV, time, _Power - _Diff);
                float voronoi2 = Voronoi(tiledUV, time, _Power);
                float speedPattern = (voronoi1 - voronoi2) * speedUV.x;
                
                if (_Sediao > 0)
                {
                    speedPattern = floor(speedPattern * _Sediao) / _Sediao;
                }
                
                float2 colorUV = polarUV * _ColorRange;
                float radialFade = saturate(1 - colorUV.x);
                
                float speedLineMask = step(_Range, speedPattern);
                
                if (_Flip > 0.5)
                {
                    speedLineMask = 1 - speedLineMask;
                }
                
                speedLineMask *= radialFade;
                
                float4 speedLineColor = _Color;
                float4 finalColor = lerp(baseColor, speedLineColor, speedLineMask);
                finalColor.a = lerp(baseColor.a, _Color.a, speedLineMask);
                
                return finalColor;
            }
            ENDCG
        }
    }
    
    FallBack "Unlit/Transparent"
}