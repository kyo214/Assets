using UnityEngine;

namespace Toked;

public static class ControlScrambler
{
	public enum ScrambleType
	{
		None = 0,
		Rotation = 1,
		StrictRotation = 2,
		DiscreteScramble = 3,
		DiscreteScramble8 = 4,
		Chaos = 5,
		DiscreteScrambleMirror = 6,
		DiscreteScramble8Mirror = 7
	}

	public static IScramble GenerateScramble(ScrambleType type)
	{
		return type switch
		{
			ScrambleType.Rotation => new QuaternionScramble(), 
			ScrambleType.StrictRotation => new QuaternionScramble(strictRandomRotation: true), 
			ScrambleType.DiscreteScramble => new DiscreteScramble(allowMirror: false), 
			ScrambleType.DiscreteScramble8 => new DiscreteScramble8(allowMirror: false), 
			ScrambleType.DiscreteScrambleMirror => new DiscreteScramble(), 
			ScrambleType.DiscreteScramble8Mirror => new DiscreteScramble8(), 
			ScrambleType.Chaos => new MatrixScramble(), 
			_ => new QuaternionScramble(Quaternion.identity), 
		};
	}
}
