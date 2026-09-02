using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Light))]
public class CustomLightSource : MonoBehaviour
{
	[SerializeField]
	private Light _light;

	[SerializeField]
	private bool _isDynamic;

	[SerializeField]
	[Min(0f)]
	private float _areaRadius;

	[SerializeField]
	[Range(0.3f, 1f)]
	private float _areaShadow = 1f;

	public Light Light => _light;

	public bool IsDynamic => _isDynamic;

	public float AreaRadius => _areaRadius;

	public float AreaShadow => _areaShadow;

	private void OnEnable()
	{
		if (!_light)
		{
			_light = GetComponentInChildren<Light>();
		}
		if (IsCustomLightAvailable())
		{
			CustomLightingManager.Instance.RemoveLight(this);
			CustomLightingManager.Instance.AddLight(this);
		}
	}

	private void OnDisable()
	{
		if (IsCustomLightAvailable())
		{
			CustomLightingManager.Instance.RemoveLight(this);
		}
	}

	private bool IsCustomLightAvailable()
	{
		if ((bool)_light)
		{
			return CustomLightingManager.Instance;
		}
		return false;
	}

	public void SetDynamic(bool isDynamic)
	{
		if (IsCustomLightAvailable() && base.enabled && base.gameObject.activeInHierarchy)
		{
			CustomLightingManager.Instance.RemoveLight(this);
			_isDynamic = isDynamic;
			CustomLightingManager.Instance.AddLight(this);
		}
		else
		{
			_isDynamic = isDynamic;
		}
	}
}
