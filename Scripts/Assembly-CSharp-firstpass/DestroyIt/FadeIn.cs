using UnityEngine;

namespace DestroyIt;

public class FadeIn : MonoBehaviour
{
	public Color startColor = Color.black;

	[Range(0f, 10f)]
	public float fadeLength = 2f;

	private Texture2D blackTexture;

	private float alphaFadeValue = 1f;

	private void Start()
	{
		blackTexture = new Texture2D(1, 1, TextureFormat.ARGB32, mipChain: false);
		blackTexture.SetPixel(0, 0, startColor);
		blackTexture.Apply();
	}

	private void Update()
	{
		alphaFadeValue -= Mathf.Clamp01(Time.deltaTime / fadeLength);
		if (alphaFadeValue <= 0f)
		{
			Object.Destroy(this);
		}
	}

	private void OnGUI()
	{
		GUI.color = new Color(alphaFadeValue, alphaFadeValue, alphaFadeValue, alphaFadeValue);
		GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), blackTexture);
	}
}
