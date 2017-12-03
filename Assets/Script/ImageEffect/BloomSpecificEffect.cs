using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]
[AddComponentMenu ("Image Effects/Bloom and Glow/SpecificBloom")]
public class BloomSpecificEffect : PostEffectsBase {

	public enum Resolution
	{
		Low = 0,
		High = 1,
	}

	public enum BlurType
	{
		Standard = 0,
		Sgx = 1,
	}

	public int EffectLayer;

	[Range(0.0f, 1.5f)]
	public float threshold = 0.25f;
	[Range(0.0f, 2.5f)]
	public float intensity = 0.75f;

	[Range(0.25f, 5.5f)]
	public float blurSize = 1.0f;

	Resolution resolution = Resolution.Low;
	[Range(1, 4)]
	public int blurIterations = 1;

	public BlurType blurType= BlurType.Standard;

	public Shader fastBloomShader = null;
	private Material fastBloomMaterial = null;

	public Shader cutShader = null;
	private Material cutMaterial = null;

	private RenderTexture cutRT;
	private RenderTexture mainRT;

	public override bool CheckResources ()
	{
		CheckSupport (false);

		fastBloomMaterial = CheckShaderAndCreateMaterial (fastBloomShader, fastBloomMaterial);
		cutMaterial = CheckShaderAndCreateMaterial (cutShader, cutMaterial);

		cutMaterial.SetFloat ("_CutLayer", 1f * EffectLayer);

		if (!isSupported)
			ReportAutoDisable ();
		return isSupported;
	}

	void OnEnable()
	{
//		CreateRT ();

//		GetComponent<Camera> ().targetTexture = mainRT;
		GetComponent<Camera> ().depthTextureMode |= DepthTextureMode.Depth;

	}

	public void CreateRT()
	{
		RenderTextureFormat format = RenderTextureFormat.Default;
		if (SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.ARGBHalf)) {
			format = RenderTextureFormat.ARGBHalf;
		}

		mainRT = RenderTexture.GetTemporary (Screen.width, Screen.height, 24 , format);
		mainRT.Create ();

		cutRT = RenderTexture.GetTemporary (Screen.width, Screen.height, 24 , format);
		cutRT.Create ();

	}

	void OnDisable ()
	{
		if (fastBloomMaterial)
			DestroyImmediate (fastBloomMaterial);

//		GetComponent<Camera> ().targetTexture = null;
//		RenderTexture.ReleaseTemporary (mainRT);
//		RenderTexture.ReleaseTemporary (cutRT);
	}



	public void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if (CheckResources () == false) {
			Graphics.Blit (source, destination);
			return;
		}

		Graphics.Blit (source, destination, cutMaterial );

	}

	public void OnPostRender () {

//		GetComponent<Camera> ().targetTexture = mainRT;
//
//		cutRT.DiscardContents ();
//		Graphics.SetRenderTarget (cutRT);
//		GL.Clear (true, true, new Color (0, 0, 0, 0));
//		Graphics.SetRenderTarget (cutRT.colorBuffer, mainRT.depthBuffer);
//
//		Graphics.Blit (mainRT, cutMaterial);
	}
}
