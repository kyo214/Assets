using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks;

public class MMMiniObjectPool : MonoBehaviour
{
	[MMFReadOnly]
	public List<GameObject> PooledGameObjects;
}
