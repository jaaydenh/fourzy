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
       _Color("Tint", Color) = (1, 1, 1, 1)
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
            Name "HSVRANGE"
            Cull Off
            Lighting Off
            ZWrite Off
            Blend One OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _HSVRangeMin;
            float _HSVRangeMax;
            float4 _HSVAAdjust;

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

            fixed4 _Color;

            Fragment vert(Vertex v)
            {
                Fragment o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv_MainTex = v.uv_MainTex;
                o.color = v.color * _Color;

                return o;
            }

            fixed3 rgb2hsv(fixed3 c) 
            {
              fixed4 K = fixed4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
              fixed4 p = lerp(fixed4(c.bg, K.wz), fixed4(c.gb, K.xy), step(c.b, c.g));
              fixed4 q = lerp(fixed4(p.xyw, c.r), fixed4(c.r, p.yzx), step(p.x, c.r));

              fixed d = q.x - min(q.w, q.y);
              fixed e = 1.0e-3;
              return fixed3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            fixed3 hsv2rgb(fixed3 c) 
            {
              c = fixed3(c.x, clamp(c.yz, 0.0, 1.0));
              fixed4 K = fixed4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
              fixed3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
              return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

            sampler2D _MainTex;

            fixed4 frag(Fragment IN) : COLOR
            {
                fixed4 color = tex2D (_MainTex, IN.uv_MainTex) * IN.color;

                fixed3 hsv = rgb2hsv(color.rgb);
                fixed affectMult = step(_HSVRangeMin, hsv.r) * step(hsv.r, _HSVRangeMax);
                fixed3 rgb = hsv2rgb(hsv + _HSVAAdjust.xyz * affectMult);

                color.rgb = rgb * color.a;
                return color;
            }

            ENDCG
        }
    }
}
