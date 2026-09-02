using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AddComponentMenu("BansheeGz/Id")]
[DisallowMultipleComponent]
[ExecuteInEditMode]
public class BGWithId : MonoBehaviour
{
	[SerializeField]
	private string IdString;

	private static readonly Dictionary<BGId, List<BGWithId>> id2Component = new Dictionary<BGId, List<BGWithId>>();

	private BGId BGId
	{
		get
		{
			string idString = IdString;
			if (idString != null && idString.Length == 22)
			{
				return new BGId(IdString);
			}
			return BGId.Empty;
		}
	}

	public BGId Id
	{
		get
		{
			return BGId;
		}
		set
		{
			Remove(this);
			IdString = value.ToString();
			Add(this);
		}
	}

	public void NewId()
	{
		Id = BGId.NewId;
	}

	private void Awake()
	{
		Add(this);
	}

	private void OnDestroy()
	{
		Remove(this);
	}

	private void OnEnable()
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			List<BGWithId> all = GetAll(BGId);
			if (all == null || !all.Any((BGWithId t) => t == this))
			{
				Add(this);
			}
		}
	}

	private void Reset()
	{
		Id = BGId.NewId;
	}

	public static BGWithId Get(BGId id)
	{
		List<BGWithId> list = BGUtil.Get(id2Component, id);
		if (list == null || list.Count == 0)
		{
			return null;
		}
		return list[0];
	}

	public static List<BGWithId> GetAll(BGId id)
	{
		List<BGWithId> list = BGUtil.Get(id2Component, id);
		if (list == null || list.Count == 0)
		{
			return null;
		}
		return list;
	}

	private static void Add(BGWithId withId)
	{
		if (!(withId == null) && !withId.Id.IsEmpty)
		{
			if (!id2Component.TryGetValue(withId.Id, out var value))
			{
				value = new List<BGWithId>();
				id2Component[withId.Id] = value;
			}
			value.Add(withId);
		}
	}

	private static void Remove(BGWithId withId)
	{
		if (withId == null || withId.Id.IsEmpty || !id2Component.TryGetValue(withId.Id, out var value) || value == null)
		{
			return;
		}
		for (int num = value.Count - 1; num >= 0; num--)
		{
			BGWithId bGWithId = value[num];
			if (!(bGWithId != withId))
			{
				value.RemoveAt(num);
			}
		}
		if (value.Count == 0)
		{
			id2Component.Remove(withId.Id);
		}
	}
}
