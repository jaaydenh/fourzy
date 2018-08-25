
Shader "Custom/HSVRangeShaderGlow"
{
    Properties
    {
       _MainTex ("Sprite Texture", 2D) = "white" {}
       _Intencity ("Intencity", Range(0, 3)) = 1
       _OutlineColor ("Outline Color", Color) = (0,0,0,1)
       _OutlineBorder ("Outline Border", float) = 1

    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct Vertex
            {
                float4 vertex : POSITION;
                float2 uv_MainTex : TEXCOORD0;
            };

            struct Fragment
            {
                float4 vertex : POSITION;
                float2 uv_MainTex : TEXCOORD0;
                float4 originalVertex : POSITION1;
            };

            float _OutlineBorder;

            Fragment vert(Vertex v)
            {
                Fragment o;
                o.originalVertex = UnityObjectToClipPos(v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex * _OutlineBorder);
                o.uv_MainTex = v.uv_MainTex;

                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            half4 _OutlineColor;
            float _Intencity;

            half4 frag(Fragment IN) : COLOR
            {
                float2 newUV = IN.uv_MainTex / _OutlineBorder;
                half4 color2 = tex2D (_MainTex, newUV);
                half4 color = tex2D (_MainTex, IN.uv_MainTex);

                //float d = distance( IN.vertex.xy, IN.originalVertex.xy) / 800;// / _OutlineBorder;
                //half4 c = _OutlineColor;

                return _OutlineColor * color.a * _Intencity;


                float sum = 0;

                float _Size = 2;

                #define GRABPIXELY(weight,kernely) tex2D( _MainTex, float2(IN.uv_MainTex.x, IN.uv_MainTex.y + _MainTex_TexelSize.y * kernely *_Size)).a * weight
                #define GRABPIXELX(weight,kernelx) tex2D( _MainTex, float2(IN.uv_MainTex.x + kernelx * _Size * _MainTex_TexelSize.x, IN.uv_MainTex.y)).a * weight

//                    sum += GRABPIXELY(0.05, -4.0);
//                    sum += GRABPIXELY(0.09, -3.0);
                    sum += GRABPIXELY(0.12, -2.0);
                    sum += GRABPIXELY(0.15, -1.0);
                    sum += GRABPIXELY(0.18,  0.0);
                    sum += GRABPIXELY(0.15, +1.0);
                    sum += GRABPIXELY(0.12, +2.0);
//                    sum += GRABPIXELY(0.09, +3.0);
//                    sum += GRABPIXELY(0.05, +4.0);

//                    sum += GRABPIXELX(0.05, -4.0);
//                    sum += GRABPIXELX(0.09, -3.0);
                    sum += GRABPIXELX(0.12, -2.0);
                    sum += GRABPIXELX(0.15, -1.0);
                    sum += GRABPIXELX(0.18,  0.0);
                    sum += GRABPIXELX(0.15, +1.0);
                    sum += GRABPIXELX(0.12, +2.0);
//                    sum += GRABPIXELX(0.09, +3.0);
//                    sum += GRABPIXELX(0.05, +4.0);

                return _OutlineColor * sum * _Intencity;
            }

            ENDCG
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct Vertex
            {
                float4 vertex : POSITION;
                float2 uv_MainTex : TEXCOORD0;
            };

            struct Fragment
            {
                float4 vertex : POSITION;
                float2 uv_MainTex : TEXCOORD0;
            };

            Fragment vert(Vertex v)
            {
                Fragment o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv_MainTex = v.uv_MainTex;

                return o;
            }

            sampler2D _MainTex;

            half4 frag(Fragment IN) : COLOR
            {
                half4 color = tex2D (_MainTex, IN.uv_MainTex);
                return color;
            }

            ENDCG
        }
    }
}
