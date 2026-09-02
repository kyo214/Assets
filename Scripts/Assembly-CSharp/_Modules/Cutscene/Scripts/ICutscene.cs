namespace _Modules.Cutscene.Scripts;

public interface ICutscene
{
	void Play();

	void Skip();

	void Stop();

	void OnStart();

	void OnComplete();
}
