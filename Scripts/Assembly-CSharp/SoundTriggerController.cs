using DarkTonic.MasterAudio;
using UnityEngine;

public class SoundTriggerController : MonoBehaviour
{
	[SerializeField]
	private bool triggerOnce = true;

	[SerializeField]
	private bool localOnly = true;

	[SerializeField]
	private bool isDialogue = true;

	[SerializeField]
	private EventSounds eventSound;

	public bool IsDialogue
	{
		get
		{
			return isDialogue;
		}
		set
		{
			isDialogue = value;
		}
	}

	public EventSounds EventSound
	{
		get
		{
			return eventSound;
		}
		set
		{
			eventSound = value;
		}
	}

	private void Start()
	{
		EventSound = GetComponent<EventSounds>();
		GameManager.Instance.arrSoundTrigger.Add(this);
		if (!localOnly)
		{
			return;
		}
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
		{
			if (item.network.isLocalPlayer)
			{
				if (!isDialogue)
				{
					continue;
				}
				foreach (AudioEvent soundEvent in EventSound.triggerSound.SoundEvents)
				{
					if (soundEvent.actionName == "Female" && item.IsMale)
					{
						EventSound.triggerSound.SoundEvents.Remove(soundEvent);
						break;
					}
					if (soundEvent.actionName == "Male" && !item.IsMale)
					{
						EventSound.triggerSound.SoundEvents.Remove(soundEvent);
						break;
					}
				}
			}
			else
			{
				Physics.IgnoreCollision(GetComponent<Collider>(), item.network.charControllerPhoton.Collider, ignore: true);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (triggerOnce && other.CompareTag("Player"))
		{
			if (IsDialogue && EventSound != null && !GameManager.Instance.isHordeMode)
			{
				Invoke("ShowDialogue", 0.5f);
			}
			GetComponent<Collider>().enabled = false;
		}
	}

	private void ShowDialogue()
	{
		if (EventSound.triggerSound.SoundEvents.Count > 0 && EventSound.triggerSound.SoundEvents[0].volume > -90f)
		{
			string text = EventSound.triggerSound.SoundEvents[0].soundType + "-" + EventSound.triggerSound.SoundEvents[0].variationName.Substring(EventSound.triggerSound.SoundEvents[0].variationName.Length - 1, 1);
			if (!NetworkGameManager.Instance.ownPlayer.IsMale)
			{
				text = text.Replace("female", "male");
			}
			ChatSystem.Instance.ShowMonologue(text);
		}
	}
}
