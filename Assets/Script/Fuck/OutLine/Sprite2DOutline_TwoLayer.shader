Shader "Hidden/Sprite2DOutline_TwoLayer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outer Outline Color", Color) = (1, 0, 0, 1)
        _OutlineWidth ("Outline Width", Float) = 1.0
        _InnerOutlineWidth ("Inner Outline Width", Float) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        // ========================================
        // Pass 0: Generate Outline Alpha
        // ========================================
        Pass
        {
            Name "Dilate"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _OutlineWidth;

            v2f vert(appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float maxAlpha = 0;
                float2 texelSize = _MainTex_TexelSize.xy * _OutlineWidth;

                for (int x=-1; x<=1; x++)
                    for (int y=-1; y<=1; y++)
                        maxAlpha = max(maxAlpha, tex2D(_MainTex, i.uv + float2(x,y)*texelSize).a);

                return fixed4(0,0,0,maxAlpha);
            }
            ENDCG
        }

        // ========================================
        // Pass 1: Outer Outline
        // ========================================
        Pass
        {
            Name "OuterOutline"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };
            sampler2D _MainTex;
            float4 _OutlineColor;
            float _OutlineWidth;
            float4 _MainTex_TexelSize;

            v2f vert(appdata_img v)
            {
                v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.uv = v.texcoord; return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float outlineAlpha = 0;
                float2 texelSize = _MainTex_TexelSize.xy * _OutlineWidth;

                for (int x=-1; x<=1; x++)
                    for (int y=-1; y<=1; y++)
                        outlineAlpha = max(outlineAlpha, tex2D(_MainTex, i.uv + float2(x,y)*texelSize).a);

                float spriteAlpha = tex2D(_MainTex, i.uv).a;
                float mask = saturate(outlineAlpha - spriteAlpha); // only outside sprite

                return fixed4(_OutlineColor.rgb, mask * _OutlineColor.a);
            }
            ENDCG
        }

        // ========================================
        // Pass 2: Inner Outline (White)
        // ========================================
        Pass
        {
            Name "InnerOutline"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };
            sampler2D _MainTex;
            float _InnerOutlineWidth;
            float4 _MainTex_TexelSize;

            v2f vert(appdata_img v)
            {
                v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.uv = v.texcoord; return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float outlineAlpha = 0;
                float2 texelSize = _MainTex_TexelSize.xy * _InnerOutlineWidth;

                for (int x=-1; x<=1; x++)
                    for (int y=-1; y<=1; y++)
                        outlineAlpha = max(outlineAlpha, tex2D(_MainTex, i.uv + float2(x,y)*texelSize).a);

                float spriteAlpha = tex2D(_MainTex, i.uv).a;
                float mask = saturate(outlineAlpha - spriteAlpha);

                return fixed4(1,1,1, mask); // inner outline always white
            }
            ENDCG
        }

        // ========================================
        // Pass 3: Original Sprite
        // ========================================
        Pass
        {
            Name "Sprite"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };
            sampler2D _MainTex;

            v2f vert(appdata_img v)
            {
                v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.uv = v.texcoord; return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv); // original sprite color
            }
            ENDCG
        }
    }

    FallBack Off
}
