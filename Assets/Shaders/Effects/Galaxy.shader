Shader "Custom/ZoomAndRotateUVsWithMask"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" { }
        
        // Zoom Effect Properties
        _SuckAmount ("Suck Amount", Range(-1.0, 1.0)) = 0.0 // Controls the strength of the suck/stretch effect
        _SuckRadius ("Suck Radius", Range(0.0, 1.0)) = 0.5 // Controls the radius for zoom effect
        
        // UV Rotation Effect Properties
        _Center ("Center", Vector) = (0.5, 0.5, 0.0, 0.0) // Center of the rotation
        _RotationRadius ("Rotation Radius", Range(0.0, 1.0)) = 0.5 // Radius for rotation transition
        _MaxRotation ("Max Rotation", Range(0.0, 360.0)) = 90.0 // Maximum rotation in degrees
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

            // Properties for Zoom Effect
            sampler2D _MainTex;
            float _SuckAmount;   // Strength of suck/stretch effect
            float _SuckRadius;   // Radius of the effect for zoom

            // Properties for UV Rotation Effect
            float4 _Center;  // Center of the UV rotation
            float _RotationRadius; // Radius for rotation transition
            float _MaxRotation;  // Maximum rotation angle in degrees

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

            // Function to rotate UVs
            float2 RotateUV(float2 uv, float angle)
            {
                // Convert angle to radians
                float rad = angle * 3.14159265359 / 180.0;
                // Rotation matrix for 2D rotation
                float2x2 rotationMatrix = float2x2(cos(rad), -sin(rad), sin(rad), cos(rad));
                return mul(uv - _Center.xy, rotationMatrix) + _Center.xy;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Get the texture coordinates (UV)
                float2 uv = i.uv;
                float2 center = float2(0.5, 0.5);  // Center of the texture

                // Apply the zoom effect first (suck/stretch)
                float distanceFromCenter = length(uv - center);
                if (distanceFromCenter < _SuckRadius)
                {
                    // Calculate the suck factor, pulling UVs inward
                    float suckFactor = 1.0 - (_SuckAmount * (1.0 - distanceFromCenter / _SuckRadius));
                    suckFactor = pow(suckFactor, 2.0); // Make the effect more gradual
                    uv = center + (uv - center) * suckFactor;
                }
                else
                {
                    // Stretch the UVs outside of the suck radius
                    float stretchFactor = 1.0 + (_SuckAmount * (distanceFromCenter - _SuckRadius) / (1.0 - _SuckRadius));
                    uv = center + (uv - center) * stretchFactor;
                }

                // Apply the UV rotation effect
                float dist = distance(uv, _Center.xy);
                float angle;

                // If inside the rotation radius, rotate counterclockwise
                if (dist < _RotationRadius)
                {
                    angle = lerp(0.0, _MaxRotation, 1.0 - dist / _RotationRadius);
                }
                else
                {
                    // If outside the radius, rotate clockwise
                    angle = lerp(0.0, -_MaxRotation, (dist - _RotationRadius) / (1.0 - _RotationRadius));
                }

                // Rotate the UV coordinates
                uv = RotateUV(uv, angle);

                // Clamp UVs to ensure they stay within bounds
                uv = clamp(uv, 0.0, 1.0);

                // Mask to ensure UVs stay within the circular region
                float mask = step(0.5, distanceFromCenter); // Returns 0 if inside the circle, 1 if outside
                if (mask > 0.0)
                {
                    discard; // Discard any pixels outside the circular region
                }

                // Sample the texture with the modified UVs
                half4 col = tex2D(_MainTex, uv);

                return col;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
