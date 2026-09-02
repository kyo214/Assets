using UnityEngine;

namespace Toked;

public interface IScramble
{
	IScramble Generate();

	Vector2 Apply(Vector2 input);
}
