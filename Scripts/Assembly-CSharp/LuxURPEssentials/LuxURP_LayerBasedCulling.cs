using UnityEngine;

namespace LuxURPEssentials;

public class LuxURP_LayerBasedCulling : MonoBehaviour
{
	[LuxURP_HelpBtn("h.2uxuzzrgrwpo", order = 0)]
	[Space(5f, order = 1)]
	public LayerMask SmallDetailsLayer;

	public float SmallDetailsDistance = 30f;

	public LayerMask MediumDetailsLayer;

	public float MediumDetailsDistance = 50f;

	private int GetLayerNumber(int LayerValue)
	{
		int num = 0;
		int num2 = LayerValue;
		while (num2 > 0)
		{
			num2 >>= 1;
			num++;
		}
		return num - 1;
	}

	private void OnEnable()
	{
		int layerNumber = GetLayerNumber(SmallDetailsLayer.value);
		int layerNumber2 = GetLayerNumber(MediumDetailsLayer.value);
		for (int i = 0; i < Camera.allCameras.Length; i++)
		{
			float[] array = new float[32];
			array = Camera.allCameras[i].layerCullDistances;
			if (layerNumber > 0)
			{
				array[layerNumber] = SmallDetailsDistance;
			}
			if (layerNumber2 > 0)
			{
				array[layerNumber2] = MediumDetailsDistance;
			}
			Camera.allCameras[i].layerCullDistances = array;
			array = null;
		}
	}
}
