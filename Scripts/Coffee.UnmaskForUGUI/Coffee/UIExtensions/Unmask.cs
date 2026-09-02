using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Coffee.UIExtensions;

[ExecuteInEditMode]
[AddComponentMenu("UI/Unmask/Unmask", 1)]
public class Unmask : MonoBehaviour, IMaterialModifier
{
	private static readonly Vector2 s_Center = new Vector2(0.5f, 0.5f);

	[Tooltip("Fit graphic's transform to target transform.")]
	[SerializeField]
	private RectTransform m_FitTarget;

	[Tooltip("Fit graphic's transform to target transform on LateUpdate every frame.")]
	[SerializeField]
	private bool m_FitOnLateUpdate;

	[Tooltip("Unmask affects only for children.")]
	[SerializeField]
	private bool m_OnlyForChildren;

	[Tooltip("Show the graphic that is associated with the unmask render area.")]
	[SerializeField]
	private bool m_ShowUnmaskGraphic;

	[Tooltip("Edge smoothing.")]
	[Range(0f, 1f)]
	[SerializeField]
	private float m_EdgeSmoothing;

	private Material _unmaskMaterial;

	private Material _revertUnmaskMaterial;

	private MaskableGraphic _graphic;

	public MaskableGraphic graphic => _graphic ?? (_graphic = GetComponent<MaskableGraphic>());

	public RectTransform fitTarget
	{
		get
		{
			return m_FitTarget;
		}
		set
		{
			m_FitTarget = value;
			FitTo(m_FitTarget);
		}
	}

	public bool fitOnLateUpdate
	{
		get
		{
			return m_FitOnLateUpdate;
		}
		set
		{
			m_FitOnLateUpdate = value;
		}
	}

	public bool showUnmaskGraphic
	{
		get
		{
			return m_ShowUnmaskGraphic;
		}
		set
		{
			m_ShowUnmaskGraphic = value;
			SetDirty();
		}
	}

	public bool onlyForChildren
	{
		get
		{
			return m_OnlyForChildren;
		}
		set
		{
			m_OnlyForChildren = value;
			SetDirty();
		}
	}

	public float edgeSmoothing
	{
		get
		{
			return m_EdgeSmoothing;
		}
		set
		{
			m_EdgeSmoothing = value;
		}
	}

	public Material GetModifiedMaterial(Material baseMaterial)
	{
		if (!base.isActiveAndEnabled)
		{
			return baseMaterial;
		}
		Transform stopAfter = MaskUtilities.FindRootSortOverrideCanvas(base.transform);
		int stencilDepth = MaskUtilities.GetStencilDepth(base.transform, stopAfter);
		int num = 1 << stencilDepth;
		StencilMaterial.Remove(_unmaskMaterial);
		_unmaskMaterial = StencilMaterial.Add(baseMaterial, num - 1, StencilOp.Invert, CompareFunction.Equal, m_ShowUnmaskGraphic ? ColorWriteMask.All : ((ColorWriteMask)0), num - 1, 255);
		CanvasRenderer canvasRenderer = graphic.canvasRenderer;
		if (m_OnlyForChildren)
		{
			StencilMaterial.Remove(_revertUnmaskMaterial);
			_revertUnmaskMaterial = StencilMaterial.Add(baseMaterial, 128, StencilOp.Invert, CompareFunction.Equal, (ColorWriteMask)0, 128, 255);
			canvasRenderer.hasPopInstruction = true;
			canvasRenderer.popMaterialCount = 1;
			canvasRenderer.SetPopMaterial(_revertUnmaskMaterial, 0);
		}
		else
		{
			canvasRenderer.hasPopInstruction = false;
			canvasRenderer.popMaterialCount = 0;
		}
		return _unmaskMaterial;
	}

	public void FitTo(RectTransform target)
	{
		RectTransform obj = base.transform as RectTransform;
		obj.pivot = target.pivot;
		obj.position = target.position;
		obj.rotation = target.rotation;
		Vector3 lossyScale = target.lossyScale;
		Vector3 lossyScale2 = obj.parent.lossyScale;
		obj.localScale = new Vector3(lossyScale.x / lossyScale2.x, lossyScale.y / lossyScale2.y, lossyScale.z / lossyScale2.z);
		obj.sizeDelta = target.rect.size;
		Vector2 anchorMax = (obj.anchorMin = s_Center);
		obj.anchorMax = anchorMax;
	}

	private void OnEnable()
	{
		if ((bool)m_FitTarget)
		{
			FitTo(m_FitTarget);
		}
		SetDirty();
	}

	private void OnDisable()
	{
		StencilMaterial.Remove(_unmaskMaterial);
		StencilMaterial.Remove(_revertUnmaskMaterial);
		_unmaskMaterial = null;
		_revertUnmaskMaterial = null;
		if ((bool)graphic)
		{
			CanvasRenderer canvasRenderer = graphic.canvasRenderer;
			canvasRenderer.hasPopInstruction = false;
			canvasRenderer.popMaterialCount = 0;
			graphic.SetMaterialDirty();
		}
		SetDirty();
	}

	private void LateUpdate()
	{
		if ((bool)m_FitTarget && m_FitOnLateUpdate)
		{
			FitTo(m_FitTarget);
		}
		Smoothing(graphic, m_EdgeSmoothing);
	}

	private void SetDirty()
	{
		if ((bool)graphic)
		{
			graphic.SetMaterialDirty();
		}
	}

	private static void Smoothing(MaskableGraphic graphic, float smooth)
	{
		if (!graphic)
		{
			return;
		}
		CanvasRenderer canvasRenderer = graphic.canvasRenderer;
		Color color = canvasRenderer.GetColor();
		float num = 1f;
		if (graphic.maskable && 0f < smooth)
		{
			float num2 = graphic.color.a * canvasRenderer.GetInheritedAlpha();
			if (0f < num2)
			{
				num = Mathf.Lerp(0.01f, 0.002f, smooth) / num2;
			}
		}
		if (!Mathf.Approximately(color.a, num))
		{
			color.a = Mathf.Clamp01(num);
			canvasRenderer.SetColor(color);
		}
	}
}
