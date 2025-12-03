Shader "Effect/TuringPattern"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Scale ("Scale", Range(1, 50)) = 10
        _Speed ("Speed", Range(0, 2)) = 0.5
        _Contrast ("Contrast", Range(0.1, 5)) = 1.5
        _Threshold ("Threshold", Range(0, 1)) = 0.5
        
        [Header(Pattern Parameters)]
        _ActivatorStrength ("Activator Strength", Range(0.5, 2.5)) = 1.5
        _InhibitorStrength ("Inhibitor Strength", Range(0.3, 1.5)) = 0.8
        _NoiseScale1 ("Noise Scale 1", Range(0.1, 3)) = 1.0
        _NoiseScale2 ("Noise Scale 2", Range(0.1, 3)) = 0.6
        _Octaves ("Noise Octaves", Range(1, 6)) = 4
        
        [Header(Colors)]
        _ColorA ("Color A", Color) = (0, 0, 0, 1)
        _ColorB ("Color B", Color) = (1, 1, 1, 1)
        
        [Header(Pattern Type)]
        [KeywordEnum(Spots, Stripes, Mixed, Labyrinth)] _PatternType ("Pattern Type", Float) = 0
        
        [Header(Animation)]
        [Toggle] _Animate ("Animate", Float) = 1
        _TimeScale ("Time Scale", Range(0.1, 3)) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _PATTERNTYPE_SPOTS _PATTERNTYPE_STRIPES _PATTERNTYPE_MIXED _PATTERNTYPE_LABYRINTH

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Scale;
            float _Speed;
            float _Contrast;
            float _Threshold;
            float _ActivatorStrength;
            float _InhibitorStrength;
            float _NoiseScale1;
            float _NoiseScale2;
            int _Octaves;
            float _TimeScale;
            fixed4 _ColorA;
            fixed4 _ColorB;
            float _Animate;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float hash(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f); 

                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));

                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            float fbm(float2 p, int octaves)
            {
                float value = 0.0;
                float amplitude = 0.5;
                float frequency = 1.0;
                
                for(int i = 0; i < octaves; i++)
                {
                    value += amplitude * noise(p * frequency);
                    frequency *= 2.0;
                    amplitude *= 0.5;
                }
                return value;
            }

            float turingPattern(float2 uv, float time)
            {
                float2 p = uv * _Scale;
                
                float activator = fbm(p + time * _Speed * _TimeScale * 0.2, _Octaves);
                float inhibitor = fbm(p * _NoiseScale2 - time * _Speed * _TimeScale * 0.1, max(1, _Octaves - 1));
                
                float pattern = 0.0;
                
                #if _PATTERNTYPE_SPOTS
                    pattern = activator * _ActivatorStrength - inhibitor * _InhibitorStrength;
                    
                #elif _PATTERNTYPE_STRIPES
                    float stripeNoise = fbm(float2(p.x * 2.0, p.y * 0.5) + time * _Speed * _TimeScale * 0.1, max(1, _Octaves - 2));
                    pattern = activator * _ActivatorStrength * 0.8 - inhibitor * _InhibitorStrength * 1.2 + stripeNoise * 0.3;
                    
                #elif _PATTERNTYPE_LABYRINTH
                    pattern = abs(activator - inhibitor) * _ActivatorStrength;
                    pattern = frac(pattern * 3.0);
                    
                #else 
                    pattern = activator * _ActivatorStrength * 0.9 - inhibitor * _InhibitorStrength * 0.9;
                    pattern += fbm(p * 1.5 + time * _Speed * _TimeScale * 0.15, max(1, _Octaves - 2)) * 0.3;
                #endif
                
                return pattern;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Animate > 0.5 ? _Time.y : 0.0;
                float pattern = turingPattern(i.uv, time);
                pattern = (pattern - 0.5) * _Contrast + 0.5;
                pattern = smoothstep(_Threshold - 0.1, _Threshold + 0.1, pattern);
                
                fixed4 col = lerp(_ColorA, _ColorB, pattern);
                
                return col;
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}