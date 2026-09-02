namespace Doozy.Runtime.UIManager.Input;

public interface IUseMultiplayerInfo
{
	MultiplayerInfo multiplayerInfo { get; }

	bool hasMultiplayerInfo { get; }

	void SetMultiplayerInfo(MultiplayerInfo reference);
}
