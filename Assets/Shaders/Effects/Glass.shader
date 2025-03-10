Shader "Custom/Glass"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _SecondTex ("Frosted Texture", 2D) = "white" {}
        [Header(Extracted)]
        intensity ("Intensity", Float) = 0.05
        _Offset ("Texture Offset", Vector) = (0.0, 0.0, 0.0, 0.0) // Offset for moving the main texture
        _Center ("Mask Center", Vector) = (0.5, 0.5, 0.0, 0.0) // UV center of the mask
        _Radius ("Mask Radius", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Built-in properties
            sampler2D _MainTex;   
            float4 _MainTex_TexelSize;
            sampler2D _SecondTex; 
            float4 _SecondTex_TexelSize;
            float4 _Center;
            float _Radius; 

            // New Offset property for texture movement
            float4 _Offset;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            const float intensity;

            float4 frag(v2f __vertex_output) : SV_Target
            {
                // Use the input UV for the mask, without the offset
                float2 maskUV = __vertex_output.uv;
                float distanceFromCenter = distance(maskUV, _Center.xy);

                // If the UV is outside the mask radius, discard the fragment
                if (distanceFromCenter > _Radius)
                    discard;

                // Apply the offset only to the main texture
                float2 uv = __vertex_output.uv + _Offset.xy; // Apply the offset to UVs for the main texture

                float4 fragColor = 0;

                // Apply the frosted glass effect by adding distortion from _SecondTex
                float2 d = tex2D(_SecondTex, maskUV).xy;  // Use original UV for the frosted effect (no offset)
                fragColor = tex2D(_MainTex, uv + d * intensity); // Apply distortion with intensity

                return fragColor;
            }
            ENDCG
        }
    }
}

