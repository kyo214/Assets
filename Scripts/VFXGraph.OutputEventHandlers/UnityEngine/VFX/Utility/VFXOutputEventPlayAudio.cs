namespace UnityEngine.VFX.Utility;

[ExecuteAlways]
[RequireComponent(typeof(VisualEffect))]
internal class VFXOutputEventPlayAudio : VFXOutputEventAbstractHandler
{
	public AudioSource audioSource;

	public override bool canExecuteInEditor => true;

	public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
	{
		if (audioSource != null)
		{
			audioSource.Play();
		}
	}
}
