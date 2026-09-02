using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Tools;

public class MMSceneLoadingTextProgress : MonoBehaviour
{
	[Tooltip("the value to which the progress' zero value should be remapped to")]
	public float RemapMin;

	[Tooltip("the value to which the progress' one value should be remapped to")]
	public float RemapMax = 100f;

	[Tooltip("the amount of decimals to display")]
	public int NumberOfDecimals;

	protected Text _text;

	protected virtual void Awake()
	{
		_text = base.gameObject.GetComponent<Text>();
	}

	public virtual void SetProgress(float newValue)
	{
		float num = MMMaths.RoundToDecimal(MMMaths.Remap(newValue, 0f, 1f, RemapMin, RemapMax), NumberOfDecimals);
		_text.text = num.ToString(CultureInfo.InvariantCulture);
	}
}
