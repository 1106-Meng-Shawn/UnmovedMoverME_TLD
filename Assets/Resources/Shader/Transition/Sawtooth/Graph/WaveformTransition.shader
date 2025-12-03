Shader "Transition/WaveTransition"
{
    Properties
    {
        _T1 ("Texture 1", 2D) = "white" {}
        _T2 ("Texture 2", 2D) = "white" {}

        _Height ("Height", Float) = 1
        _WaveMulti ("Wave Multi", Float) = 10
        _FinalWaveHeight ("Final Wave Height", Float) = 1
        _TransitionSlider ("Transition Slider", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _T1;
            sampler2D _T2;

            float _Height;
            float _WaveMulti;
            float _FinalWaveHeight;
            float _TransitionSlider;

            // simple hash function for Voronoi-like noise
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            // simplified Voronoi noise (approximation)
            float voronoiNoise(float2 uv)
            {
                uv *= _WaveMulti;
                float2 i = floor(uv);

                float minDist = 1e5;

                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 neighbor = i + float2(x, y);

                        float2 rndPoint = hash(neighbor); // FIXED
                        float2 diff = (neighbor + rndPoint) - uv;

                        float dist = length(diff);
                        minDist = min(minDist, dist);
                    }
                }

                return minDist;
            }


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Voronoi wave
                float wave = voronoiNoise(uv);
                wave = saturate(wave * _FinalWaveHeight + _Height);

                // transition mask
                float mask = step(wave, _TransitionSlider);

                fixed4 t1 = tex2D(_T1, uv);
                fixed4 t2 = tex2D(_T2, uv);

                // blend based on the procedural mask
                fixed4 col = lerp(t1, t2, mask);

                return col;
            }

            ENDCG
        }
    }
}
