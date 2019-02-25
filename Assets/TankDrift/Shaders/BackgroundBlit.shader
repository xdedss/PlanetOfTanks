Shader "Unlit/BackgroundBlit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BackTex("BackTex", 2D) = "black" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		ZWrite Off

		Pass
		{
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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BackTex;
			sampler2D _CameraDepthTexture;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				
				float depth = tex2D(_CameraDepthTexture, i.uv.xy).r;
				if (depth > 0 && depth < 1) {
					return tex2D(_MainTex, i.uv);
				}
				fixed4 col = tex2D(_BackTex, i.uv);
				//return float4(depth * float3(1, 1, 1), 1);
				return col;
			}
			ENDCG
		}
	}
}
