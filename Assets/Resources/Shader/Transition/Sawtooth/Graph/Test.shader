Shader "Transition/Sawtooth"
{
    Properties
    {
        _TransitionSlider ("Transition Progress", Range(0, 1)) = 0.5
        [NoScaleOffset]_T1 ("Texture 1", 2D) = "white" {}
        [NoScaleOffset]_T2 ("Texture 2", 2D) = "white" {}
        _Height ("Wave Height", Float) = 1.0 // ?????????
        _WaveMulti ("Wave Multiplier", Float) = 2.0 // ??????
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            Cull Back
            ZWrite On
            Blend One Zero

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // ----------------------------------------------------
            // ???? (? Properties ???)
            // ----------------------------------------------------
            sampler2D _T1;
            sampler2D _T2;
            float _TransitionSlider;
            float _Height;
            float _WaveMulti;

            // ----------------------------------------------------
            // ???
            // ----------------------------------------------------
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

            // ----------------------------------------------------
            // ?????
            // ----------------------------------------------------
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // ----------------------------------------------------
            // ?????
            // ----------------------------------------------------
            float4 frag (v2f i) : SV_Target
            {
                // ??????
                float4 col1 = tex2D(_T1, i.uv);
                float4 col2 = tex2D(_T2, i.uv);

                // ? Slider [0, 1] ??????? [_Height, -_Height]
                // ????: H * (2*S - 1) => lerp(-H, H, S)
                float transitionCenter = lerp(-_Height, _Height, _TransitionSlider);

                // --- ????????? ---
                float u = i.uv.x;
                float v = i.uv.y;

                // --------------------------------------
                // ?????? (Sawtooth Logic)
                // --------------------------------------

                // T1 -> T2 ???
                // 1. ???????: abs(frac(u * M) - 0.5)
                float saw1 = abs(frac(u * _WaveMulti) - 0.5);
                // 2. ?? V ?????: (v - 0.5) * H
                float vOffset1 = (v - 0.5) * _Height;
                // 3. ?????: ??? + (V?? - ????)
                float blendValue1 = saw1 + (vOffset1 - transitionCenter);
                // 4. Step ???? T1/T2 ?? (0.5 ???)
                float step1 = step(0.5, blendValue1);


                // T2 -> T1 ??? (? T1 ????????????????)
                // 1. ????????????: abs(frac(u * M + 0.5) - 0.5)
                float saw2 = abs(frac(u * _WaveMulti + 0.5) - 0.5);
                // 2. ?? V ????? (???? V ???????????): (1 - v - 0.5) * H
                float vOffset2 = (1.0 - v - 0.5) * _Height;
                // 3. ?????: ??? + (V?? - ????)
                float blendValue2 = saw2 + (vOffset2 - transitionCenter);
                // 4. Step ???? T2/T1 ??
                float step2 = step(0.5, blendValue2);


                // --------------------------------------
                // ???? (Final Blend)
                // --------------------------------------

                // ???? Step ?????? (1 - (step1 + step2))
                // ??? step1 ? step2 ?? 0 ?????? 1 (????)
                // ???????????????????BlendFactor ?? 1 (?? col2)
                float finalBlend = saturate(1.0 - (step1 + step2));

                // ??????????
                return lerp(col1, col2, finalBlend);
            }
            ENDHLSL
        }
    }
}