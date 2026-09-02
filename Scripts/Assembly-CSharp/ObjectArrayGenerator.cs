using UnityEngine;

[ExecuteInEditMode]
public class ObjectArrayGenerator : MonoBehaviour
{
	[SerializeField]
	private GameObject[] _randomPrefabList;

	[Space(10f)]
	[SerializeField]
	private bool _useWestPrefab;

	[SerializeField]
	private GameObject _westPrefab;

	[Space(10f)]
	[SerializeField]
	private bool _useEastPrefab;

	[SerializeField]
	private GameObject _eastPrefab;

	[SerializeField]
	[Range(1f, 50f)]
	private int _objectArrayWidth;

	[SerializeField]
	[Range(1f, 50f)]
	private int _objectArrayLength;

	[SerializeField]
	private Vector3 _startPosition;

	[Space(10f)]
	[SerializeField]
	private bool _autoSpacing = true;

	[SerializeField]
	private Vector2 _prefabSpacing;

	private Vector3 _meshBoundSize;

	private void GeneratePrefabsButton()
	{
		GeneratePrefabArray();
	}

	private void GeneratePrefabArray()
	{
		DestroyAllChildren();
		if (_randomPrefabList == null || _randomPrefabList.Length == 0)
		{
			return;
		}
		_meshBoundSize = GetMeshBoundSize();
		Vector3 vector = (_autoSpacing ? _meshBoundSize : new Vector3(_prefabSpacing.x, 0f, _prefabSpacing.y));
		for (int i = 0; i < _objectArrayLength; i++)
		{
			Vector3 localPosition = _startPosition + Vector3.forward * vector.z * i;
			for (int j = 0; j < _objectArrayWidth; j++)
			{
				Object.Instantiate(GetPrefab(j, i), base.transform).transform.localPosition = localPosition;
				localPosition += Vector3.right * vector.x;
			}
		}
	}

	private void DestroyAllChildren()
	{
	}

	private Vector3 GetMeshBoundSize()
	{
		GameObject obj = _randomPrefabList[0];
		Transform transform = obj.transform;
		Mesh sharedMesh = obj.GetComponentInChildren<MeshFilter>().sharedMesh;
		if (sharedMesh == null)
		{
			return Vector3.zero;
		}
		Vector3 result = transform.TransformVector(sharedMesh.bounds.size);
		result.x = Mathf.Abs(result.x);
		result.y = Mathf.Abs(result.y);
		result.z = Mathf.Abs(result.z);
		return result;
	}

	private GameObject GetPrefab(int x, int z)
	{
		if (_objectArrayWidth > 1)
		{
			if (x == 0 && _westPrefab != null && _useWestPrefab)
			{
				return _westPrefab;
			}
			if (x == _objectArrayWidth - 1 && _eastPrefab != null && _useEastPrefab)
			{
				return _eastPrefab;
			}
		}
		int num = Random.Range(0, _randomPrefabList.Length);
		return _randomPrefabList[num];
	}
}
