using System.Collections.Generic;
using UnityEngine;

namespace DestroyIt;

public static class TagItExtensions
{
	public static bool HasTag(this GameObject go, params Tag[] searchTags)
	{
		TagIt component = go.GetComponent<TagIt>();
		if (component == null)
		{
			return false;
		}
		for (int i = 0; i < searchTags.Length; i++)
		{
			for (int j = 0; j < component.tags.Count; j++)
			{
				if (searchTags[i] == component.tags[j])
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool HasTagInParent(this GameObject go, params Tag[] searchTags)
	{
		TagIt componentInParent = go.GetComponentInParent<TagIt>();
		if (componentInParent == null)
		{
			return false;
		}
		for (int i = 0; i < searchTags.Length; i++)
		{
			for (int j = 0; j < componentInParent.tags.Count; j++)
			{
				if (searchTags[i] == componentInParent.tags[j])
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void AddTag(this GameObject go, Tag tag)
	{
		TagIt tagIt = go.GetComponent<TagIt>();
		if (tagIt == null)
		{
			tagIt = go.AddComponent<TagIt>();
			tagIt.tags = new List<Tag>();
		}
		else if (tagIt.tags.Contains(tag))
		{
			return;
		}
		tagIt.tags.Add(tag);
	}

	public static void RemoveTag(this GameObject go, Tag tag)
	{
		TagIt component = go.GetComponent<TagIt>();
		if (!(component == null))
		{
			component.tags.Remove(tag);
		}
	}

	public static GameObject GetHighestParentWithTag(this GameObject go, Tag tag)
	{
		List<Transform> list = new List<Transform>();
		Transform transform = go.transform;
		while (transform != null)
		{
			list.Add(transform);
			transform = transform.parent;
		}
		TagIt tagIt = null;
		for (int num = list.Count - 1; num >= 0; num--)
		{
			tagIt = list[num].GetComponent<TagIt>();
			if (tagIt != null && tagIt.tags.Contains(tag))
			{
				return list[num].gameObject;
			}
		}
		return null;
	}
}
