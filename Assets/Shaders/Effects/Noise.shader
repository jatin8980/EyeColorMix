Shader "Custom/Noise"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" { }
        _NoiseStrength ("Noise Strength", Range(0.0, 1.0)) = 0.1
        _CustomTime ("Time", Range(0, 1)) = 0.0
        _Radius ("Mask Radius", Range(0.0, 1.0)) = 0.5
        _Center ("Mask Center", Vector) = (0.5, 0.5, 0.0, 0.0) // UV center of the mask

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
            float _NoiseStrength;
            float _CustomTime;  // Custom time variable for noise
            float _Radius;      // Mask radius
            float4 _Center;     // Mask center (in UV coordinates)

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

            // Random function based on texture coordinates and time
            float rand(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            half4 frag(v2f i) : SV_Target
            {
                // Get the UV coordinates
                float2 uv = i.uv;

                // Calculate the distance from the center of the mask
                float distanceFromCenter = distance(uv, _Center.xy);

                // If the UV is outside the mask radius, discard the fragment
                if (distanceFromCenter > _Radius)
                    discard;

                // Use the custom time to generate random noise
                float time = _CustomTime;
                float noise = rand(uv + time);  // random value based on UV and time
                
                // Apply noise strength to the effect (ensure it doesn't distort too much)
                float noiseEffect = (noise - 0.5) * _NoiseStrength;

                // Make the noise move the texture in one of 4 directions (up, down, left, right)
                float2 movement = float2(0.0, 0.0);

                if(noise%0.1f<0.025f)
                {
                    movement = float2(noiseEffect, 0.0);   // Move right
                }
                else if(noise%0.1f<0.05f)
                {
                    movement = float2(-noiseEffect, 0.0);  // Move left
                }
                else if(noise%0.1f<0.75f)
                {
                    movement = float2(0.0, noiseEffect);   // Move up
                }
                else
                {
                    movement = float2(0.0, -noiseEffect);  // Move down
                }              

                // Apply the movement to the UV coordinates
                uv += movement;

                // Sample the base texture with the adjusted UV coordinates
                half4 col = tex2D(_MainTex, uv);

                // Return the color with the noise effect applied
                return col;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
