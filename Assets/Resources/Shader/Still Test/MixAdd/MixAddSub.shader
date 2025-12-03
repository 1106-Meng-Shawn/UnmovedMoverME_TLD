Shader "NV/MixAddSub"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Mask ("Mask", 2D) = "white" {}
        _T ("Time", Range(0,1)) = 0.0
        _ColorMul ("Color Multiplier", Color) = (1,1,1,1)
        _ColorAdd ("Color Offset", Vector) = (0,0,0,0)
        _InvertMask ("Invert Mask", Float) = 0.0
        _AlphaFactor ("Alpha Factor", Float) = 1.0
        _UseAdd ("Use Add", Float) = 1.0
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
                float2 uvMask : TEXCOORD1;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _Mask;
            float4 _Mask_ST;
            float4 _ColorMul;
            float4 _ColorAdd;
            float _T;
            float _InvertMask;
            float _AlphaFactor;
            float _UseAdd;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uvMask = TRANSFORM_TEX(v.uv, _Mask);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv) * i.color;

                // ???
                float mask = tex2D(_Mask, i.uvMask).r;
                if (_InvertMask > 0.5) mask = 1.0 - mask;

                // ????
                float4 maskColor = float4(mask, mask, mask, col.a);
                maskColor = maskColor * _ColorMul + _ColorAdd;
                maskColor.a *= _AlphaFactor;

                // ?? _UseAdd ?????
                if (_UseAdd > 0.5)
                    col = saturate(col + _T * maskColor); // Add
                else
                    col = saturate(col - _T * maskColor); // Sub

                return col;
            }
            ENDCG
        }
    }
}
