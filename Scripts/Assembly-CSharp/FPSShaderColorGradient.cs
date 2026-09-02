using UnityEngine;

public class FPSShaderColorGradient : MonoBehaviour
{
	public enum RFX4_ShaderProperties
	{
		_TintColor = 0,
		_Cutoff = 1,
		_Color = 2,
		_EmissionColor = 3,
		_MaskPow = 4,
		_Cutout = 5,
		_Speed = 6,
		_BumpAmt = 7,
		_MainColor = 8,
		_Distortion = 9,
		_FresnelColor = 10
	}

	public RFX4_ShaderProperties ShaderColorProperty;

	public Gradient Color = new Gradient();

	public float TimeMultiplier = 1f;

	public bool IsLoop;

	[HideInInspector]
	public bool canUpdate;

	private int propertyID;

	private float startTime;

	private Color startColor;

	private bool isInitialized;

	private string shaderProperty;

	private MaterialPropertyBlock props;

	private Renderer rend;

	private void Awake()
	{
		if (props == null)
		{
			props = new MaterialPropertyBlock();
		}
		if (rend == null)
		{
			rend = GetComponent<Renderer>();
		}
		shaderProperty = ShaderColorProperty.ToString();
		propertyID = Shader.PropertyToID(shaderProperty);
		startColor = rend.sharedMaterial.GetColor(propertyID);
	}

	private void OnEnable()
	{
		startTime = Time.time;
		canUpdate = true;
		rend.GetPropertyBlock(props);
		startColor = rend.sharedMaterial.GetColor(propertyID);
		props.SetColor(propertyID, startColor * Color.Evaluate(0f));
		rend.SetPropertyBlock(props);
	}

	private void Update()
	{
		rend.GetPropertyBlock(props);
		float num = Time.time - startTime;
		if (canUpdate)
		{
			Color color = Color.Evaluate(num / TimeMultiplier);
			props.SetColor(propertyID, color * startColor);
		}
		if (num >= TimeMultiplier)
		{
			if (IsLoop)
			{
				startTime = Time.time;
			}
			else
			{
				canUpdate = false;
			}
		}
		rend.SetPropertyBlock(props);
	}
}
