using System;
using UnityEngine;

public class FPSFireManager : MonoBehaviour
{
	[Serializable]
	public class ImpactInfo
	{
		public MaterialType.MaterialTypeEnum MaterialType;

		public GameObject ImpactEffect;
	}

	public ImpactInfo[] ImpactElemets = new ImpactInfo[0];

	[Space]
	public float BulletDistance = 100f;

	public GameObject ImpactEffect;

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && Physics.Raycast(new Ray(base.transform.position, base.transform.forward), out var hitInfo, BulletDistance))
		{
			GameObject impactEffect = GetImpactEffect(hitInfo.transform.gameObject);
			if (!(impactEffect == null))
			{
				GameObject obj = UnityEngine.Object.Instantiate(impactEffect, hitInfo.point, default);
				obj.transform.LookAt(hitInfo.point + hitInfo.normal);
				UnityEngine.Object.Destroy(obj, 20f);
				UnityEngine.Object.Destroy(UnityEngine.Object.Instantiate(ImpactEffect, base.transform.position, base.transform.rotation), 4f);
			}
		}
	}

	private GameObject GetImpactEffect(GameObject impactedGameObject)
	{
		MaterialType component = impactedGameObject.GetComponent<MaterialType>();
		if (component == null)
		{
			return null;
		}
		ImpactInfo[] impactElemets = ImpactElemets;
		foreach (ImpactInfo impactInfo in impactElemets)
		{
			if (impactInfo.MaterialType == component.TypeOfMaterial)
			{
				return impactInfo.ImpactEffect;
			}
		}
		return null;
	}
}
