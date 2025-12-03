Shader "Transition/VenetianBlinds"
{
    Properties
    {
        _MainTex ("From Texture", 2D) = "white" {}
        _ToTex ("To Texture", 2D) = "white" {}
        _Progress ("Progress", Range(0, 1)) = 0
        _BlindsCount ("Blinds Count", Range(2, 50)) = 10
        _Direction ("Direction", Float) = 0  // 0=??, 1=??
        _Smoothness ("Smoothness", Range(0, 0.2)) = 0.05
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _ToTex;
            float4 _ToTex_ST;
            float _Progress;
            float _BlindsCount;
            float _Direction;
            float _Smoothness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // ??????
                fixed4 fromColor = tex2D(_MainTex, i.uv);
                fixed4 toColor = tex2D(_ToTex, i.uv);
                
                // ????????
                float coord = (_Direction < 0.5) ? i.uv.y : i.uv.x;
                
                // ???????????
                float blindPosition = frac(coord * _BlindsCount);
                
                // ??smoothstep??????????
                float edge1 = _Progress - _Smoothness;
                float edge2 = _Progress + _Smoothness;
                float blend = smoothstep(edge1, edge2, blindPosition);
                
                // ??????
                fixed4 finalColor = lerp(fromColor, toColor, blend);
                
                return finalColor;
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}