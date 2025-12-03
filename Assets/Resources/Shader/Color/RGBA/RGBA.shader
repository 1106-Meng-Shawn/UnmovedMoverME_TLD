Shader "Color/RGBA"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _BaseColor("Base Color", Color) = (0,0,0,1)
        [Enum(Red,1, Green,2, Blue,3, Alpha,4)]
        _RgbIndex("Color Mode", Float) = 1
        _MainTex("MainTex", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _RgbIndex;
            fixed4 _Color;
            fixed4 _BaseColor;
            sampler2D _MainTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color;

                if (_RgbIndex == 1)       // Red: (0,G,B) -> (1,G,B)
                {
                    color = fixed4(i.uv.x, _BaseColor.g, _BaseColor.b, 1);
                }
                else if (_RgbIndex == 2)  // Green: (R,0,B) -> (R,1,B)
                {
                    color = fixed4(_BaseColor.r, i.uv.x, _BaseColor.b, 1);
                }
                else if (_RgbIndex == 3)  // Blue: (R,G,0) -> (R,G,1)
                {
                    color = fixed4(_BaseColor.r, _BaseColor.g, i.uv.x, 1);
                }
                else if (_RgbIndex == 4)  // Alpha (with texture)
                {
                    // 从透明背景到完全不透明的当前颜色
                    fixed4 bgColor = tex2D(_MainTex, i.uv);
                    fixed4 fgColor = _BaseColor;
                    fgColor.a = i.uv.x;
                    
                    // Alpha 混合
                    color = lerp(bgColor, fgColor, i.uv.x);
                }
                else
                {
                    color = fixed4(1, 1, 1, 1);
                }

                return color;
            }
            ENDCG
        }
    }
}
