
Shader "Custom/Fog"
{
	Properties
	{
		_FogHeight("FogHeight", float) = 1
		_FogColor("FogColor", vector) = (0.8, 0.8, 0.8, 1.0)
	}
	SubShader
	{
		Tags { "Queue" = "Geometry+600" "RenderType" = "Transparent"}
		//LOD 200

		Cull Off
		ZWrite OFF
		Blend SrcAlpha OneMinusSrcAlpha
		//Offset 0, 1

		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 wpos : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _CameraDepthTexture;

			float _FogHeight;
			float4 _FogColor;

			float clamp(float v, float min, float max) {
				if (v > max) {
					return max;
				}
				if (v < min) {
					return min;
				}
				return v;
			}
			
			float centerRange(float v, float center, float range) {
				return max(range - abs(v - center), 0) / range;
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.wpos = v.vertex;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 scrpos = i.vertex.xy / _ScreenParams.xy;
				float depth = tex2D(_CameraDepthTexture, scrpos).r;

				float4 col = float4(_FogColor.rgb, centerRange(i.wpos.y, 0, _FogHeight) * _FogColor.a);

				if (depth > 0 && depth < 1) {
					discard;
				}
				return col;
			}

			ENDCG
		}
	}
}
