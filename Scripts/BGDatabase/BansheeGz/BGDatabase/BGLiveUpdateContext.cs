using System;

namespace BansheeGz.BGDatabase;

public class BGLiveUpdateContext
{
	public BGRepo Repo;

	public BGAddonLiveUpdate addon;

	public int timeOut;

	public bool isAsynchronous;

	public Action asyncComplete;

	public BGLiveUpdateLoaderA loader;

	public BGLiveUpdateContext(BGRepo repo, BGAddonLiveUpdate addon, int timeOut, bool isAsynchronous, Action asyncComplete)
	{
		Repo = repo;
		this.addon = addon;
		this.timeOut = timeOut;
		this.isAsynchronous = isAsynchronous;
		this.asyncComplete = asyncComplete;
	}

	public BGLiveUpdateContext Clone()
	{
		return new BGLiveUpdateContext(Repo, addon, timeOut, isAsynchronous, asyncComplete)
		{
			loader = loader
		};
	}
}
