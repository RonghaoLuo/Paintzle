Shader "Paintzle/PaintableNoise"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _Color ("Base Color", Color) = (1,1,1,1)
        
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 1
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
            #include "PaintableOverlay.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL; // 1. Added Normal input
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 normal : TEXCOORD3; // 2. Pass Normal to fragment
            };

            sampler2D _MainTex;
            float4 _Color;

            sampler2D _NoiseTex;
            float _NoiseScale;

            float _PaintEnabled;
            float4 _PaintColor;
            float _PaintCoverage;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // Calculate world position
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                // Calculate world normal
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv = v.uv;
                return o;
            }

            // Helper function for Triplanar mapping
            float GetTriplanarNoise(float3 worldPos, float3 worldNormal, float scale)
            {
                // Calculate blending weights based on how much the surface faces X, Y, or Z
                float3 blend = abs(worldNormal);
                // Tighten the blend slightly to reduce overlapping blur (optional)
                blend = pow(blend, 4); 
                blend /= (blend.x + blend.y + blend.z); // Normalize so they add up to 1

                // Project from X axis (Side) -> Use YZ coordinates
                float noiseX = tex2D(_NoiseTex, worldPos.yz * scale).r;
                
                // Project from Y axis (Top) -> Use XZ coordinates (This was your original code)
                float noiseY = tex2D(_NoiseTex, worldPos.xz * scale).r;
                
                // Project from Z axis (Front) -> Use XY coordinates
                float noiseZ = tex2D(_NoiseTex, worldPos.xy * scale).r;

                // Blend the three samples
                return noiseX * blend.x + noiseY * blend.y + noiseZ * blend.z;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 baseColor = tex2D(_MainTex, i.uv) * _Color;

                // 3. Use Triplanar Noise instead of just .xz
                float noise = GetTriplanarNoise(i.worldPos, i.normal, _NoiseScale);

                float3 painted = ApplyPaintOverlayColor(
                    baseColor.rgb,
                    _PaintColor.rgb,
                    noise,
                    _PaintCoverage,
                    _PaintEnabled
                );

                return float4(painted, baseColor.a);
            }

            ENDCG
        }
    }
}