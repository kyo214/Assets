using UnityEngine;

namespace Toked;

public class MatrixScramble : IScramble
{
	private Matrix4x4 _matrix;

	public MatrixScramble(Matrix4x4 matrix)
	{
		_matrix = matrix;
	}

	public MatrixScramble()
	{
		Generate();
	}

	public IScramble Generate()
	{
		float z = Random.Range(0f, 360f);
		float x = Random.Range(0.8f, 1.2f);
		float y = Random.Range(0.8f, 1.2f);
		_matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, z), new Vector3(x, y, 1f));
		return this;
	}

	public Vector2 Apply(Vector2 input)
	{
		Vector3 vector = _matrix.MultiplyVector(input);
		return new Vector2(vector.x, vector.y).normalized * input.magnitude;
	}
}
