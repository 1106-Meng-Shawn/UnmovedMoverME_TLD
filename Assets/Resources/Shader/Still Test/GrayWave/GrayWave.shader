Shader "NV/GrayWave"
{
    Properties
    {
        _MainTex ("Current Texture", 2D) = "white" {}
        _SecondTex ("Next Texture", 2D) = "black" {}
        _T ("Transition Progress", Range(0,1)) = 0.0
        _Freq ("Wave Frequency", Float) = 5.0
        _Amplitude ("Wave Amplitude", Range(0,1)) = 0.3
        _Direction ("Wave Direction", Vector) = (1,0,0,0)
        _MaskColor ("Mask Color", Color) = (0,0,0,1)
        _MaskPeak ("Mask Peak Position", Range(0,1)) = 0.5  // ???????
        _MaskWidth ("Mask Width", Range(0.1,1)) = 0.4       // ??????
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZTest Always
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
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            
            sampler2D _MainTex;
            sampler2D _SecondTex;
            float _T;
            float _Freq;
            float _Amplitude;
            float2 _Direction;
            float4 _MaskColor;
            float _MaskPeak;
            float _MaskWidth;
            
            fixed4 frag(v2f i) : SV_Target
            {
                float4 col1 = tex2D(_MainTex, i.uv);
                float4 col2 = tex2D(_SecondTex, i.uv);
                
                // ?????
                float2 dir = normalize(_Direction.xy);
                
                // ???????UV?????
                float phase = dot(i.uv - 0.5, dir) * _Freq * 6.2831853 + _T * 3.14159;
                float wave = sin(phase);
                
                // ??????0~1
                float waveFactor = wave * 0.5 + 0.5;
                
                // ??????
                waveFactor = lerp(1.0, waveFactor, _Amplitude);
                
                // ????????_MaskPeak????
                // ?????????
                float distFromPeak = abs(_T - _MaskPeak) / (_MaskWidth * 0.5);
                float maskStrength = 1.0 - smoothstep(0.0, 1.0, distFromPeak);
                
                // ????????
                maskStrength *= waveFactor;
                
                // ???????????
                // T < 0.5: ???????
                // T > 0.5: ???????
                float imageMix = smoothstep(0.3, 0.7, _T);
                float4 baseColor = lerp(col1, col2, imageMix);
                
                // ????
                float4 result = lerp(baseColor, _MaskColor, maskStrength);
                
                return result * i.color;
            }
            ENDCG
        }
    }
}