Shader "Effect/RandRoll"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Progress ("Progress", Range(0.0, 1.0)) = 0.5
        _Freq ("Frequency", Float) = 10.0
        _MaxOffset ("Max Offset", Float) = 0.1
        _Smooth ("Smooth Transition", Float) = 0.0 // 0 = false (每帧跳), 1 = true (平滑)
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

            sampler2D _MainTex;
            float _Progress;
            float _Freq;
            float _MaxOffset;
            float _Smooth;

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

            // 简单随机函数
            float2 rand2(float x)
            {
                float sx = frac(sin(x * 12.9898) * 43758.5453);
                float sy = frac(sin((x + 1.0) * 78.233) * 43758.5453);
                return float2(sx, sy);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                float2 offset = float2(0,0);

                if (_Smooth >= 0.5)
                {
                    // 平滑过渡: 使用小幅连续偏移
                    offset = rand2(_Progress * _Freq) * _MaxOffset * _Progress;
                }
                else
                {
                    // 每帧跳变: 偏移只取整数时间块
                    offset = rand2(floor(_Progress * _Freq)) * _MaxOffset * _Progress;
                }

                o.uv = v.uv + offset;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = frac(i.uv);
                float4 col = tex2D(_MainTex, uv) * i.color;
                return col;
            }
            ENDCG
        }
    }
}
