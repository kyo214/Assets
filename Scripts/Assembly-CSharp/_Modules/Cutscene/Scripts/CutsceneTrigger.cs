using UnityEngine;

namespace _Modules.Cutscene.Scripts;

public class CutsceneTrigger : MonoBehaviour
{
	[Tooltip("Automatically generate ID when empty")]
	[SerializeField]
	private string _id;

	[SerializeField]
	private CutsceneManager _cutsceneManager;

	[SerializeField]
	protected CutsceneScriptableObject _cutsceneScriptableObject;

	[SerializeField]
	protected bool _onlyTriggerOnce = true;

	private bool _isCutscenePlayed;

	public string ID
	{
		get
		{
			if (string.IsNullOrEmpty(_id))
			{
				_id = base.gameObject.name + base.gameObject.transform.position.ToString();
			}
			return _id;
		}
	}

	protected CutsceneManager CutsceneManager => _cutsceneManager ?? (_cutsceneManager = GenericSingleton<CutsceneManager>.Instance);

	protected virtual void Start()
	{
		if (_cutsceneManager == null)
		{
			_cutsceneManager = GenericSingleton<CutsceneManager>.Instance;
		}
	}

	public void PlayCutscene()
	{
		CutsceneManager.PlayCutsceneNetwork(ID);
	}

	public virtual void TriggerCutscene()
	{
		if (!_onlyTriggerOnce || !_isCutscenePlayed)
		{
			if ((bool)UIGameManager.Instance && (bool)UIGameManager.Instance.UIMenuPuzzle && !UIGameManager.Instance.UIMenuPuzzle.isHidden)
			{
				NetworkGameManager.Instance.ownPlayer.ClosePuzzle();
			}
			CutsceneManager.PlayCutscene(_cutsceneScriptableObject);
			_isCutscenePlayed = true;
		}
	}

	private void OnEnable()
	{
		CutsceneManager.Add(this);
		_isCutscenePlayed = false;
	}

	private void OnDisable()
	{
		CutsceneManager.Remove(this);
	}
}
