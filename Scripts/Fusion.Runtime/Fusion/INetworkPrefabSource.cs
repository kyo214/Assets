namespace Fusion;

public interface INetworkPrefabSource
{
	string EditorSummary { get; }

	void Load(in NetworkPrefabLoadContext context);

	void Unload();
}
