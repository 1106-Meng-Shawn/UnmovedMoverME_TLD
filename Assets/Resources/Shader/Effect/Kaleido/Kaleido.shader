Shader "Effect/Kaleido"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Blend ("Blend", Range(0,1)) = 0.5         // <—— ?????
        _Repeat ("Repeat", Float) = 8.0
        _Speed ("Rotation Speed", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
                float4 color : COLOR;
                float2 cosSinTime : TEXCOORD1;
            };

            sampler2D _MainTex;
            float _Repeat;
            float _Blend;        // <—— rename done
            float _Speed;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;

                float angle = _Time.y * _Speed;
                o.cosSinTime = float2(cos(angle), sin(angle));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);

                // uv ??? [-0.5,0.5]
                float2 uv = i.uv - 0.5;

                // Kaleido ??
                for (int repeat = 0; repeat < (int)_Repeat; ++repeat)
                {
                    uv = abs(uv) - 0.25;
                    uv *= sign(uv + uv.yx);
                    uv = i.cosSinTime.x * uv + i.cosSinTime.y * uv.yx * float2(1.0, -1.0);
                }

                uv += 0.5;
                float4 col2 = tex2D(_MainTex, uv);

                // ?? ? Kaleido
                col = lerp(col, col2, _Blend) * i.color;

                return col;
            }
            ENDCG
        }
    }
}
