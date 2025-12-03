Shader "Effect/Rain"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _RainAmount ("Rain Amount", Range(0.0, 2.0)) = 1.0
        [Toggle] _ExternalControl ("External Control (Toggle)", Float) = 0.0
        _RainTime ("Rain Time", Range(0.0, 10.0)) = 0.0
        _Size ("Size", Float) = 2.0
        _Aspect ("Aspect Ratio", Float) = 1.77777778
        _TailLength ("Tail Length", Float) = 5.0
        _Center ("Rain Center (UV)", Vector) = (0.5, 0.5, 0, 0)
        _Area ("Rain Area (UV)", Vector) = (0.5, 0.5, 0, 0)
        _RainAngleDeg ("Rain Angle (Deg)", Range(0, 360)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvEffect : TEXCOORD1;
                float4 color : COLOR;
            };

            float _Size, _Aspect, _TailLength, _RainAmount, _RainTime, _RainAngleDeg;
            float _ExternalControl; // 仍然是 float 类型
            float4 _Center;
            float4 _Area;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uvEffect = float2(v.uv.x * _Size * _Aspect, v.uv.y * _Size);
                o.color = v.color;
                return o;
            }

            // --- (S, saw, rand3, rand, rotateUV, staticDrops, dropLayer, drops 函数保持不变) ---
            
            float S(float a, float b, float t)
            {
                t = saturate((t - a) / (b - a));
                return t * t * (3.0 - 2.0 * t);
            }

            float saw(float b, float t)
            {
                return S(0.0, b, t) * S(1.0, b, t);
            }

            float3 rand3(float2 uv)
            {
                return frac(sin(float3(dot(uv, float2(12.9898,78.233)),
                                         dot(uv, float2(39.3468,11.1357)),
                                         dot(uv, float2(73.1561,19.2157)))) * 43758.5453);
            }

            float rand(float x)
            {
                return frac(sin(x) * 43758.5453);
            }

            float2 rotateUV(float2 uv, float angle)
            {
                float cosA = cos(angle);
                float sinA = sin(angle);
                return float2(uv.x * cosA - uv.y * sinA,
                              uv.x * sinA + uv.y * cosA);
            }

            float staticDrops(float2 uv, float globalT)
            {
                float3 n = rand3(floor(uv));
                float d = length(frac(uv) - n.xy);
                float fade = saw(0.5, frac(globalT + n.z));
                float c = S(0.3, 0.0, d) * frac(n.z * 1000.0) * fade;
                return c * _RainAmount;
            }

            float2 dropLayer(float2 uv, float globalT, float angle)
            {
                float2 rotatedUV = rotateUV(uv, angle);
                float2 UV = rotatedUV;
                
                float2 grid = float2(12.0, 2.0);
                float colShift = rand(floor(rotatedUV.x * grid.x));

                float yOffset = frac(globalT + colShift);

                rotatedUV.y += yOffset;

                float2 uvGrid = rotatedUV * grid;
                float3 n = rand3(floor(uvGrid));
                float2 st = frac(uvGrid) - float2(0.5, 0.0);

                float x = n.x - 0.5;
                float wiggle = UV.y * 20.0;
                wiggle = sin(wiggle + sin(wiggle));
                x += wiggle * (0.5 - abs(x)) * (n.y - 0.5);
                x *= 0.7;

                float speed = 0.2 + n.z * 0.1;
                float y = frac(UV.y * speed + globalT);

                float d = length((st - float2(x, y)) * grid.yx);
                float mainDrop = S(0.4, 0.0, d);

                float r = S(1.0, y, st.y);
                float trail = S(0.2, 0.0, abs(st.x - x));
                float trailFront = S(-0.2, 0.2, st.y - y) * _TailLength;
                trail *= trailFront * r;

                y = frac(UV.y * 10.0) + (st.y - 0.5);
                float dd = length(st - float2(x, y));
                float droplets = S(0.3, 0.0, dd);

                float c = (mainDrop + trail * droplets) * _RainAmount;
                return float2(c, trail);
            }

            float2 drops(float2 uv, float globalT, float angle)
            {
                float2 rotatedUVStatic = rotateUV(uv * 40.0, angle);
                float s = staticDrops(rotatedUVStatic, globalT);
                
                float2 m1 = dropLayer(uv * 0.5, globalT, angle);
                float2 m2 = dropLayer(uv, globalT, angle);
                float c = s + m1.x + m2.x;
                c = S(0.0, 1.0, c);
                float trail = m1.y + m2.y;
                return float2(c, trail);
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            fixed4 frag(v2f i) : SV_Target
            {
                float globalT = lerp(_Time.y, _RainTime, _ExternalControl);

                float rainAngleRad = _RainAngleDeg * (UNITY_PI / 180.0);

                float2 diff = i.uv - _Center.xy;
                if(abs(diff.x) > _Area.x * 0.5 || abs(diff.y) > _Area.y * 0.5)
                    return tex2D(_MainTex, i.uv) * i.color;

                float2 uv = i.uvEffect;

                float2 c = drops(uv, globalT, rainAngleRad);
                float2 e = float2(1e-3, 0.0);
                float cx = drops(uv + e, globalT, rainAngleRad).x;
                float cy = drops(uv + e.yx, globalT, rainAngleRad).x;
                float2 n = float2(cx - c.x, cy - c.x);

                float4 col = tex2D(_MainTex, i.uv + n) * i.color;

                return col;
            }
            ENDCG
        }
    }
}