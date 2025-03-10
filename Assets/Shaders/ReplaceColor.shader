Shader "Custom/ReplaceColor"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" { } // Add the main texture property
        _ToReplaceColor ("Color to Replace", Color) = (1, 1, 1, 1)
        _NewColor ("New Color", Color) = (0, 0, 1, 1)
        _Tolerance ("Color Tolerance", Range(0, 1)) = 0.1
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

            // Declare shader properties
            sampler2D _MainTex;   // Declare the texture sampler
            fixed4 _ToReplaceColor;
            fixed4 _NewColor;
            float _Tolerance;

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
                // Sample the current pixel color from the main texture
                half4 currentColor = tex2D(_MainTex, i.uv);

                // Calculate the color difference between the current pixel and the color to replace
                half diff = distance(currentColor.rgb, _ToReplaceColor.rgb);

                // If the color difference is within tolerance, replace with new color
                if (diff < _Tolerance)
                {
                    return _NewColor;
                }

                // Otherwise, return the original color
                return currentColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
