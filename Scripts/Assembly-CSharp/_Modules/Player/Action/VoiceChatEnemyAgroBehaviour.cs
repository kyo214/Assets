using System.Collections.Generic;
using UnityEngine;

namespace _Modules.Player.Action;

public class VoiceChatEnemyAgroBehaviour : MonoBehaviour
{
	[SerializeField]
	private PlayerController _playerController;

	[SerializeField]
	private Collider _collider;

	[Header("Voice Chat Detector")]
	[SerializeField]
	private float _amplitude = 0.1f;

	private float _currentAmplitude;

	private List<EnemyController> _listEnemyInsideCollider = new List<EnemyController>();

	private void OnEnable()
	{
		InitVoiceChatDetectorEvent();
	}

	private void OnDisable()
	{
		RemoveVoiceChatDetectorEvent();
	}

	private void InitVoiceChatDetectorEvent()
	{
		VoiceBroadcastController.OnPlayerSpeakingEvent += OnPlayerSpeakingAction;
		VoiceBroadcastController.OnPlayerEndSpeakEvent += OnPlayerEndSpeakAction;
	}

	private void RemoveVoiceChatDetectorEvent()
	{
		VoiceBroadcastController.OnPlayerSpeakingEvent -= OnPlayerSpeakingAction;
		VoiceBroadcastController.OnPlayerEndSpeakEvent -= OnPlayerEndSpeakAction;
	}

	private void OnPlayerSpeakingAction(PlayerController playerController)
	{
		_collider.enabled = true;
		_currentAmplitude = _playerController.network.VoicePlayerState?.Amplitude ?? 0f;
	}

	private void OnPlayerEndSpeakAction(PlayerController playerController)
	{
		_collider.enabled = false;
		_currentAmplitude = 0f;
		_listEnemyInsideCollider.Clear();
	}

	private void VoiceChatDetector(EnemyController enemyController)
	{
		if (!(enemyController == null))
		{
			enemyController.CheckEnemyAggroNetwork(_playerController);
		}
	}

	private void FixedUpdate()
	{
		if (_collider.enabled && _currentAmplitude >= _amplitude && _currentAmplitude < 0.3f)
		{
			for (int num = _listEnemyInsideCollider.Count - 1; num >= 0; num--)
			{
				EnemyController enemyController = _listEnemyInsideCollider[num];
				VoiceChatDetector(enemyController);
				_listEnemyInsideCollider.RemoveAt(num);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag(EnemyController.EMEMY_TAG))
		{
			EnemyController component = other.GetComponent<EnemyController>();
			if (!_listEnemyInsideCollider.Contains(component))
			{
				_listEnemyInsideCollider.Add(component);
			}
		}
		else if (other.gameObject.CompareTag(EnemyController.EMEMY_COLLIDER_TAG))
		{
			EnemyController enemyControler = other.GetComponent<EnemyCollider>().enemyControler;
			if (!_listEnemyInsideCollider.Contains(enemyControler))
			{
				_listEnemyInsideCollider.Add(enemyControler);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag(EnemyController.EMEMY_TAG))
		{
			EnemyController component = other.GetComponent<EnemyController>();
			_listEnemyInsideCollider.Remove(component);
		}
		else if (other.gameObject.CompareTag(EnemyController.EMEMY_COLLIDER_TAG))
		{
			EnemyController enemyControler = other.GetComponent<EnemyCollider>().enemyControler;
			_listEnemyInsideCollider.Remove(enemyControler);
		}
	}
}
