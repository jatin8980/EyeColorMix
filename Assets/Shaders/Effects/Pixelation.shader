Shader "Custom/Pixelate"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" { }
        _PixelControl ("Pixel Control", Range(0.0, 1.0)) = 0.0 // Control from 0 (original resolution) to 1 (maximum pixelation)
        _Radius ("Mask Radius", Range(0.0, 1.0)) = 0.5 // Defines the radius where pixels are visible
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
            float _PixelControl;  // Control the pixelation from 0 (original resolution) to 1 (maximum pixelation)
            float _Radius;     // The mask radius, controls how much of the texture is visible

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
                // Get the UV coordinates
                float2 uv = i.uv;

                // Calculate the distance from the center of the image (0.5, 0.5)
                float dist = length(uv - 0.5);

                // Apply the radius mask: if the distance is greater than the radius, discard the fragment
                if (dist > _Radius)
                    discard;

                // Use an exponential curve for pixel size adjustment (non-linear)
                float pixelSize = lerp(1024.0, 15.0, pow(_PixelControl, 2.0)); // Exponential easing

                // Step 1: Scale the UVs to form the blocks but preserve the center alignment
                float2 scaledUV = uv * pixelSize;

                // Step 2: Snap to the nearest integer grid (round to nearest whole number)
                scaledUV = round(scaledUV);

                // Step 3: Normalize the UVs back to [0, 1] space by dividing by pixelSize
                scaledUV /= pixelSize;

                // Sample the texture at the new UV coordinates (inside the radius)
                half4 col = tex2D(_MainTex, scaledUV);

                // Don't override the alpha; keep the alpha from the texture
                return col; // The alpha will be preserved from the texture sample
            }

            ENDCG
        }
    }

    FallBack "Diffuse"
}
