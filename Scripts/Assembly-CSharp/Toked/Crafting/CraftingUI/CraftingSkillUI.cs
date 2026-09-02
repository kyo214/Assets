using System;
using Doozy.Runtime.UIManager.Components;
using Toked.Skill;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Toked.Crafting.CraftingUI;

public class CraftingSkillUI : MonoBehaviour
{
	[SerializeField]
	private SkillScriptableObject _skillScriptableObject;

	[SerializeField]
	private UISelectable _uiSelectable;

	[SerializeField]
	private Image _skillImage;

	[SerializeField]
	private Image _highlightImage;

	[SerializeField]
	private Sprite _defaultSprite;

	private Action<SkillScriptableObject> onHoverEvent;

	public UnityEvent OnHoverUnityAction;

	public UnityEvent OnUnhoverUnityAction;

	public SkillScriptableObject SkillScriptableObject
	{
		get
		{
			return _skillScriptableObject;
		}
		set
		{
			_skillScriptableObject = value;
		}
	}

	public UISelectable UISelectable => _uiSelectable;

	public void Init(SkillScriptableObject skillSo, Action<SkillScriptableObject> onHoverAction)
	{
		if (this != null)
		{
			SetActive(active: true);
			_skillScriptableObject = skillSo;
			_skillImage.sprite = skillSo.SkillSprite;
			onHoverEvent = onHoverAction;
		}
	}

	public void Select()
	{
		_uiSelectable?.Select();
	}

	public void Selected()
	{
		_highlightImage.gameObject.SetActive(value: true);
		onHoverEvent?.Invoke(_skillScriptableObject);
		OnHoverUnityAction?.Invoke();
	}

	public void Deselected()
	{
		_highlightImage.gameObject.SetActive(value: false);
		OnUnhoverUnityAction?.Invoke();
	}

	public void SetActive(bool active)
	{
		if (this != null)
		{
			base.gameObject.SetActive(active);
		}
	}

	public void ResetImage()
	{
		if ((bool)_skillImage)
		{
			_skillImage.sprite = _defaultSprite;
		}
	}
}
