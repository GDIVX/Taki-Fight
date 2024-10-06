Shader "Custom/SpriteHighlight"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _HighlightColor ("Highlight Color", Color) = (1, 1, 0, 1)
        _HighlightIntensity ("Highlight Intensity", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _HighlightColor;
            float _HighlightIntensity;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.texcoord);
                
                // If the texture is fully transparent, discard the fragment
                if (texColor.a < 0.1)
                {
                    discard;
                }

                // Blend between the original texture color and the highlight color
                fixed4 finalColor = lerp(texColor, _HighlightColor, _HighlightIntensity);
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}
