Shader "NV/Ripple"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _T ("Time", Range(0.0, 1.0)) = 0.0
        _Aspect ("Aspect Ratio", Float) = 1.77777778
        _Amp ("Amplitude", Float) = 0.01
        _RFreq ("Radial Frequency", Float) = 50.0
        _TFreq ("Time Frequency", Float) = 3.0
        _BlurSize ("Blur Size", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _T, _Aspect, _Amp, _RFreq, _TFreq, _BlurSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            // ?? 9 ?????
            float4 GaussianBlur9(sampler2D tex, float2 uv, float2 texelSize, float radius)
            {
                float4 c = float4(0,0,0,0);
                float total = 0;
                for(int x=-1; x<=1; x++)
                {
                    for(int y=-1; y<=1; y++)
                    {
                        float weight = 1.0; // ????????????
                        c += tex2D(tex, uv + float2(x, y) * texelSize * radius) * weight;
                        total += weight;
                    }
                }
                return c / total;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv -= 0.5;
                uv.x *= _Aspect;
                float r = length(uv);
                r = max(r, 1e-3);
                float dr = sin(_RFreq * r + _TFreq * _T);
                dr = dr * dr * dr * _Amp;
                uv *= (r + min(r, _Amp) * dr) / r;
                uv.x /= _Aspect;
                uv += 0.5;

                float4 col;
                if(_BlurSize > 1e-3)
                    col = GaussianBlur9(_MainTex, uv, _MainTex_TexelSize.xy, _BlurSize * abs(dr));
                else
                    col = tex2D(_MainTex, uv);

                col *= i.color;
                return col;
            }
            ENDCG
        }
    }
}
