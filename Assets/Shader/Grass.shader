Shader "Game/Grass"
{
     Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RootWorldPos ("Root World Pos", Vector) = (0,0,0,0)
        _SwayStrength ("Sway Strength", Float) = 0.05
        _SwaySpeed ("Sway Speed", Float) = 2.0
        _NoiseScale ("Noise Scale", Float) = 3.0
        _SwayRange ("Sway Range (Height)", Float) = 1
        _ManualSwayOffset ("Manual Sway Offset", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            //float4 _RootWorldPos;
            float _SwayStrength;
            float _RepelStrength;
            float _SwaySpeed;
            float4 _MousePos;
            float _NoiseScale;
            float _SwayRange;
            float4 _ManualSwayOffset;

              // --- GPU Instancing support ---
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _RootWorldPos)
            UNITY_INSTANCING_BUFFER_END(Props)


            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;

                float time = _SinTime.x;
                float2 rootWorld = UNITY_ACCESS_INSTANCED_PROP(Props, _RootWorldPos).xy;
                //float2 rootWorld = _RootWorldPos.xy;

                // 噪声自然摇摆
                float noise = hash21(rootWorld * _NoiseScale);
                float swayAngle = sin(time * _SwaySpeed + noise * 6.2831) * _SwayStrength;

                float manualAngle = _ManualSwayOffset.x;
                float localHeight = v.vertex.y;
                float weight = saturate(localHeight / _SwayRange);

                float totalAngle = (swayAngle + manualAngle) * weight;

                float cosA = cos(totalAngle);
                float sinA = sin(totalAngle);

                float2 rotated;
                rotated.x = v.vertex.x * cosA - v.vertex.y * sinA;
                rotated.y = v.vertex.x * sinA + v.vertex.y * cosA;

                v.vertex.xy = rotated;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
