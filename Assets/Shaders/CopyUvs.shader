Shader "Custom/CopyUvsWithDestRotation"
{
    Properties
    {
        _MainTex ("Source Texture", 2D) = "white" { }
        _DestTex ("Destination Texture", 2D) = "white" { }
        _AlphaThreshold ("Alpha Threshold", Range(0, 1)) = 0.005 // 10/255
        _DestRotation ("Destination Rotation", Range(0, 360)) = 0.0 // Rotation angle for destination texture
    }
    SubShader
    {
        Tags { "Queue"="Overlay" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex; // Source texture
            sampler2D _DestTex; // Destination texture
            float _AlphaThreshold; // Alpha threshold (10 / 255)
            float _DestRotation; // Rotation angle for the destination texture

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Convert the destination rotation angle to radians
                float rad = _DestRotation * 3.14159 / 180.0;

                // Get the center of the texture (assuming the center is the pivot point for rotation)
                float2 center = float2(0.5, 0.5);

                // Translate the UV coordinates to the center, apply rotation, then translate back
                float2 rotatedUV = i.uv - center;
                rotatedUV = float2(
                    rotatedUV.x * cos(rad) - rotatedUV.y * sin(rad),
                    rotatedUV.x * sin(rad) + rotatedUV.y * cos(rad)
                );
                rotatedUV = rotatedUV + center;

                // Sample source color (no rotation, just use the original UVs)
                half4 sourceColor = tex2D(_MainTex, i.uv);
                
                // Sample destination color (apply rotation to UVs)
                half4 destColor = tex2D(_DestTex, rotatedUV);

                // If destination color alpha is less than threshold, keep the destination color
                if (destColor.a < _AlphaThreshold)
                {
                    return destColor;
                }

                // Otherwise, copy the RGB values from the source texture, but keep the destination alpha
                return half4(sourceColor.rgb, destColor.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
