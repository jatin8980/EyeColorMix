Shader "Custom/CenterUVExpandAndOutsideCompressWithMask"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" { }
        _MoveAmount ("Move Amount", Range(-2.0, 1.0)) = 0.5 // Move range from -2 to 1
        _MoveRadius ("Move Radius", Range(0.0, 1.0)) = 0.2 // Controls the radius of the effect (central 20%)
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

            // Properties
            sampler2D _MainTex;
            float _MoveAmount;  // Controls how far the center UV moves (range -2 to 1)
            float _MoveRadius;  // Controls the radius of the effect

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Get the texture coordinates (UV)
                float2 uv = i.uv;
                float2 center = float2(0.5, 0.5);  // Center of the texture (eye's center)

                // Calculate distance from the center
                float distanceFromCenter = length(uv - center);

                // Apply effect only to the central part of the texture (central 20%)
                if (distanceFromCenter < _MoveRadius)
                {
                    // Apply radial movement for the center region
                    uv = center + (uv - center) * (1 + _MoveAmount * (1 - distanceFromCenter / _MoveRadius));
                }
                else if (distanceFromCenter < 1.0)
                {
                    // For the outside region (outside the central 20%)
                    // Compress the UVs outward based on the distance from the center
                    float compressionFactor = 1.0 - (distanceFromCenter - _MoveRadius) * (1 - _MoveAmount) / (1 - _MoveRadius);
                    uv = center + (uv - center) * compressionFactor;
                }

                // Clamp UVs to the [0, 1] range to prevent them from going out of bounds
                uv = clamp(uv, 0.0, 1.0);

                // Apply circular mask to ensure the texture doesn't show outside its bounds
                // The mask will discard pixels outside the circle (radius 0.5)
                float mask = step(0.5, distanceFromCenter); // Returns 0 if inside the circle, 1 if outside
                if (mask > 0.0)
                {
                    discard; // Discard any pixels outside the circular region
                }

                // Sample the base texture with the adjusted UV coordinates
                half4 col = tex2D(_MainTex, uv);

                // Return the color with the effect applied
                return col;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
