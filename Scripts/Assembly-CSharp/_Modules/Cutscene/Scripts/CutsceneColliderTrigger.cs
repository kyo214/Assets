using UnityEngine;

namespace _Modules.Cutscene.Scripts;

public class CutsceneColliderTrigger : CutsceneTrigger
{
	[SerializeField]
	private Collider _collider;

	protected override void Start()
	{
		base.Start();
		if (_collider == null)
		{
			_collider = GetComponent<Collider>();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("PlayerCollider"))
		{
			PlayCutscene();
		}
	}

	public override void TriggerCutscene()
	{
		base.TriggerCutscene();
		SetActiveCollider(setActive: false);
	}

	public void SetActiveCollider(bool setActive)
	{
		if ((bool)_collider)
		{
			_collider.enabled = setActive;
		}
	}
}
