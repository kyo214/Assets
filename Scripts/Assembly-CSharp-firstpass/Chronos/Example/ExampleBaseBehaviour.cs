using UnityEngine;

namespace Chronos.Example;

[RequireComponent(typeof(Timeline))]
public abstract class ExampleBaseBehaviour : MonoBehaviour
{
	public Timeline time => GetComponent<Timeline>();
}
