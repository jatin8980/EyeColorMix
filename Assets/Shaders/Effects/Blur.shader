Shader "Custom/BlurWithMasking"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" { }
        _ScreenResolution ("Screen Resolution", Vector) = (1, 1, 0, 0)
        _BlurSize ("Blur Size", Range(0.0, 0.1)) = 0
        _SampleCount ("Sample Count", Range(1, 10)) = 5 // Number of samples to take for the blur effect
        _MaskRadius ("Mask Radius", Range(0.0, 1.0)) = 0.5 // Radius for the mask
        _Center ("Mask Center", Vector) = (0.5, 0.5, 0.0, 0.0) // Center of the mask in UV space (defaults to (0.5, 0.5))
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
            float4 _ScreenResolution;
            float _BlurSize;
            int _SampleCount;
            float _MaskRadius;
            float4 _Center;

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
                // Get the screen resolution
                float2 screenResolution = _ScreenResolution.xy;

                // Calculate texture size (pixel size)
                float2 texelSize = 1.0 / screenResolution;

                // Blur offset based on the blur size
                float2 blurOffset = texelSize * _BlurSize;

                // Get the UV coordinates
                float2 uv = i.uv;

                // Calculate the distance from the center of the mask
                float distanceFromCenter = distance(uv, _Center.xy);

                // If the UV is outside the mask radius, discard the fragment
                if (distanceFromCenter > _MaskRadius)
                    discard;

                // Start with the center sample
                half4 color = tex2D(_MainTex, uv);

                // Sample additional pixels around the center
                for (int x = -_SampleCount; x <= _SampleCount; x++)
                {
                    for (int y = -_SampleCount; y <= _SampleCount; y++)
                    {
                        // Avoid sampling the same texel at (0,0) twice
                        if (x == 0 && y == 0)
                            continue;

                        // Calculate new sample position based on the offset
                        float2 offsetUV = uv + float2(x, y) * blurOffset;

                        // Sample the texture at the new position
                        color += tex2D(_MainTex, offsetUV);
                    }
                }

                // Normalize by the total number of samples
                float numSamples = (2 * _SampleCount + 1) * (2 * _SampleCount + 1) - 1;
                color /= numSamples;

                return color;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
