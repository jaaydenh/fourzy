
Shader "Custom/HSVRangeOutlineShader"
{
    Properties
    {
       _MainTex ("Sprite Texture", 2D) = "white" {}
       _ColorRampTex("Color ramp Texture", 2D) = "white" {}
       _Intensity ("Intensity", Range(0, 3)) = 1
       _OutlineColor ("Outline Color", Color) = (0,0,0,1)
       _OutlineBorder ("Outline Border", float) = 1
       _BlurSize ("Blur size", float) = 2

       _HSVRangeMin ("HSV Affect Range Min", Range(0, 1)) = 0
       _HSVRangeMax ("HSV Affect Range Max", Range(0, 1)) = 1
       _HSVAAdjust ("HSVA Adjust", Vector) = (0, 0, 0, 0)
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
            float _Intensity;
            float _BlurSize;

            sampler2D _ColorRampTex;

            half4 frag(Fragment IN) : COLOR
            {
                /*float2 glintPosition = IN.uv_MainTex;
                glintPosition.x += _SinTime.w;
                half4 glintWave = tex2D (_ColorRampTex, glintPosition);
                half wave = (glintWave.r + glintWave.g + glintWave.b) / 3.0;
                wave *= 0.5;

                _OutlineColor.r += wave;
                _OutlineColor.g += wave;
                _OutlineColor.b += wave;*/

                //half4 color = tex2D (_MainTex, IN.uv_MainTex);

                //return _OutlineColor * color.a * _Intencity;


                float sum = 0;

                float _Size = 2;

                #define GRABPIXELY(weight,kernely) tex2D( _MainTex, float2(IN.uv_MainTex.x, IN.uv_MainTex.y + _MainTex_TexelSize.y * kernely *_BlurSize)).a * weight
                #define GRABPIXELX(weight,kernelx) tex2D( _MainTex, float2(IN.uv_MainTex.x + kernelx * _BlurSize * _MainTex_TexelSize.x, IN.uv_MainTex.y)).a * weight

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

                    _OutlineColor.a = _OutlineColor.a * sum * _Intensity;

                return _OutlineColor;
            }

            ENDCG
        }

        UsePass "Custom/HSVRangeShader/HSVRANGE"
    }
}
