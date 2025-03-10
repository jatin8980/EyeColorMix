Shader "Custom/LineDrawingShader"
{
    Properties
    {
        _MainTex ("Render Texture", 2D) = "white" { }
        _LineColor ("Line Color", Color) = (1, 0, 0, 1) // Default red color
        _TexturePos ("Texture Position 1", Vector) = (0, 0, 0, 0) // First texture position
        _TexturePos2 ("Texture Position 2", Vector) = (0, 0, 0, 0) // Second texture position
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
            float4 _TexturePos;  // First texture position
            float4 _TexturePos2; // Second texture position
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

            // Function to compute the perpendicular distance from a point to a line segment
            float2 closestPointOnLine(float2 p, float2 lineStart, float2 lineEnd)
            {
                float2 lineDir = lineEnd - lineStart;
                float2 pointDir = p - lineStart;
                float t = dot(pointDir, lineDir) / dot(lineDir, lineDir);
                t = clamp(t, 0.0, 1.0); // Clamping t to stay within the line segment
                return lineStart + t * lineDir;
            }

            // Function to check if a pixel is close enough to the line between two texture positions
            bool IsPointNearLine(float2 uv, float2 lineStart, float2 lineEnd, float lineWidth)
            {
                float2 closestPoint = closestPointOnLine(uv, lineStart, lineEnd);
                float dist = distance(uv, closestPoint);
                return dist < lineWidth;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Fetch the texture color
                half4 texColor = tex2D(_MainTex, i.uv);

                // Check if the pixel is close to the line between the two texture positions
                if (IsPointNearLine(i.uv, _TexturePos.xy, _TexturePos2.xy, _LineWidth))
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
