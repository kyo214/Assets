using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;

namespace Dissonance.Demo;

public class TriggerVisualizer : MonoBehaviour
{
	private GameObject _visualisations;

	private BaseCommsTrigger[] _triggers;

	private Material _fillMaterial;

	private Material _outlineMaterial;

	private float _alpha;

	public Color Color;

	private void Awake()
	{
		_visualisations = new GameObject("Trigger Visualisations");
		_visualisations.transform.parent = base.gameObject.transform;
		_visualisations.transform.localPosition = Vector3.zero;
		_visualisations.transform.localRotation = Quaternion.identity;
		_fillMaterial = UnityEngine.Object.Instantiate(Resources.Load<Material>("TriggerMaterial"));
		_outlineMaterial = UnityEngine.Object.Instantiate(Resources.Load<Material>("TriggerEdgeMaterial"));
		_triggers = GetComponents<BaseCommsTrigger>();
		SphereCollider[] components = GetComponents<SphereCollider>();
		foreach (SphereCollider sphere in components)
		{
			CreateCircle(sphere);
		}
		BoxCollider[] components2 = GetComponents<BoxCollider>();
		foreach (BoxCollider box in components2)
		{
			CreateBox(box);
		}
	}

	private void Update()
	{
		if (_triggers.Any((BaseCommsTrigger baseCommsTrigger) => baseCommsTrigger.CanTrigger))
		{
			_visualisations.SetActive(value: true);
			_alpha = (_triggers.Any((BaseCommsTrigger baseCommsTrigger) => baseCommsTrigger.IsColliderTriggered) ? Mathf.Clamp01(_alpha + Time.unscaledDeltaTime * 4f) : Mathf.Clamp01(_alpha - Time.unscaledDeltaTime * 4f));
			float t = Mathf.Lerp(0.7f, 1f, _alpha);
			Color value = Color.Lerp(default, Color, t);
			_fillMaterial.SetColor("_TintColor", value);
			_outlineMaterial.color = Color;
		}
		else
		{
			_visualisations.SetActive(value: false);
			_alpha = 1f;
		}
	}

	private void CreateCircle(SphereCollider sphere)
	{
		GameObject obj = new GameObject("sphere collider");
		obj.transform.parent = _visualisations.transform;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.identity;
		MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
		MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		List<Vector3> list = new List<Vector3> { Vector3.zero };
		for (int i = 0; i < 64; i++)
		{
			Vector3 item = new Vector3(sphere.radius * Mathf.Sin(MathF.PI * 2f * (float)i / 64f), 0.1f, sphere.radius * Mathf.Cos(MathF.PI * 2f * (float)i / 64f));
			list.Add(item);
		}
		List<Vector3> list2 = new List<Vector3>();
		for (int j = 0; j < list.Count; j++)
		{
			list2.Add(Vector3.up);
		}
		List<Color> list3 = new List<Color>();
		for (int k = 0; k < list.Count; k++)
		{
			list3.Add(new Color(1f, 1f, 1f, 0.2f));
		}
		List<int> list4 = new List<int>();
		for (int l = 0; l < 64; l++)
		{
			list4.Add(0);
			list4.Add(l);
			if (l < 63)
			{
				list4.Add(l + 1);
			}
			else
			{
				list4.Add(1);
			}
		}
		List<int> list5 = new List<int>();
		for (int m = 1; m < 64; m++)
		{
			list5.Add(m);
		}
		list5.Add(1);
		mesh.vertices = list.ToArray();
		mesh.normals = list2.ToArray();
		mesh.colors = list3.ToArray();
		mesh.subMeshCount = 2;
		mesh.SetIndices(list4.ToArray(), MeshTopology.Triangles, 0);
		mesh.SetIndices(list5.ToArray(), MeshTopology.LineStrip, 1);
		meshFilter.mesh = mesh;
		meshRenderer.materials = new Material[2] { _fillMaterial, _outlineMaterial };
		meshRenderer.receiveShadows = false;
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
	}

	private void CreateBox([NotNull] BoxCollider box)
	{
		GameObject obj = new GameObject("box collider");
		obj.transform.parent = _visualisations.transform;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.identity;
		MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
		MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		Vector3 vector = box.center - box.size * 0.5f;
		Vector3 vector2 = box.center + box.size * 0.5f;
		List<Vector3> list = new List<Vector3>
		{
			new Vector3(vector.x, 0.1f, vector.z),
			new Vector3(vector.x, 0.1f, vector2.z),
			new Vector3(vector2.x, 0.1f, vector2.z),
			new Vector3(vector2.x, 0.1f, vector.z)
		};
		List<Vector3> list2 = new List<Vector3>();
		for (int i = 0; i < list.Count; i++)
		{
			list2.Add(Vector3.up);
		}
		List<Color> list3 = new List<Color>();
		for (int j = 0; j < list.Count; j++)
		{
			list3.Add(new Color(1f, 1f, 1f, 0.2f));
		}
		List<int> list4 = new List<int> { 0, 1, 2, 2, 3, 0 };
		List<int> list5 = new List<int> { 0, 1, 2, 3, 0 };
		mesh.vertices = list.ToArray();
		mesh.normals = list2.ToArray();
		mesh.colors = list3.ToArray();
		mesh.subMeshCount = 2;
		mesh.SetIndices(list4.ToArray(), MeshTopology.Triangles, 0);
		mesh.SetIndices(list5.ToArray(), MeshTopology.LineStrip, 1);
		meshFilter.mesh = mesh;
		meshRenderer.materials = new Material[2] { _fillMaterial, _outlineMaterial };
		meshRenderer.receiveShadows = false;
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
	}
}
