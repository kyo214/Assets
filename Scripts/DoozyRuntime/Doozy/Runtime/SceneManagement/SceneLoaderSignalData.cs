using System;

namespace Doozy.Runtime.SceneManagement;

[Serializable]
public struct SceneLoaderSignalData
{
	public SceneLoader source { get; private set; }

	public SceneLoaderSignalData(SceneLoader sceneLoader)
	{
		source = sceneLoader;
	}

	public override string ToString()
	{
		if (!(source == null))
		{
			return string.Format("({0}) [{1}] state: {2}", "SceneLoader", source.name, source.currentState);
		}
		return "Source is null!";
	}
}
