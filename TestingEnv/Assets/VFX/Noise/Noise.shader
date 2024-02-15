Shader "Custom/MyUVColorShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        

        // Added the mandatory pass statement
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            float2 gradient(float2 p)
            {
                float2 gradient1 = normalize(float2(1.0, 1.0));
                float2 gradient2 = normalize(float2(-1.0, -1.0));
                float2 cell = floor(p);
                float2 uv = frac(p);
                float2 uv00 = uv - (0.0, 0.0);
                float2 uv10 = uv - (1.0, 0.0);
                float2 uv01 = uv - (0.0, 1.0);
                float v00 = dot(uv00, gradient1);
                float v10 = dot(uv10, gradient1);
                float v01 = dot(uv00, gradient2);
                float v11 = dot(uv10, gradient2);
                float sx = smoothstep(0.0, 1.0, uv.x);
                float sy = smoothstep(0.0, 1.0, uv.y);
                return lerp(lerp(v00, v10, sx), lerp(v01, v11, sx), sy);
            }

            float randomFloat(float2 uv)
            {
                float seed = dot(uv, float2(12.345, 23.456));
                float2 noiseInput = uv * 10.0 + seed; // Adjust multiplier for frequency
                float2 gradientValue = gradient(noiseInput);
                return frac(sin(dot(gradientValue, float2(17.2937, 31.2937))) * 43989.0);
            }
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // Optional: transform UVs if needed
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                // Combine UV color with texture (optional)
                // return fixed4(i.uv.x, i.uv.y, 0.0, 1.0); // Use UV coordinates directly
                float av = 0;
                for(int it = 0; it < 1000; it++){
                    av += randomFloat(i.uv.xy + it/10);
                }
                av/= 1000;
                return av; // Combine with texture
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
