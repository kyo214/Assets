using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using I2.Loc;
using TMPro;
using Toked;
using UnityEngine;

public class MissionBriefing : MonoBehaviour
{
	public int page;

	[SerializeField]
	private TextMeshProUGUI txtPage;

	[SerializeField]
	private Localize Head1;

	[SerializeField]
	private Localize Head2;

	[SerializeField]
	private Localize Body;

	[SerializeField]
	private GameObject content;

	[SerializeField]
	private GameObject _blackBG;

	[SerializeField]
	private GameObject _buttonInfo;

	[SerializeField]
	private RectTransform _contentFrame;

	public List<int> totalPageScenario = new List<int>();

	private void OnEnable()
	{
		page = 1;
		content.SetActive(value: true);
		WritePage();
		_blackBG.SetActive(value: true);
		_buttonInfo.SetActive(value: true);
		_contentFrame.DOScale(0f, 0f);
		_contentFrame.DOScale(1.65f, 0.2f);
	}

	private void OnDisable()
	{
		_blackBG.SetActive(value: false);
		_buttonInfo.SetActive(value: false);
		_contentFrame.DOScale(0f, 0.2f);
		UniTaskUtil.DelayedCall(this, 0.2f, () =>
		{
			content.SetActive(value: false);
		}).Forget();
	}

	public void CloseMissionBriefing()
	{
		base.enabled = false;
		AudioManager.PlaySFX("ui_cancel");
	}

	public void changePage(bool IncreasePage)
	{
		if (IncreasePage)
		{
			if (page < totalPageScenario[GameManager.Instance.gameManagerPhoton.Scenario])
			{
				page++;
			}
		}
		else if (page > 1)
		{
			page--;
		}
		WritePage();
	}

	public void WritePage()
	{
		Head1.SetTerm("Note/BriefingHead" + GameManager.Instance.gameManagerPhoton.Scenario + "-1");
		Head2.SetTerm("Note/BriefingHead" + GameManager.Instance.gameManagerPhoton.Scenario + "-2");
		Body.SetTerm("Note/BriefingContent" + GameManager.Instance.gameManagerPhoton.Scenario + "-" + page);
		txtPage.text = page + " / " + totalPageScenario[GameManager.Instance.gameManagerPhoton.Scenario];
	}
}
