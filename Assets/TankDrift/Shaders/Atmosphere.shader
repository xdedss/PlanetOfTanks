
Shader "Custom/Atmosphere"
{
	Properties
	{
		_FogColor("FogColor", Color) = (0.8, 0.8, 0.8, 1.0)
		_FogDistance("FogDistance&Angle", vector) = (100, 200, -10, 30)
		_SkyColor("SkyColor", Color) = (0.8, 0.8, 0.8, 1.0)
		_SkyDistance("SkyGradientAngle", vector) = (0, 0, 0, 0)
	}
	SubShader
	{
		Tags { "Queue" = "Geometry+600" "RenderType" = "Transparent"}
		//LOD 200

		Cull Front
		ZWrite OFF
		ZTest Always
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
				float4 lpos : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _CameraDepthTexture;

			float4 _SkyColor;
			float4 _SkyDistance;
			float4 _FogColor;
			float4 _FogDistance;


			float4 getWorldPosition(float2 uv, float linearDepth)
			{
				float camPosZ = _ProjectionParams.y + (_ProjectionParams.z - _ProjectionParams.y) * linearDepth;

				float height = 2 * camPosZ / unity_CameraProjection._m11;
				float width = _ScreenParams.x / _ScreenParams.y * height;

				float camPosX = width * uv.x - width / 2;
				float camPosY = height * uv.y - height / 2;
				float4 camPos = float4(camPosX, camPosY, camPosZ, 1.0);
				//return mul(unity_CameraToWorld, camPos);
				return camPos;
			}

			float map(float v, float vmin, float vmax, float omin, float omax) {
				return ((v - vmin) / (vmax - vmin)) * (omax - omin) + omin;
			}

			float clamp(float v, float min, float max) {
				if (v > max) {
					return max;
				}
				if (v < min) {
					return min;
				}
				return v;
			}

			float4 blend(float4 c1, float4 c2) {
				return float4((c1.rgb * c1.a + c2.rgb * c2.a)/(c1.a + c2.a), max(c1.a, c2.a));
			}
			
			float centerRange(float v, float center, float range) {
				return max(range - abs(v - center), 0) / range;
			}

			float4 getColor(float angle) {//angle from horizon
				float fogAlpha, skyAlpha;
				fogAlpha = pow(clamp(map(angle, _FogDistance.z, _FogDistance.w, 1, 0) ,0, 1), 3);
				skyAlpha = clamp(map(angle, _SkyDistance.z, _SkyDistance.w, _SkyDistance.y, _SkyDistance.x), _SkyDistance.x, _SkyDistance.y);

				return blend(float4(_FogColor.xyz, fogAlpha * _FogColor.a), float4(_SkyColor.xyz, skyAlpha));
			}

			float4 getIntensity(float distance) {
				return clamp((distance - _FogDistance.x) / _FogDistance.y, 0, 1);
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.lpos = v.vertex;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 scrpos = i.vertex.xy / _ScreenParams.xy;
				float depth = tex2D(_CameraDepthTexture, scrpos).r;

				//float4 col = float4(_FogColor.rgb, centerRange(i.lpos.y, 0, _FogHeight) * _FogColor.a);
				float heightAngle = atan(i.lpos.y / pow(i.lpos.x * i.lpos.x + i.lpos.z * i.lpos.z, 0.5));
				float4 pixelPos = getWorldPosition(scrpos, Linear01Depth(depth));
				float4 col = getColor(heightAngle);
				float4 intensity = getIntensity(length(pixelPos.xyz));
				col.a *= intensity;
				//col = float4(pixelPos.xyz/1000, 1);

			/*	if (depth > 0 && depth < 1) {
					discard;
				}*/
				return col;
			}

			ENDCG
		}
	}
}
