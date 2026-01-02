Shader "UI/RainbowFill"
{
    Properties
    {
        [HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}
        _RainbowTex ("Rainbow Texture (Flow)", 2D) = "white" {}
        _Speed ("Flow Speed", Float) = 0.5
        
        // Required for UI Masking
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        
        Stencil { Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] }
        Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode] Blend SrcAlpha OneMinusSrcAlpha ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            sampler2D _MainTex; // This is your "Source Image" (the mask)
            sampler2D _RainbowTex; // This is your scrolling rainbow
            float _Speed;

            v2f vert (appdata v) {
                v2f o;
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(o.worldPosition);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 1. Get the Alpha from the UI Image (Source Image slot)
                // This includes the "Fill Amount" transparency!
                fixed4 mask = tex2D(_MainTex, i.uv);
                
                // 2. Scroll the Rainbow UVs
                float2 rainbowUV = i.uv;
                rainbowUV.x -= _Time.y * _Speed;
                fixed4 rainbowCol = tex2D(_RainbowTex, rainbowUV);

                // 3. Combine: Rainbow Color * Mask Alpha * Vertex Color
                fixed4 finalCol = rainbowCol;
                finalCol.a = mask.a * i.color.a; 
                
                // Clip pixels if alpha is extremely low to ensure transparency
                clip(finalCol.a - 0.001);

                return finalCol * finalCol.a;
            }
            ENDCG
        }
    }
}