Shader "Custom/FadeSlideBlend"
{
    Properties
    {
        [HideInInspector] _MainTex ("Main Texture", 2D) = "white" {}
        [NoScaleOffset] _SubTex ("Next Texture", 2D) = "black" {}
        _SubColor ("Next Texture Tint", Color) = (1,1,1,1)
        _T ("Transition Progress", Range(0.0,1.0)) = 0.0
        _Vague ("Edge Softness", Range(0.0,0.5)) = 0.1
        _Mode ("Transition Mode (0=Fade,1=LeftRight,2=UpDown,3=Mask)", Float) = 1
        [NoScaleOffset] _Mask ("Transition Mask (optional)", 2D) = "white" {}
        _InvertMask ("Invert Mask", Float) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex, _SubTex, _Mask;
            float4 _SubColor;
            float _T, _Vague, _InvertMask, _Mode;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // ???????
                float4 mainCol = tex2D(_MainTex, i.uv);
                float4 nextCol = tex2D(_SubTex, i.uv) * _SubColor;

                // ??????? mask ?
                float t0;

                // ????
                if (_Mode < 0.5) // ?? 0???????
                {
                    t0 = _T;
                }
                else if (_Mode < 1.5) // ?? 1???????
                {
                    t0 = i.uv.x;
                    if (_InvertMask > 0.5) t0 = 1 - t0;
                }
                else if (_Mode < 2.5) // ?? 2???????
                {
                    t0 = i.uv.y;
                    if (_InvertMask > 0.5) t0 = 1 - t0;
                }
                else // ?? 3????? Mask ??
                {
                    t0 = tex2D(_Mask, i.uv).r;
                    if (_InvertMask > 0.5) t0 = 1 - t0;
                }

                // ????
                float slope = 0.5 / (_Vague + 0.001);
                float mask = smoothstep(0.0, 1.0, 0.5 + slope * (_T - t0));

                // ?????????
                return lerp(mainCol, nextCol, mask);
            }
            ENDCG
        }
    }
}
