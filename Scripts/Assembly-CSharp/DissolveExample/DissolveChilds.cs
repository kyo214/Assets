using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace DissolveExample;

public class DissolveChilds : MonoBehaviour
{
	public List<Material> material;

	private float dissolveValue;

	public float dissolvingDuration;

	public void ResetDissolveValue()
	{
		foreach (Material item in material)
		{
			item.SetFloat("_Dissolve", 0f);
		}
	}

	public void StartDissolve()
	{
		ResetDissolveValue();
		foreach (Material item in material)
		{
			item.DOFloat(0f, "_Dissolve", 0f).SetUpdate(UpdateType.Normal);
			item.DOFloat(1f, "_Dissolve", dissolvingDuration).SetUpdate(UpdateType.Normal);
		}
	}
}
