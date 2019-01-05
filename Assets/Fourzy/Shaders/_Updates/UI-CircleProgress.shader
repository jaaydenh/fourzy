

Shader "Custom/UICircleProgress"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		//custom
		_HeadTex("Head Texture", 2D) = "white" {}
		[PerRendererData] _Progress("Circle progress", Float) = 0
		[PerRendererData] _RectMainTex("Rect main texture in Atlas", Vector) = (0, 0, 1, 1)
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
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				float2 texcoord1  : TEXCOORD3;
                float4 worldPosition : TEXCOORD1;
				float2 uv : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

			float _Progress;
			float4 _RectMainTex;
			sampler2D _HeadTex;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

				float s = sin(-_Progress);
				float c = cos(-_Progress);
				float2x2 rotationMatrix = float2x2(c, -s, s, c);
				rotationMatrix *= 0.5;
				rotationMatrix += 0.5;
				rotationMatrix = rotationMatrix * 2 - 1;

				float2 uv = (v.texcoord - _RectMainTex.xy) / (_RectMainTex.zw - _RectMainTex.xy);
				OUT.uv = uv;

				uv -= 0.5;
				uv = mul(uv, rotationMatrix);
				uv += 0.5;

				OUT.texcoord1 = uv;
				OUT.color = v.color * _Color;

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
				half4 color;

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
					angle = (2 * PI - angle);
				}

				if (angle > _Progress)
				{
					color = tex2D(_HeadTex, IN.texcoord1) * IN.color;
				}
				else
				{
					color = tex2D(_MainTex, IN.texcoord) * IN.color;
				}

				color.rgb *= color.a;
                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return color;
            }
        ENDCG
        }
    }
}
