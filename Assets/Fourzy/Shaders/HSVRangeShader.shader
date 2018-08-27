// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
MIT License

Copyright 2015, Gregg Tavares.
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are
met:

    * Redistributions of source code must retain the above copyright
notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above
copyright notice, this list of conditions and the following disclaimer
in the documentation and/or other materials provided with the
distribution.
    * Neither the name of Gregg Tavares. nor the names of its
contributors may be used to endorse or promote products derived from
this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
Shader "Custom/HSVRangeShader"
{
    Properties
    {
       _MainTex ("Sprite Texture", 2D) = "white" {}
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
            Name "HSVRANGE"
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile DUMMY PIXELSNAP_ON

            sampler2D _MainTex;

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

            half3 rgb2hsv(half3 c) 
            {
              half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
              half4 p = lerp(half4(c.bg, K.wz), half4(c.gb, K.xy), step(c.b, c.g));
              half4 q = lerp(half4(p.xyw, c.r), half4(c.r, p.yzx), step(p.x, c.r));

              half d = q.x - min(q.w, q.y);
              half e = 1.0e-10;
              return half3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            half3 hsv2rgb(half3 c) 
            {
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
        }
    }
}
