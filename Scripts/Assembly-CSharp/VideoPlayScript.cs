using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

public class VideoPlayScript : MonoBehaviour
{
	[SerializeField]
	private int idleTimeSetting = 60;

	[SerializeField]
	private float lastIdleTime;

	[SerializeField]
	private bool _changeScene;

	private void Awake()
	{
		if (GameModes.Instance != null && !GameModes.Instance.isEvent)
		{
			Object.Destroy(base.gameObject);
		}
		lastIdleTime = Time.time;
	}

	private void FixedUpdate()
	{
		InputSystem.onAnyButtonPress.CallOnce((InputControl ctrl) =>
		{
			lastIdleTime = Time.time;
		});
		IdleCheck();
	}

	public void IdleCheck()
	{
		if (!_changeScene && Time.time - lastIdleTime > (float)idleTimeSetting)
		{
			SceneManager.LoadScene("VideoTrailer");
			_changeScene = true;
		}
	}
}
