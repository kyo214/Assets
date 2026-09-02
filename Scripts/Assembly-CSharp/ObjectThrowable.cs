using System;
using DG.Tweening;
using UnityEngine;

public class ObjectThrowable : MonoBehaviour
{
	public enum ThrowableType
	{
		Grenade = 0,
		Molotov = 1,
		Landmine = 2,
		GLauncher = 3
	}

	public Rigidbody rigidBody;

	[SerializeField]
	private GameObject[] _gameObject;

	private Vector3 _defaultRotation = new Vector3(-90f, 0f, 0f);

	public void Init(ThrowableType throwableType)
	{
		base.transform.localEulerAngles = _defaultRotation;
		ResetRigidbody();
		SetActiveCurrentType(throwableType);
	}

	private void ResetRigidbody()
	{
		rigidBody.velocity = Vector3.zero;
		rigidBody.angularVelocity = Vector3.zero;
		rigidBody.DOKill();
	}

	private void SetActiveCurrentType(ThrowableType throwableType)
	{
		ResetAllGameObject((int)throwableType);
	}

	private void ResetAllGameObject(int exceptionIndex)
	{
		if (exceptionIndex < 0 || exceptionIndex >= Enum.GetNames(typeof(ThrowableType)).Length)
		{
			exceptionIndex = 0;
		}
		for (int i = 0; i < _gameObject.Length; i++)
		{
			_gameObject[i].SetActive(i == exceptionIndex);
		}
	}
}
