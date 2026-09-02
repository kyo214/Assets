namespace UnityEngine.VFX.Utility;

[ExecuteAlways]
[RequireComponent(typeof(VisualEffect))]
internal class VFXOutputEventPrefabSpawn : VFXOutputEventAbstractHandler
{
	[SerializeField]
	[Tooltip("The maximum number of prefabs that can be active at a time")]
	private uint m_InstanceCount = 5u;

	[SerializeField]
	[Tooltip("The prefab to enable upon event received. Prefabs are created as hidden and stored in a pool, upon enabling this behavior. Upon receiving an event a prefab from the pool is enabled and will be disabled when reaching its lifetime.")]
	private GameObject m_PrefabToSpawn;

	[SerializeField]
	[Tooltip("Whether to attach prefab instances to current game object. Use this setting to treat position and angle attributes as local space.")]
	private bool m_ParentInstances;

	[Tooltip("Whether to use the position attribute to set prefab position on spawn")]
	public bool usePosition = true;

	[Tooltip("Whether to use the angle attribute to set prefab rotation on spawn")]
	public bool useAngle = true;

	[Tooltip("Whether to use the scale attribute to set prefab localScale on spawn")]
	public bool useScale = true;

	[Tooltip("Whether to use the lifetime attribute to determine how long the prefab will be enabled")]
	public bool useLifetime = true;

	private static readonly GameObject[] k_EmptyGameObjects = new GameObject[0];

	private static readonly float[] k_EmptyTimeToLive = new float[0];

	private GameObject[] m_Instances = k_EmptyGameObjects;

	private float[] m_TimesToLive = k_EmptyTimeToLive;

	private static readonly int k_PositionID = Shader.PropertyToID("position");

	private static readonly int k_AngleID = Shader.PropertyToID("angle");

	private static readonly int k_ScaleID = Shader.PropertyToID("scale");

	private static readonly int k_LifetimeID = Shader.PropertyToID("lifetime");

	public override bool canExecuteInEditor => true;

	public uint instanceCount => m_InstanceCount;

	public GameObject prefabToSpawn => m_PrefabToSpawn;

	public bool parentInstances => m_ParentInstances;

	protected override void OnDisable()
	{
		base.OnDisable();
		GameObject[] instances = m_Instances;
		for (int i = 0; i < instances.Length; i++)
		{
			instances[i].SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		DisposeInstances();
	}

	private void DisposeInstances()
	{
		GameObject[] instances = m_Instances;
		foreach (GameObject gameObject in instances)
		{
			if ((bool)gameObject)
			{
				if (Application.isPlaying)
				{
					Object.Destroy(gameObject);
				}
				else
				{
					Object.DestroyImmediate(gameObject);
				}
			}
		}
		m_Instances = k_EmptyGameObjects;
		m_TimesToLive = k_EmptyTimeToLive;
	}

	private void UpdateHideFlag(GameObject instance)
	{
		instance.hideFlags = HideFlags.HideAndDontSave;
	}

	private void CheckAndRebuildInstances()
	{
		if (m_Instances.Length == m_InstanceCount)
		{
			return;
		}
		DisposeInstances();
		if (m_PrefabToSpawn != null && m_InstanceCount != 0)
		{
			m_Instances = new GameObject[m_InstanceCount];
			m_TimesToLive = new float[m_InstanceCount];
			for (int i = 0; i < m_Instances.Length; i++)
			{
				GameObject gameObject = null;
				gameObject = Object.Instantiate(m_PrefabToSpawn);
				gameObject.name = $"{base.name} - #{i} - {m_PrefabToSpawn.name}";
				gameObject.SetActive(value: false);
				gameObject.transform.parent = (m_ParentInstances ? base.transform : null);
				UpdateHideFlag(gameObject);
				m_Instances[i] = gameObject;
				m_TimesToLive[i] = float.NegativeInfinity;
			}
		}
	}

	public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
	{
		CheckAndRebuildInstances();
		int num = -1;
		for (int i = 0; i < m_Instances.Length; i++)
		{
			if (!m_Instances[i].activeSelf)
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			return;
		}
		GameObject gameObject = m_Instances[num];
		gameObject.SetActive(value: true);
		if (usePosition && eventAttribute.HasVector3(k_PositionID))
		{
			if (m_ParentInstances)
			{
				gameObject.transform.localPosition = eventAttribute.GetVector3(k_PositionID);
			}
			else
			{
				gameObject.transform.position = eventAttribute.GetVector3(k_PositionID);
			}
		}
		if (useAngle && eventAttribute.HasVector3(k_AngleID))
		{
			if (parentInstances)
			{
				gameObject.transform.localEulerAngles = eventAttribute.GetVector3(k_AngleID);
			}
			else
			{
				gameObject.transform.eulerAngles = eventAttribute.GetVector3(k_AngleID);
			}
		}
		if (useScale && eventAttribute.HasVector3(k_ScaleID))
		{
			gameObject.transform.localScale = eventAttribute.GetVector3(k_ScaleID);
		}
		if (useLifetime && eventAttribute.HasFloat(k_LifetimeID))
		{
			m_TimesToLive[num] = eventAttribute.GetFloat(k_LifetimeID);
		}
		else
		{
			m_TimesToLive[num] = float.NegativeInfinity;
		}
		VFXOutputEventPrefabAttributeAbstractHandler[] componentsInChildren = gameObject.GetComponentsInChildren<VFXOutputEventPrefabAttributeAbstractHandler>();
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			componentsInChildren[j].OnVFXEventAttribute(eventAttribute, base.m_VisualEffect);
		}
	}

	private void Update()
	{
		if (Application.isPlaying || (executeInEditor && canExecuteInEditor))
		{
			CheckAndRebuildInstances();
			float deltaTime = Time.deltaTime;
			for (int i = 0; i < m_Instances.Length; i++)
			{
				if (m_TimesToLive[i] != float.NegativeInfinity)
				{
					if (m_TimesToLive[i] <= 0f && m_Instances[i].activeSelf)
					{
						m_Instances[i].SetActive(value: false);
					}
					else
					{
						m_TimesToLive[i] -= deltaTime;
					}
				}
			}
		}
		else
		{
			DisposeInstances();
		}
	}
}
