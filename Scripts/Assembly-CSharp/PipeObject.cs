using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PipeObject
{
	public List<Animator> AnimatorPipeList = new List<Animator>();

	public List<GameObject> ColliderPipeList = new List<GameObject>();
}
