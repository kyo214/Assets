using System.Collections;
using System.Threading;
using DG.Tweening;
using Pathfinding.Examples;
using UnityEngine;

public class CameraMiniMap : MonoBehaviour
{
	public SpriteRenderer map;

	public RenderTexture miniMap2;

	public LayerMask mapMask;

	public int fps = 20;

	private float elapsed;

	private Camera cam;

	private Texture2D textureMap;

	private float _renderFps;

	private Camera cam2;

	private CancellationTokenSource cts;

	private bool isRendering;

	[SerializeField]
	private AstarSmoothFollow2 _smoothFollow;

	public static CameraMiniMap Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	private IEnumerator Start()
	{
		cam = GetComponent<Camera>();
		cam.nearClipPlane = -3f;
		cam.enabled = false;
		yield return new WaitForSeconds(0.5f);
		while (NetworkGameManager.Instance.ownPlayer == null)
		{
			yield return null;
		}
		cam.cullingMask = mapMask;
		if (NetworkGameManager.Instance.ownPlayer != null)
		{
			_smoothFollow.target = NetworkGameManager.Instance.ownPlayer.transform;
		}
		if (GlobalSaveData.instance.optionData.autoMinimap == 1)
		{
			cam.transform.DOLocalRotate(new Vector3(90f, CameraGame.Instance.camRotate, 0f), 0.1f).SetEase(Ease.Linear);
		}
		UIGameManager.Instance.mapUI.SetActive(value: true);
		CameraGame.Instance.RotateCamera(0);
		foreach (RoomCollider item in GameManager.Instance.arrRoom)
		{
			foreach (SpriteRenderer item2 in item.arrMapSquare)
			{
				item2.enabled = true;
				item2.gameObject.layer = 28;
			}
		}
		cam.targetTexture.Release();
		cam.targetTexture = miniMap2;
		SetRenderFps();
		foreach (PlayerController item3 in NetworkGameManager.Instance.arrPlayerController)
		{
			if (!item3.network.isLocalPlayer)
			{
				item3.iconCharMap.parent = item3.transform;
				item3.iconCharMapAnimator.Play(item3.data.PlayerSkinData.GetPlayerAvatarSkin());
				item3.iconCharMap.DOScale(20f, 0f);
				if (GlobalSaveData.instance.optionData.autoMinimap == 1)
				{
					item3.iconCharMap.DORotate(new Vector3(90f, 0f, CameraGame.Instance.camRotate - 90), 0f);
				}
				else
				{
					item3.iconCharMap.DORotate(new Vector3(90f, 0f, CameraGame.Instance.camRotate - 45), 0f);
				}
			}
		}
		cam.enabled = false;
	}

	private Texture2D RTImage(Camera camera)
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = camera.targetTexture;
		camera.Render();
		Texture2D texture2D = new Texture2D(camera.targetTexture.width, camera.targetTexture.height, TextureFormat.RGBA32, mipChain: false);
		texture2D.ReadPixels(new Rect(0f, 0f, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = active;
		return texture2D;
	}

	private void OnDestroy()
	{
		if (map != null)
		{
			miniMap2.Release();
			Object.Destroy(textureMap);
			Object.Destroy(map.sprite);
			textureMap = null;
			map.sprite = null;
		}
	}

	private void OnEnable()
	{
		cts = new CancellationTokenSource();
	}

	private void OnDisable()
	{
		cts?.Cancel();
		cts?.Dispose();
	}

	private void Update()
	{
		elapsed += Time.deltaTime;
		if (elapsed > _renderFps)
		{
			elapsed = 0f;
			cam.Render();
		}
	}

	private void SetRenderFps()
	{
		_renderFps = 1f / (float)fps;
	}
}
