Shader "Custom/SuckAndTwirlUVsWithMask"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" { }
        _SuckAmount ("Suck Amount", Range(-1.0, 1.0)) = 0.0 // Amount of suck (negative pulls UVs inward, positive pushes them outward)
        _SuckRadius ("Suck Radius", Range(0.0, 1.0)) = 0.5 // Controls the radius of the effect
        _MaxRotation ("Max Rotation", Range(0.0, 360.0)) = 90.0 // Max rotation angle at the outer edges
        _MaskRadius ("Mask Radius", Range(0.0, 1.0)) = 0.5 // Controls the radius for masking outside which UVs will be discarded
        _Center ("Center", Vector) = (0.5, 0.5, 0.0, 0.0) // UV center for rotation and suck effect
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
            float _SuckAmount;
            float _SuckRadius;
            float _MaxRotation;
            float _MaskRadius;
            float4 _Center;  // Center of the UV effects (both suck and rotation)

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

            // Rotate UV function
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

                // Calculate the distance from the center
                float dist = distance(uv, _Center.xy);

                // Apply the suck effect (zoom-out or zoom-in) if within the suck radius
                if (dist < _SuckRadius)
                {
                    // Calculate the suck factor, pulling UVs toward the center
                    float suckFactor = 1.0 - (_SuckAmount * (1.0 - dist / _SuckRadius));
                    suckFactor = pow(suckFactor, 2.0);  // Smooth out the effect
                    uv = _Center.xy + (uv - _Center.xy) * suckFactor;
                }
                else
                {
                    // Stretch the UVs outside the suck radius to compensate for the sucked-in region
                    float stretchFactor = 1.0 + (_SuckAmount * (dist - _SuckRadius) / (1.0 - _SuckRadius));
                    uv = _Center.xy + (uv - _Center.xy) * stretchFactor;
                }

                // Apply the rotation (twirl effect)
                float angle = lerp(0.0, _MaxRotation, dist);  // Linear interpolation from 0 to max rotation based on distance
                uv = RotateUV(uv, angle);

                // Mask: Check if the UV is inside the mask radius
                if (dist > _MaskRadius)
                {
                    discard; // Discard fragments outside the mask radius
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
