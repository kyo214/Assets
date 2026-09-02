using UnityEngine;

namespace DestroyIt;

public class ObjectToFade
{
	public MeshRenderer MeshRenderer { get; set; }

	public bool IsStripped { get; set; }

	public bool CanBeFaded { get; set; }

	public Rigidbody Rigidbody { get; set; }

	public Collider[] Colliders { get; set; }

	public bool IsTransparencyChecked { get; set; }
}
