Shader "Custom/IrisSpikeEffectWithMask"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _SpikeIntensity ("Spike Intensity", Range(0, 1)) = 0.5
        _SpikeFrequency ("Spike Frequency", Range(1, 50)) = 20
        _StretchAmount ("Stretch Amount", Range(-1, 1)) = 0.2
        _StretchThreshold ("Stretch Threshold", Range(0, 1)) = 0.5 // Threshold for applying stretch
        _MaskRadius ("Mask Radius", Range(0, 1)) = 0.4 // Radius beyond which UVs are masked
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _SpikeIntensity;
            float _SpikeFrequency;
            float _StretchAmount;
            float _StretchThreshold; // The distance threshold for applying UV stretching
            float _MaskRadius; // Masking radius
            float4 _MainTex_ST;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Center UVs for radial effect
                float2 center = float2(0.5, 0.5);
                float2 dir = uv - center;
                float dist = length(dir);
                float angle = atan2(dir.y, dir.x); // Angle for periodic effect

                // Generate spikes using sine wave
                float spikePattern = sin(angle * _SpikeFrequency) * _SpikeIntensity;

                // Stretch UVs outward with the spike pattern
                float stretchFactor = 1.0 + dist * (_StretchAmount + spikePattern);

                // Apply stretching only if the distance is beyond the threshold
                if (dist >= _StretchThreshold)
                {
                    uv = center + dir * stretchFactor; // Stretch outside
                }
                else
                {
                    uv = uv; // Keep the inner UVs unchanged
                }

                // Masking: Set alpha to 0 outside the mask radius, 1 inside the radius
                float alpha = (dist < _MaskRadius) ? 1.0 : 0.0;

                // Sample the texture with distorted UVs
                fixed4 color = tex2D(_MainTex, uv);
                color.a *= alpha; // Apply the alpha mask

                return color;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
