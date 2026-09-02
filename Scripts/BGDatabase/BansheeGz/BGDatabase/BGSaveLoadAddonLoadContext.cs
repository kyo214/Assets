using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGSaveLoadAddonLoadContext
{
	public class LoadRequest
	{
		public readonly string ConfigName;

		public readonly byte[] data;

		public LoadRequest(string configName, byte[] data)
		{
			ConfigName = configName;
			this.data = data;
		}
	}

	public class PreserveRequest
	{
		public readonly string ConfigName;

		public PreserveRequest(string configName)
		{
			ConfigName = configName;
		}
	}

	public readonly List<LoadRequest> Requests = new List<LoadRequest>();

	public bool ReloadDatabase = true;

	public bool FireAfterLoadEvents = true;

	public List<PreserveRequest> PreserveRequests;

	public BGSaveLoadAddonLoadContext(params LoadRequest[] requests)
	{
		if (requests != null)
		{
			Requests.AddRange(requests);
		}
	}
}
