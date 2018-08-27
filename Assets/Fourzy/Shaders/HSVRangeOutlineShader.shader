
Shader "Custom/HSVRangeOutlineShader"
{
    Properties
    {
       _MainTex ("Sprite Texture", 2D) = "white" {}
       _ColorRampTex("Color ramp Texture", 2D) = "white" {}
       _Intencity ("Intencity", Range(0, 3)) = 1
       _OutlineColor ("Outline Color", Color) = (0,0,0,1)
       _OutlineBorder ("Outline Border", float) = 1

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
            float _Intencity;

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

                half4 color = tex2D (_MainTex, IN.uv_MainTex);

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

        /*Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile DUMMY PIXELSNAP_ON

            sampler2D _MainTex;
            float4 _Color;
            float _HSVRangeMin;
            float _HSVRangeMax;
            float4 _HSVAAdjust;

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

            half3 rgb2hsv(half3 c) {
              half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
              half4 p = lerp(half4(c.bg, K.wz), half4(c.gb, K.xy), step(c.b, c.g));
              half4 q = lerp(half4(p.xyw, c.r), half4(c.r, p.yzx), step(p.x, c.r));

              half d = q.x - min(q.w, q.y);
              half e = 1.0e-10;
              return half3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            half3 hsv2rgb(half3 c) {
              c = half3(c.x, clamp(c.yz, 0.0, 1.0));
              half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
              half3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
              return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

            half4 frag(Fragment IN) : COLOR
            {
                half4 o = half4(1, 0, 0, 0.2);

                half4 color = tex2D (_MainTex, IN.uv_MainTex);
                half3 hsv = rgb2hsv(color.rgb);
                half affectMult = step(_HSVRangeMin, hsv.r) * step(hsv.r, _HSVRangeMax);
                half3 rgb = hsv2rgb(hsv + _HSVAAdjust.xyz * affectMult);
                return half4(rgb, color.a + _HSVAAdjust.a);
            }

            ENDCG
        }*/

        UsePass "Custom/HSVRangeShader/HSVRANGE"
    }
}
