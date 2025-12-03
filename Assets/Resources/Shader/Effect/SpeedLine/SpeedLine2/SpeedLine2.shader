Shader "Effect/SpeedLine2"
{
    Properties
    {
        [NoScaleOffset]_MainTex("Main Texture", 2D) = "white" {}
        _MainColor("Main Color", Color) = (1, 1, 1, 0)

        [Header(Appearance)]
        _Color("Speed Line Color", Color) = (0, 0, 0, 1)
        _Center("Effect Center", Vector) = (0.5, 0.5, 0, 0)

        
        [Header(Animation)]
        _Speed("Animation Speed", Range(0, 20)) = 5
        [ToggleUI]_ModeChange("Smooth Animation", Float) = 0
        [ToggleUI]_UseExternalTime("Use External Time", Float) = 0
        _ExternalTime("External Time Value", Float) = 0
        
        [Header(Speed Line Pattern)]
        _Tiling("Pattern Tiling", Vector) = (1, 50, 0, 0)
        _Diff1("Pattern Variation 1", Vector) = (0.5, 0.5, 0, 0)
        _Diff2("Pattern Variation 2", Vector) = (10, 10, 0, 0)
        
        [Header(Visibility Control)]
        _Step("Start Threshold", Range(0, 2)) = 0.5
        _Fenli("End Threshold", Range(0, 2)) = 0.5
        [ToggleUI]_Flip("Flip Visibility", Float) = 0
        
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
            float _Speed;
            float _ModeChange;
            float _UseExternalTime;
            float _ExternalTime;
            float2 _Tiling;
            float3 _Diff1;
            float2 _Diff2;
            float _Step;
            float _Fenli;
            float _Flip;
            float4 _Color;
            float2 _Center;

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
                float radius = length(delta) * 2 * 0.39;
                float angle = atan2(delta.x, delta.y) / 6.28318530718;
                return float2(radius, angle);
            }

            float2 Hash22(float2 p)
            {
                p = frac(p * float2(443.897, 441.423));
                p += dot(p, p.yx + 19.19);
                return frac((p.xx + p.yx) * p.xy);
            }

            float2 Voronoi(float2 uv, float angleOffset, float density)
            {
                float2 gridPos = floor(uv * density);
                float2 fracPos = frac(uv * density);
                float minDist = 8.0;
                float cells = 0.0;
                
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
                        if (dist < minDist)
                        {
                            minDist = dist;
                            cells = randomPoint.x;
                        }
                    }
                }
                
                return float2(minDist, cells);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 mainTex = tex2D(_MainTex, i.uv);
                float4 baseColor = mainTex * _MainColor * i.color;
                
                float2 polarUV = ToPolar(i.uv, _Center);
                
                float radialIntensity = polarUV.x * 2.0;
                
                float time;
                if (_UseExternalTime > 0.5)
                {
                    time = _ExternalTime * _Speed;
                }
                else
                {
                    time = _Time.y * _Speed;
                }
                
                if (_ModeChange < 0.5)
                {
                    time = floor(time);
                }
                
                float2 tiledUV1 = polarUV * _Tiling.xy;
                float2 voronoi1 = Voronoi(tiledUV1, time, _Diff2.x);
                
                float2 tiledUV2 = polarUV * (_Tiling.xy - _Diff1.xy);
                float2 voronoi2 = Voronoi(tiledUV2, time - 1.0, _Diff2.y);
                
                float pattern = (voronoi1.x - voronoi2.y) * radialIntensity;
                
                float mask1 = step(_Step, pattern);
                float mask2 = step(_Step + _Fenli, pattern);
                float finalMask = mask1 - mask2;
                
                if (_Flip > 0.5)
                {
                    finalMask = 1.0 - finalMask;
                }
                
                // 速度线颜色完全独立，不受 _MainColor 和 i.color 影响
                float4 speedLineColor = _Color;
                float4 finalColor = lerp(baseColor, speedLineColor, finalMask);
                
                finalColor.a = lerp(baseColor.a, _Color.a, finalMask);
                
                return finalColor;
            }
            ENDCG
        }
    }
    
    FallBack "Unlit/Transparent"
}