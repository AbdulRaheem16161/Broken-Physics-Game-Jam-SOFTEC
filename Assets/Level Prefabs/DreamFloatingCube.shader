Shader "Dream/LitFloatingCube"
{
    Properties
    {
        _Color ("Color", Color) = (0.6, 0.8, 1, 1)

        _FloatStrength ("Float Strength", Float) = 0.2
        _FloatSpeed ("Float Speed", Float) = 1.0
        _WobbleStrength ("Wobble Strength", Float) = 0.05
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normalDir : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            float4 _Color;
            float _FloatStrength;
            float _FloatSpeed;
            float _WobbleStrength;

            v2f vert (appdata v)
            {
                v2f o;

                float3 pos = v.vertex.xyz;

                float time = _Time.y * _FloatSpeed;

                // Floating
                pos.y += sin(time + pos.x + pos.z) * _FloatStrength;

                // Wobble
                pos.x += sin(time + pos.y) * _WobbleStrength;
                pos.z += cos(time + pos.x) * _WobbleStrength;

                o.pos = UnityObjectToClipPos(float4(pos, 1.0));

                // World space calculations
                o.worldPos = mul(unity_ObjectToWorld, float4(pos,1)).xyz;
                o.normalDir = UnityObjectToWorldNormal(v.normal);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Normalize vectors
                float3 normal = normalize(i.normalDir);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                // Diffuse lighting (Lambert)
                float NdotL = max(0, dot(normal, lightDir));

                float3 diffuse = _LightColor0.rgb * NdotL;

                // Ambient light
                float3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

                float3 finalColor = (_Color.rgb) * (diffuse + ambient);

                return float4(finalColor, 1.0);
            }

            ENDCG
        }
    }
}