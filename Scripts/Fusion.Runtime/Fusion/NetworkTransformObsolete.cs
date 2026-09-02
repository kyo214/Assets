using System;
using UnityEngine;

namespace Fusion;

[NetworkBehaviourWeaved(25)]
[OrderAfter(new Type[] { typeof(NetworkTransformAnchor) })]
[DisallowMultipleComponent]
[Obsolete("This class has been replaced by a new NetworkTransform class and is now obsolete.")]
public class NetworkTransformObsolete : NetworkTransformAnchor, IAfterPhysicsStep, IAfterTick
{
	protected struct UpdateTransformParameters
	{
		public Vector3 InterpolatedPosition;

		public Vector3 UninterpolatedPosition;

		public Vector3 InterpolatedPositionErrorCorrection;

		public Quaternion InterpolatedRotation;

		public Quaternion UninterpolatedRotation;

		public Quaternion InterpolatedRotationErrorCorrection;

		internal Vector3 BufferToPosition;

		internal Quaternion BufferToRotation;
	}

	public const int POSITION_OFFSET = 4;

	public const int ROTATION_OFFSET = 7;

	public const int SCALE_OFFSET = 11;

	protected const int TELE_POS_OFFSET = 14;

	protected const int TELE_ROT_OFFSET = 17;

	protected const int TELE_SCL_OFFSET = 21;

	protected const int TELEPORT_OFFSET = 24;

	protected const int BASE_WORD_COUNT = 25;

	public bool InterpolateErrorCorrection = true;

	[DrawIf("InterpolateErrorCorrection")]
	public InterpolatedErrorCorrectionSettings InterpolatedErrorCorrectionSettings;

	[InlineHelp]
	[SerializeField]
	protected bool SyncScale;

	[InlineHelp]
	[SerializeField]
	protected Spaces Space = Spaces.World;

	protected float LastInterpolatedAtTime;

	private Tick _lastRenderToTick;

	private bool _lastRenderToTickResim;

	private Vector3 _lastRenderToPos;

	private Quaternion _lastRenderToRot = Quaternion.identity;

	private Vector3 _accumulatedErrorPos;

	private Quaternion _accumulatedErrorRot = Quaternion.identity;

	protected (bool includeParent, Transform parent, Vector3? position, Rotation? rotation, Vector3? localScale, bool reset, Vector3? velocity, Vector3? angularVelocity)? _queuedTeleport;

	public override int PositionWordOffset => 4;

	protected unsafe int* TeleportCounter => Ptr + 24;

	protected virtual void Reset()
	{
		if ((bool)GetComponent<NetworkObject>())
		{
			Space = Spaces.World;
		}
		else
		{
			Space = Spaces.Local;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (!_interpolationTarget)
		{
			_interpolationTarget = base.Transform;
		}
	}

	public override void Spawned()
	{
		base.Spawned();
		if (DetachInterpTarget && (bool)_interpolationTarget && _interpolationTarget != base.Transform && _interpolationDataSource != InterpolationDataSources.NoInterpolation)
		{
			_interpolationTarget.parent = null;
		}
	}

	private void OnDestroy()
	{
		if (DetachInterpTarget && (bool)_interpolationTarget && _interpolationTarget != base.Transform)
		{
			UnityEngine.Object.Destroy(_interpolationTarget.gameObject, 0.1f);
		}
	}

	public override void CopyBackingFieldsToState(bool firstTime)
	{
		CopyEngine2Buffers(posRotOnly: true);
	}

	public void AfterTick()
	{
		if (InterpolateErrorCorrection && Runner.IsResimulation && Runner._simulation.Tick == _lastRenderToTick)
		{
			GetEnginePositionRotation2Buffer(out var pos, out var rot);
			pos = WriteReadVector3(pos, Runner._positionWriteAccuracy, Runner._positionReadAccuracy);
			rot = WriteReadQuaternion(rot, Runner._rotationWriteAccuracy, Runner._rotationReadAccuracy);
			_accumulatedErrorPos += _lastRenderToPos - pos;
			_accumulatedErrorRot = Quaternion.Inverse(rot) * _lastRenderToRot * _accumulatedErrorRot;
			_lastRenderToTickResim = true;
		}
	}

	private unsafe Vector3 WriteReadVector3(Vector3 value, WriteAccuracy writeAccuracy, ReadAccuracy readAccuracy)
	{
		float value2 = writeAccuracy.Value;
		if (value2 != 0f)
		{
			*(int*)(&value.x) = ((value.x < 0f) ? ((int)(value.x * value2 - 0.5f)) : ((int)(value.x * value2 + 0.5f)));
			*(int*)(&value.y) = ((value.y < 0f) ? ((int)(value.y * value2 - 0.5f)) : ((int)(value.y * value2 + 0.5f)));
			*(int*)(&value.z) = ((value.z < 0f) ? ((int)(value.z * value2 - 0.5f)) : ((int)(value.z * value2 + 0.5f)));
		}
		float value3 = readAccuracy.Value;
		if (value3 == 0f)
		{
			value.x = value.x;
			value.y = value.y;
			value.z = value.z;
		}
		else
		{
			value.x = (float)(*(int*)(&value.x)) * value3;
			value.y = (float)(*(int*)(&value.y)) * value3;
			value.z = (float)(*(int*)(&value.z)) * value3;
		}
		return value;
	}

	private unsafe Quaternion WriteReadQuaternion(Quaternion value, WriteAccuracy writeAccuracy, ReadAccuracy readAccuracy)
	{
		float value2 = writeAccuracy.Value;
		if (value2 != 0f)
		{
			*(int*)(&value.x) = ((value.x < 0f) ? ((int)(value.x * value2 - 0.5f)) : ((int)(value.x * value2 + 0.5f)));
			*(int*)(&value.y) = ((value.y < 0f) ? ((int)(value.y * value2 - 0.5f)) : ((int)(value.y * value2 + 0.5f)));
			*(int*)(&value.z) = ((value.z < 0f) ? ((int)(value.z * value2 - 0.5f)) : ((int)(value.z * value2 + 0.5f)));
			*(int*)(&value.w) = ((value.w < 0f) ? ((int)(value.w * value2 - 0.5f)) : ((int)(value.w * value2 + 0.5f)));
		}
		float value3 = readAccuracy.Value;
		if (value3 == 0f)
		{
			value.x = value.x;
			value.y = value.y;
			value.z = value.z;
			value.w = value.w;
		}
		else
		{
			value.x = (float)(*(int*)(&value.x)) * value3;
			value.y = (float)(*(int*)(&value.y)) * value3;
			value.z = (float)(*(int*)(&value.z)) * value3;
			value.w = (float)(*(int*)(&value.w)) * value3;
		}
		return value;
	}

	internal override void CopyBuffers2Engine(bool posRotOnly = false)
	{
		base.CopyBuffers2Engine(posRotOnly);
		Copy2EngineTRSState(4);
	}

	internal override void CopyEngine2Buffers(bool posRotOnly = false)
	{
		base.CopyEngine2Buffers(posRotOnly);
		if (_queuedTeleport.HasValue)
		{
			ApplyQueuedTeleport();
		}
		Copy2BufferTRSState(4);
	}

	protected virtual void GetEnginePositionRotation2Buffer(out Vector3 pos, out Quaternion rot)
	{
		Transform transform = base.transform;
		if (Space == Spaces.World)
		{
			pos = transform.position;
			rot = transform.rotation;
		}
		else
		{
			pos = transform.localPosition;
			rot = transform.localRotation;
		}
	}

	protected unsafe virtual void Copy2BufferTRSState(int offset)
	{
		NetworkRunner runner = Runner;
		GetEnginePositionRotation2Buffer(out var pos, out var rot);
		ReadWriteUtils.WriteVector3(Ptr + offset, runner._positionWriteAccuracy, pos);
		ReadWriteUtils.WriteQuaternion(Ptr + offset + 3, runner._rotationWriteAccuracy, rot);
		if (SyncScale)
		{
			ReadWriteUtils.WriteVector3(Ptr + offset + 7, runner._positionWriteAccuracy, base.Transform.localScale);
		}
	}

	protected unsafe virtual void Copy2EngineTRSState(int offset)
	{
		Transform transform = base.Transform;
		NetworkRunner runner = Runner;
		if (SyncScale)
		{
			Vector3 localScale = ReadWriteUtils.ReadVector3(Ptr + offset + 7, runner._positionReadAccuracy);
			transform.localScale = localScale;
		}
		Quaternion quaternion = ReadWriteUtils.ReadQuaternion(Ptr + offset + 3, runner._rotationReadAccuracy);
		if (Space == Spaces.World)
		{
			transform.rotation = quaternion;
		}
		else
		{
			transform.localRotation = quaternion;
		}
		Vector3 vector = ReadWriteUtils.ReadVector3(Ptr + offset, runner._positionReadAccuracy);
		if (Space == Spaces.World)
		{
			transform.position = vector;
		}
		else
		{
			transform.localPosition = vector;
		}
	}

	public void Teleport(Vector3? position, Rotation? rotation = null, Vector3? localScale = null, bool reset = false, Vector3? velocity = null, Vector3? angularVelocity = null)
	{
		_queuedTeleport = (false, null, position, rotation, localScale, reset, velocity, angularVelocity);
	}

	public void Teleport(NetworkTransformAnchor newParent, Vector3? position, Rotation? rotation = null, Vector3? localScale = null, bool reset = false, Vector3? velocity = null, Vector3? angularVelocity = null)
	{
		_queuedTeleport = (true, newParent ? newParent.transform : null, position, rotation, localScale, reset, velocity, angularVelocity);
	}

	public unsafe void PreTeleport()
	{
		Copy2BufferTRSState(14);
		(*TeleportCounter)++;
	}

	void IAfterPhysicsStep.AfterPhysicsStep()
	{
		if (_queuedTeleport.HasValue)
		{
			ApplyQueuedTeleport();
		}
	}

	protected unsafe virtual void ApplyQueuedTeleport()
	{
		bool item = _queuedTeleport.Value.includeParent;
		Vector3? item2 = _queuedTeleport.Value.position;
		Rotation? item3 = _queuedTeleport.Value.rotation;
		Vector3? item4 = _queuedTeleport.Value.localScale;
		Transform transform = base.Transform;
		if (SyncParent)
		{
			Copy2BufferAnchorState(2);
		}
		Copy2BufferTRSState(14);
		if (_queuedTeleport.Value.includeParent)
		{
			transform.SetParent(_queuedTeleport.Value.parent);
		}
		if (item4.HasValue)
		{
			transform.localScale = item4.Value;
		}
		if (item3.HasValue)
		{
			transform.rotation = item3.Value;
		}
		if (item2.HasValue)
		{
			transform.position = item2.Value;
		}
		_queuedTeleport = null;
		(*TeleportCounter)++;
	}

	public override void Render()
	{
		if (base.InterpolationDataSource != InterpolationDataSources.NoInterpolation && (bool)_interpolationTarget && GetInterpolationData(out var data, out var predicted))
		{
			InterpolateTransform(ref data, predicted);
		}
	}

	protected unsafe virtual Quaternion ReadRotationToQuaternion(int* offset, ReadAccuracy readAccuracy)
	{
		return ReadWriteUtils.ReadQuaternion(offset, readAccuracy);
	}

	protected unsafe void InterpolateTransform(ref InterpolationData data, bool isState)
	{
		UpdateTransformParameters param = default;
		if (!ComputeInterpolatedTransform(ref data, ref param))
		{
			return;
		}
		if ((!Object.HasStateAuthority & isState) && InterpolateErrorCorrection)
		{
			if (!_lastRenderToTickResim && Runner._simulation.SnapshotHistory.TryGet(_lastRenderToTick, out var snapshot) && snapshot.TryGetObject(Object.Id, out var header))
			{
				Vector3 vector = ReadWriteUtils.ReadVector3((int*)((byte*)header + (nint)WordOffset * (nint)4) + 4, Runner._positionReadAccuracy);
				Quaternion rotation = ReadRotationToQuaternion((int*)((byte*)header + (nint)WordOffset * (nint)4) + 7, Runner._rotationReadAccuracy);
				_accumulatedErrorPos += _lastRenderToPos - vector;
				_accumulatedErrorRot = Quaternion.Inverse(rotation) * _lastRenderToRot * _accumulatedErrorRot;
			}
			UpdateInterpolatedErrorCorrection(ref param, InterpolatedErrorCorrectionSettings);
			_lastRenderToTick = data.ToTick;
			_lastRenderToTickResim = false;
			_lastRenderToPos = param.BufferToPosition;
			_lastRenderToRot = param.BufferToRotation;
		}
		else
		{
			_accumulatedErrorPos = default;
			_accumulatedErrorRot = Quaternion.identity;
			param.InterpolatedRotationErrorCorrection = Quaternion.identity;
		}
		ApplyTransform(ref param);
	}

	private unsafe bool ComputeInterpolatedTransform(ref InterpolationData data, ref UpdateTransformParameters param)
	{
		float time = Time.time;
		if (LastInterpolatedAtTime == time)
		{
			return false;
		}
		LastInterpolatedAtTime = time;
		ReadAccuracy positionReadAccuracy = Runner._positionReadAccuracy;
		ReadAccuracy rotationReadAccuracy = Runner._rotationReadAccuracy;
		bool flag = data.To[24] > data.From[24];
		if (Space == Spaces.World)
		{
			Vector3 a = ReadWriteUtils.ReadVector3(data.From + 4, positionReadAccuracy);
			param.UninterpolatedPosition = ReadWriteUtils.ReadVector3(data.To + (flag ? 14 : 4), positionReadAccuracy);
			param.InterpolatedPosition = Vector3.Lerp(a, param.UninterpolatedPosition, data.Alpha);
			Quaternion a2 = ReadRotationToQuaternion(data.From + 7, rotationReadAccuracy);
			param.UninterpolatedRotation = ReadRotationToQuaternion(data.To + (flag ? 17 : 7), rotationReadAccuracy);
			param.InterpolatedRotation = Quaternion.Slerp(a2, param.UninterpolatedRotation, data.Alpha);
			param.BufferToPosition = param.UninterpolatedPosition;
			param.BufferToRotation = param.UninterpolatedRotation;
			if (SyncScale)
			{
				Vector3 a3 = ReadWriteUtils.ReadVector3(data.From + 11, positionReadAccuracy);
				Vector3 b = ReadWriteUtils.ReadVector3(data.To + (flag ? 21 : 11), positionReadAccuracy);
				Vector3 localScale = Vector3.Lerp(a3, b, data.Alpha);
				_interpolationTarget.transform.localScale = localScale;
			}
		}
		else
		{
			Transform transform = base.Transform;
			Transform transform2 = null;
			Transform transform3;
			Transform transform4;
			if (SyncParent)
			{
				(NetworkTransformAnchor, NetworkTransformAnchor) parentsForInterpolation = NetworkTransformAnchor.GetParentsForInterpolation(this, Runner, flag, ref data, out var fromParentIsValid, out var toParentIsValid);
				if (fromParentIsValid)
				{
					if ((bool)parentsForInterpolation.Item1)
					{
						transform2 = parentsForInterpolation.Item1.transform;
						transform3 = parentsForInterpolation.Item1.InterpolationTarget;
						if (transform3 == null)
						{
							transform3 = parentsForInterpolation.Item1.transform;
						}
					}
					else
					{
						transform3 = null;
					}
					if (DetachInterpTarget)
					{
						_interpolationTarget.SetParent(transform3);
					}
				}
				else
				{
					transform3 = transform.parent;
				}
				if (toParentIsValid)
				{
					if ((bool)parentsForInterpolation.Item2)
					{
						transform4 = parentsForInterpolation.Item2.InterpolationTarget;
						if (transform4 == null)
						{
							transform4 = parentsForInterpolation.Item2.transform;
						}
					}
					else
					{
						transform4 = null;
					}
				}
				else
				{
					transform4 = transform.parent;
				}
			}
			else
			{
				transform3 = transform.parent;
				transform4 = transform.parent;
			}
			if (SyncScale)
			{
				Vector3 a4 = ReadWriteUtils.ReadVector3(data.From + 11, positionReadAccuracy);
				Vector3 b2 = ReadWriteUtils.ReadVector3(data.To + (flag ? 21 : 11), positionReadAccuracy);
				Vector3 localScale2 = Vector3.Lerp(a4, b2, data.Alpha);
				if (!DetachInterpTarget)
				{
					Transform parent = _interpolationTarget.parent;
					if ((bool)parent)
					{
						Vector3 vector = parent.transform.localScale;
						if ((bool)transform.parent && transform3 == null)
						{
							vector = parent.transform.lossyScale;
							localScale2 = new Vector3(vector.x / localScale2.x, vector.y / localScale2.y, vector.z / localScale2.z);
							_interpolationTarget.localScale = localScale2;
						}
						if (transform.parent == null && (bool)transform3)
						{
							Transform parent2 = _interpolationTarget.parent;
							_interpolationTarget.SetParent(transform3);
							_interpolationTarget.localScale = localScale2;
							_interpolationTarget.SetParent(parent2);
						}
						else
						{
							localScale2 = new Vector3(vector.x / localScale2.x, vector.y / localScale2.y, vector.z / localScale2.z);
							_interpolationTarget.localScale = localScale2;
						}
					}
				}
				else
				{
					_interpolationTarget.localScale = localScale2;
				}
			}
			Quaternion quaternion = ReadRotationToQuaternion(data.From + 7, rotationReadAccuracy);
			param.BufferToRotation = ReadRotationToQuaternion(data.To + (flag ? 17 : 7), rotationReadAccuracy);
			Quaternion a5 = (transform3 ? (transform3.transform.rotation * quaternion) : quaternion);
			param.UninterpolatedRotation = (transform4 ? (transform4.transform.rotation * param.BufferToRotation) : param.BufferToRotation);
			param.InterpolatedRotation = Quaternion.Slerp(a5, param.UninterpolatedRotation, data.Alpha);
			Vector3 vector2 = ReadWriteUtils.ReadVector3(data.From + 4, positionReadAccuracy);
			param.BufferToPosition = ReadWriteUtils.ReadVector3(data.To + (flag ? 14 : 4), positionReadAccuracy);
			Vector3 a6 = (transform3 ? transform3.TransformPoint(vector2) : vector2);
			param.UninterpolatedPosition = (transform4 ? transform4.TransformPoint(param.BufferToPosition) : param.BufferToPosition);
			param.InterpolatedPosition = Vector3.Lerp(a6, param.UninterpolatedPosition, data.Alpha);
		}
		return true;
	}

	private void UpdateInterpolatedErrorCorrection(ref UpdateTransformParameters param, InterpolatedErrorCorrectionSettings settings)
	{
		float num = settings.MinRate;
		float num2 = settings.MinRate;
		float magnitude = _accumulatedErrorPos.magnitude;
		if (magnitude > settings.PosTeleportDistance)
		{
			_accumulatedErrorPos = default;
		}
		else
		{
			float num3 = settings.PosBlendEnd - settings.PosBlendStart;
			float t = Mathf.Clamp01((magnitude - settings.PosBlendStart) / num3);
			num = Mathf.Lerp(settings.MinRate, settings.MaxRate, t);
		}
		float value = Quaternion.Dot(_accumulatedErrorRot, Quaternion.identity);
		value = Mathf.Clamp(value, -1f, 1f);
		float num4 = Mathf.Acos(value) * 2f;
		if (num4 > settings.RotTeleportRadians)
		{
			_accumulatedErrorRot = Quaternion.identity;
		}
		else
		{
			float num5 = settings.RotBlendEnd - settings.RotBlendStart;
			float t2 = Mathf.Clamp01((num4 - settings.RotBlendStart) / num5);
			num2 = Mathf.Lerp(settings.MinRate, settings.MaxRate, t2);
		}
		param.InterpolatedPositionErrorCorrection = _accumulatedErrorPos;
		param.InterpolatedRotationErrorCorrection = _accumulatedErrorRot;
		float num6 = 1f - Time.deltaTime * num;
		if ((_accumulatedErrorPos * num6).magnitude < settings.PosMinCorrection)
		{
			UpdateMinPositionCorrection(settings);
		}
		else
		{
			_accumulatedErrorPos *= num6;
		}
		_accumulatedErrorRot = Quaternion.Slerp(_accumulatedErrorRot, Quaternion.identity, Time.deltaTime * num2);
	}

	private void UpdateMinPositionCorrection(InterpolatedErrorCorrectionSettings settings)
	{
		if (_accumulatedErrorPos.x != 0f || _accumulatedErrorPos.y != 0f || _accumulatedErrorPos.z != 0f)
		{
			Vector3 normalized = _accumulatedErrorPos.normalized;
			bool flag = _accumulatedErrorPos.x >= 0f;
			bool flag2 = _accumulatedErrorPos.y >= 0f;
			bool flag3 = _accumulatedErrorPos.z >= 0f;
			_accumulatedErrorPos -= normalized * settings.PosMinCorrection;
			if (flag != _accumulatedErrorPos.x >= 0f)
			{
				_accumulatedErrorPos.x = 0f;
			}
			if (flag2 != _accumulatedErrorPos.y >= 0f)
			{
				_accumulatedErrorPos.y = 0f;
			}
			if (flag3 != _accumulatedErrorPos.z >= 0f)
			{
				_accumulatedErrorPos.z = 0f;
			}
		}
	}

	protected virtual void ApplyTransform(ref UpdateTransformParameters param)
	{
		Vector3 position = param.InterpolatedPosition + param.InterpolatedPositionErrorCorrection;
		Quaternion rotation = param.InterpolatedRotationErrorCorrection * param.InterpolatedRotation;
		base.InterpolationTarget.SetPositionAndRotation(position, rotation);
	}
}
