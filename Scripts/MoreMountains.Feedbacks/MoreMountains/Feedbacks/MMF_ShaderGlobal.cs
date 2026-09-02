using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback allows you to set global properties on your shader, or enable/disable keywords.")]
[FeedbackPath("Renderer/Shader Global")]
public class MMF_ShaderGlobal : MMF_Feedback
{
	public enum Modes
	{
		SetGlobalColor = 0,
		SetGlobalFloat = 1,
		SetGlobalInt = 2,
		SetGlobalMatrix = 3,
		SetGlobalTexture = 4,
		SetGlobalVector = 5,
		EnableKeyword = 6,
		DisableKeyword = 7,
		WarmupAllShaders = 8
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Shader Global", true, 24, false, false)]
	[Tooltip("the selected mode for this feedback")]
	public Modes Mode = Modes.SetGlobalFloat;

	[Tooltip("the name of the global property")]
	[MMFEnumCondition("Mode", new int[] { 0, 1, 2, 3, 4, 5 })]
	public string PropertyName = "";

	[Tooltip("the name ID of the property retrieved by Shader.PropertyToID")]
	[MMFEnumCondition("Mode", new int[] { 0, 1, 2, 3, 4, 5 })]
	public int PropertyNameID;

	[Tooltip("a global color property for all shaders")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public Color GlobalColor = Color.yellow;

	[Tooltip("a global float property for all shaders")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public float GlobalFloat = 1f;

	[Tooltip("a global int property for all shaders")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public int GlobalInt = 1;

	[Tooltip("a global matrix property for all shaders")]
	[MMFEnumCondition("Mode", new int[] { 3 })]
	public Matrix4x4 GlobalMatrix = Matrix4x4.identity;

	[Tooltip("a global texture property for all shaders")]
	[MMFEnumCondition("Mode", new int[] { 4 })]
	public RenderTexture GlobalTexture;

	[Tooltip("a global vector property for all shaders")]
	[MMFEnumCondition("Mode", new int[] { 5 })]
	public Vector4 GlobalVector;

	[Tooltip("a global shader keyword")]
	[MMFEnumCondition("Mode", new int[] { 6, 7 })]
	public string Keyword;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized)
		{
			return;
		}
		switch (Mode)
		{
		case Modes.SetGlobalColor:
			if (PropertyName == "")
			{
				Shader.SetGlobalColor(PropertyNameID, GlobalColor);
			}
			else
			{
				Shader.SetGlobalColor(PropertyName, GlobalColor);
			}
			break;
		case Modes.SetGlobalFloat:
			if (PropertyName == "")
			{
				Shader.SetGlobalFloat(PropertyNameID, GlobalFloat);
			}
			else
			{
				Shader.SetGlobalFloat(PropertyName, GlobalFloat);
			}
			break;
		case Modes.SetGlobalInt:
			if (PropertyName == "")
			{
				Shader.SetGlobalInt(PropertyNameID, GlobalInt);
			}
			else
			{
				Shader.SetGlobalInt(PropertyName, GlobalInt);
			}
			break;
		case Modes.SetGlobalMatrix:
			if (PropertyName == "")
			{
				Shader.SetGlobalMatrix(PropertyNameID, GlobalMatrix);
			}
			else
			{
				Shader.SetGlobalMatrix(PropertyName, GlobalMatrix);
			}
			break;
		case Modes.SetGlobalTexture:
			if (PropertyName == "")
			{
				Shader.SetGlobalTexture(PropertyNameID, GlobalTexture);
			}
			else
			{
				Shader.SetGlobalTexture(PropertyName, GlobalTexture);
			}
			break;
		case Modes.SetGlobalVector:
			if (PropertyName == "")
			{
				Shader.SetGlobalVector(PropertyNameID, GlobalVector);
			}
			else
			{
				Shader.SetGlobalVector(PropertyName, GlobalVector);
			}
			break;
		case Modes.EnableKeyword:
			Shader.EnableKeyword(Keyword);
			break;
		case Modes.DisableKeyword:
			Shader.DisableKeyword(Keyword);
			break;
		case Modes.WarmupAllShaders:
			Shader.WarmupAllShaders();
			break;
		}
	}
}
