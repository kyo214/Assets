using System.Collections.Generic;
using UnityEngine;

public class CutoutObject : MonoBehaviour
{
	[SerializeField]
	public Transform targetObject;

	public LayerMask wallMask;

	public LayerMask initWallMask;

	private Camera mainCamera;

	private static readonly int CutoutPos = Shader.PropertyToID("_CutoutPos");

	private static readonly int CutoutShow = Shader.PropertyToID("_CutoutShow");

	private static readonly int CutoutSize = Shader.PropertyToID("_CutoutSize");

	private static readonly int CutoutTransparentSize = Shader.PropertyToID("_CutoutTransparentSize");

	private static readonly int FalloffSize = Shader.PropertyToID("_FalloffSize");

	[SerializeField]
	private float _radiusSphere = 0.5f;

	[SerializeField]
	private float _cutoutSize = 0.22f;

	[SerializeField]
	private float _cutoutTransparentSize = 0.12f;

	[SerializeField]
	private float _falloffSize = 0.05f;

	[SerializeField]
	private float _offsetDistance;

	[SerializeField]
	private float _offsetY;

	private const int MaxHits = 32;

	private readonly RaycastHit[] hitBuffer = new RaycastHit[32];

	private readonly HashSet<Renderer> currentRenderers = new HashSet<Renderer>();

	private readonly HashSet<Renderer> previousRenderers = new HashSet<Renderer>();

	private readonly Dictionary<Transform, Renderer> rendererCache = new Dictionary<Transform, Renderer>(64);

	private MaterialPropertyBlock mpb;

	private float timer;

	public static CutoutObject Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		Instance = this;
		mainCamera = GetComponent<Camera>();
		initWallMask = wallMask;
		mpb = new MaterialPropertyBlock();
	}

	private void OnDisable()
	{
		Shader.SetGlobalVector(CutoutPos, -Vector2.one);
	}

	private void Update()
	{
		CutoutPosUpdate();
		timer += Time.deltaTime;
		if (!(timer < 0.08f))
		{
			timer = 0f;
			CamUpdate();
		}
	}

	private void CutoutPosUpdate()
	{
		if (!(targetObject == null))
		{
			Vector3 position = targetObject.position;
			position.y -= _offsetY;
			Vector2 vector = mainCamera.WorldToViewportPoint(position);
			Shader.SetGlobalVector(CutoutPos, vector);
		}
	}

	private void CamUpdate()
	{
		if (targetObject == null || CameraGame.Instance.mainCam == null)
		{
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 position2 = targetObject.position;
		position2.y -= _offsetY;
		Vector3 vector = position2 - position;
		float maxDistance = vector.magnitude - _offsetDistance;
		int num = Physics.SphereCastNonAlloc(position, _radiusSphere, vector.normalized, hitBuffer, maxDistance, wallMask);
		currentRenderers.Clear();
		for (int i = 0; i < num; i++)
		{
			Transform transform = hitBuffer[i].transform;
			if (!rendererCache.TryGetValue(transform, out var value))
			{
				value = transform.GetComponent<Renderer>();
				if (value != null)
				{
					rendererCache.Add(transform, value);
				}
			}
			if (!(value == null))
			{
				currentRenderers.Add(value);
				value.GetPropertyBlock(mpb);
				mpb.SetFloat(CutoutShow, 1f);
				mpb.SetFloat(CutoutSize, _cutoutSize);
				mpb.SetFloat(CutoutTransparentSize, _cutoutTransparentSize);
				mpb.SetFloat(FalloffSize, _falloffSize);
				value.SetPropertyBlock(mpb);
			}
		}
		foreach (Renderer previousRenderer in previousRenderers)
		{
			if (!currentRenderers.Contains(previousRenderer) && previousRenderer != null)
			{
				previousRenderer.GetPropertyBlock(mpb);
				mpb.SetFloat(CutoutShow, 0f);
				mpb.SetFloat(CutoutSize, 0f);
				mpb.SetFloat(CutoutTransparentSize, 0f);
				mpb.SetFloat(FalloffSize, 0f);
				previousRenderer.SetPropertyBlock(mpb);
			}
		}
		_ = previousRenderers;
		previousRenderers.Clear();
		foreach (Renderer currentRenderer in currentRenderers)
		{
			previousRenderers.Add(currentRenderer);
		}
	}
}
