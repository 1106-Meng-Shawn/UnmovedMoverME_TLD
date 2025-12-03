Shader "Hidden/Sprite2DOutlineOld"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 1, 0, 1)
        _OutlineWidth ("Outline Width", Float) = 1.0
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
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
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
                // 3x3 kernel to dilate alpha
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        float2 offset = float2(x, y) * texelSize;
                        float alpha = tex2D(_MainTex, i.uv + offset).a;
                        maxAlpha = max(maxAlpha, alpha);
                    }
                }
                return fixed4(0, 0, 0, maxAlpha); // ?? alpha mask (????)
            }
            ENDCG
        }
        // ========================================
        // Pass 1: Composite Original Sprite + Outline
        // ========================================
        Pass
        {
            Name "Composite"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            sampler2D _MainTex;
            float _OutlineWidth;
            float4 _OutlineColor;
            float4 _MainTex_TexelSize;
            v2f vert(appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                // Original Sprite color
                fixed4 col = tex2D(_MainTex, i.uv);
                // Compute outline alpha
                float outlineAlpha = 0;
                float2 texelSize = _MainTex_TexelSize.xy * _OutlineWidth;
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        float2 offset = float2(x, y) * texelSize;
                        float sampleAlpha = tex2D(_MainTex, i.uv + offset).a;
                        outlineAlpha = max(outlineAlpha, sampleAlpha);
                    }
                }
                // Outline only outside the sprite (dilate alpha - sprite alpha)
                float mask = saturate(outlineAlpha - col.a);
                // Blend outline color with original color
                col.rgb = lerp(col.rgb, _OutlineColor.rgb, mask * _OutlineColor.a);
                return col;
            }
            ENDCG
        }
    }
    FallBack Off
}