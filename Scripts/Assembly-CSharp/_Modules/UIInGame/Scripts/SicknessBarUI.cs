using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using _Modules.Player.BaseScripts;

namespace _Modules.UIInGame.Scripts;

public class SicknessBarUI : MonoBehaviour
{
	[SerializeField]
	private Slider _sicknessBarSlider;

	[SerializeField]
	private Slider _sicknessThresholdBarSlider;

	private DizzinessManager _dizzinessManager;

	private bool _initEvents;

	private IEnumerator Start()
	{
		yield return new WaitUntil(() => NetworkGameManager.Instance?.ownPlayer != null);
		InitUI();
		InitEvents();
	}

	private void OnDestroy()
	{
		RemoveEvents();
	}

	private void InitUI()
	{
		_dizzinessManager = NetworkGameManager.Instance.ownPlayer.DizzinessManager;
		_sicknessBarSlider.value = _dizzinessManager.CurrentPointsPercentage;
		_sicknessThresholdBarSlider.value = _dizzinessManager.IntoxicatedThresholdPercentage;
	}

	private void InitEvents()
	{
		if (!_initEvents)
		{
			_dizzinessManager.OnPointsChanged += OnPointsChangedAction;
			_initEvents = true;
		}
	}

	private void RemoveEvents()
	{
		if (_initEvents)
		{
			_dizzinessManager.OnPointsChanged -= OnPointsChangedAction;
			_initEvents = false;
		}
	}

	private void OnPointsChangedAction(int currentPoints)
	{
		float endValue = (float)currentPoints * _dizzinessManager.MaxPointPercentage;
		_sicknessBarSlider?.DOKill();
		_sicknessBarSlider.DOValue(endValue, 0.3f).SetEase(Ease.OutQuad);
	}
}
