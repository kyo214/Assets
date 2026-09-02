using UnityEngine;

namespace DestroyIt;

public class ActiveParticle
{
	public GameObject GameObject { get; set; }

	public float InstantiatedTime { get; set; }

	public int ParentId { get; set; }
}
