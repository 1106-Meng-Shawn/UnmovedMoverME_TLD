Shader "Hidden/Sprite2DOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        // ????
        _InnerOutlineColor ("Inner Outline Color", Color) = (1, 1, 0, 1)
        _InnerOutlineWidth ("Inner Outline Width", Range(0, 10)) = 1.0
        
        // ????
        _OuterOutlineColor ("Outer Outline Color", Color) = (1, 0, 0, 1)
        _OuterOutlineWidth ("Outer Outline Width", Range(0, 10)) = 2.0
        
        // ?????????=???????
        _SampleQuality ("Sample Quality", Range(1, 5)) = 3
        
        // SpriteRenderer?????????
        _Color ("Tint Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            Name "DoubleOutlineSmooth"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            
            float _InnerOutlineWidth;
            float4 _InnerOutlineColor;
            
            float _OuterOutlineWidth;
            float4 _OuterOutlineColor;
            
            float _SampleQuality;
            
            fixed4 _Color;
            
            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color * _Color;
                return o;
            }
            
            // ??????? - ????????
            float GetDilatedAlpha(float2 uv, float radius)
            {
                if (radius < 0.01) return tex2D(_MainTex, uv).a;
                
                float maxAlpha = 0;
                float2 texelSize = _MainTex_TexelSize.xy;
                
                // ????????
                int sampleRange = max(1, (int)ceil(radius * _SampleQuality));
                float radiusSq = radius * radius;
                
                // ???????????????
                for (int x = -sampleRange; x <= sampleRange; x++)
                {
                    for (int y = -sampleRange; y <= sampleRange; y++)
                    {
                        // ??????????
                        float distSq = x * x + y * y;
                        if (distSq <= radiusSq * _SampleQuality * _SampleQuality)
                        {
                            float2 offset = float2(x, y) * texelSize;
                            float alpha = tex2D(_MainTex, uv + offset).a;
                            maxAlpha = max(maxAlpha, alpha);
                        }
                    }
                }
                
                return maxAlpha;
            }
            
            // ??????
            float smoothEdge(float value)
            {
                return smoothstep(0.0, 1.0, value);
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                // 1. ???????sprite??
                fixed4 spriteCol = tex2D(_MainTex, i.uv);
                spriteCol *= i.color;
                
                // 2. ???????alpha
                float currentAlpha = spriteCol.a;
                
                // ??sprite?????0?
                float baseAlpha = currentAlpha;
                
                // ?????????????sprite????
                float innerRadius = _InnerOutlineWidth;
                float innerDilate = GetDilatedAlpha(i.uv, innerRadius);
                
                // ?????????????sprite????
                float outerRadius = _InnerOutlineWidth + _OuterOutlineWidth;
                float outerDilate = GetDilatedAlpha(i.uv, outerRadius);
                
                // 3. ????????????????
                // ????????????????????
                float outerMask = smoothEdge(saturate(outerDilate - innerDilate));
                
                // ???????????????sprite?
                float innerMask = smoothEdge(saturate(innerDilate - baseAlpha));
                
                // 4. ????????
                fixed4 finalCol = fixed4(0, 0, 0, 0);
                
                // ????????
                if (outerMask > 0.01)
                {
                    finalCol = _OuterOutlineColor;
                    finalCol.a *= outerMask;
                }
                
                // ????????
                if (innerMask > 0.01)
                {
                    float innerAlpha = innerMask * _InnerOutlineColor.a;
                    finalCol.rgb = lerp(finalCol.rgb, _InnerOutlineColor.rgb, innerAlpha / max(finalCol.a + innerAlpha, 0.001));
                    finalCol.a = max(finalCol.a, innerAlpha);
                }
                
                // ????sprite??
                if (currentAlpha > 0.01)
                {
                    finalCol.rgb = lerp(finalCol.rgb, spriteCol.rgb, currentAlpha / max(finalCol.a + currentAlpha, 0.001));
                    finalCol.a = max(finalCol.a, currentAlpha);
                }
                
                return finalCol;
            }
            ENDCG
        }
    }
    FallBack Off
}