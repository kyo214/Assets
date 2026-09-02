using System.Collections.Generic;
using UnityEngine;

namespace DissolveExample;

public class RotatorDissolveDir : MonoBehaviour
{
	public Vector3 Speed;

	private List<Material> materials = new List<Material>();

	private void Start()
	{
		materials.AddRange(GetComponent<Renderer>().materials);
	}

	private void Update()
	{
		for (int i = 0; i < materials.Count; i++)
		{
			Vector4 vector = materials[i].GetVector("_DissolveDirection");
			Vector3 vector2 = Speed * Time.deltaTime;
			vector += new Vector4(vector2.x, vector2.y, vector2.z, 0f);
			materials[i].SetVector("_DissolveDirection", vector);
		}
	}
}
