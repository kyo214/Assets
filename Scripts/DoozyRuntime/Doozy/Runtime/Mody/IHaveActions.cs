namespace Doozy.Runtime.Mody;

public interface IHaveActions
{
	ModyAction GetAction(string actionName);

	bool ContainsAction(string actionName);

	void ActivateActions();

	void DeactivateActions();

	void Execute(string actionName, RunAction method, bool ignoreCooldown = false, bool forced = false);

	void StartAction(string actionName, bool ignoreCooldown, bool forced = false);

	void StopAction(string actionName);

	void FinishAction(string actionName);

	void StopAllActions();

	void FinishAllActions();
}
