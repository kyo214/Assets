using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public abstract class BGMergeSettingsA
{
	[SerializeField]
	protected BGMergeModeEnum mode;

	[SerializeField]
	protected bool addMissing;

	[SerializeField]
	protected bool updateMatching;

	[SerializeField]
	protected bool removeOrphaned;

	public bool IncludedByDefault
	{
		get
		{
			if (!addMissing && !updateMatching)
			{
				return removeOrphaned;
			}
			return true;
		}
	}

	public BGMergeModeEnum Mode
	{
		get
		{
			return mode;
		}
		set
		{
			if (mode != value)
			{
				mode = value;
				FireOnChange();
			}
		}
	}

	public bool AddMissing
	{
		get
		{
			return addMissing;
		}
		set
		{
			if (addMissing != value)
			{
				addMissing = value;
				FireOnChange();
			}
		}
	}

	public bool UpdateMatching
	{
		get
		{
			return updateMatching;
		}
		set
		{
			if (updateMatching != value)
			{
				updateMatching = value;
				FireOnChange();
			}
		}
	}

	public bool RemoveOrphaned
	{
		get
		{
			return removeOrphaned;
		}
		set
		{
			if (removeOrphaned != value)
			{
				removeOrphaned = value;
				FireOnChange();
			}
		}
	}

	public event Action OnChange;

	protected void FireOnChange()
	{
		OnChange?.Invoke();
	}
}
