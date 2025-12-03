Shader "Camera/ZoomAndPan"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Zoom ("Zoom", Range(1.0, 10.0)) = 1.0
        _Pan ("Pan", Vector) = (0.5, 0.5, 0, 0)
        [ToggleUI]_ClampToBounds("Clamp To Bounds", Float) = 1
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
            float4 _MainTex_ST;
            float _Zoom;
            float2 _Pan;
            float _ClampToBounds;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            // 将 UV 坐标系的 Pan (0~1) 转换为中心偏移量 (-0.5~0.5)
            float2 PanToOffset(float2 pan)
            {
                return pan - 0.5;
            }
            
            // 限制 Pan 在有效范围内
            float2 ClampPan(float2 pan, float zoom)
            {
                float2 offset = PanToOffset(pan);
                float visibleSize = 1.0 / zoom;
                float maxOffset = (1.0 - visibleSize) * 0.5;
                
                offset.x = clamp(offset.x, -maxOffset, maxOffset);
                offset.y = clamp(offset.y, -maxOffset, maxOffset);
                
                return offset + 0.5;
            }
            
            // 应用缩放和平移
            float2 ApplyZoomPan(float2 uv, float zoom, float2 pan)
            {
                float2 offset = PanToOffset(pan);
                
                // 1. UV 移至中心 (0,0)
                uv -= 0.5;
                
                // 2. 应用缩放
                uv /= zoom;
                
                // 3. 应用平移（除以 zoom 使其在原始图像空间中一致）
                uv += offset / zoom;
                
                // 4. 恢复 UV
                uv += 0.5;
                
                return uv;
            }
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                float zoom = max(_Zoom, 1.0);
                float2 pan = _Pan;
                
                if (_ClampToBounds > 0.5)
                {
                    pan = ClampPan(pan, zoom);
                }
                
                float2 uv = ApplyZoomPan(i.uv, zoom, pan);
                
                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}