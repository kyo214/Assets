using UnityEngine;

public class SoundStepCollider : MonoBehaviour
{
	[SerializeField]
	private SoundStepType soundStepType;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && (bool)NetworkGameManager.Instance.ownPlayer && other.transform == NetworkGameManager.Instance.ownPlayer.transform)
		{
			NetworkGameManager.Instance.ownPlayer.ArrSoundStepTypeCollide.Add(soundStepType);
			NetworkGameManager.Instance.ownPlayer.soundStepType = soundStepType;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player") && other.transform == NetworkGameManager.Instance.ownPlayer.transform)
		{
			NetworkGameManager.Instance.ownPlayer.ArrSoundStepTypeCollide.Remove(soundStepType);
			if (NetworkGameManager.Instance.ownPlayer.ArrSoundStepTypeCollide.Count > 0)
			{
				NetworkGameManager.Instance.ownPlayer.soundStepType = NetworkGameManager.Instance.ownPlayer.ArrSoundStepTypeCollide[0];
			}
			else
			{
				NetworkGameManager.Instance.ownPlayer.soundStepType = SoundStepType.CONCRETE;
			}
		}
	}
}
