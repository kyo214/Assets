using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.U2D.IK;

[MovedFrom("UnityEngine.Experimental.U2D.IK")]
[Solver2DMenu("Chain (FABRIK)")]
public sealed class FabrikSolver2D : Solver2D
{
	private const float k_MinTolerance = 0.001f;

	private const int k_MinIterations = 1;

	[SerializeField]
	private IKChain2D m_Chain = new IKChain2D();

	[SerializeField]
	[Range(1f, 50f)]
	private int m_Iterations = 10;

	[SerializeField]
	[Range(0.001f, 0.1f)]
	private float m_Tolerance = 0.01f;

	private float[] m_Lengths;

	private Vector2[] m_Positions;

	private Vector3[] m_WorldPositions;

	public int iterations
	{
		get
		{
			return m_Iterations;
		}
		set
		{
			m_Iterations = Mathf.Max(value, 1);
		}
	}

	public float tolerance
	{
		get
		{
			return m_Tolerance;
		}
		set
		{
			m_Tolerance = Mathf.Max(value, 0.001f);
		}
	}

	protected override int GetChainCount()
	{
		return 1;
	}

	public override IKChain2D GetChain(int index)
	{
		return m_Chain;
	}

	protected override void DoPrepare()
	{
		if (m_Positions == null || m_Positions.Length != m_Chain.transformCount)
		{
			m_Positions = new Vector2[m_Chain.transformCount];
			m_Lengths = new float[m_Chain.transformCount - 1];
			m_WorldPositions = new Vector3[m_Chain.transformCount];
		}
		for (int i = 0; i < m_Chain.transformCount; i++)
		{
			m_Positions[i] = GetPointOnSolverPlane(m_Chain.transforms[i].position);
		}
		for (int j = 0; j < m_Chain.transformCount - 1; j++)
		{
			m_Lengths[j] = (m_Positions[j + 1] - m_Positions[j]).magnitude;
		}
	}

	protected override void DoUpdateIK(List<Vector3> targetPositions)
	{
		Vector3 worldPosition = targetPositions[0];
		worldPosition = GetPointOnSolverPlane(worldPosition);
		if (FABRIK2D.Solve(worldPosition, iterations, tolerance, m_Lengths, ref m_Positions))
		{
			for (int i = 0; i < m_Positions.Length; i++)
			{
				m_WorldPositions[i] = GetWorldPositionFromSolverPlanePoint(m_Positions[i]);
			}
			for (int j = 0; j < m_Chain.transformCount - 1; j++)
			{
				Vector2 vector = m_Chain.transforms[j + 1].localPosition;
				Vector2 to = m_Chain.transforms[j].InverseTransformPoint(m_WorldPositions[j + 1]);
				m_Chain.transforms[j].localRotation *= Quaternion.AngleAxis(Vector2.SignedAngle(vector, to), Vector3.forward);
			}
		}
	}
}
