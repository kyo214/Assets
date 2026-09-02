using System;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;
using UnityEngine.U2D.Common;

namespace UnityEngine.U2D.IK;

[MovedFrom("UnityEngine.Experimental.U2D.IK")]
public abstract class Solver2D : MonoBehaviour, IPreviewable
{
	[SerializeField]
	private bool m_ConstrainRotation = true;

	[FormerlySerializedAs("m_RestoreDefaultPose")]
	[SerializeField]
	private bool m_SolveFromDefaultPose = true;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_Weight = 1f;

	private Plane m_Plane;

	private List<Vector3> m_TargetPositions = new List<Vector3>();

	private float m_LastFinalWeight;

	public int chainCount => GetChainCount();

	public bool constrainRotation
	{
		get
		{
			return m_ConstrainRotation;
		}
		set
		{
			m_ConstrainRotation = value;
		}
	}

	public bool solveFromDefaultPose
	{
		get
		{
			return m_SolveFromDefaultPose;
		}
		set
		{
			m_SolveFromDefaultPose = value;
		}
	}

	public bool isValid => Validate();

	public bool allChainsHaveTargets => HasTargets();

	public float weight
	{
		get
		{
			return m_Weight;
		}
		set
		{
			m_Weight = Mathf.Clamp01(value);
		}
	}

	protected virtual void OnValidate()
	{
		m_Weight = Mathf.Clamp01(m_Weight);
		if (!isValid)
		{
			Initialize();
		}
	}

	private bool Validate()
	{
		for (int i = 0; i < GetChainCount(); i++)
		{
			if (!GetChain(i).isValid)
			{
				return false;
			}
		}
		return DoValidate();
	}

	private bool HasTargets()
	{
		for (int i = 0; i < GetChainCount(); i++)
		{
			if (GetChain(i).target == null)
			{
				return false;
			}
		}
		return true;
	}

	public void Initialize()
	{
		DoInitialize();
		for (int i = 0; i < GetChainCount(); i++)
		{
			GetChain(i).Initialize();
		}
	}

	private void Prepare()
	{
		Transform planeRootTransform = GetPlaneRootTransform();
		if (planeRootTransform != null)
		{
			m_Plane.normal = planeRootTransform.forward;
			m_Plane.distance = 0f - Vector3.Dot(m_Plane.normal, planeRootTransform.position);
		}
		for (int i = 0; i < GetChainCount(); i++)
		{
			IKChain2D chain = GetChain(i);
			bool targetRotationIsConstrained = constrainRotation && chain.target != null;
			if (m_SolveFromDefaultPose)
			{
				chain.RestoreDefaultPose(targetRotationIsConstrained);
			}
		}
		DoPrepare();
	}

	private void PrepareEffectorPositions()
	{
		m_TargetPositions.Clear();
		for (int i = 0; i < GetChainCount(); i++)
		{
			IKChain2D chain = GetChain(i);
			if ((bool)chain.target)
			{
				m_TargetPositions.Add(chain.target.position);
			}
		}
	}

	public void UpdateIK(float globalWeight)
	{
		if (allChainsHaveTargets)
		{
			PrepareEffectorPositions();
			UpdateIK(m_TargetPositions, globalWeight);
		}
	}

	public void UpdateIK(List<Vector3> targetPositions, float globalWeight)
	{
		if (targetPositions.Count != chainCount)
		{
			return;
		}
		float num = globalWeight * weight;
		bool flag = Math.Abs(num - m_LastFinalWeight) > 0.0001f;
		m_LastFinalWeight = num;
		if ((num == 0f && !flag) || !isValid)
		{
			return;
		}
		if (num < 1f)
		{
			StoreLocalRotations();
		}
		Prepare();
		DoUpdateIK(targetPositions);
		if (constrainRotation)
		{
			for (int i = 0; i < GetChainCount(); i++)
			{
				IKChain2D chain = GetChain(i);
				if ((bool)chain.target)
				{
					chain.effector.rotation = chain.target.rotation;
				}
			}
		}
		if (num < 1f)
		{
			BlendFkToIk(num);
		}
	}

	private void StoreLocalRotations()
	{
		for (int i = 0; i < GetChainCount(); i++)
		{
			GetChain(i).StoreLocalRotations();
		}
	}

	private void BlendFkToIk(float finalWeight)
	{
		for (int i = 0; i < GetChainCount(); i++)
		{
			IKChain2D chain = GetChain(i);
			bool targetRotationIsConstrained = constrainRotation && chain.target != null;
			chain.BlendFkToIk(finalWeight, targetRotationIsConstrained);
		}
	}

	public abstract IKChain2D GetChain(int index);

	protected abstract int GetChainCount();

	protected abstract void DoUpdateIK(List<Vector3> targetPositions);

	protected virtual bool DoValidate()
	{
		return true;
	}

	protected virtual void DoInitialize()
	{
	}

	protected virtual void DoPrepare()
	{
	}

	protected virtual Transform GetPlaneRootTransform()
	{
		if (chainCount <= 0)
		{
			return null;
		}
		return GetChain(0).rootTransform;
	}

	protected Vector3 GetPointOnSolverPlane(Vector3 worldPosition)
	{
		return GetPlaneRootTransform().InverseTransformPoint(m_Plane.ClosestPointOnPlane(worldPosition));
	}

	protected Vector3 GetWorldPositionFromSolverPlanePoint(Vector2 planePoint)
	{
		return GetPlaneRootTransform().TransformPoint(planePoint);
	}

	public void OnPreviewUpdate()
	{
	}
}
