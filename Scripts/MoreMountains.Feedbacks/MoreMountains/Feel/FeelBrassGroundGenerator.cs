using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Feel;

public class FeelBrassGroundGenerator : MonoBehaviour
{
	[Header("Dimensions")]
	public int NumberOfRows = 10;

	public int NumberOfColumns = 10;

	public Vector3 Offset;

	public Vector3 DancerOffset;

	public AnimationCurve Amplitude;

	public float Width = 0.5f;

	public float Depth = 0.5f;

	public float MinRandom = 1f;

	public float MaxRandom = 2f;

	public float AmplitudeMultiplier = 2f;

	public int FloatingCubesAmount = 20;

	[Header("Air Cubes")]
	public int FloatingBlockChance = 3;

	public float MinHeight = 1f;

	public float MaxHeight = 5f;

	public float MinDistanceToDancer = 2f;

	public float MinScale = 0.5f;

	public float MaxScale = 2f;

	[Header("Materials")]
	public Material GroundMaterial;

	public Material GroundMaterialAlt1;

	public Material GroundMaterialAlt2;

	[Header("Bindings")]
	public MMRadioReceiver GroundPrefabToInstantiate;

	public Transform ParentContainer;

	public Transform Dancer;

	[Header("Behaviour")]
	public bool GenerateOnAwake;

	[Header("Debug")]
	[MMInspectorButton("GenerateGround")]
	public bool GenerateGroundBtn;

	protected MMRadioReceiver _receiver;

	protected Vector3 _wipPosition;

	protected string _wipName;

	protected int _counter;

	protected virtual void Awake()
	{
		if (GenerateOnAwake)
		{
			GenerateGround();
		}
	}

	protected virtual void GenerateGround()
	{
		int num = 0;
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < ParentContainer.transform.childCount; i++)
		{
			list.Add(ParentContainer.transform.GetChild(i).gameObject);
		}
		foreach (GameObject item in list)
		{
			num++;
			if (Application.isPlaying)
			{
				Object.Destroy(item.gameObject);
			}
			else
			{
				Object.DestroyImmediate(item.gameObject);
			}
		}
		_counter = 0;
		for (int j = 0; j < NumberOfRows; j++)
		{
			for (int k = 0; k < NumberOfColumns; k++)
			{
				_wipPosition.x = (float)j * Width;
				_wipPosition.y = 0f;
				_wipPosition.z = (float)k * Depth;
				_wipPosition += Offset;
				_wipName = "GroundBlock_" + _counter;
				InstantiateBlock(_wipPosition, _wipName);
				_counter++;
			}
		}
		for (int l = 0; l < NumberOfRows; l++)
		{
			for (int m = 0; m < NumberOfColumns; m++)
			{
				_wipPosition.x = (float)l * Width;
				_wipPosition.y = Random.Range(MinHeight, MaxHeight);
				_wipPosition.z = (float)m * Depth;
				_wipPosition += Offset;
				if (MMMaths.Chance(FloatingBlockChance) && Vector3.Distance(_wipPosition, Dancer.transform.position) > MinDistanceToDancer)
				{
					_wipName = "AirBlock_" + _counter;
					_receiver = InstantiateBlock(_wipPosition, _wipName);
					_receiver.transform.localScale = _receiver.transform.localScale * Random.Range(MinScale, MaxScale);
					_receiver.MinRandomLevelMultiplier *= 3f;
					_receiver.MaxRandomLevelMultiplier *= 3f;
					_receiver.gameObject.AddComponent<MMAutoRotate>().RotationSpeed = new Vector3(0f, 100f, 0f);
					_counter++;
				}
			}
		}
	}

	protected virtual MMRadioReceiver InstantiateBlock(Vector3 newPosition, string newName)
	{
		_receiver = Object.Instantiate(GroundPrefabToInstantiate, newPosition, Quaternion.identity, ParentContainer);
		if (ParentContainer == null)
		{
			SceneManager.MoveGameObjectToScene(_receiver.gameObject, base.gameObject.scene);
		}
		_receiver.name = newName;
		float x = Vector3.Distance(Dancer.transform.position + DancerOffset, newPosition);
		float b = Mathf.Max((float)NumberOfColumns * Depth, (float)NumberOfRows * Width) / 2f;
		float time = MMMaths.Remap(x, 0f, b, 0f, 1f);
		float num = Amplitude.Evaluate(time);
		float num2 = Random.Range(MinRandom, MaxRandom);
		num *= num2;
		num *= AmplitudeMultiplier;
		int channel = Random.Range(0, 2);
		_receiver.MinRandomLevelMultiplier = num;
		_receiver.MaxRandomLevelMultiplier = num;
		_receiver.GenerateRandomLevelMultiplier();
		_receiver.Channel = channel;
		float num3 = Random.Range(0f, 100f);
		if (num3 < 80f)
		{
			_receiver.GetComponent<MeshRenderer>().material = GroundMaterial;
		}
		else if (num3 < 90f)
		{
			_receiver.GetComponent<MeshRenderer>().material = GroundMaterialAlt1;
		}
		else
		{
			_receiver.GetComponent<MeshRenderer>().material = GroundMaterialAlt2;
		}
		_receiver.transform.position = newPosition;
		return _receiver;
	}
}
