Shader "Custom/CircleDrawingShader"
{
    Properties
    {
        _MainTex ("Render Texture", 2D) = "white" { }
        _LineColor ("Line Color", Color) = (1, 0, 0, 1) // Default red color
        _TexturePos ("Texture Position", Vector) = (0, 0, 0, 0) // x, y texture position
        _LineWidth ("Line Width", float) = 0.01 // Line width
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
            float4 _TexturePos;  // Texture position to draw the line
            float _LineWidth;
            float4 _LineColor;

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

            // Function to check if a pixel is close enough to the given texture position to draw a line
            bool IsPointNearLine(float2 uv, float2 linePos, float lineWidth)
            {
                float dist = distance(uv, linePos);
                return dist < lineWidth;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Fetch the texture color
                half4 texColor = tex2D(_MainTex, i.uv);

                // Check if the pixel is close to the texture position (draw a line)
                if (IsPointNearLine(i.uv, _TexturePos.xy, _LineWidth))
                {
                    return _LineColor; // Draw the line with the specified color
                }

                return texColor; // Otherwise, return the original texture color
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}

