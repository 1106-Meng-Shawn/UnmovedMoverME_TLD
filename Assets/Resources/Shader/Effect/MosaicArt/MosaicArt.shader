Shader "Effect/MosaicArt"
{
    Properties
    {
        _MainTex ("Source Image", 2D) = "white" {}
        
        [Header(Mosaic Settings)]
        _TileSize ("Tile Size", Range(0.005, 0.1)) = 0.02
        _TileVariation ("Tile Size Variation", Range(0, 1)) = 0.3
        _GapSize ("Gap Size", Range(0, 0.5)) = 0.1
        _TileRotation ("Tile Rotation", Range(0, 1)) = 0.2
        
        [Header(Color Settings)]
        _Saturation ("Saturation", Range(0, 2)) = 1.2
        _Brightness ("Brightness", Range(0, 2)) = 1.0
        _ColorVariation ("Color Variation", Range(0, 1)) = 0.15
        
        [Header(Tile Shape)]
        _TileRoundness ("Tile Roundness", Range(0, 1)) = 0.1
        _EdgeSoftness ("Edge Softness", Range(0, 0.5)) = 0.05
        
        [Header(Background)]
        _GroutColor ("Grout Color", Color) = (0.1, 0.1, 0.12, 1)
        
        [Header(Advanced)]
        _Seed ("Random Seed", Float) = 12345
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
            
            float _TileSize;
            float _TileVariation;
            float _GapSize;
            float _TileRotation;
            float _Saturation;
            float _Brightness;
            float _ColorVariation;
            float _TileRoundness;
            float _EdgeSoftness;
            float4 _GroutColor;
            float _Seed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float hash(float2 p)
            {
                float2 seed = float2(123.34, 456.21);
                p = frac(p * (seed + _Seed));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float2 hash2(float2 p)
            {
                float2 seed = float2(123.34, 456.21);
                p = frac(p * (seed + _Seed));
                p += dot(p, p + 45.32);
                return frac(float2(p.x * p.y, p.y * p.x));
            }

            float2 rotate(float2 p, float angle)
            {
                float c = cos(angle);
                float s = sin(angle);
                float2 result;
                result.x = p.x * c - p.y * s;
                result.y = p.x * s + p.y * c;
                return result;
            }

            float3 hsv2rgb(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            float3 rgb2hsv(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float roundedBox(float2 p, float2 boxSize, float radius)
            {
                float2 d = abs(p) - boxSize + radius;
                return min(max(d.x, d.y), 0.0) + length(max(d, 0.0)) - radius;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                float tileSize = _TileSize;
                float2 gridUV = uv / tileSize;
                float2 gridID = floor(gridUV);
                float2 gridLocal = frac(gridUV);
                
                float2 randomVal = hash2(gridID);
                float tileRandom = hash(gridID + 0.5);
                
                float sizeVar = 1.0 + (randomVal.x - 0.5) * _TileVariation;
                float2 tileScale = float2(sizeVar, sizeVar);
                
                float rotation = (randomVal.y - 0.5) * _TileRotation * 3.14159;
                
                float2 localPos = (gridLocal - 0.5) * 2.0; // -1 到 1
                localPos = rotate(localPos, rotation);
                localPos /= tileScale;
                
                float2 boxSize = float2(1.0 - _GapSize, 1.0 - _GapSize);
                float tileShape = roundedBox(localPos, boxSize, _TileRoundness);
                float tileMask = 1.0 - smoothstep(-_EdgeSoftness, _EdgeSoftness, tileShape);
                
                float2 sampleUV = (gridID + 0.5) * tileSize;
                fixed4 baseColor = tex2D(_MainTex, sampleUV);
                
                float3 hsv = rgb2hsv(baseColor.rgb);
                hsv.y *= _Saturation; // 饱和度
                hsv.z *= _Brightness; // 亮度
                
                hsv.x += (tileRandom - 0.5) * _ColorVariation * 0.1;
                hsv.y += (hash(gridID + 1.0) - 0.5) * _ColorVariation * 0.2;
                hsv.z += (hash(gridID + 2.0) - 0.5) * _ColorVariation * 0.3;
                
                hsv.y = saturate(hsv.y);
                hsv.z = saturate(hsv.z);
                
                float3 tileColor = hsv2rgb(hsv);
                
                float highlight = pow(1.0 - length(localPos) * 0.5, 2.0) * 0.2;
                tileColor += highlight;
                
                float3 finalColor = lerp(_GroutColor.rgb, tileColor, tileMask);
                
                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}