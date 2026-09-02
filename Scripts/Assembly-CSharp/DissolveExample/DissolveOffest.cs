using System.Collections.Generic;
using UnityEngine;

namespace DissolveExample;

public class DissolveOffest : MonoBehaviour
{
	private List<Material> materials = new List<Material>();

	private bool PingPong;

	private void Start()
	{
		Renderer[] components = GetComponents<Renderer>();
		for (int i = 0; i < components.Length; i++)
		{
			materials.AddRange(components[i].materials);
		}
	}

	private void Reset()
	{
		Start();
		SetValue(0f);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.I))
		{
			PingPong = true;
		}
		if (PingPong)
		{
			float num = Mathf.PingPong(Time.time * 0.5f, 1.6f);
			num -= 0.5f;
			SetValue(num);
		}
	}

	public void SetValue(float value)
	{
		for (int i = 0; i < materials.Count; i++)
		{
			materials[i].SetVector("_DissolveOffest", new Vector4(0f, value, 0f, 0f));
		}
	}
}
