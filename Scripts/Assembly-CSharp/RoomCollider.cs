using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using VolumetricFogAndMist2;

public class RoomCollider : MonoBehaviour
{
	[SerializeField]
	private List<Light> lightList = new List<Light>();

	[SerializeField]
	private List<MeshRenderer> listMeshEmission = new List<MeshRenderer>();

	[SerializeField]
	private List<Color> listEmissionColor = new List<Color>();

	[SerializeField]
	private List<GameObject> emitter = new List<GameObject>();

	public List<Animator> animatedObjectList = new List<Animator>();

	public List<VolumetricFog> FogObjectList = new List<VolumetricFog>();

	[SerializeField]
	private List<float> lightIntensity = new List<float>();

	public List<ItemPickable> itemList = new List<ItemPickable>();

	public List<ItemInteractable> interactionList = new List<ItemInteractable>();

	public Collider col;

	public bool isCollided;

	public bool isRevealed;

	public bool isComplete;

	public bool isRevealedByAllPlayer;

	public bool canHorde = true;

	public List<bool> listPlayerCollided = new List<bool>();

	public string RoomName;

	public List<BoxCollider> boxColliders = new List<BoxCollider>();

	public int maxEnemy = 8;

	public int minEnemyStay = 2;

	public int minEnemyRoaming = 5;

	public List<EnemyController> listEnemy = new List<EnemyController>();

	public Transform gameObjectMapSquare;

	public MaterialPropertyBlock MPB;

	private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

	public List<SpriteRenderer> arrMapSquare = new List<SpriteRenderer>();

	private void Awake()
	{
		MPB = new MaterialPropertyBlock();
		if (gameObjectMapSquare != null)
		{
			arrMapSquare.Clear();
			for (int i = 0; i < gameObjectMapSquare.childCount; i++)
			{
				arrMapSquare.Add(gameObjectMapSquare.GetChild(i).GetComponent<SpriteRenderer>());
				arrMapSquare[i].enabled = false;
			}
		}
		BoxCollider[] componentsInChildren = GetComponentsInChildren<BoxCollider>();
		GameObject[] array = GameObject.FindGameObjectsWithTag("LightRoom");
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			boxColliders.Add(componentsInChildren[j]);
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				if (!componentsInChildren[j].bounds.Contains(gameObject.transform.position))
				{
					continue;
				}
				Light component = gameObject.GetComponent<Light>();
				if (component != null)
				{
					lightList.Add(component);
					lightIntensity.Add(lightList[lightList.Count - 1].intensity);
					lightList[lightList.Count - 1].intensity = 0f;
					lightList[lightList.Count - 1].gameObject.SetActive(value: false);
					continue;
				}
				MeshRenderer component2 = gameObject.GetComponent<MeshRenderer>();
				if (component2 != null)
				{
					listMeshEmission.Add(component2);
					listEmissionColor.Add(component2.material.GetColor(EmissionColor) / (component2.material.GetColor(EmissionColor).maxColorComponent / 7f));
				}
			}
		}
		for (int l = 0; l < 9; l++)
		{
			listPlayerCollided.Add(item: false);
		}
		if (GameManager.Instance != null && GameManager.Instance.enableRoomFogOfWar)
		{
			GameManager.Instance.arrRoom.Add(this);
			for (int m = 0; m < base.transform.childCount; m++)
			{
				if (base.transform.GetChild(m).GetComponent<Light>() != null)
				{
					lightList.Add(base.transform.GetChild(m).GetComponent<Light>());
					lightIntensity.Add(lightList[lightList.Count - 1].intensity);
					lightList[lightList.Count - 1].intensity = 0f;
					lightList[lightList.Count - 1].gameObject.SetActive(value: false);
				}
			}
			for (int n = 0; n < listMeshEmission.Count; n++)
			{
				listMeshEmission[n].GetPropertyBlock(MPB);
				MPB.SetColor(EmissionColor, Color.black);
				listMeshEmission[n].SetPropertyBlock(MPB);
			}
			Transform transform = base.transform.parent.Find("AnimatedObjects");
			if (transform != null && transform.gameObject.activeSelf)
			{
				for (int num = 0; num < transform.childCount; num++)
				{
					if (transform.GetChild(num).GetComponent<Animator>() != null)
					{
						animatedObjectList.Add(transform.GetChild(num).GetComponent<Animator>());
						if (GlobalSaveData.instance != null && GlobalSaveData.instance.optionData.graphic == 2 && animatedObjectList[animatedObjectList.Count - 1].enabled)
						{
							animatedObjectList[animatedObjectList.Count - 1].Play("Animated", -1, UnityEngine.Random.Range(0f, 1f));
						}
					}
				}
			}
			FogObjectList = base.transform.parent.GetComponentsInChildren<VolumetricFog>().ToList();
			foreach (VolumetricFog fogObject in FogObjectList)
			{
				fogObject.enabled = false;
			}
		}
		RoomName = base.transform.parent.name;
		int num2 = RoomName.IndexOf("-");
		if (num2 > 0)
		{
			RoomName = RoomName.Remove(num2);
		}
		num2 = RoomName.IndexOf("_");
		if (num2 > 0)
		{
			RoomName = RoomName.Remove(num2);
		}
		GameManager.Instance.arrRoom = GameManager.Instance.arrRoom.OrderBy((RoomCollider room) => room.RoomName, StringComparer.Ordinal).ToList();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("OclusionLightCollider"))
		{
			OclusionLightCollider component = other.GetComponent<OclusionLightCollider>();
			bool flag = false;
			if (component != null)
			{
				flag = CheckRoom(component.roomColliders);
				component.roomColliders.Add(RoomName);
			}
			if (!flag)
			{
				foreach (Light light in lightList)
				{
					light.gameObject.SetActive(value: true);
				}
				foreach (Animator animatedObject in animatedObjectList)
				{
					animatedObject.gameObject.SetActive(value: true);
					animatedObject.Play("Animated", -1, UnityEngine.Random.Range(0f, 1f));
				}
				foreach (VolumetricFog fogObject in FogObjectList)
				{
					fogObject.enabled = true;
				}
			}
		}
		if (other.CompareTag("Player"))
		{
			PlayerController component2 = other.transform.GetComponent<PlayerController>();
			component2.RoomName = RoomName;
			bool flag2 = CheckRoom(component2.roomColliders);
			component2.roomColliders.Add(RoomName);
			CheckMap(component2);
			if ((bool)NetworkGameManager.Instance.ownPlayer && component2.network.GetIDX() == NetworkGameManager.Instance.ownPlayer.network.GetIdxTargetCam())
			{
				foreach (ItemPickable item in itemList.ToList())
				{
					if (item == null)
					{
						itemList.Remove(item);
					}
					else if (item.itemCollider.enabled)
					{
						item.SetSpriteEnable(value: true);
					}
				}
			}
			if (!flag2)
			{
				foreach (EnemyController item2 in listEnemy)
				{
					if ((bool)item2 && !item2.aiPath.enabled && !item2.attack.fov.isDisable && !item2.isDead && item2.network.GetHealth() > 0f && !item2.isDown)
					{
						item2.AIEnable = true;
						item2.SetEnableAI(value: true);
					}
				}
				if ((bool)NetworkGameManager.Instance.ownPlayer && component2.network.GetIDX() == NetworkGameManager.Instance.ownPlayer.network.GetIdxTargetCam())
				{
					listPlayerCollided[component2.network.GetIDX()] = true;
					TurnOnLight(SetItemVisiblity: false);
					isCollided = true;
					for (int i = 0; i < 9; i++)
					{
						if (!listPlayerCollided[i])
						{
							continue;
						}
						PlayerController player = NetworkGameManager.Instance.GetPlayer(i);
						if (player != null)
						{
							for (int j = 0; j < player.allLights.Count; j++)
							{
								player.allLights[j].DOIntensity(component2.allLightIntensity[j], 0.5f);
							}
							UIGameManager.Instance.ArrPlayerInfo[player.network.GetIDX()].gameObject.SetActive(value: true);
						}
					}
				}
				else
				{
					listPlayerCollided[component2.network.GetIDX()] = true;
					if (isCollided)
					{
						for (int k = 0; k < component2.allLights.Count; k++)
						{
							component2.allLights[k].DOIntensity(component2.allLightIntensity[k], 0.5f);
						}
						UIGameManager.Instance.ArrPlayerInfo[component2.network.GetIDX()].gameObject.SetActive(value: true);
					}
				}
			}
		}
		if (!other.CompareTag("EnemyLightCollider"))
		{
			return;
		}
		EnemyLightCollider component3 = other.GetComponent<EnemyLightCollider>();
		if (!(component3 != null))
		{
			return;
		}
		EnemyController enemyController = component3.enemyController;
		bool flag3 = false;
		foreach (EnemyController item3 in listEnemy)
		{
			if (item3 == enemyController)
			{
				flag3 = true;
			}
		}
		if (!flag3)
		{
			listEnemy.Add(enemyController);
		}
		bool flag4 = CheckRoom(enemyController.roomColliders);
		if (!flag4)
		{
			enemyController.roomColliders.Add(RoomName);
		}
		if (!isCollided || flag4 || enemyController.network.IsNonActive() || !(enemyController.network.GetHealth() > 0f) || (enemyController.attack.fov.isDisable && NetworkGameManager.Instance.isServer))
		{
			return;
		}
		foreach (SpriteRenderer item4 in enemyController.eyeGlow)
		{
			item4.enabled = true;
		}
		enemyController.VisibleSprite();
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("OclusionLightCollider"))
		{
			OclusionLightCollider component = other.GetComponent<OclusionLightCollider>();
			int num = CtrCheckRoom(component.roomColliders);
			component.roomColliders.Remove(RoomName);
			if (num == 1)
			{
				foreach (Light light in lightList)
				{
					light.gameObject.SetActive(value: false);
				}
				foreach (Animator animatedObject in animatedObjectList)
				{
					animatedObject.gameObject.SetActive(value: false);
				}
				foreach (VolumetricFog fogObject in FogObjectList)
				{
					fogObject.enabled = false;
				}
			}
		}
		if (other.CompareTag("EnemyLightCollider") && other.GetComponent<EnemyLightCollider>() != null)
		{
			EnemyController enemyController = other.GetComponent<EnemyLightCollider>().enemyController;
			int num2 = CtrCheckRoom(enemyController.roomColliders);
			if (num2 == 1 && enemyController.roomColliders.Count > 1)
			{
				listEnemy.Remove(enemyController);
				enemyController.roomColliders.Remove(RoomName);
			}
			if (isCollided && num2 == 1 && !enemyController.isDead)
			{
				foreach (SpriteRenderer item in enemyController.eyeGlow)
				{
					item.enabled = false;
				}
				if (!enemyController.isSpriteInactive)
				{
					bool flag = false;
					foreach (Transform visibleTarget in NetworkGameManager.Instance.GetPlayer(NetworkGameManager.Instance.ownPlayer.network.GetIdxTargetCam()).fov.visibleTargets)
					{
						if (visibleTarget == enemyController.colliderFOV.transform)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						enemyController.isSpriteInactive = true;
						enemyController.HideSprite();
					}
				}
			}
		}
		if (!other.CompareTag("Player"))
		{
			return;
		}
		PlayerController component2 = other.GetComponent<PlayerController>();
		int num3 = CtrCheckRoom(component2.roomColliders);
		component2.roomColliders.Remove(RoomName);
		if (num3 != 1)
		{
			return;
		}
		if (component2.network.GetIDX() == NetworkGameManager.Instance.ownPlayer.network.GetIdxTargetCam())
		{
			if (!(component2.network.GetHealth() > 0f))
			{
				return;
			}
			listPlayerCollided[component2.network.GetIDX()] = false;
			TurnOffLight();
			isCollided = false;
			for (int i = 0; i < 9; i++)
			{
				if (!listPlayerCollided[i])
				{
					continue;
				}
				PlayerController player = NetworkGameManager.Instance.GetPlayer(i);
				int num4 = 0;
				if (!(player != null))
				{
					continue;
				}
				foreach (RoomCollider item2 in GameManager.Instance.arrRoom)
				{
					if (item2.listPlayerCollided[player.network.GetIDX()] && item2.isCollided && item2 != this)
					{
						num4++;
					}
				}
				if (num4 == 0)
				{
					for (int j = 0; j < player.allLights.Count; j++)
					{
						player.allLights[j].DOIntensity(0f, 0.5f);
					}
					UIGameManager.Instance.ArrPlayerInfo[i].gameObject.SetActive(value: false);
				}
			}
			return;
		}
		int num5 = 0;
		listPlayerCollided[component2.network.GetIDX()] = false;
		foreach (RoomCollider item3 in GameManager.Instance.arrRoom)
		{
			if (item3.listPlayerCollided[component2.network.GetIDX()] && item3.isCollided && item3 != this)
			{
				num5++;
			}
		}
		if (num5 == 0 && isCollided)
		{
			for (int k = 0; k < component2.allLights.Count; k++)
			{
				component2.allLights[k].DOIntensity(0f, 0.5f);
			}
			UIGameManager.Instance.ArrPlayerInfo[component2.network.GetIDX()].gameObject.SetActive(value: false);
		}
	}

	public void CheckMap(PlayerController player)
	{
		if (isComplete)
		{
			return;
		}
		bool flag = true;
		foreach (ItemPickable item in itemList.ToList())
		{
			if (item == null)
			{
				itemList.Remove(item);
			}
			else if (item.isActiveAndEnabled && item.itemCollider.enabled && item.itemType == "Item" && !BGDatabase_Item.GetEntityByKeyid(item.itemID).IsNotKeyItem)
			{
				flag = false;
			}
		}
		foreach (ItemInteractable interaction in interactionList)
		{
			BGDatabase_Item entityByKeyid = BGDatabase_Item.GetEntityByKeyid(interaction.spawnItemID);
			if (interaction != null && ((interaction.isActiveAndEnabled && !interaction.IsSolved && entityByKeyid != null && !entityByKeyid.IsNotKeyItem) || (!interaction.IsSolved && interaction.IsPuzzle) || (interaction.listItemToActivate.Count > 0 && !interaction.IsBRIMCar && interaction.isActiveAndEnabled)))
			{
				flag = false;
			}
		}
		if (!UIGameManager.Instance.loading.loadingUI.activeSelf || (UIGameManager.Instance.loading.loadingUI.activeSelf && (bool)NetworkGameManager.Instance.ownPlayer && !NetworkGameManager.Instance.ownPlayer.initPos && player.network.isLocalPlayer))
		{
			if (flag)
			{
				SetCompleteMap();
			}
			else if (!isRevealedByAllPlayer)
			{
				SetRevealedMap();
			}
		}
	}

	public void SetCompleteMap()
	{
		isComplete = true;
		isRevealed = true;
		isRevealedByAllPlayer = true;
		foreach (SpriteRenderer item in arrMapSquare)
		{
			item.color = new Color(0.8f, 0.8f, 0.8f);
		}
	}

	public void SetRevealedMap()
	{
		isRevealedByAllPlayer = true;
		isRevealed = true;
		foreach (SpriteRenderer item in arrMapSquare)
		{
			item.color = new Color(1f, 0.2f, 0.2f);
		}
	}

	public void TurnOnLight(bool SetItemVisiblity = true)
	{
		if (!GameManager.Instance.enableRoomFogOfWar)
		{
			return;
		}
		isRevealed = true;
		for (int i = 0; i < lightList.Count; i++)
		{
			lightList[i].gameObject.SetActive(value: true);
			lightList[i].DOKill();
			lightList[i].DOIntensity(lightIntensity[i], 0.3f);
		}
		for (int j = 0; j < listMeshEmission.Count; j++)
		{
			listMeshEmission[j].GetPropertyBlock(MPB);
			MPB.SetColor(EmissionColor, listEmissionColor[j]);
			listMeshEmission[j].SetPropertyBlock(MPB);
		}
		if (SetItemVisiblity)
		{
			foreach (ItemPickable item in itemList.ToList())
			{
				if (item == null)
				{
					itemList.Remove(item);
				}
				else if (item.itemCollider.enabled)
				{
					item.SetSpriteEnable(value: true);
				}
			}
		}
		foreach (EnemyController item2 in GameManager.Instance.arrEnemyController)
		{
			if (!CheckRoom(item2.roomColliders))
			{
				continue;
			}
			if (!item2.network.IsNonActive() && item2.network.GetHealth() > 0f && (!item2.attack.fov.isDisable || !NetworkGameManager.Instance.isServer))
			{
				foreach (SpriteRenderer item3 in item2.eyeGlow)
				{
					item3.enabled = true;
				}
				item2.VisibleSprite();
			}
			if (item2.isDead || item2.network.GetHealth() <= 0f)
			{
				item2.myrigidbody.isKinematic = true;
				item2.bodyCollider.enabled = false;
			}
		}
	}

	public void TurnOffLight()
	{
		if (!isRevealed || !GameManager.Instance.enableRoomFogOfWar)
		{
			return;
		}
		for (int i = 0; i < lightList.Count; i++)
		{
			Light lightObj = lightList[i];
			lightObj.gameObject.SetActive(value: true);
			lightList[i].DOKill();
			lightObj.DOIntensity(0f, 0.3f).OnComplete(() =>
			{
				lightObj.gameObject.SetActive(value: false);
			});
		}
		CustomLightingManager.Instance?.SetStaticUpdateTimer(0.3f);
		for (int num = 0; num < listMeshEmission.Count; num++)
		{
			listMeshEmission[num].GetPropertyBlock(MPB);
			MPB.SetColor(EmissionColor, Color.black);
			listMeshEmission[num].SetPropertyBlock(MPB);
		}
		foreach (ItemPickable item in itemList.ToList())
		{
			if (item == null)
			{
				itemList.Remove(item);
			}
			else
			{
				if (!item.itemCollider.enabled || !(RoomName != NetworkGameManager.Instance.ownPlayer.RoomName))
				{
					continue;
				}
				bool flag = false;
				foreach (string roomCollider2 in NetworkGameManager.Instance.ownPlayer.roomColliders)
				{
					RoomCollider roomCollider = GameManager.Instance.GetRoomCollider(roomCollider2);
					foreach (ItemPickable item2 in roomCollider.itemList.ToList())
					{
						if (item == null)
						{
							roomCollider.itemList.Remove(item);
						}
						else if (item == item2)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
				if (!flag)
				{
					item.SetSpriteEnable(value: false);
				}
			}
		}
		foreach (EnemyController item3 in GameManager.Instance.arrEnemyController)
		{
			if (!CheckRoom(item3.roomColliders) || item3.isDead)
			{
				continue;
			}
			foreach (SpriteRenderer item4 in item3.eyeGlow)
			{
				item4.enabled = false;
			}
			if (item3.isSpriteInactive)
			{
				continue;
			}
			bool flag2 = false;
			foreach (Transform visibleTarget in NetworkGameManager.Instance.GetPlayer(NetworkGameManager.Instance.ownPlayer.network.GetIdxTargetCam()).fov.visibleTargets)
			{
				if (visibleTarget == item3.colliderFOV.transform)
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				item3.isSpriteInactive = true;
				item3.HideSprite();
			}
		}
	}

	private Mesh CreateBoxMesh(Vector3 size)
	{
		Mesh mesh = new Mesh();
		Vector3[] vertices = new Vector3[8]
		{
			new Vector3((0f - size.x) * 0.5f, (0f - size.y) * 0.5f, (0f - size.z) * 0.5f),
			new Vector3((0f - size.x) * 0.5f, (0f - size.y) * 0.5f, size.z * 0.5f),
			new Vector3((0f - size.x) * 0.5f, size.y * 0.5f, (0f - size.z) * 0.5f),
			new Vector3((0f - size.x) * 0.5f, size.y * 0.5f, size.z * 0.5f),
			new Vector3(size.x * 0.5f, (0f - size.y) * 0.5f, (0f - size.z) * 0.5f),
			new Vector3(size.x * 0.5f, (0f - size.y) * 0.5f, size.z * 0.5f),
			new Vector3(size.x * 0.5f, size.y * 0.5f, (0f - size.z) * 0.5f),
			new Vector3(size.x * 0.5f, size.y * 0.5f, size.z * 0.5f)
		};
		int[] triangles = new int[36]
		{
			0, 2, 1, 1, 2, 3, 4, 5, 6, 5,
			7, 6, 0, 1, 4, 1, 5, 4, 2, 6,
			3, 3, 6, 7, 0, 4, 2, 2, 4, 6,
			1, 3, 5, 3, 7, 5
		};
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		return mesh;
	}

	private bool CheckRoom(List<string> roomColliders)
	{
		bool result = false;
		foreach (string roomCollider in roomColliders)
		{
			if (roomCollider == RoomName)
			{
				result = true;
			}
		}
		return result;
	}

	private int CtrCheckRoom(List<string> roomColliders)
	{
		int num = 0;
		foreach (string roomCollider in roomColliders)
		{
			if (roomCollider == RoomName)
			{
				num++;
			}
		}
		return num;
	}
}
