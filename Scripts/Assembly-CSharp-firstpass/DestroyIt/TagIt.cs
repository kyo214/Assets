using System.Collections.Generic;
using UnityEngine;

namespace DestroyIt;

[DisallowMultipleComponent]
public class TagIt : MonoBehaviour
{
	public List<Tag> tags;

	public void OnEnable()
	{
		if (tags == null)
		{
			tags = new List<Tag>();
		}
		if (tags.Count == 0)
		{
			tags.Add(Tag.Untagged);
		}
	}
}
