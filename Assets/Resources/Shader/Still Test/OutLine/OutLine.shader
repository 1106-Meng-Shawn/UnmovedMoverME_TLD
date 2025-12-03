Shader "Custom/UIOutlineDual8Dir"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Main Tint Color", Color) = (1,1,1,1)
        _InnerOutlineColor("Inner Outline Color", Color) = (0,0,0,1)
        _OuterOutlineColor("Outer Outline Color", Color) = (1,1,1,1)
        _InnerSize("Inner Outline Size", Range(0,10)) = 1
        _OuterSize("Outer Outline Size", Range(0,10)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
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
            float4 _Color;
            float4 _InnerOutlineColor;
            float4 _OuterOutlineColor;
            float _InnerSize;
            float _OuterSize;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float alpha = tex2D(_MainTex, i.uv).a;

                float2 offsets[8] = {
                    float2(1,0), float2(-1,0), float2(0,1), float2(0,-1),
                    float2(1,1), float2(-1,1), float2(1,-1), float2(-1,-1)
                };

                // ???
                float outer = 0;
                for(int k=0;k<8;k++)
                {
                    float2 off = offsets[k] * (_OuterSize / 10.0);
                    outer = max(outer, tex2D(_MainTex, i.uv + off).a);
                }
                outer = saturate(outer - alpha);

                // ???
                float inner = 0;
                for(int k=0;k<8;k++)
                {
                    float2 off = offsets[k] * (_InnerSize / 10.0);
                    inner = max(inner, alpha - tex2D(_MainTex, i.uv + off).a);
                }
                inner = saturate(inner);

                fixed4 mainCol = tex2D(_MainTex, i.uv) * _Color;

                // ????
                float3 col = mainCol.rgb;
                col = lerp(col, _OuterOutlineColor.rgb, outer);
                col = lerp(col, _InnerOutlineColor.rgb, inner);

                float finalAlpha = saturate(mainCol.a + outer + inner);

                return fixed4(col, finalAlpha);
            }
            ENDCG
        }
    }
}

