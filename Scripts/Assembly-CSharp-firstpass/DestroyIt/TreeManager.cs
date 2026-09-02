using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DestroyIt;

[RequireComponent(typeof(TerrainPreserver))]
[DisallowMultipleComponent]
public class TreeManager : MonoBehaviour
{
	[Tooltip("The terrain managed by this script. Leave empty to manage the current active terrain.")]
	public Terrain terrain;

	[Tooltip("Backs up the active terrain in the editor when you play the scene. This way if a crash occurs, you'll be able to restore from the backup and won't lose your placed trees, since the TreeManager replaces terrain trees with destructible stand-ins at runtime.")]
	public bool backupTerrain = true;

	[Tooltip("The folder where the stripped-down destructible terrain tree prototype prefabs are stored.\n\nYou can change this if you want to store your tree stand-in resources somewhere else.")]
	public string pathToStandIns = "Assets/DestroyIt/Resources/TreeStandIns/";

	[Tooltip("These are stripped-down tree prototype objects, containing only colliders and other essential components to make them destructible.\n\nYou don't need to change these - they are automatically generated when the Update Destructible Trees button is clicked.")]
	public List<DestructibleTree> destructibleTrees;

	[HideInInspector]
	public List<TreeReset> treesToReset;

	private static TreeManager _instance;

	private List<TreeInstance> currentTreeInstances;

	private TreeInstance[] originalTreeInstances;

	private bool isTerrainDataDirty;

	public static TreeManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<TreeManager>();
			}
			return _instance;
		}
	}

	private TreeManager()
	{
	}

	private void Awake()
	{
		SceneManager.activeSceneChanged += OnActiveSceneChanged;
	}

	private void Start()
	{
		if (destructibleTrees == null || destructibleTrees.Count == 0)
		{
			return;
		}
		if (terrain == null)
		{
			terrain = Terrain.activeTerrain;
		}
		if (terrain == null || terrain.terrainData == null)
		{
			Debug.LogWarning("No terrain to manage destructible trees on.");
			return;
		}
		TreePrototype[] treePrototypes = terrain.terrainData.treePrototypes;
		TreeInstance[] treeInstances = terrain.terrainData.treeInstances;
		if (treeInstances == null || treeInstances.Length == 0 || treePrototypes == null || treePrototypes.Length == 0)
		{
			Debug.LogWarning("No trees found on terrain. Nothing to manage.");
			return;
		}
		if (treePrototypes.Length != destructibleTrees.Count)
		{
			Debug.LogWarning("Tree prototypes do not match DestroyIt's tree stand-in prefabs. Please click the \"Update Trees\" button on the TreeManager script.");
			return;
		}
		for (int i = 0; i < treePrototypes.Length; i++)
		{
			if (destructibleTrees[i].Prefab == null || treePrototypes[i].prefab == null || treePrototypes[i].prefab.name != destructibleTrees[i].Prefab.name)
			{
				Debug.LogWarning("Tree prototype names do not match Destructible tree stand-in prefab names. You may need to click the \"Update Trees\" button on the TreeManager script.");
				return;
			}
		}
		originalTreeInstances = treeInstances;
		currentTreeInstances = new List<TreeInstance>(treeInstances);
		treesToReset = new List<TreeReset>();
		for (int j = 0; j < treeInstances.Length; j++)
		{
			TreeInstance tree = treeInstances[j];
			DestructibleTree destructibleTree = destructibleTrees.Find((DestructibleTree x) => x.prototypeIndex == tree.prototypeIndex);
			if (destructibleTree != null)
			{
				GameObject obj = UnityEngine.Object.Instantiate(destructibleTree.Prefab, terrain.transform.parent, worldPositionStays: true);
				obj.transform.position = terrain.WorldPositionOfTree(j);
				obj.transform.localScale = new Vector3(tree.widthScale, tree.heightScale, tree.widthScale);
				obj.transform.rotation = Quaternion.AngleAxis(tree.rotation * 57.29578f, Vector3.up);
			}
		}
	}

	private void FixedUpdate()
	{
		if (treesToReset == null || treesToReset.Count == 0)
		{
			return;
		}
		List<TreeReset> list = new List<TreeReset>();
		foreach (TreeReset item in treesToReset)
		{
			if (DateTime.Now >= item.resetTime)
			{
				TreeInstance instance = new TreeInstance
				{
					position = item.position,
					color = Color.white,
					heightScale = 1f,
					widthScale = 1f,
					prototypeIndex = item.prototypeIndex
				};
				terrain.AddTreeInstance(instance);
				list.Add(item);
			}
		}
		foreach (TreeReset item2 in list)
		{
			treesToReset.Remove(item2);
		}
	}

	private bool IsSpeedTree(GameObject treeObj)
	{
		MeshRenderer[] componentsInChildren = treeObj.gameObject.GetComponentsInChildren<MeshRenderer>();
		if (componentsInChildren == null || componentsInChildren.Length == 0)
		{
			return false;
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material[] sharedMaterials = componentsInChildren[i].sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				if (sharedMaterials[j].shader.name.Contains("SpeedTree"))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void DestroyTreeAt(Vector3 worldPoint)
	{
		TerrainTree terrainTree = terrain.ClosestTreeToPoint(worldPoint);
		if (terrainTree != null)
		{
			DestroyTree(terrainTree);
		}
	}

	private void DestroyTree(TerrainTree tree)
	{
		TreeInstance value = currentTreeInstances[tree.Index];
		value.heightScale = 0f;
		value.widthScale = 0f;
		currentTreeInstances[tree.Index] = value;
		terrain.terrainData.treeInstances = currentTreeInstances.ToArray();
	}

	public void RestoreTrees()
	{
		if (originalTreeInstances != null && !(terrain == null) && !(terrain.terrainData == null) && terrain.terrainData.treeInstances != null)
		{
			terrain.terrainData.treeInstances = originalTreeInstances;
		}
	}

	private void OnActiveSceneChanged(Scene current, Scene next)
	{
		RestoreTrees();
	}

	private void OnApplicationQuit()
	{
		RestoreTrees();
	}

	private void OnDestroy()
	{
		RestoreTrees();
	}

	private string GetMD5Hash(string input, int length)
	{
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] bytes = Encoding.UTF8.GetBytes(input);
		byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
		int num = ((length <= array.Length) ? length : array.Length);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < num; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}
}
