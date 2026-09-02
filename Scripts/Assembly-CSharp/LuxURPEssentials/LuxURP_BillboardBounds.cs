using UnityEngine;

namespace LuxURPEssentials;

[RequireComponent(typeof(MeshFilter))]
public class LuxURP_BillboardBounds : MonoBehaviour
{
	[Space(5f)]
	[LuxURP_HelpBtn("h.9i03ddhmnooa")]
	[Space(18f)]
	[SerializeField]
	[Tooltip("Scale of the tweaked bounding box.")]
	private Vector3 _Scale = new Vector3(1f, 1f, 1f);

	[SerializeField]
	[Tooltip("If checked Unity will instantiate the assigned mesh on Start().")]
	private bool _createUniqueMesh;

	[Space(8f)]
	[SerializeField]
	[Tooltip("Check this to preview the scaled bounding box.")]
	private bool _drawBounds = true;

	private Mesh _Mesh;

	private void Start()
	{
		if (_createUniqueMesh)
		{
			SetBounds();
		}
	}

	private void SetBounds()
	{
		if (_Mesh == null)
		{
			if (!_createUniqueMesh)
			{
				_Mesh = GetComponent<MeshFilter>().sharedMesh;
			}
			else
			{
				_Mesh = GetComponent<MeshFilter>().mesh;
			}
		}
		if (_Mesh != null)
		{
			_Mesh.RecalculateBounds();
			Bounds bounds = _Mesh.bounds;
			Vector3 size = bounds.size;
			size.x = _Scale.x;
			size.y = _Scale.y;
			size.z = _Scale.z;
			bounds.center = new Vector3(bounds.center.x, bounds.center.y, bounds.center.z);
			_Mesh.bounds = new Bounds(bounds.center, size);
			if (!_createUniqueMesh)
			{
				GetComponent<MeshFilter>().sharedMesh = _Mesh;
			}
			else
			{
				GetComponent<MeshFilter>().mesh = _Mesh;
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (_drawBounds)
		{
			if (_Mesh == null)
			{
				_Mesh = GetComponent<MeshFilter>().sharedMesh;
			}
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.red;
			Bounds bounds = _Mesh.bounds;
			Vector3 size = bounds.size;
			if (!Application.isPlaying)
			{
				size.x = _Scale.x;
				size.y = _Scale.y;
				size.z = _Scale.z;
				bounds.center = new Vector3(bounds.center.x, bounds.center.y, bounds.center.z);
			}
			Gizmos.DrawWireCube(bounds.center, size);
			Gizmos.matrix = Matrix4x4.identity;
		}
	}
}
