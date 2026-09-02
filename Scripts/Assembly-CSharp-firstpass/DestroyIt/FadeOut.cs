using System.Collections.Generic;
using UnityEngine;

namespace DestroyIt;

public class FadeOut : MonoBehaviour
{
	[Range(0f, 30f)]
	public float afterSeconds = 6f;

	[Range(0f, 10f)]
	public float fadeLength = 2f;

	private List<ObjectToFade> objectsToFade;

	private float timeLeft;

	private bool isInitialized;

	private bool isBeingDestroyed;

	private void Start()
	{
		timeLeft = afterSeconds;
		isInitialized = true;
		MeshRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<MeshRenderer>();
		if (componentsInChildren.Length == 0)
		{
			Debug.LogWarning("FadeOut: No MeshRenderers found under \"" + base.transform.name + "\". Cannot fade out.");
			Object.Destroy(this);
			return;
		}
		objectsToFade = new List<ObjectToFade>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			objectsToFade.Add(new ObjectToFade
			{
				MeshRenderer = componentsInChildren[i],
				Colliders = componentsInChildren[i].GetComponentsInChildren<Collider>(),
				Rigidbody = componentsInChildren[i].GetComponent<Rigidbody>(),
				CanBeFaded = true
			});
		}
	}

	private void OnEnable()
	{
		timeLeft = afterSeconds;
	}

	private void Update()
	{
		if (!isInitialized || isBeingDestroyed)
		{
			return;
		}
		timeLeft -= Time.deltaTime;
		if (timeLeft <= 0f)
		{
			if (timeLeft <= -1f * fadeLength)
			{
				isBeingDestroyed = true;
				Object.Destroy(base.transform.gameObject);
			}
			else
			{
				Fade();
			}
		}
	}

	private void StripColliders(ObjectToFade obj)
	{
		if (obj.Colliders.Length == 0)
		{
			obj.IsStripped = true;
			return;
		}
		for (int i = 0; i < obj.Colliders.Length; i++)
		{
			Object.Destroy(obj.Colliders[i]);
		}
		obj.IsStripped = true;
	}

	private void Fade()
	{
		foreach (ObjectToFade item in objectsToFade)
		{
			if (item.MeshRenderer == null)
			{
				continue;
			}
			if (!item.IsStripped)
			{
				if (item.Rigidbody == null)
				{
					StripColliders(item);
				}
				else if (item.Rigidbody.IsSleeping())
				{
					Object.Destroy(item.Rigidbody);
					StripColliders(item);
				}
			}
			if (!item.IsTransparencyChecked)
			{
				Material[] materials = item.MeshRenderer.materials;
				for (int i = 0; i < materials.Length; i++)
				{
					if (!materials[i].HasProperty("_Transparency"))
					{
						materials[i].shader = materials[i].shader.GetTransparentVersion();
						materials[i].SetFloat("_Transparency", 0f);
					}
				}
				item.MeshRenderer.materials = materials;
				item.IsTransparencyChecked = true;
			}
			for (int j = 0; j < item.MeshRenderer.materials.Length; j++)
			{
				float num = item.MeshRenderer.materials[j].GetFloat("_Transparency");
				if (!(num >= 1f))
				{
					num += Mathf.Clamp01(Time.deltaTime / fadeLength);
					item.MeshRenderer.materials[j].SetFloat("_Transparency", num);
				}
			}
		}
	}
}
