using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Utilities/MMScreenshot")]
public class MMScreenshot : MonoBehaviour
{
	public enum Methods
	{
		ScreenCapture = 0,
		RenderTexture = 1
	}

	public string FolderName = "Screenshots";

	[Header("Screenshot")]
	public Methods Method;

	public Key ScreenshotKey = Key.K;

	[MMEnumCondition("Method", new int[] { 0 })]
	public int GameViewSizeMultiplier = 3;

	[MMEnumCondition("Method", new int[] { 1 })]
	public Camera TargetCamera;

	[MMEnumCondition("Method", new int[] { 1 })]
	public int ResolutionWidth;

	[MMEnumCondition("Method", new int[] { 1 })]
	public int ResolutionHeight;

	[Header("Controls")]
	[MMInspectorButton("TakeScreenshot")]
	public bool TakeScreenshotButton;

	protected virtual void LateUpdate()
	{
		DetectInput();
	}

	protected virtual void DetectInput()
	{
		if (Keyboard.current[ScreenshotKey].wasPressedThisFrame)
		{
			TakeScreenshot();
		}
	}

	protected virtual void TakeScreenshot()
	{
		if (!Directory.Exists(FolderName))
		{
			Directory.CreateDirectory(FolderName);
		}
		string text = "";
		switch (Method)
		{
		case Methods.ScreenCapture:
			text = TakeScreenCaptureScreenshot();
			break;
		case Methods.RenderTexture:
			text = TakeRenderTextureScreenshot();
			break;
		}
		Debug.Log("[MMScreenshot] Screenshot taken and saved at " + text);
	}

	protected virtual string TakeScreenCaptureScreenshot()
	{
		float num = Screen.width * GameViewSizeMultiplier;
		float num2 = Screen.height * GameViewSizeMultiplier;
		string text = FolderName + "/screenshot_" + num + "x" + num2 + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
		ScreenCapture.CaptureScreenshot(text, GameViewSizeMultiplier);
		return text;
	}

	protected virtual string TakeRenderTextureScreenshot()
	{
		string text = FolderName + "/screenshot_" + ResolutionWidth + "x" + ResolutionHeight + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
		RenderTexture renderTexture = new RenderTexture(ResolutionWidth, ResolutionHeight, 24);
		TargetCamera.targetTexture = renderTexture;
		Texture2D texture2D = new Texture2D(ResolutionWidth, ResolutionHeight, TextureFormat.RGB24, mipChain: false);
		TargetCamera.Render();
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, ResolutionWidth, ResolutionHeight), 0, 0);
		TargetCamera.targetTexture = null;
		RenderTexture.active = null;
		UnityEngine.Object.Destroy(renderTexture);
		byte[] bytes = texture2D.EncodeToPNG();
		File.WriteAllBytes(text, bytes);
		return text;
	}
}
