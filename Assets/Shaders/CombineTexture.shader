Shader "Custom/CombineTexture"
{
    Properties
    {
        _MainTex ("Source Texture", 2D) = "white" { }
        _DestTex ("Destination Texture", 2D) = "white" { }
        _Radius ("Radius", Range(0, 1)) = 0.5 // The radius where blending happens
        _Center ("Center", Vector) = (0.5, 0.5, 0.0, 0.0) // The center of the UV space
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

            // Properties
            sampler2D _MainTex; // Source texture
            sampler2D _DestTex; // Destination texture
            float _Radius; // The radius for blending
            float4 _Center; // The center of the UV space

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
                // Sample source and destination colors
                half4 sourceColor = tex2D(_MainTex, i.uv);
                half4 destColor = tex2D(_DestTex, i.uv);

                // Calculate the distance from the center
                float2 centeredUV = i.uv - _Center.xy; // Offset UVs by center
                float dist = length(centeredUV); // Calculate distance from center

                // If outside the radius, sample the color from the edge (radius)
                if (dist > _Radius)
                {
                    // Calculate the UV on the edge of the radius
                    float2 edgeUV = _Center.xy + normalize(centeredUV) * (_Radius-0.01f);
                    // Sample the color from the edge
                    return tex2D(_DestTex, edgeUV);
                }

                // Blend both colors based on the alpha of the source texture
                half alphaSum = sourceColor.a + destColor.a * (1 - sourceColor.a);
                half4 finalColor = (sourceColor * sourceColor.a + destColor * destColor.a * (1 - sourceColor.a)) / alphaSum;

                // Return the final blended color with the computed alpha
                return half4(finalColor.rgb, alphaSum);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
