Shader "Effect/Sharpen"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Progress ("Sharpen Progress", Range(0.0, 1.0)) = 0.5
        _Size ("Blur Size", Float) = 1.0
        _Strength ("Sharpen Strength", Float) = 1.0
        [ToggleUI]_UseExternal ("Use External Progress", Float) = 0
        _Speed ("Internal Speed", Float) = 1.0
        [ToggleUI]_PingPong ("PingPong Loop", Float) = 0
        _Range ("Progress Range", Vector) = (0,1,0,0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Progress, _Size, _Strength, _UseExternal, _Speed, _PingPong;
            float2 _Range;

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

            float4 tex2DGaussianBlur(sampler2D tex, float4 texelSize, float2 uv, float blurSize)
            {
                float4 color = float4(0, 0, 0, 0);
                float totalWeight = 0.0;
                float weights[5] = {0.06136, 0.24477, 0.38774, 0.24477, 0.06136};
                
                for(int x = -2; x <= 2; x++)
                {
                    for(int y = -2; y <= 2; y++)
                    {
                        float2 offset = float2(x, y) * texelSize.xy * blurSize;
                        float weight = weights[x + 2] * weights[y + 2];
                        color += tex2D(tex, uv + offset) * weight;
                        totalWeight += weight;
                    }
                }
                
                return color / totalWeight;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // ?? Progress
                float prog;
                if(_UseExternal > 0.5)
                {
                    prog = _Progress;
                }
                else
                {
                    float t = _Time.y * _Speed;
                    if(_PingPong > 0.5)
                    {
                        // PingPong 0->1->0 ??
                        t = abs(frac(t * 0.5) * 2.0 * 2.0 - 1.0); // ??? 0-1-0
                    }
                    else
                    {
                        t = frac(t);
                    }

                    // ??? Range
                    prog = lerp(_Range.x, _Range.y, t);
                }

                float4 col = tex2D(_MainTex, i.uv);
                float4 blur = tex2DGaussianBlur(_MainTex, _MainTex_TexelSize, i.uv, _Size * prog);
                col += _Strength * prog * (col - blur);
                col *= i.color;
                return col;
            }
            ENDCG
        }
    }
}
