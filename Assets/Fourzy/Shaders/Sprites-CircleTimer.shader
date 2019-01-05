// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/CircleTimer"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _HeadTex ("Head Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Progress ("Circle progress", Float) = 0
        _RectMainTex ("Rect main texture in Atlas", Vector) = (0, 0, 1, 1)
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

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
            {
                return float4(pos.xy * flip, pos.z, 1.0);
            }

            fixed4 _Color;
            float _Progress;
            float4 _RectMainTex;

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;

                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;

                float s = sin ( -_Progress );
                float c = cos ( -_Progress );
                float2x2 rotationMatrix = float2x2( c, -s, s, c);
                rotationMatrix *= 0.5;
                rotationMatrix += 0.5;
                rotationMatrix = rotationMatrix * 2 - 1;

                float2 uv = (IN.texcoord - _RectMainTex.xy) / (_RectMainTex.zw - _RectMainTex.xy);
                OUT.uv = uv;
                  
                uv -= 0.5;
                uv = mul ( uv, rotationMatrix );
                uv += 0.5;

                OUT.texcoord1 = uv;

                OUT.color = IN.color * _Color;

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

            sampler2D _MainTex;
            sampler2D _HeadTex;
  
            fixed4 SpriteFrag(v2f IN) : SV_Target
            {
                if (_Progress == 0)
                {
                    return fixed4(0, 0, 0, 0);
                }

                float2 uv = IN.uv;

                const float PI = 3.14159;

                uv = normalize(uv * 2 - 1);   // make it [-1, 1];
                float angle = acos(-uv.x);

                if (uv.y < 0)
                {
                    angle =  ( 2 * PI - angle);
                }

                fixed4 color;

                if (angle > _Progress)
                {
                    color = tex2D (_HeadTex, IN.texcoord1) * IN.color;
                }
                else
                {
                    color = tex2D (_MainTex, IN.texcoord) * IN.color;
                } 

                color.rgb *= color.a;

                return color;
            }

        ENDCG
        }
    }
}
