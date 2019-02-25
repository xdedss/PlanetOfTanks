// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Carve"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}

	_DebugPar("DebugPar", Float) = 0
	_CutoffMap("Cutoff Map", 2D) = "white" {}
	_TerrainMap("Terrain Map", 2D) = "red" {}

	_TerrainR("TerrainR", Color) = (1,0,0,1)
	_TerrainG("TerrainG", Color) = (0,1,0,1)
	_TerrainB("TerrainB", Color) = (0,0,1,1)

	_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
		[Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel("Smoothness texture channel", Float) = 0

		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic", 2D) = "white" {}

	[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

	_Parallax("Height Scale", Range(0.005, 0.08)) = 0.02
		_ParallaxMap("Height Map", 2D) = "black" {}

	_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}

	_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}

	_DetailMask("Detail Mask", 2D) = "white" {}

	_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
	_DetailNormalMapScale("Scale", Float) = 1.0
		_DetailNormalMap("Normal Map", 2D) = "bump" {}

	[Enum(UV0,0,UV1,1)] _UVSec("UV Set for secondary textures", Float) = 0


		// Blending state
		[HideInInspector] _Mode("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
	}

		CGINCLUDE
#define UNITY_SETUP_BRDF_INPUT MetallicSetup
		ENDCG

		SubShader
	{
		Tags{ "RenderType" = "Opaque" "PerformanceChecks" = "False" "Queue" = "Geometry-9" }
		//ZWrite Off
		LOD 300
		ZTest GEqual
		Cull Front
		//Cull Off
		// ------------------------------------------------------------------
		//  Base forward pass (directional light, emission, lightmaps, ...)
		Pass
	{
		Name "FORWARD"
		Tags{ "LightMode" = "ForwardBase" }

		Blend[_SrcBlend][_DstBlend]
		ZWrite[_ZWrite]

		CGPROGRAM
#pragma target 3.0

		// -------------------------------------

#pragma shader_feature _NORMALMAP
#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#pragma shader_feature _EMISSION
#pragma shader_feature _METALLICGLOSSMAP
#pragma shader_feature ___ _DETAIL_MULX2
#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
#pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
#pragma shader_feature _PARALLAXMAP

#pragma multi_compile_fwdbase
#pragma multi_compile_fog
#pragma multi_compile_instancing
		// Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
		//#pragma multi_compile _ LOD_FADE_CROSSFADE

#pragma vertex vertBase
#pragma fragment fragBase
#include "UnityStandardCoreForward.cginc"

		ENDCG
	}
		// ------------------------------------------------------------------
		//  Additive forward pass (one light per pass)
Pass
{
	Name "FORWARD_DELTA"
	Tags{ "LightMode" = "ForwardAdd" }
	Blend[_SrcBlend] One
	Fog{ Color(0,0,0,0) } // in additive pass fog should be black
	ZWrite Off
	ZTest LEqual

	CGPROGRAM
#pragma target 3.0

	// -------------------------------------


#pragma shader_feature _NORMALMAP
#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#pragma shader_feature _METALLICGLOSSMAP
#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
#pragma shader_feature ___ _DETAIL_MULX2
#pragma shader_feature _PARALLAXMAP

#pragma multi_compile_fwdadd_fullshadows
#pragma multi_compile_fog
		// Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
		//#pragma multi_compile _ LOD_FADE_CROSSFADE

#pragma vertex vertAdd
#pragma fragment fragAdd
#include "UnityStandardCoreForward.cginc"

		ENDCG
}

// ------------------------------------------------------------------
//  Shadow rendering pass
Pass{
Name "ShadowCaster"
Tags{ "LightMode" = "ShadowCaster" }

ZWrite On ZTest LEqual

CGPROGRAM
#pragma target 3.0

// -------------------------------------


#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#pragma shader_feature _METALLICGLOSSMAP
#pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature _PARALLAXMAP
#pragma multi_compile_shadowcaster
#pragma multi_compile_instancing
		// Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
		//#pragma multi_compile _ LOD_FADE_CROSSFADE

#pragma vertex vertShadowCaster
#pragma fragment fragShadowCaster

#include "UnityStandardShadow.cginc"

		ENDCG
}

// ------------------------------------------------------------------
//  Deferred pass
Pass
{
	Name "DEFERRED"
	Tags{ "LightMode" = "Deferred" }


	CGPROGRAM
#pragma target 3.0
#pragma exclude_renderers nomrt


	// -------------------------------------

#pragma shader_feature _NORMALMAP
#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#pragma shader_feature _EMISSION
#pragma shader_feature _METALLICGLOSSMAP
#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
#pragma shader_feature ___ _DETAIL_MULX2
#pragma shader_feature _PARALLAXMAP

#pragma multi_compile_prepassfinal
#pragma multi_compile_instancing
		// Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
		//#pragma multi_compile _ LOD_FADE_CROSSFADE

#pragma vertex vertDeferred_
#pragma fragment fragDeferred_

#include "UnityStandardCore.cginc"

			sampler2D _CutoffMap;
			sampler2D _TerrainMap;
			float4 _TerrainMap_ST;
			float4 _TerrainR;
			float4 _TerrainG;
			float4 _TerrainB;
			sampler2D _CameraDepthTexture;
			float _DebugPar;


			struct VertexOutputDeferred_
			{
				UNITY_POSITION(pos);
				float4 tex                            : TEXCOORD0;
				float3 eyeVec                         : TEXCOORD1;
				float4 tangentToWorldAndPackedData[3] : TEXCOORD2;    // [3x3:tangentToWorld | 1x3:viewDirForParallax or worldPos]
				half4 ambientOrLightmapUV             : TEXCOORD5;    // SH or Lightmap UVs

//#if UNITY_REQUIRE_FRAG_WORLDPOS && !UNITY_PACK_WORLDPOS_WITH_TANGENT
				float3 posWorld                     : TEXCOORD6;
//#endif

				UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
			};


	VertexOutputDeferred_ vertDeferred_(VertexInput v)
	{//important! Vertex
		v.normal = -v.normal;

		UNITY_SETUP_INSTANCE_ID(v);
		VertexOutputDeferred_ o;
		//UNITY_INITIALIZE_OUTPUT(VertexOutputDeferred, o);
		//UNITY_TRANSFER_INSTANCE_ID(v, o);
		//UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		float4 posWorld = mul(unity_ObjectToWorld, v.vertex);

#if UNITY_REQUIRE_FRAG_WORLDPOS
#if UNITY_PACK_WORLDPOS_WITH_TANGENT
		o.tangentToWorldAndPackedData[0].w = posWorld.x;
		o.tangentToWorldAndPackedData[1].w = posWorld.y;
		o.tangentToWorldAndPackedData[2].w = posWorld.z;
#else
		o.posWorld = posWorld.xyz;
#endif
#endif

		o.tex = TexCoords(v);
		o.eyeVec = -NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
		float3 normalWorld = UnityObjectToWorldNormal(v.normal);

		// biashere
		//posWorld += float4(0, dist/10, 0, 0);
		//float4 zeroWorld = mul(unity_ObjectToWorld, float4(0,0,0,1));
		//float4 biasNormal = SampleBiasNormal(v.vertex.xy, 0.04, zeroWorld.x + zeroWorld.y + zeroWorld.z);
		//posWorld += float4(normalWorld, 0) * biasNormal.w;
		//posWorld += float4(0, -GravityBias(v.vertex.xy), 0, 0);
		//normalWorld = UnityObjectToWorldNormal(biasNormal.xyz);

		float dist = distance(posWorld.xyz , _WorldSpaceCameraPos);
		posWorld += float4(0, dist /500, 0, 0);
		o.pos = UnityWorldToClipPos(posWorld);
		o.posWorld = mul(UNITY_MATRIX_M, v.vertex).xyz;

#ifdef _TANGENT_TO_WORLD
		float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

		float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
		o.tangentToWorldAndPackedData[0].xyz = tangentToWorld[0];
		o.tangentToWorldAndPackedData[1].xyz = tangentToWorld[1];
		o.tangentToWorldAndPackedData[2].xyz = tangentToWorld[2];
#else
		o.tangentToWorldAndPackedData[0].xyz = 0;
		o.tangentToWorldAndPackedData[1].xyz = 0;
		o.tangentToWorldAndPackedData[2].xyz = normalWorld;
#endif

		o.ambientOrLightmapUV = 0;
#ifdef LIGHTMAP_ON
		o.ambientOrLightmapUV.xy = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#elif UNITY_SHOULD_SAMPLE_SH
		o.ambientOrLightmapUV.rgb = ShadeSHPerVertex(normalWorld, o.ambientOrLightmapUV.rgb);
#endif
#ifdef DYNAMICLIGHTMAP_ON
		o.ambientOrLightmapUV.zw = v.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#endif

#ifdef _PARALLAXMAP
		TANGENT_SPACE_ROTATION;
		half3 viewDirForParallax = mul(rotation, ObjSpaceViewDir(v.vertex));
		o.tangentToWorldAndPackedData[0].w = viewDirForParallax.x;
		o.tangentToWorldAndPackedData[1].w = viewDirForParallax.y;
		o.tangentToWorldAndPackedData[2].w = viewDirForParallax.z;
#endif

		return o;
	}




	void fragDeferred_(
		VertexOutputDeferred_ i,
		out half4 outGBuffer0 : SV_Target0,
		out half4 outGBuffer1 : SV_Target1,
		out half4 outGBuffer2 : SV_Target2,
		out half4 outEmission : SV_Target3          // RT3: emission (rgb), --unused-- (a)
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
		,out half4 outShadowMask : SV_Target4       // RT4: shadowmask (rgba)
#endif
	)
	{
#if (SHADER_TARGET < 30)
		outGBuffer0 = 1;
		outGBuffer1 = 1;
		outGBuffer2 = 0;
		outEmission = 0;
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
		outShadowMask = 1;
#endif
		return;
#endif

		//here
		float2 scrpos = i.pos.xy / _ScreenParams.xy;
		float depth = tex2D(_CameraDepthTexture, scrpos).r;
		float cutoff = tex2D(_CutoffMap, scrpos).r;
		if (cutoff < 0.5) {
			discard;
		}
		if (LinearEyeDepth(i.pos.z) - LinearEyeDepth(depth)  > 2) {
			discard;
		}
		float4 colcoor = tex2D(_TerrainMap, i.posWorld.xz * _TerrainMap_ST.xy + _TerrainMap_ST.zw);
		float4 col = float4(_TerrainR.rgb * colcoor.r + _TerrainG.rgb * colcoor.g + _TerrainB.rgb * colcoor.b, 1);

		UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);

		FRAGMENT_SETUP(s)
			UNITY_SETUP_INSTANCE_ID(i);

		s.diffColor = col;

		// no analytic lights in this pass
		UnityLight dummyLight = DummyLight();
		half atten = 1;

		// only GI
		half occlusion = Occlusion(i.tex.xy);
#if UNITY_ENABLE_REFLECTION_BUFFERS
		bool sampleReflectionsInDeferred = false;
#else
		bool sampleReflectionsInDeferred = true;
#endif

		UnityGI gi = FragmentGI(s, occlusion, i.ambientOrLightmapUV, atten, dummyLight, sampleReflectionsInDeferred);

		half3 emissiveColor = UNITY_BRDF_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect).rgb;

#ifdef _EMISSION
		emissiveColor += Emission(i.tex.xy);
#endif

#ifndef UNITY_HDR_ON
		emissiveColor.rgb = exp2(-emissiveColor.rgb);
#endif

		UnityStandardData data;
		data.diffuseColor = s.diffColor;
		data.occlusion = occlusion;
		data.specularColor = s.specColor /*float4(_DebugPar,0,0,1)*/;
		data.smoothness = s.smoothness;
		data.normalWorld = s.normalWorld;

		UnityStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);

		// Emissive lighting buffer
		outEmission = half4(emissiveColor, 1);

		// Baked direct lighting occlusion if any
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
		outShadowMask = UnityGetRawBakedOcclusions(i.ambientOrLightmapUV.xy, IN_WORLDPOS(i));
#endif
	}

	ENDCG
	}


	}




		FallBack "VertexLit"
		//CustomEditor "StandardShaderGUI"
}
