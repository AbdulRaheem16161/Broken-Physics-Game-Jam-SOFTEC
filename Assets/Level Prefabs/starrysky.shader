Shader "Dream/LitStarryFloor"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.02, 0.02, 0.05, 1)
        _StarColor ("Star Color", Color) = (1, 1, 1, 1)

        _StarDensity ("Star Density", Float) = 50
        _StarSize ("Star Size", Float) = 0.02
        _TwinkleSpeed ("Twinkle Speed", Float) = 2.0
        _GlowStrength ("Glow Strength", Float) = 2.0
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
                float3 worldPos : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
            };

            float4 _BaseColor;
            float4 _StarColor;

            float _StarDensity;
            float _StarSize;
            float _TwinkleSpeed;
            float _GlowStrength;

            // Random
            float rand(float2 co)
            {
                return frac(sin(dot(co, float2(12.9898,78.233))) * 43758.5453);
            }

            v2f vert (appdata v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normalDir = UnityObjectToWorldNormal(v.normal);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // ===== LIGHTING =====
                float3 normal = normalize(i.normalDir);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                float NdotL = max(0, dot(normal, lightDir));
                float3 diffuse = _LightColor0.rgb * NdotL;
                float3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

                float3 lighting = diffuse + ambient;

                // ===== STAR FIELD =====
                float2 uv = i.worldPos.xz * _StarDensity;

                float2 grid = floor(uv);
                float2 local = frac(uv) - 0.5;

                float r = rand(grid);

                float star = step(0.98, r);

                float dist = length(local);
                float shape = smoothstep(_StarSize, 0.0, dist);

                float twinkle = sin(_Time.y * _TwinkleSpeed + r * 10.0) * 0.5 + 0.5;

                float intensity = star * shape * twinkle * _GlowStrength;

                // ===== FINAL COLOR =====
                float3 baseLit = _BaseColor.rgb * lighting;

                // Stars ADD on top (important)
                float3 finalColor = baseLit + (_StarColor.rgb * intensity);

                return float4(finalColor, 1.0);
            }

            ENDCG
        }
    }
}