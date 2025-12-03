Shader "NV/Overlay"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _T ("Time", Range(0,1)) = 0.0
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

            sampler2D _MainTex;
            float _T; // ??C#???

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
            #ifdef UNITY_UV_STARTS_AT_TOP
                o.uv.y = 1.0 - o.uv.y;
            #endif
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // ??????
                fixed4 col = tex2D(_MainTex, i.uv);

                // ?????????????
                col.rgb *= (0.5 + 0.5 * sin(_T * 6.2831)); // 0~1?????

                return col;
            }
            ENDCG
        }
    }
}
