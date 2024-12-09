Shader "Custom/UIOutline"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 1, 0, 1)
        _OutlineThickness ("Outline Thickness", Range(0.0, 0.01)) = 0.002
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Overlay" "RenderType" = "Transparent"
        }
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
            fixed4 _OutlineColor;
            float _OutlineThickness;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 offset = float2(_OutlineThickness, _OutlineThickness);

                // Sample surrounding pixels to detect edges
                fixed4 texColor = tex2D(_MainTex, i.texcoord);
                fixed4 texColorN = tex2D(_MainTex, i.texcoord + float2(0, offset.y));
                fixed4 texColorS = tex2D(_MainTex, i.texcoord - float2(0, offset.y));
                fixed4 texColorE = tex2D(_MainTex, i.texcoord + float2(offset.x, 0));
                fixed4 texColorW = tex2D(_MainTex, i.texcoord - float2(offset.x, 0));

                // If the current pixel is transparent but one of its neighbors is not, draw the outline color
                if (texColor.a == 0 && (texColorN.a > 0 || texColorS.a > 0 || texColorE.a > 0 || texColorW.a > 0))
                {
                    return _OutlineColor;
                }

                // Otherwise, return the original color
                return texColor;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}