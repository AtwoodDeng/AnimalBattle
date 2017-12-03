Shader "Hidden/StencilCut"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_CutLayer("Cut" , float ) = 1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		// ColorMask 0

		Pass
		{
//			Stencil
//			{
//				Ref [_CutLayer]
//				ReadMask 7
//				Comp Equal
//			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				return col * fixed4( 1 , 0 , 0 , 1);
			}
			ENDCG
		}
	}
}
