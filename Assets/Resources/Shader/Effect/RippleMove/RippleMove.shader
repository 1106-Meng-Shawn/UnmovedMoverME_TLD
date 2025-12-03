Shader "Effect/RippleMove"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Progress ("Progress", Range(0.0, 1.0)) = 0.5
        _Center ("Wave Center", Vector) = (0.5,0.5,0,0)
        _Aspect ("Aspect Ratio", Float) = 1.77777778
        _Amp ("Amplitude", Float) = 0.01
        _RFreq ("Radial Frequency", Float) = 50.0
        _TFreq ("Time Frequency", Float) = 3.0
        _BlurSize ("Blur Size", Float) = 1.0
        _Width ("Peak Width", Float) = 0.1
        _Fade ("Fade Time", Float) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 tex1 : TEXCOORD1; // x: ????, y: fade factor
                float4 color : COLOR;
            };

            float _Progress, _Aspect, _Fade, _Amp, _RFreq, _TFreq, _BlurSize, _Width;
            float2 _Center;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.tex1.x = 0.5 * sqrt(1.0 + _Aspect * _Aspect);
                o.tex1.y = saturate(_Progress / _Fade) * saturate((1.0 - _Progress) / _Fade);
                o.color = v.color;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float4 tex2DGaussianBlur(sampler2D tex, float2 texelSize, float2 uv, float blur)
            {
                float4 sum = float4(0,0,0,0);
                float weightSum = 0.0;
                int radius = 2;
                for (int x=-radius; x<=radius; x++)
                {
                    for (int y=-radius; y<=radius; y++)
                    {
                        float2 offset = float2(x,y) * texelSize * blur;
                        float w = exp(-(x*x+y*y)/(2.0*radius*radius));
                        sum += tex2D(tex, uv + offset) * w;
                        weightSum += w;
                    }
                }
                return sum / weightSum;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv -= _Center;
                uv.x *= _Aspect;
                float r = length(uv);
                r = max(r, 1e-3);

                float dr = sin(_RFreq * r + _TFreq * _Progress);
                float t = r / i.tex1.x;
                float dt = (_Progress - t) / _Width;
                dr /= 1.0 + dt * dt;
                dr *= i.tex1.y;
                dr = dr * dr * dr;

                uv *= (r + min(r, _Amp) * dr) / r;
                uv.x /= _Aspect;
                uv += _Center;

                float4 col;
                if (_BlurSize > 1e-3)
                {
                    col = tex2DGaussianBlur(_MainTex, _MainTex_TexelSize.xy, uv, _BlurSize * abs(dr));
                }
                else
                {
                    col = tex2D(_MainTex, uv);
                }
                col *= i.color;
                return col;
            }
            ENDCG
        }
    }
}
