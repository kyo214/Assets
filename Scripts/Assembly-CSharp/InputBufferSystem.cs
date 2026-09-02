using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputBufferSystem : MonoBehaviour
{
	[Header("Buffer Settings")]
	public float bufferTime = 0.2f;

	[SerializeField]
	private InputActionReference _attack;

	private Queue<BufferedInput> inputBuffer = new Queue<BufferedInput>();

	[SerializeField]
	private PlayerController playerController;

	[SerializeField]
	private PlayerInput _playerInput;

	private void Start()
	{
		UniTaskUtil.DelayedCall(this, 1f, () =>
		{
			if (playerController.network.isLocalPlayer)
			{
				_attack.action.performed += (InputAction.CallbackContext ctx) =>
				{
					AddToBuffer("Shoot/Attack");
				};
			}
		}).Forget();
	}

	private void OnDisable()
	{
		if (playerController.network.isLocalPlayer)
		{
			_attack.action.performed -= (InputAction.CallbackContext ctx) =>
			{
				AddToBuffer("Shoot/Attack");
			};
		}
	}

	private void Update()
	{
		ProcessBuffer();
	}

	private void AddToBuffer(string actionName)
	{
		if (!playerController.network.isLocalPlayer)
		{
			base.enabled = false;
		}
		else if ((bool)playerController.fsmUpperBody && !playerController.fsmUpperBody.GetBool("isReviving"))
		{
			inputBuffer.Enqueue(new BufferedInput(actionName, Time.time));
		}
	}

	private void ProcessBuffer()
	{
		while (inputBuffer.Count > 0)
		{
			BufferedInput bufferedInput = inputBuffer.Peek();
			if (Time.time - bufferedInput.time > bufferTime)
			{
				inputBuffer.Dequeue();
				continue;
			}
			ExecuteAction(bufferedInput.actionName);
			break;
		}
	}

	private void ExecuteAction(string actionName)
	{
		if (actionName == "Shoot/Attack")
		{
			if (!_playerInput.enabled || playerController.DelayInputTimer.isRunning || !playerController.network.GetEnableControl() || !playerController.enableMoveChar || playerController.fsmUpperBody.GetBool("isMelee") || playerController.fsmUpperBody.GetCurrentAnimatorStateInfo(0).IsName("Melee"))
			{
				return;
			}
			float num = Quaternion.LookRotation(playerController.angleInput - playerController.weaponPos.position, Vector3.up).eulerAngles.y;
			if (num < 0f)
			{
				num += 360f;
			}
			if (!playerController.weaponController.attackTimer.isRunning)
			{
				if (!playerController.isAiming || !playerController.weaponController.timerDelayShoot.isRunning)
				{
					playerController.network.ExecAttackTriggered((short)num);
				}
				inputBuffer.Dequeue();
			}
		}
		else
		{
			Debug.LogWarning("Unknown action: " + actionName);
		}
	}
}
