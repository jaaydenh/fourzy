Shader "Custom/GrayScaleShader"
{
    Properties
    {
       [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
       _GrayScale ("GrayScale", Vector) = (0.3, 0.59, 0.11, 0)
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

        Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            Blend One OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct Vertex
            {
                float4 vertex : POSITION;
                float4 color    : COLOR;
                float2 uv_MainTex : TEXCOORD0;
            };

            struct Fragment
            {
                float4 vertex : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv_MainTex : TEXCOORD0;
            };

            Fragment vert(Vertex v)
            {
                Fragment o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv_MainTex = v.uv_MainTex;
                o.color = v.color;

                return o;
            }

            sampler2D _MainTex;
            fixed4 _GrayScale;

            fixed4 frag(Fragment IN) : COLOR
            {
                fixed4 color = tex2D (_MainTex, IN.uv_MainTex) * IN.color;
                color.rgb = dot(color.rgb, _GrayScale.xyz);
                color.rgb *= color.a;
                return color;
            }

            ENDCG
        }
    }
}