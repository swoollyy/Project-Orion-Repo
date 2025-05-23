using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Collections;

#if UNITY_6000_0_OR_NEWER
using System;
using UnityEngine.Rendering.RenderGraphModule;
#endif

/// <summary>
/// The volumetric fog render pass.
/// </summary>
public sealed class VolumetricFogRenderPass : ScriptableRenderPass
{
	#region Definitions

#if UNITY_6000_0_OR_NEWER

	/// <summary>
	/// The subpasses the volumetric fog render pass is made of.
	/// </summary>
	private enum PassStage : byte
	{
		DownsampleDepth,
		RenderFog,
		BlurFog,
		CompositeFog
	}

	/// <summary>
	/// Holds the data needed by the execution of the volumetric fog render pass subpasses.
	/// </summary>
	private class PassData
	{
		public PassStage stage;

		public TextureHandle target;
		public TextureHandle source;

		public Material material;
		public int materialPassIndex;

		public UniversalLightData lightData;
		public TextureHandle halfResCameraDepthTarget;
		public TextureHandle volumetricFogTarget;
	}

#endif

	#endregion

	#region Private Attributes

	private const int MaxPerCameraVisibleLights = 256;

	private static readonly float[] Anisotropies = new float[MaxPerCameraVisibleLights];
	private static readonly float[] Scatterings = new float[MaxPerCameraVisibleLights];
	private static readonly float[] RadiiSq = new float[MaxPerCameraVisibleLights];

	private static readonly int AnisotropiesArrayId = Shader.PropertyToID("_Anisotropies");
	private static readonly int ScatteringsArrayId = Shader.PropertyToID("_Scatterings");
	private static readonly int RadiiSqArrayId = Shader.PropertyToID("_RadiiSq");

	private static readonly int MainLightIndexId = Shader.PropertyToID("_MainLightIndex");
	private static readonly int FrameCountId = Shader.PropertyToID("_FrameCount");
	private static readonly int CustomAdditionalLightsCountId = Shader.PropertyToID("_CustomAdditionalLightsCount");
	private static readonly int DistanceId = Shader.PropertyToID("_Distance");
	private static readonly int BaseHeightId = Shader.PropertyToID("_BaseHeight");
	private static readonly int MaximumHeightId = Shader.PropertyToID("_MaximumHeight");
	private static readonly int GroundHeightId = Shader.PropertyToID("_GroundHeight");
	private static readonly int DensityId = Shader.PropertyToID("_Density");
	private static readonly int AbsortionId = Shader.PropertyToID("_Absortion");
	private static readonly int MainLightColorTintId = Shader.PropertyToID("_MainLightColorTint");
	private static readonly int MaxStepsId = Shader.PropertyToID("_MaxSteps");

	private static readonly int HalfResCameraDepthTextureId = Shader.PropertyToID("_HalfResCameraDepthTexture");
	private static readonly int VolumetricFogTextureId = Shader.PropertyToID("_VolumetricFogTexture");

	private Material downsampleDepthMaterial;
	private Material volumetricFogMaterial;

	private RTHandle halfResCameraDepthRTHandle;
	private RTHandle volumetricFogRenderRTHandle;
	private RTHandle volumetricFogAuxRenderRTHandle;
	private RTHandle volumetricFogCompositionRTHandle;

	private ProfilingSampler downsampleDepthProfilingSampler;

	#endregion

	#region Initialization Methods

	public VolumetricFogRenderPass(Material downsampleDepthMaterial, Material volumetricFogMaterial) : base()
	{
		// Use BeforeRenderingPostprocessing instead of AfterRenderingTransparents. It works better
		// with motion blur. BeforeRenderingTransparents is also an option depending on the needs.
		profilingSampler = new ProfilingSampler("Volumetric Fog");
		downsampleDepthProfilingSampler = new ProfilingSampler("Downsample Depth");
		renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

#if UNITY_6000_0_OR_NEWER
		requiresIntermediateTexture = false;
#endif

		this.downsampleDepthMaterial = downsampleDepthMaterial;
		this.volumetricFogMaterial = volumetricFogMaterial;
	}

	#endregion

	#region Scriptable Render Pass Methods

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="cmd"></param>
	/// <param name="renderingData"></param>
#if UNITY_6000_0_OR_NEWER
	[Obsolete]
#endif
	public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
	{
		base.OnCameraSetup(cmd, ref renderingData);

		RenderTextureDescriptor cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
		cameraTargetDescriptor.depthBufferBits = (int)DepthBits.None;

		RenderTextureFormat originalColorFormat = cameraTargetDescriptor.colorFormat;
		Vector2Int originalResolution = new Vector2Int(cameraTargetDescriptor.width, cameraTargetDescriptor.height);

		cameraTargetDescriptor.width /= 2;
		cameraTargetDescriptor.height /= 2;
		cameraTargetDescriptor.graphicsFormat = GraphicsFormat.R32_SFloat;
		RenderingUtils.ReAllocateIfNeeded(ref halfResCameraDepthRTHandle, cameraTargetDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_HalfResCameraDepth");

		cameraTargetDescriptor.colorFormat = RenderTextureFormat.ARGBHalf;
		RenderingUtils.ReAllocateIfNeeded(ref volumetricFogRenderRTHandle, cameraTargetDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_VolumetricFog");
		RenderingUtils.ReAllocateIfNeeded(ref volumetricFogAuxRenderRTHandle, cameraTargetDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_VolumetricFogAux");

		cameraTargetDescriptor.width = originalResolution.x;
		cameraTargetDescriptor.height = originalResolution.y;
		cameraTargetDescriptor.colorFormat = originalColorFormat;
		RenderingUtils.ReAllocateIfNeeded(ref volumetricFogCompositionRTHandle, cameraTargetDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_VolumetricFogComposition");
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="context"></param>
	/// <param name="renderingData"></param>
#if UNITY_6000_0_OR_NEWER
	[Obsolete]
#endif
	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
		CommandBuffer cmd = CommandBufferPool.Get();

		using (new ProfilingScope(cmd, downsampleDepthProfilingSampler))
		{
			Blitter.BlitCameraTexture(cmd, halfResCameraDepthRTHandle, halfResCameraDepthRTHandle, downsampleDepthMaterial, 0);
			volumetricFogMaterial.SetTexture(HalfResCameraDepthTextureId, halfResCameraDepthRTHandle);
		}

		using (new ProfilingScope(cmd, profilingSampler))
		{
			VolumetricFogVolumeComponent fogVolume = VolumeManager.instance.stack.GetComponent<VolumetricFogVolumeComponent>();

			int frameCount = Time.renderedFrameCount % 64;
			float absortion = 1.0f / fogVolume.attenuationDistance.value;

			bool enableMainLightContribution = fogVolume.enableMainLightContribution.value && fogVolume.mainLightScattering.value > 0.0f && renderingData.lightData.mainLightIndex > -1;
			EnableMainLightContribution(volumetricFogMaterial, enableMainLightContribution);
			EnableAdditionalLightsContribution(volumetricFogMaterial, fogVolume.enableAdditionalLightsContribution.value);
			UpdateLightsProperties(enableMainLightContribution, fogVolume, volumetricFogMaterial, renderingData.lightData.visibleLights, renderingData.lightData.mainLightIndex);
			volumetricFogMaterial.SetInteger(FrameCountId, frameCount);
			volumetricFogMaterial.SetInteger(MainLightIndexId, renderingData.lightData.mainLightIndex);
			volumetricFogMaterial.SetInteger(CustomAdditionalLightsCountId, renderingData.lightData.additionalLightsCount);
			volumetricFogMaterial.SetFloat(DistanceId, fogVolume.distance.value);
			volumetricFogMaterial.SetFloat(BaseHeightId, fogVolume.baseHeight.value);
			volumetricFogMaterial.SetFloat(MaximumHeightId, fogVolume.maximumHeight.value);
			UpdateGroundHeightFromMaterial(volumetricFogMaterial, fogVolume);
			volumetricFogMaterial.SetFloat(DensityId, fogVolume.density.value);
			volumetricFogMaterial.SetFloat(AbsortionId, absortion);
			volumetricFogMaterial.SetColor(MainLightColorTintId, fogVolume.mainLightColorTint.value);
			volumetricFogMaterial.SetInteger(MaxStepsId, fogVolume.maxSteps.value);

			Blitter.BlitCameraTexture(cmd, volumetricFogRenderRTHandle, volumetricFogRenderRTHandle, volumetricFogMaterial, 0);

			for (int i = 0; i < fogVolume.blurIterations.value; ++i)
			{
				Blitter.BlitCameraTexture(cmd, volumetricFogRenderRTHandle, volumetricFogAuxRenderRTHandle, volumetricFogMaterial, 1);
				Blitter.BlitCameraTexture(cmd, volumetricFogAuxRenderRTHandle, volumetricFogRenderRTHandle, volumetricFogMaterial, 2);
			}

			volumetricFogMaterial.SetTexture(VolumetricFogTextureId, volumetricFogRenderRTHandle);

			RTHandle cameraColorRt = renderingData.cameraData.renderer.cameraColorTargetHandle;
			Blitter.BlitCameraTexture(cmd, cameraColorRt, volumetricFogCompositionRTHandle, volumetricFogMaterial, 3);
			Blitter.BlitCameraTexture(cmd, volumetricFogCompositionRTHandle, cameraColorRt);
		}

		context.ExecuteCommandBuffer(cmd);

		cmd.Clear();

		CommandBufferPool.Release(cmd);
	}

#if UNITY_6000_0_OR_NEWER

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="renderGraph"></param>
	/// <param name="frameData"></param>
	public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
	{
		UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
		UniversalLightData lightData = frameData.Get<UniversalLightData>();
		UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

		CreateRenderGraphTextures(renderGraph, cameraData, out TextureHandle halfResCameraDepthTarget, out TextureHandle volumetricFogRenderTarget, out TextureHandle volumetricFogAuxRenderTarget, out TextureHandle volumetricFogCompositionTarget);

		using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass("Downsample Depth Pass", out PassData passData, downsampleDepthProfilingSampler))
		{
			passData.stage = PassStage.DownsampleDepth;
			passData.target = halfResCameraDepthTarget;
			passData.source = resourceData.cameraDepthTexture;
			passData.material = downsampleDepthMaterial;
			passData.materialPassIndex = 0;

			builder.SetRenderAttachment(halfResCameraDepthTarget, 0, AccessFlags.WriteAll);
			builder.UseTexture(resourceData.cameraDepthTexture);
			builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
		}

		using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass("Volumetric Fog Render Pass", out PassData passData, profilingSampler))
		{
			passData.stage = PassStage.RenderFog;
			passData.target = volumetricFogRenderTarget;
			passData.source = halfResCameraDepthTarget;
			passData.material = volumetricFogMaterial;
			passData.materialPassIndex = 0;
			passData.lightData = lightData;
			passData.halfResCameraDepthTarget = halfResCameraDepthTarget;

			builder.SetRenderAttachment(volumetricFogRenderTarget, 0, AccessFlags.WriteAll);
			builder.UseTexture(halfResCameraDepthTarget);
			if (resourceData.mainShadowsTexture.IsValid())
				builder.UseTexture(resourceData.mainShadowsTexture);
			if (resourceData.additionalShadowsTexture.IsValid())
				builder.UseTexture(resourceData.additionalShadowsTexture);
			builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
		}

		using (IUnsafeRenderGraphBuilder builder = renderGraph.AddUnsafePass("Volumetric Fog Blur Pass", out PassData passData, profilingSampler))
		{
			passData.stage = PassStage.BlurFog;
			passData.target = volumetricFogAuxRenderTarget;
			passData.source = volumetricFogRenderTarget;
			passData.material = volumetricFogMaterial;

			// Access flags are theoretically incorrect for one separable blur pass, but it is not
			// going to make any difference.
			builder.UseTexture(volumetricFogRenderTarget, AccessFlags.ReadWrite);
			builder.UseTexture(volumetricFogAuxRenderTarget, AccessFlags.ReadWrite);
			builder.SetRenderFunc((PassData data, UnsafeGraphContext context) => ExecuteUnsafeBlurPass(data, context));
		}

		using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass("Volumetric Fog Composition Pass", out PassData passData, profilingSampler))
		{
			passData.stage = PassStage.CompositeFog;
			passData.target = volumetricFogCompositionTarget;
			passData.source = resourceData.cameraColor;
			passData.material = volumetricFogMaterial;
			passData.materialPassIndex = 3;
			passData.halfResCameraDepthTarget = halfResCameraDepthTarget;
			passData.volumetricFogTarget = volumetricFogRenderTarget;

			builder.SetRenderAttachment(volumetricFogCompositionTarget, 0, AccessFlags.WriteAll);
			builder.UseTexture(resourceData.cameraColor);
			builder.UseTexture(resourceData.cameraDepthTexture);
			builder.UseTexture(halfResCameraDepthTarget);
			builder.UseTexture(volumetricFogRenderTarget);
			builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
		}

		resourceData.cameraColor = volumetricFogCompositionTarget;
	}

#endif

	#endregion

	#region Methods

	/// <summary>
	/// Enables or disables the computations from the main light to influence the volumetric fog.
	/// </summary>
	/// <param name="volumetricFogMaterial"></param>
	/// <param name="enabled"></param>
	private static void EnableMainLightContribution(Material volumetricFogMaterial, bool enabled)
	{
		if (enabled)
			volumetricFogMaterial.DisableKeyword("_MAIN_LIGHT_CONTRIBUTION_DISABLED");
		else
			volumetricFogMaterial.EnableKeyword("_MAIN_LIGHT_CONTRIBUTION_DISABLED");
	}

	/// <summary>
	/// Enables or disables the computations from additional lights to influence the volumetric fog.
	/// </summary>
	/// <param name="volumetricFogMaterial"></param>
	/// <param name="enabled"></param>
	private static void EnableAdditionalLightsContribution(Material volumetricFogMaterial, bool enabled)
	{
		if (enabled)
			volumetricFogMaterial.DisableKeyword("_ADDITIONAL_LIGHTS_CONTRIBUTION_DISABLED");
		else
			volumetricFogMaterial.EnableKeyword("_ADDITIONAL_LIGHTS_CONTRIBUTION_DISABLED");
	}

	/// <summary>
	/// Updates the lights parameters from the material.
	/// </summary>
	/// <param name="enableMainLightContribution"></param>
	/// <param name="fogVolume"></param>
	/// <param name="volumetricFogMaterial"></param>
	/// <param name="visibleLights"></param>
	/// <param name="mainLightIndex"></param>
	private static void UpdateLightsProperties(bool enableMainLightContribution, VolumetricFogVolumeComponent fogVolume, Material volumetricFogMaterial, NativeArray<VisibleLight> visibleLights, int mainLightIndex)
	{
		if (!enableMainLightContribution && !fogVolume.enableAdditionalLightsContribution.value)
			return;

		// ScriptableCullingParameters.maximumVisibleLights determines the length of this array.
		// In forward+, it defaults to 256, but deferred and deferred+ may have an even higher
		// amount set. Since this package works best in forward+ and deferred+ is still in alpha on
		// 6000.1, we just assume to have a maximum of 256.
		int length = visibleLights.Length;

		for (int i = 0; i < MaxPerCameraVisibleLights; ++i)
		{
			float anisotropy = 0.0f;
			float scattering = 0.0f;
			float radius = 0.0f;

			if (i < length)
			{
				if (i == mainLightIndex)
				{
					anisotropy = fogVolume.mainLightAnisotropy.value;
					scattering = fogVolume.mainLightScattering.value;
				}
				else if (visibleLights[i].light.TryGetComponent(out VolumetricAdditionalLight volumetricLight))
				{
					if (volumetricLight.enabled)
					{
						anisotropy = volumetricLight.Anisotropy;
						scattering = volumetricLight.Scattering;
						radius = volumetricLight.Radius;
					}
				}
			}

			Anisotropies[i] = anisotropy;
			Scatterings[i] = scattering;
			RadiiSq[i] = radius * radius;
		}

		volumetricFogMaterial.SetFloatArray(AnisotropiesArrayId, Anisotropies);
		volumetricFogMaterial.SetFloatArray(ScatteringsArrayId, Scatterings);
		volumetricFogMaterial.SetFloatArray(RadiiSqArrayId, RadiiSq);
	}

	/// <summary>
	/// Updates the ground height parameter from the material.
	/// </summary>
	/// <param name="volumetricFogMaterial"></param>
	/// <param name="volume"></param>
	private static void UpdateGroundHeightFromMaterial(Material volumetricFogMaterial, VolumetricFogVolumeComponent volume)
	{
		float groundValue = float.MinValue;
		groundValue = (volume.enableGround.overrideState && volume.enableGround.value) ? volume.groundHeight.value : groundValue;
		volumetricFogMaterial.SetFloat(GroundHeightId, groundValue);
	}

#if UNITY_6000_0_OR_NEWER

	/// <summary>
	/// Creates and returns all the necessary render graph textures.
	/// </summary>
	/// <param name="renderGraph"></param>
	/// <param name="cameraData"></param>
	/// <param name="halfResCameraDepthTarget"></param>
	/// <param name="volumetricFogRenderTarget"></param>
	/// <param name="volumetricFogAuxRenderTarget"></param>
	/// <param name="volumetricFogCompositionTarget"></param>
	private void CreateRenderGraphTextures(RenderGraph renderGraph, UniversalCameraData cameraData, out TextureHandle halfResCameraDepthTarget, out TextureHandle volumetricFogRenderTarget, out TextureHandle volumetricFogAuxRenderTarget, out TextureHandle volumetricFogCompositionTarget)
	{
		RenderTextureDescriptor cameraTargetDescriptor = cameraData.cameraTargetDescriptor;
		cameraTargetDescriptor.depthBufferBits = (int)DepthBits.None;

		RenderTextureFormat originalColorFormat = cameraTargetDescriptor.colorFormat;
		Vector2Int originalResolution = new Vector2Int(cameraTargetDescriptor.width, cameraTargetDescriptor.height);

		cameraTargetDescriptor.width /= 2;
		cameraTargetDescriptor.height /= 2;
		cameraTargetDescriptor.graphicsFormat = GraphicsFormat.R32_SFloat;
		halfResCameraDepthTarget = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor, "_HalfResCameraDepth", false);

		cameraTargetDescriptor.colorFormat = RenderTextureFormat.ARGBHalf;
		volumetricFogRenderTarget = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor, "_VolumetricFog", false, FilterMode.Bilinear);
		volumetricFogAuxRenderTarget = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor, "_VolumetricFogAux", false, FilterMode.Bilinear);

		cameraTargetDescriptor.width = originalResolution.x;
		cameraTargetDescriptor.height = originalResolution.y;
		cameraTargetDescriptor.colorFormat = originalColorFormat;
		volumetricFogCompositionTarget = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor, "_VolumetricFogComposition", false);
	}

	/// <summary>
	/// Updates the material properties that are needed to render the volumetric fog.
	/// </summary>
	/// <param name="passData"></param>
	private static void UpdateVolumetricFogMaterialProperties(PassData passData)
	{
		PassStage stage = passData.stage;

		if (stage == PassStage.RenderFog)
		{
			VolumetricFogVolumeComponent fogVolume = VolumeManager.instance.stack.GetComponent<VolumetricFogVolumeComponent>();

			int frameCount = Time.renderedFrameCount % 64;
			float absortion = 1.0f / fogVolume.attenuationDistance.value;

			Material volumetricFogMaterial = passData.material;
			bool enableMainLightContribution = fogVolume.enableMainLightContribution.value && fogVolume.mainLightScattering.value > 0.0f && passData.lightData.mainLightIndex > -1;
			EnableMainLightContribution(volumetricFogMaterial, enableMainLightContribution);
			EnableAdditionalLightsContribution(volumetricFogMaterial, fogVolume.enableAdditionalLightsContribution.value);
			UpdateLightsProperties(enableMainLightContribution, fogVolume, volumetricFogMaterial, passData.lightData.visibleLights, passData.lightData.mainLightIndex);
			volumetricFogMaterial.SetTexture(HalfResCameraDepthTextureId, passData.halfResCameraDepthTarget);
			volumetricFogMaterial.SetInteger(FrameCountId, frameCount);
			volumetricFogMaterial.SetInteger(MainLightIndexId, passData.lightData.mainLightIndex);
			volumetricFogMaterial.SetInteger(CustomAdditionalLightsCountId, passData.lightData.additionalLightsCount);
			volumetricFogMaterial.SetFloat(DistanceId, fogVolume.distance.value);
			volumetricFogMaterial.SetFloat(BaseHeightId, fogVolume.baseHeight.value);
			volumetricFogMaterial.SetFloat(MaximumHeightId, fogVolume.maximumHeight.value);
			UpdateGroundHeightFromMaterial(volumetricFogMaterial, fogVolume);
			volumetricFogMaterial.SetFloat(DensityId, fogVolume.density.value);
			volumetricFogMaterial.SetFloat(AbsortionId, absortion);
			volumetricFogMaterial.SetColor(MainLightColorTintId, fogVolume.mainLightColorTint.value);
			volumetricFogMaterial.SetInteger(MaxStepsId, fogVolume.maxSteps.value);
		}
		else if (stage == PassStage.CompositeFog)
		{
			Material volumetricFogMaterial = passData.material;
			volumetricFogMaterial.SetTexture(VolumetricFogTextureId, passData.volumetricFogTarget);
		}
	}

	/// <summary>
	/// Executes the pass with the information from the pass data.
	/// </summary>
	/// <param name="passData"></param>
	/// <param name="context"></param>
	private static void ExecutePass(PassData passData, RasterGraphContext context)
	{
		UpdateVolumetricFogMaterialProperties(passData);

		Blitter.BlitTexture(context.cmd, passData.source, Vector2.one, passData.material, passData.materialPassIndex);
	}

	/// <summary>
	/// Executes the unsafe pass that does up to multiple separable blurs to the volumetric fog.
	/// </summary>
	/// <param name="passData"></param>
	/// <param name="context"></param>
	private static void ExecuteUnsafeBlurPass(PassData passData, UnsafeGraphContext context)
	{
		CommandBuffer unsafeCmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);

		TextureHandle source = passData.source;
		TextureHandle target = passData.target;

		int blurIterations = VolumeManager.instance.stack.GetComponent<VolumetricFogVolumeComponent>().blurIterations.value;

		for (int i = 0; i < blurIterations; ++i)
		{
			Blitter.BlitCameraTexture(unsafeCmd, source, target, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, passData.material, 1);
			Blitter.BlitCameraTexture(unsafeCmd, target, source, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, passData.material, 2);
		}
	}

#endif

	/// <summary>
	/// Disposes the resources used by this pass.
	/// </summary>
	public void Dispose()
	{
		halfResCameraDepthRTHandle?.Release();
		volumetricFogRenderRTHandle?.Release();
		volumetricFogAuxRenderRTHandle?.Release();
		volumetricFogCompositionRTHandle?.Release();
	}

	#endregion
}