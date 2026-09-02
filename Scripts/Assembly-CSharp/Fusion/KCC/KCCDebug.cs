using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCDebug
{
	public EKCCStage TraceStage;

	public bool UseFixedData;

	public bool EnableLogs;

	public bool ShowPath;

	public bool ShowGrounding;

	public bool ShowSteppingUp;

	public bool ShowGroundSnapping;

	public bool ShowGroundNormal;

	public bool ShowGroundTangent;

	public bool ShowKinematicTangent;

	public float DisplayTime = 10f;

	public readonly List<IKCCProcessor> ProcessorsStack = new List<IKCCProcessor>();

	public static readonly Color FixedPathColor = Color.red;

	public static readonly Color RenderPathColor = Color.green;

	public static readonly Color FixedToRenderPathColor = Color.blue;

	public static readonly Color PredictionCorrectionColor = Color.magenta;

	public static readonly Color PredictionErrorColor = Color.yellow;

	public static readonly Color IsGroundedColor = Color.green;

	public static readonly Color WasGroundedColor = Color.red;

	public static readonly Color IsSteppingUpColor = Color.green;

	public static readonly Color WasSteppingUpColor = Color.red;

	public static readonly Color GroundNormalColor = Color.magenta;

	public static readonly Color GroundTangentColor = Color.yellow;

	public static readonly Color GroundSnapingColor = Color.cyan;

	public static readonly Color GroundSnapTargetColor = Color.blue;

	public static readonly Color GroundSnapPositionColor = Color.red;

	public static readonly Color KinematicTangentColor = Color.yellow;

	public StringBuilder _stringBuilder = new StringBuilder(1024);

	public void SetDefaults()
	{
		TraceStage = EKCCStage.None;
		UseFixedData = true;
		EnableLogs = false;
		ShowPath = false;
		ShowGrounding = false;
		ShowSteppingUp = false;
		ShowGroundSnapping = false;
		ShowGroundNormal = false;
		ShowGroundTangent = false;
		ShowKinematicTangent = false;
		ProcessorsStack.Clear();
	}

	public void FixedUpdate(KCC kcc)
	{
		Log(kcc, isInFixedUpdate: true);
	}

	public void RenderUpdate(KCC kcc)
	{
		KCCData fixedData = kcc.FixedData;
		KCCData renderData = kcc.RenderData;
		if (ShowPath)
		{
			Debug.DrawLine(fixedData.BasePosition, fixedData.TargetPosition, FixedPathColor, DisplayTime);
			Debug.DrawLine(renderData.BasePosition, renderData.TargetPosition, RenderPathColor, DisplayTime);
		}
		KCCData kCCData = (UseFixedData ? fixedData : renderData);
		if (ShowGrounding)
		{
			if (kCCData.IsGrounded && !kCCData.WasGrounded)
			{
				Debug.DrawLine(kCCData.TargetPosition, kCCData.TargetPosition + Vector3.up, IsGroundedColor, DisplayTime);
			}
			else if (!kCCData.IsGrounded && kCCData.WasGrounded)
			{
				Debug.DrawLine(kCCData.TargetPosition, kCCData.TargetPosition + Vector3.up, WasGroundedColor, DisplayTime);
			}
		}
		if (ShowSteppingUp)
		{
			if (kCCData.IsSteppingUp && !kCCData.WasSteppingUp)
			{
				Debug.DrawLine(kCCData.TargetPosition, kCCData.TargetPosition + Vector3.up, IsSteppingUpColor, DisplayTime);
			}
			else if (!kCCData.IsSteppingUp && kCCData.WasSteppingUp)
			{
				Debug.DrawLine(kCCData.TargetPosition, kCCData.TargetPosition + Vector3.up, WasSteppingUpColor, DisplayTime);
			}
		}
		if (ShowGroundNormal)
		{
			Debug.DrawLine(kCCData.TargetPosition, kCCData.TargetPosition + kCCData.GroundNormal, GroundNormalColor, DisplayTime);
		}
		if (ShowGroundTangent)
		{
			Debug.DrawLine(kCCData.TargetPosition, kCCData.TargetPosition + kCCData.GroundTangent, GroundTangentColor, DisplayTime);
		}
		if (ShowKinematicTangent)
		{
			Debug.DrawLine(kCCData.TargetPosition, kCCData.TargetPosition + kCCData.KinematicTangent, KinematicTangentColor, DisplayTime);
		}
		Log(kcc, isInFixedUpdate: false);
	}

	public void Reset()
	{
		ProcessorsStack.Clear();
	}

	public void DrawGroundSnapping(Vector3 targetPosition, Vector3 targetGroundedPosition, Vector3 targetSnappedPosition, bool isInFixedUpdate)
	{
		if (ShowGroundSnapping && UseFixedData == isInFixedUpdate)
		{
			Debug.DrawLine(targetPosition, targetPosition + Vector3.up, GroundSnapingColor, DisplayTime);
			Debug.DrawLine(targetPosition, targetGroundedPosition, GroundSnapTargetColor, DisplayTime);
			Debug.DrawLine(targetPosition, targetSnappedPosition, GroundSnapPositionColor, DisplayTime);
		}
	}

	private void Log(KCC kcc, bool isInFixedUpdate)
	{
		if (EnableLogs)
		{
			_stringBuilder.Clear();
			KCCData kCCData;
			if (isInFixedUpdate)
			{
				kCCData = kcc.FixedData;
				_stringBuilder.Append("[F]");
			}
			else
			{
				kCCData = kcc.RenderData;
				_stringBuilder.Append("[R]");
			}
			_stringBuilder.Append(" | Frame " + kCCData.Frame);
			_stringBuilder.Append(" | Tick " + kCCData.Tick);
			_stringBuilder.Append(" | Alpha " + kCCData.Alpha.ToString("F4"));
			_stringBuilder.Append(" | Time " + kCCData.Time.ToString("F6"));
			_stringBuilder.Append(" | DeltaTime " + kCCData.DeltaTime.ToString("F6"));
			_stringBuilder.Append(" | BasePosition " + kCCData.BasePosition.ToString("F4"));
			_stringBuilder.Append(" | DesiredPosition " + kCCData.DesiredPosition.ToString("F4"));
			_stringBuilder.Append(" | TargetPosition " + kCCData.TargetPosition.ToString("F4"));
			_stringBuilder.Append(" | LookPitch " + kCCData.LookPitch.ToString("0.00°"));
			_stringBuilder.Append(" | LookYaw " + kCCData.LookYaw.ToString("0.00°"));
			_stringBuilder.Append(" | InputDirection " + kCCData.InputDirection.ToString("F4"));
			_stringBuilder.Append(" | ExternalVelocity " + kCCData.ExternalVelocity.ToString("F4"));
			_stringBuilder.Append(" | ExternalAcceleration " + kCCData.ExternalAcceleration.ToString("F4"));
			_stringBuilder.Append(" | ExternalImpulse " + kCCData.ExternalImpulse.ToString("F4"));
			_stringBuilder.Append(" | ExternalForce " + kCCData.ExternalForce.ToString("F4"));
			_stringBuilder.Append(" | DynamicVelocity " + kCCData.DynamicVelocity.ToString("F4"));
			_stringBuilder.Append(" | KinematicSpeed " + kCCData.KinematicSpeed.ToString("F4"));
			_stringBuilder.Append(" | KinematicTangent " + kCCData.KinematicTangent.ToString("F4"));
			_stringBuilder.Append(" | KinematicDirection " + kCCData.KinematicDirection.ToString("F4"));
			_stringBuilder.Append(" | KinematicVelocity " + kCCData.KinematicVelocity.ToString("F4"));
			_stringBuilder.Append(" | IsGrounded " + (kCCData.IsGrounded ? "1" : "0"));
			_stringBuilder.Append(" | WasGrounded " + (kCCData.WasGrounded ? "1" : "0"));
			_stringBuilder.Append(" | IsOnEdge " + (kCCData.IsOnEdge ? "1" : "0"));
			_stringBuilder.Append(" | IsSteppingUp " + (kCCData.IsSteppingUp ? "1" : "0"));
			_stringBuilder.Append(" | WasSteppingUp " + (kCCData.WasSteppingUp ? "1" : "0"));
			_stringBuilder.Append(" | IsSnappingToGround " + (kCCData.IsSnappingToGround ? "1" : "0"));
			_stringBuilder.Append(" | WasSnappingToGround " + (kCCData.WasSnappingToGround ? "1" : "0"));
			_stringBuilder.Append(" | HasJumped " + (kCCData.HasJumped ? "1" : "0"));
			_stringBuilder.Append(" | HasTeleported " + (kCCData.HasTeleported ? "1" : "0"));
			_stringBuilder.Append(" | GroundNormal " + kCCData.GroundNormal.ToString("F4"));
			_stringBuilder.Append(" | GroundTangent " + kCCData.GroundTangent.ToString("F4"));
			_stringBuilder.Append(" | GroundPosition " + kCCData.GroundPosition.ToString("F4"));
			_stringBuilder.Append(" | GroundDistance " + kCCData.GroundDistance.ToString("F4"));
			_stringBuilder.Append(" | GroundAngle " + kCCData.GroundAngle.ToString("0.00°"));
			_stringBuilder.Append(" | RealSpeed " + kCCData.RealSpeed.ToString("F4"));
			_stringBuilder.Append(" | RealVelocity " + kCCData.RealVelocity.ToString("F4"));
			_stringBuilder.Append(" | Collisions " + kCCData.Collisions.Count);
			_stringBuilder.Append(" | Modifiers " + kCCData.Modifiers.Count);
			_stringBuilder.Append(" | Ignores " + kCCData.Ignores.Count);
			_stringBuilder.Append(" | Hits " + kCCData.Hits.Count);
			if (isInFixedUpdate)
			{
				Debug.LogWarning(_stringBuilder.ToString());
				return;
			}
			_stringBuilder.Append(" | PredictionError " + kcc.PredictionError.ToString("F4"));
			Debug.Log(_stringBuilder.ToString());
		}
	}
}
