using System.Collections.Generic;
using UnityEngine;

namespace _Modules.Cloth;

public class ClothWindController : MonoBehaviour
{
	[SerializeField]
	private List<ClothController> _clothController = new List<ClothController>();

	[SerializeField]
	private float _windPower = 1f;

	private Vector3 _windDirection;

	private void Start()
	{
		SetWind();
	}

	private void OnValidate()
	{
		SetWind();
	}

	private void OnDrawGizmos()
	{
		if (base.transform.hasChanged)
		{
			SetWind();
			base.transform.hasChanged = false;
		}
		DrawArrow.ForGizmo(base.transform.position, base.transform.rotation * Vector3.forward);
	}

	private void SetWind()
	{
		_windDirection = base.transform.rotation * Vector3.forward;
		Vector3 wind = _windDirection * _windPower;
		foreach (ClothController item in _clothController)
		{
			if (item != null)
			{
				item.SetWind(wind);
			}
		}
	}

	private void RemoveNullReference()
	{
		List<ClothController> list = new List<ClothController>();
		foreach (ClothController item in _clothController)
		{
			if (item != null)
			{
				list.Add(item);
			}
		}
		_clothController = list;
	}
}
