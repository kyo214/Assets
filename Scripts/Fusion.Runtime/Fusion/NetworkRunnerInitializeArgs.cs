using System;
using Fusion.Sockets;

namespace Fusion;

internal struct NetworkRunnerInitializeArgs
{
	public SceneRef? Scene;

	public NetAddress? Address;

	public NetAddress? PublicAddress;

	public int? PlayerCount;

	public SimulationModes? SimulationMode;

	public int? InputWordCount;

	public NetworkProjectConfig Config;

	public Action<NetworkRunner> Initialized;

	public INetworkObjectPool ObjectPool;

	public INetworkSceneManager SceneManager;

	public Type[] CustomCallbackInterfaces;

	public byte[] ConnectionToken;

	public bool HostMigrationEnabled;

	public NetworkId? ResumeId;

	public Tick? ResumeTick;

	public byte[] ResumeState;

	public Action<NetworkRunner> HostMigrationResume;

	public bool IsSinglePlayer => SimulationMode == SimulationModes.Host && PlayerCount == 1;
}
