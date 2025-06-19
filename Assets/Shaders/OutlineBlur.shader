Shader "Custom/CircleEdgeBlur"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _BlurAmount ("Blur Amount", Range(0, 1)) = 0.0
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane" 
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "CircleEdgeBlur"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _BlurAmount;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float radius = 0.5; // fixed outer radius of pupil

                float fadeWidth = saturate(_BlurAmount) * 0.2; // fade width inside edge

                fixed4 texColor = tex2D(_MainTex, i.uv) * i.color;

                float dist = length(i.uv - center);

                float alpha = 0.0;

                if (dist <= radius - fadeWidth)
                {
                    alpha = 1.0;
                }
                else if (dist <= radius)
                {
                    alpha = smoothstep(radius, radius - fadeWidth, dist);
                }
                else
                {
                    alpha = 0.0;
                }

                texColor.a *= alpha;

                return texColor;
            }
            ENDCG
        }
    }
}
