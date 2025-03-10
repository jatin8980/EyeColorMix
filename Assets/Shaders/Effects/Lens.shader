Shader "Custom/SuckUVsAndStretchWithMask"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" { }
        _SuckAmount ("Suck Amount", Range(-1.0, 1.0)) = 0.0 // Reduced range for more subtle effect
        _SuckRadius ("Suck Radius", Range(0.0, 1.0)) = 0.5 // Controls the radius of the effect (central part)
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
            float _SuckAmount;   // Controls how much the UVs get sucked toward or pushed away from the center
            float _SuckRadius;   // Controls the radius of the effect (central part)

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
                float2 center = float2(0.5, 0.5);  // Center of the texture

                // Calculate distance from the center
                float distanceFromCenter = length(uv - center);

                // Apply effect only if the distance is within the specified radius
                if (distanceFromCenter < _SuckRadius)
                {
                    // Calculate the suck factor, pulling UVs toward the center
                    // We use a softer transition by scaling down the suck factor
                    float suckFactor = 1.0 - (_SuckAmount * (1.0 - distanceFromCenter / _SuckRadius));

                    // Reduce the suck effect to avoid aggressive scaling
                    suckFactor = pow(suckFactor, 2.0); // Squared suck factor for a more gradual effect

                    // Adjust the UVs by sucking them inward based on suckFactor
                    uv = center + (uv - center) * suckFactor;
                }
                else
                {
                    // Stretch the UVs outside of the suck radius to compensate for the sucked-in region
                    float stretchFactor = 1.0 + (_SuckAmount * (distanceFromCenter - _SuckRadius) / (1.0 - _SuckRadius));
                    uv = center + (uv - center) * stretchFactor;
                }

                // Mask the UVs to make sure they don't go out of bounds (clamp UVs to [0, 1])
                uv = clamp(uv, 0.0, 1.0);

                // Sample the base texture with the adjusted UV coordinates
                half4 col = tex2D(_MainTex, uv);

                float mask = step(0.5, distanceFromCenter); // Returns 0 if inside the circle, 1 if outside
                if (mask > 0.0)
                {
                    discard; // Discard any pixels outside the circular region
                }

                // Return the color with the effect applied
                return col;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
