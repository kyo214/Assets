#define DEBUG
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fusion;

[NetworkBehaviourWeaved(20)]
[OrderAfter(new Type[] { typeof(NetworkPositionRotation) })]
[DisallowMultipleComponent]
[AddComponentMenu("Fusion/Network Transform")]
[HelpURL("https://doc.photonengine.com/fusion/current/manual/prebuilt-components#networktransform")]
public class NetworkTransform : NetworkPositionRotation, IAfterTick, IPredictedSpawnBehaviour
{
	private class ConsecutiveStateInterp : IInterpolationImplementation
	{
		private NetworkTransform _nt;

		public void Reset(NetworkTransform nt)
		{
			_nt = nt;
		}

		void IInterpolationImplementation.AfterApplyInterpolatedTransform()
		{
		}

		bool IInterpolationImplementation.TryComputeInterpolatedTransform(out InterpolatedTransformParameters param)
		{
			if (_nt.GetInterpolationData(out var data))
			{
				ComputeInterpolatedTransform(ref data, out param);
				return true;
			}
			param = default;
			return false;
		}

		public void ComputeInterpolatedTransform(ref InterpolationData data, out InterpolatedTransformParameters param)
		{
			param.InterpolatedPositionErrorCorrection = default;
			param.InterpolatedRotationErrorCorrection = Quaternion.identity;
			param.InterpolationAlpha = data.Alpha;
			_nt.GetUninterpolatedWorldPositions(ref data, out param.UninterpolatedPositionFrom, out param.UninterpolatedPositionTo);
			_nt.GetUninterpolatedWorldRotations(ref data, out param.UninterpolatedRotationFrom, out param.UninterpolatedRotationTo);
			InterpolatePositionRotation(_nt, param.UninterpolatedPositionFrom, param.UninterpolatedRotationFrom, param.UninterpolatedPositionTo, param.UninterpolatedRotationTo, param.InterpolationAlpha, out param.InterpolatedPosition, out param.InterpolatedRotation);
		}

		public unsafe static void InterpolatePositionRotation(NetworkTransform nt, Vector3 fromPos, Quaternion fromRot, Vector3 toPos, Quaternion toRot, float alpha, out Vector3 pos, out Quaternion rot)
		{
			if (nt.InterpolationSpace == Spaces.World || nt.Transform.parent == null)
			{
				pos = Vector3.Lerp(fromPos, toPos, alpha);
				rot = Quaternion.Slerp(fromRot, toRot, alpha);
				return;
			}
			Assert.Check(nt.InterpolationSpace == Spaces.Local);
			NetworkPositionRotation componentInParent = nt.Transform.parent.GetComponentInParent<NetworkPositionRotation>();
			if (BehaviourUtils.IsNotAlive(componentInParent) || !componentInParent.GetInterpolationData(out var data))
			{
				pos = Vector3.Lerp(fromPos, toPos, alpha);
				rot = Quaternion.Slerp(fromRot, toRot, alpha);
				return;
			}
			Vector3 vector = nt.ReadPosition(data.From);
			Quaternion quaternion = nt.ReadRotation(data.From);
			Vector3 vector2 = nt.ReadPosition(data.To);
			Quaternion quaternion2 = nt.ReadRotation(data.To);
			Quaternion quaternion3 = Quaternion.Inverse(quaternion);
			Quaternion quaternion4 = Quaternion.Inverse(quaternion2);
			Vector3 a = quaternion3 * (fromPos - vector);
			Quaternion a2 = fromRot * quaternion3;
			Vector3 b = quaternion4 * (toPos - vector2);
			Quaternion b2 = toRot * quaternion4;
			pos = Vector3.Lerp(a, b, alpha);
			rot = Quaternion.Slerp(a2, b2, alpha);
			Vector3 vector3 = Vector3.Lerp(vector, vector2, alpha);
			Quaternion quaternion5 = Quaternion.Slerp(quaternion, quaternion2, alpha);
			pos = vector3 + quaternion5 * pos;
			rot *= quaternion5;
		}
	}

	protected struct InterpolatedTransformParameters
	{
		public float InterpolationAlpha;

		public Vector3 UninterpolatedPositionFrom;

		public Vector3 UninterpolatedPositionTo;

		public Vector3 InterpolatedPosition;

		public Vector3 InterpolatedPositionErrorCorrection;

		public Quaternion UninterpolatedRotationFrom;

		public Quaternion UninterpolatedRotationTo;

		public Quaternion InterpolatedRotation;

		public Quaternion InterpolatedRotationErrorCorrection;
	}

	private interface IInterpolationImplementation
	{
		bool TryComputeInterpolatedTransform(out InterpolatedTransformParameters param);

		void Reset(NetworkTransform nt);

		void AfterApplyInterpolatedTransform();
	}

	private class IntermittentStateInterp : IInterpolationImplementation
	{
		private struct PositionState(Vector3 pos, Tick tick, bool stoppedChanging = false)
		{
			public readonly Vector3 Position = pos;

			public readonly Tick Tick = tick;

			public bool StoppedChanging = stoppedChanging;
		}

		private struct RotationState(Quaternion rot, Tick tick, bool stoppedChanging = false)
		{
			public readonly Quaternion Rotation = rot;

			public readonly Tick Tick = tick;

			public bool StoppedChanging = stoppedChanging;
		}

		private const int _initBufferCapacity = 5;

		private const int _bufferIncreaseStep = 3;

		private const int _bufferMaxSize = 30;

		private const float _limitExtrapolationSeconds = 1f;

		private NetworkTransform _nt;

		private PositionRotationValues _accumulatedError;

		private float _lastUpdateSimTime;

		private Tick _latestTeleportPosTick;

		private Tick _latestTeleportRotTick;

		private PlayerRef _latestStateAuth;

		private Tick _latestSnapshotTick;

		private Vector3 _currInterpPos;

		private Quaternion _currInterpRot;

		private float _currInterpPosAccTime;

		private float _currInterpRotAccTime;

		private float _currPosStateDt;

		private float _currRotStateDt;

		private int _posStateHead;

		private int _posStateTail;

		private PositionState[] _posStateBuffer;

		private int _rotStateHead;

		private int _rotStateTail;

		private RotationState[] _rotStateBuffer;

		private const float initializationDuration = 5f;

		private const float initializationMaxDeltaTimeIncrease = 1.5f;

		private bool initializationFinished = false;

		private float initializationStart = -1f;

		public void Reset(NetworkTransform nt)
		{
			Assert.Check(condition: true, 3);
			_nt = nt;
			_accumulatedError = PositionRotationValues.Default();
			_lastUpdateSimTime = 0f;
			_latestTeleportPosTick = default;
			_latestTeleportRotTick = default;
			_latestStateAuth = default;
			_latestSnapshotTick = default;
			_currInterpPos = default;
			_currInterpRot = Quaternion.identity;
			_currInterpPosAccTime = 0f;
			_currInterpRotAccTime = 0f;
			_currPosStateDt = 0f;
			_currRotStateDt = 0f;
			_posStateHead = 0;
			_posStateTail = 0;
			_rotStateHead = 0;
			_rotStateTail = 0;
			float num = Mathf.Max(0f, _nt.TargetInterpolationDelay);
			int num2 = 5 + Mathf.CeilToInt(num * 15f);
			if (_posStateBuffer == null)
			{
				_posStateBuffer = new PositionState[num2];
			}
			else
			{
				Array.Clear(_posStateBuffer, 0, _posStateBuffer.Length);
			}
			if (_rotStateBuffer == null)
			{
				_rotStateBuffer = new RotationState[num2];
			}
			else
			{
				Array.Clear(_rotStateBuffer, 0, _rotStateBuffer.Length);
			}
		}

		public unsafe bool TryComputeInterpolatedTransform(out InterpolatedTransformParameters param)
		{
			if (initializationStart == -1f)
			{
				initializationStart = Time.time;
			}
			if (!initializationFinished && Time.time > initializationStart + 5f)
			{
				initializationFinished = true;
			}
			Simulation simulation = _nt.Runner.Simulation;
			float num = (float)(simulation.State.Time + (double)(simulation.StateAlpha * simulation.DeltaTime));
			float num2;
			if (_lastUpdateSimTime < float.Epsilon)
			{
				num2 = 0f;
				_lastUpdateSimTime = num;
			}
			else
			{
				num2 = num - _lastUpdateSimTime;
				_lastUpdateSimTime = num;
			}
			if (!initializationFinished)
			{
				num2 = Mathf.Min(1.5f * Time.deltaTime, num2);
			}
			float num3 = (float)_nt.Runner.Simulation.Config.DeltaTime;
			float num4 = 0f;
			float num5 = 0f;
			SimulationSnapshot latest = _nt.Runner.Simulation.SnapshotHistory.Latest;
			if (latest != null && latest.Tick.Raw > _latestSnapshotTick.Raw && latest.TryGetObject(_nt.Object.Id, out var header))
			{
				_latestSnapshotTick = latest.Tick;
				int* ptr = (int*)header + _nt.WordOffset;
				Vector3 vector = _nt.ReadPosition(ptr);
				Quaternion quaternion = _nt.ReadRotation(ptr);
				int num6 = ReadPositionChangedTick(ptr);
				int num7 = ReadRotationChangedTick(ptr);
				int num8 = ReadPositionStoppedChangingTick(ptr);
				int num9 = ReadRotationStoppedChangingTick(ptr);
				bool flag = num6 > _posStateBuffer[_posStateHead].Tick.Raw;
				bool flag2 = num7 > _rotStateBuffer[_rotStateHead].Tick.Raw;
				bool flag3 = header->StateAuthority != _latestStateAuth;
				if (flag3 || _posStateBuffer[_posStateTail].Tick.Raw == 0)
				{
					_latestStateAuth = header->StateAuthority;
					bool flag4 = num8 > num6;
					_posStateBuffer[_posStateTail] = new PositionState(vector, num6, flag4);
					_posStateHead = (_posStateTail + 1) % _posStateBuffer.Length;
					_posStateBuffer[_posStateHead] = _posStateBuffer[_posStateTail];
					_accumulatedError.Position = default;
					_currInterpPosAccTime = 0f;
					_currPosStateDt = (flag4 ? (-1f) : 0f);
				}
				else if (flag)
				{
					int posStateHead = _posStateHead;
					PositionState positionState = _posStateBuffer[posStateHead];
					_posStateHead = (posStateHead + 1) % _posStateBuffer.Length;
					if (_posStateHead == _posStateTail)
					{
						int num10 = Math.Min(_posStateBuffer.Length + 3, 30);
						if (num10 > _posStateBuffer.Length)
						{
							PositionState[] array = new PositionState[num10];
							int num11 = (_posStateTail + 3) % num10;
							Array.Copy(_posStateBuffer, 0, array, 0, _posStateHead);
							Array.Copy(_posStateBuffer, _posStateTail, array, num11, _posStateBuffer.Length - _posStateTail);
							_posStateBuffer = array;
							_posStateTail = num11;
							_posStateHead = (posStateHead + 1) % _posStateBuffer.Length;
						}
						else
						{
							_posStateTail = (_posStateTail + 1) % _posStateBuffer.Length;
							_accumulatedError.Position += _currInterpPos - _posStateBuffer[_posStateTail].Position;
							_currInterpPosAccTime = 0f;
							int num12 = (_posStateTail + 1) % _posStateBuffer.Length;
							_currPosStateDt = (float)(_posStateBuffer[num12].Tick.Raw - _posStateBuffer[_posStateTail].Tick.Raw) * num3;
						}
					}
					Tick latestTeleportPosTick = _latestTeleportPosTick;
					_latestTeleportPosTick = ReadTeleportPositionTick(ptr);
					if (_latestTeleportPosTick.Raw > latestTeleportPosTick.Raw && _currPosStateDt > 0f)
					{
						Vector3 vector2 = _nt.ReadTeleportInterpolationVelocity(ptr);
						if (ReadTeleportPositionInterpolateBackwards(ptr))
						{
							Vector3 pos = vector - _currPosStateDt * vector2;
							_posStateBuffer[_posStateHead] = new PositionState(pos, positionState.Tick);
						}
						else
						{
							Vector3 pos2 = positionState.Position + _currPosStateDt * vector2;
							_posStateBuffer[_posStateHead] = new PositionState(pos2, num6);
						}
						_posStateHead = (_posStateHead + 1) % _posStateBuffer.Length;
					}
					_posStateBuffer[_posStateHead] = new PositionState(vector, num6);
					if (_currPosStateDt < 0f)
					{
						_posStateTail = posStateHead;
						_posStateBuffer[_posStateTail] = _posStateBuffer[_posStateHead];
						_currPosStateDt = 0f;
						_currInterpPosAccTime = 0f;
					}
					else
					{
						float num13 = Mathf.Sign(_currInterpPosAccTime - _currPosStateDt);
						num4 = num13 * Mathf.Max(num3 * -0.8f, Mathf.Min(num13 * (_currPosStateDt - _currInterpPosAccTime - _nt.TargetInterpolationDelay), 0f));
						if (_currInterpPosAccTime >= _currPosStateDt)
						{
							_posStateTail = (_posStateTail + 1) % _posStateBuffer.Length;
							_currInterpPosAccTime -= _currPosStateDt;
							_currPosStateDt = (float)(num6 - (int)positionState.Tick) * num3;
							Vector3 vector3 = Vector3.LerpUnclamped(positionState.Position, vector, positionState.StoppedChanging ? 0f : (_currInterpPosAccTime / _currPosStateDt));
							_accumulatedError.Position += _currInterpPos - vector3;
						}
					}
				}
				_posStateBuffer[_posStateHead].StoppedChanging = num8 > num6;
				if (flag3 || _rotStateBuffer[_rotStateTail].Tick.Raw == 0)
				{
					_latestStateAuth = header->StateAuthority;
					bool flag5 = num9 > num7;
					_rotStateBuffer[_rotStateTail] = new RotationState(quaternion, num7, flag5);
					_rotStateHead = (_rotStateTail + 1) % _rotStateBuffer.Length;
					_rotStateBuffer[_rotStateHead] = _rotStateBuffer[_rotStateTail];
					_accumulatedError.Rotation = Quaternion.identity;
					_currInterpRotAccTime = 0f;
					_currRotStateDt = (flag5 ? (-1f) : 0f);
				}
				else if (flag2)
				{
					int rotStateHead = _rotStateHead;
					RotationState rotationState = _rotStateBuffer[rotStateHead];
					_rotStateHead = (rotStateHead + 1) % _rotStateBuffer.Length;
					if (_rotStateHead == _rotStateTail)
					{
						int num14 = Math.Min(_rotStateBuffer.Length + 3, 30);
						if (num14 > _rotStateBuffer.Length)
						{
							RotationState[] array2 = new RotationState[num14];
							int num15 = (_rotStateTail + 3) % num14;
							Array.Copy(_rotStateBuffer, 0, array2, 0, _rotStateHead);
							Array.Copy(_rotStateBuffer, _rotStateTail, array2, num15, _rotStateBuffer.Length - _rotStateTail);
							_rotStateBuffer = array2;
							_rotStateTail = num15;
							_rotStateHead = (rotStateHead + 1) % _rotStateBuffer.Length;
						}
						else
						{
							_accumulatedError.Rotation = _currInterpRot * Quaternion.Inverse(_rotStateBuffer[(_rotStateTail + 1) % _rotStateBuffer.Length].Rotation) * _accumulatedError.Rotation;
							_currInterpRotAccTime = 0f;
							_rotStateTail = (_rotStateTail + 1) % _rotStateBuffer.Length;
							int num16 = (_rotStateTail + 1) % _rotStateBuffer.Length;
							_currRotStateDt = (float)(_rotStateBuffer[num16].Tick.Raw - _rotStateBuffer[_rotStateTail].Tick.Raw) * num3;
						}
					}
					Tick latestTeleportRotTick = _latestTeleportRotTick;
					_latestTeleportRotTick = ReadTeleportRotationTick(ptr);
					if (_latestTeleportRotTick.Raw > latestTeleportRotTick.Raw && _currRotStateDt > 0f)
					{
						Vector3 vector4 = _nt.ReadTeleportInterpolationAngularVelocity(ptr);
						if (ReadTeleportRotationInterpolateBackwards(ptr))
						{
							Quaternion rot = Quaternion.Euler(vector4 * (0f - _currRotStateDt)) * quaternion;
							_rotStateBuffer[_rotStateHead] = new RotationState(rot, rotationState.Tick);
						}
						else
						{
							Quaternion rot2 = Quaternion.Euler(vector4 * _currRotStateDt) * rotationState.Rotation;
							_rotStateBuffer[_rotStateHead] = new RotationState(rot2, num7);
						}
						_rotStateHead = (_rotStateHead + 1) % _rotStateBuffer.Length;
					}
					_rotStateBuffer[_rotStateHead] = new RotationState(quaternion, num7);
					if (_currRotStateDt < 0f)
					{
						_rotStateTail = rotStateHead;
						_rotStateBuffer[_rotStateTail] = _rotStateBuffer[_rotStateHead];
						_currRotStateDt = 0f;
						_currInterpRotAccTime = 0f;
					}
					else
					{
						float num17 = Mathf.Sign(_currInterpRotAccTime - _currRotStateDt);
						num5 = num17 * Mathf.Max(num3 * -0.8f, Mathf.Min(num17 * (_currRotStateDt - _currInterpRotAccTime - _nt.TargetInterpolationDelay), 0f));
						if (_currInterpRotAccTime >= _currRotStateDt)
						{
							_rotStateTail = (_rotStateTail + 1) % _rotStateBuffer.Length;
							_currInterpRotAccTime -= _currRotStateDt;
							_currRotStateDt = (float)(num7 - (int)rotationState.Tick) * num3;
							Quaternion rotation = Quaternion.SlerpUnclamped(rotationState.Rotation, quaternion, rotationState.StoppedChanging ? 0f : (_currInterpRotAccTime / _currRotStateDt));
							_accumulatedError.Rotation = _currInterpRot * Quaternion.Inverse(rotation) * _accumulatedError.Rotation;
						}
					}
				}
				_rotStateBuffer[_rotStateHead].StoppedChanging = num9 > num7;
			}
			int num18 = (_posStateTail + 1) % _posStateBuffer.Length;
			int num19 = (_rotStateTail + 1) % _rotStateBuffer.Length;
			PositionState positionState2 = _posStateBuffer[_posStateTail];
			RotationState rotationState2 = _rotStateBuffer[_rotStateTail];
			PositionState positionState3 = _posStateBuffer[num18];
			RotationState rotationState3 = _rotStateBuffer[num19];
			float num20 = _currInterpPosAccTime + num2 + num4;
			float num21 = _currInterpRotAccTime + num2 + num5;
			while (num20 > _currPosStateDt && _posStateHead != num18)
			{
				num20 -= _currPosStateDt;
				_posStateTail = num18;
				num18 = (num18 + 1) % _posStateBuffer.Length;
				positionState2 = positionState3;
				positionState3 = _posStateBuffer[num18];
				_currPosStateDt = (float)(positionState3.Tick.Raw - positionState2.Tick.Raw) * num3;
			}
			if (_currPosStateDt <= 0f)
			{
				param.InterpolationAlpha = 0f;
			}
			else if (num20 < _currPosStateDt)
			{
				param.InterpolationAlpha = ((positionState2.StoppedChanging || num20 < 0f) ? 0f : (num20 / _currPosStateDt));
			}
			else if (positionState3.StoppedChanging || num20 - _currPosStateDt > 1f)
			{
				param.InterpolationAlpha = 1f;
				if (_currInterpPosAccTime > _currPosStateDt)
				{
					_accumulatedError.Position += _currInterpPos - positionState3.Position;
				}
			}
			else
			{
				param.InterpolationAlpha = num20 / _currPosStateDt;
			}
			_currInterpPosAccTime = num20;
			param.UninterpolatedPositionFrom = positionState2.Position;
			param.UninterpolatedPositionTo = positionState3.Position;
			param.InterpolatedPosition = Vector3.LerpUnclamped(param.UninterpolatedPositionFrom, param.UninterpolatedPositionTo, param.InterpolationAlpha);
			while (num21 > _currRotStateDt && _rotStateHead != num19)
			{
				num21 -= _currRotStateDt;
				_rotStateTail = num19;
				num19 = (num19 + 1) % _rotStateBuffer.Length;
				rotationState2 = rotationState3;
				rotationState3 = _rotStateBuffer[num19];
				_currRotStateDt = (float)(rotationState3.Tick.Raw - rotationState2.Tick.Raw) * num3;
			}
			if (_currRotStateDt <= 0f)
			{
				param.InterpolationAlpha = 0f;
			}
			else if (num21 < _currRotStateDt)
			{
				param.InterpolationAlpha = ((rotationState2.StoppedChanging || num21 < 0f) ? 0f : (num21 / _currRotStateDt));
			}
			else if (rotationState3.StoppedChanging || num21 - _currRotStateDt > 1f)
			{
				param.InterpolationAlpha = 1f;
				if (_currInterpRotAccTime > _currRotStateDt)
				{
					_accumulatedError.Rotation = _currInterpRot * Quaternion.Inverse(rotationState3.Rotation) * _accumulatedError.Rotation;
				}
			}
			else
			{
				param.InterpolationAlpha = num21 / _currRotStateDt;
			}
			_currInterpRotAccTime = num21;
			param.UninterpolatedRotationFrom = rotationState2.Rotation;
			param.UninterpolatedRotationTo = rotationState3.Rotation;
			param.InterpolatedRotation = Quaternion.SlerpUnclamped(param.UninterpolatedRotationFrom, param.UninterpolatedRotationTo, param.InterpolationAlpha);
			_currInterpPos = param.InterpolatedPosition;
			_currInterpRot = param.InterpolatedRotation;
			param.InterpolatedPositionErrorCorrection = default;
			param.InterpolatedRotationErrorCorrection = Quaternion.identity;
			SmoothErrorCorrectionInterp.UpdateInterpolatedErrorCorrection(ref _accumulatedError, ref param, _nt.InterpolatedErrorCorrectionSettings);
			return true;
		}

		public void AfterApplyInterpolatedTransform()
		{
		}
	}

	private class LegacyIntermittentStateInterp : IInterpolationImplementation
	{
		private NetworkTransform _nt;

		private Vector3 _interpErrorPos;

		private Quaternion _interpErrorRot;

		private Vector3 _interpLinearVel;

		private Quaternion _deltaStateRot;

		private Tick _latestTeleportPosTick;

		private Tick _latestTeleportRotTick;

		private PlayerRef _latestStateAuth;

		private Tick _latestStateTick;

		private Vector3 _latestStatePos;

		private Quaternion _latestStateRot;

		private Vector3 _latestInterpPos;

		private Quaternion _latestInterpRot;

		private float _timeBetweenStates;

		public void Reset(NetworkTransform nt)
		{
			_nt = nt;
			_interpErrorPos = default;
			_interpErrorRot = Quaternion.identity;
			_interpLinearVel = default;
			_deltaStateRot = Quaternion.identity;
			_latestStateAuth = default;
			_latestStateTick = default;
			_latestStatePos = default;
			_latestStateRot = Quaternion.identity;
			_latestTeleportPosTick = default;
			_latestTeleportRotTick = default;
			_latestInterpPos = default;
			_latestInterpRot = Quaternion.identity;
			_timeBetweenStates = 0f;
		}

		public unsafe bool TryComputeInterpolatedTransform(out InterpolatedTransformParameters param)
		{
			Vector3 vector = _latestInterpPos;
			Quaternion quaternion = _latestInterpRot;
			SimulationSnapshot latest = _nt.Runner.Simulation.SnapshotHistory.Latest;
			if (latest != null && latest.TryGetObject(_nt.Object.Id, out var header))
			{
				int* ptr = (int*)header + _nt.WordOffset;
				int num = ReadPositionStoppedChangingTick(ptr);
				bool flag = (num & 1) == 1;
				bool flag2 = (num & 2) == 2;
				int num2 = ReadPositionChangedTick(ptr);
				if (num2 > _latestStateTick.Raw || _latestStateAuth != header->StateAuthority)
				{
					Vector3 vector2 = _nt.ReadPosition(ptr);
					Quaternion quaternion2 = _nt.ReadRotation(ptr);
					int num3 = ReadTeleportPositionTick(ptr);
					int num4 = ReadTeleportRotationTick(ptr);
					if (_latestStateTick.Raw == 0 || _latestStateAuth != header->StateAuthority)
					{
						_interpErrorPos = default;
						_interpErrorRot = Quaternion.identity;
						_interpLinearVel = default;
						_deltaStateRot = Quaternion.identity;
						vector = (_latestInterpPos = vector2);
						quaternion = (_latestInterpRot = quaternion2);
					}
					else
					{
						bool flag3 = _latestTeleportPosTick.Raw < num3;
						bool flag4 = _latestTeleportRotTick.Raw < num4;
						float num5 = (float)_nt.Runner.Simulation.Config.DeltaTime;
						_timeBetweenStates = (float)(num2 - (int)_latestStateTick) * num5;
						if (flag3)
						{
							_interpLinearVel = _nt.ReadTeleportInterpolationVelocity(ptr);
							vector += vector2 - _interpLinearVel * _timeBetweenStates - _latestStatePos;
						}
						if (flag4)
						{
							_deltaStateRot = Quaternion.Euler(_nt.ReadTeleportInterpolationAngularVelocity(ptr) * _timeBetweenStates);
							quaternion = Quaternion.Inverse(_deltaStateRot) * quaternion2 * Quaternion.Inverse(_latestStateRot) * quaternion;
						}
						_interpErrorPos = vector2 - vector;
						_interpErrorRot = Quaternion.Inverse(quaternion) * quaternion2;
						if (!flag)
						{
							_interpLinearVel = default;
						}
						else if (!flag3)
						{
							_interpLinearVel = (vector2 - _latestStatePos) / _timeBetweenStates;
						}
						if (!flag2)
						{
							_deltaStateRot = Quaternion.identity;
						}
						else if (!flag4)
						{
							_deltaStateRot = Quaternion.Inverse(_latestStateRot) * quaternion2;
						}
					}
					_latestStateAuth = header->StateAuthority;
					_latestStateTick = num2;
					_latestStatePos = vector2;
					_latestStateRot = quaternion2;
					_latestTeleportPosTick = num3;
					_latestTeleportRotTick = num4;
				}
				else
				{
					if (!flag)
					{
						_interpLinearVel = default;
					}
					if (!flag2)
					{
						_deltaStateRot = Quaternion.identity;
					}
				}
			}
			float deltaTime = Time.deltaTime;
			float num6 = deltaTime * 8f;
			float t = deltaTime * 8f;
			param.UninterpolatedPositionFrom = vector;
			param.UninterpolatedRotationFrom = quaternion;
			param.UninterpolatedPositionTo = _latestStatePos;
			param.UninterpolatedRotationTo = _latestStateRot;
			param.InterpolatedPositionErrorCorrection = default;
			param.InterpolatedRotationErrorCorrection = Quaternion.identity;
			param.InterpolationAlpha = 0f;
			param.InterpolatedPosition = vector + _interpLinearVel * deltaTime;
			param.InterpolatedRotation = Quaternion.SlerpUnclamped(quaternion, _deltaStateRot * quaternion, deltaTime / _timeBetweenStates);
			Vector3 vector3 = _interpErrorPos * num6;
			param.InterpolatedPosition += vector3;
			param.InterpolatedRotation = Quaternion.Slerp(param.InterpolatedRotation, param.InterpolatedRotation * _interpErrorRot, t);
			_interpErrorPos -= vector3;
			_interpErrorRot = Quaternion.Slerp(_interpErrorRot, Quaternion.identity, t);
			return true;
		}

		public void AfterApplyInterpolatedTransform()
		{
			_latestInterpPos = _nt.InterpolationTarget.position;
			_latestInterpRot = _nt.InterpolationTarget.rotation;
		}
	}

	private struct PositionRotationValues
	{
		public Vector3 Position;

		public Quaternion Rotation;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PositionRotationValues Default()
		{
			PositionRotationValues result = default;
			result.Position = default;
			result.Rotation = Quaternion.identity;
			return result;
		}
	}

	private class SmoothErrorCorrectionInterp : IInterpolationImplementation, IAfterTick
	{
		private NetworkTransform _nt;

		private readonly ConsecutiveStateInterp _csInterp = new ConsecutiveStateInterp();

		private float _lastRenderAlpha;

		private Tick _lastRenderFromTick;

		private Tick _lastRenderToTick;

		private Tick _lastRenderTickResimulated;

		private Vector3 _lastRenderInterpolatedPos;

		private Quaternion _lastRenderInterpolatedRot = Quaternion.identity;

		private Vector3 _lastRenderInterpolatedPosResim;

		private Quaternion _lastRenderInterpolatedRotResim = Quaternion.identity;

		private PositionRotationValues _accumulatedError = PositionRotationValues.Default();

		public void Reset(NetworkTransform nt)
		{
			_nt = nt;
			_csInterp.Reset(nt);
			_lastRenderAlpha = 0f;
			_lastRenderFromTick = default;
			_lastRenderToTick = default;
			_lastRenderTickResimulated = default;
			_lastRenderInterpolatedPos = default;
			_lastRenderInterpolatedRot = Quaternion.identity;
			_lastRenderInterpolatedPosResim = default;
			_lastRenderInterpolatedRotResim = Quaternion.identity;
			_accumulatedError = PositionRotationValues.Default();
		}

		public void AfterApplyInterpolatedTransform()
		{
		}

		public unsafe void AfterTick()
		{
			if (_nt.Object.HasStateAuthority || !_nt.InterpolateErrorCorrection || !_nt.Runner.IsResimulation)
			{
				return;
			}
			int raw = _nt.Runner._simulation.Tick.Raw;
			ReadAccuracy positionReadAccuracy = _nt.Runner._positionReadAccuracy;
			ReadAccuracy rotationReadAccuracy = _nt.Runner._rotationReadAccuracy;
			WriteAccuracy positionWriteAccuracy = _nt.Runner._positionWriteAccuracy;
			WriteAccuracy rotationWriteAccuracy = _nt.Runner._rotationWriteAccuracy;
			if (raw == _lastRenderFromTick.Raw)
			{
				_lastRenderInterpolatedPosResim = _nt.GetEnginePosition();
				_lastRenderInterpolatedRotResim = _nt.GetEngineRotation();
				_lastRenderInterpolatedPosResim = WriteReadVector3(_lastRenderInterpolatedPosResim, positionWriteAccuracy, positionReadAccuracy);
				_lastRenderInterpolatedRotResim = WriteReadQuaternion(_lastRenderInterpolatedRotResim, rotationWriteAccuracy, rotationReadAccuracy);
				_lastRenderTickResimulated = _lastRenderFromTick;
			}
			else if (raw == _lastRenderToTick.Raw)
			{
				Vector3 enginePosition = _nt.GetEnginePosition();
				Quaternion engineRotation = _nt.GetEngineRotation();
				enginePosition = WriteReadVector3(enginePosition, positionWriteAccuracy, positionReadAccuracy);
				engineRotation = WriteReadQuaternion(engineRotation, rotationWriteAccuracy, rotationReadAccuracy);
				Vector3 fromPos;
				Quaternion fromRot;
				if (_lastRenderTickResimulated.Raw == _lastRenderFromTick.Raw)
				{
					fromPos = _lastRenderInterpolatedPosResim;
					fromRot = _lastRenderInterpolatedRotResim;
				}
				else
				{
					if (!_nt.Runner._simulation.SnapshotHistory.TryGet(_lastRenderFromTick, out var snapshot) || !snapshot.TryGetObject(_nt.Object.Id, out var header))
					{
						_lastRenderTickResimulated = _lastRenderToTick.Raw;
						return;
					}
					int* ptr = (int*)header + _nt.WordOffset;
					fromPos = _nt.ReadPosition(ptr);
					fromRot = _nt.ReadRotation(ptr);
				}
				_nt.InterpolatePositionRotation(fromPos, fromRot, enginePosition, engineRotation, _lastRenderAlpha, out _lastRenderInterpolatedPosResim, out _lastRenderInterpolatedRotResim);
				_accumulatedError.Position += _lastRenderInterpolatedPos - _lastRenderInterpolatedPosResim;
				_accumulatedError.Rotation = _lastRenderInterpolatedRot * Quaternion.Inverse(_lastRenderInterpolatedRotResim) * _accumulatedError.Rotation;
				_lastRenderTickResimulated = _lastRenderToTick.Raw;
			}
			else if (raw == _nt._predictedSpawnLatestToTick.Raw)
			{
				_accumulatedError.Position += _nt._predictedSpawnPosTo - _nt.GetEnginePosition();
				_accumulatedError.Rotation = _nt._predictedSpawnRotTo * Quaternion.Inverse(_nt.GetEngineRotation()) * _accumulatedError.Rotation;
				_nt._predictedSpawnLatestToTick = default;
			}
		}

		public bool TryComputeInterpolatedTransform(out InterpolatedTransformParameters param)
		{
			Assert.Check(_nt.IsInterpolationDataPredicted());
			if (_nt.GetInterpolationData(out var data, true))
			{
				_csInterp.ComputeInterpolatedTransform(ref data, out param);
				ComputePredictionErrorCorrection(ref data, ref param);
				return true;
			}
			param = default;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void ComputePredictionErrorCorrection(ref InterpolationData data, ref InterpolatedTransformParameters param)
		{
			if (_nt.Object.HasStateAuthority || !_nt.InterpolateErrorCorrection)
			{
				_accumulatedError = PositionRotationValues.Default();
				return;
			}
			ReadAccuracy positionReadAccuracy = _nt.Runner._positionReadAccuracy;
			ReadAccuracy rotationReadAccuracy = _nt.Runner._rotationReadAccuracy;
			WriteAccuracy positionWriteAccuracy = _nt.Runner._positionWriteAccuracy;
			WriteAccuracy rotationWriteAccuracy = _nt.Runner._rotationWriteAccuracy;
			SimulationSnapshot snapshot3;
			NetworkObjectHeader* header3;
			if (_lastRenderTickResimulated.Raw != _lastRenderToTick.Raw)
			{
				bool flag = _nt.Runner._simulation.SnapshotHistory.TryGet(_lastRenderFromTick, out var snapshot);
				bool flag2 = _nt.Runner._simulation.SnapshotHistory.TryGet(_lastRenderToTick, out var snapshot2);
				NetworkObjectHeader* header = default;
				if (((flag && snapshot.TryGetObject(_nt.Object.Id, out header)) & flag2) && snapshot2.TryGetObject(_nt.Object.Id, out var header2))
				{
					int* ptr = (int*)header + _nt.WordOffset;
					int* ptr2 = (int*)header2 + _nt.WordOffset;
					ConsecutiveStateInterp.InterpolatePositionRotation(_nt, _nt.ReadPosition(ptr), _nt.ReadRotation(ptr), _nt.ReadPosition(ptr2), _nt.ReadRotation(ptr2), _lastRenderAlpha, out _lastRenderInterpolatedPosResim, out _lastRenderInterpolatedRotResim);
					_accumulatedError.Position += _lastRenderInterpolatedPos - _lastRenderInterpolatedPosResim;
					_accumulatedError.Rotation = _lastRenderInterpolatedRot * Quaternion.Inverse(_lastRenderInterpolatedRotResim) * _accumulatedError.Rotation;
				}
			}
			else if (_nt._predictedSpawnLatestToTick.Raw != 0 && _nt.Runner._simulation.SnapshotHistory.TryGet(_nt._predictedSpawnLatestToTick, out snapshot3) && snapshot3.TryGetObject(_nt.Object.Id, out header3))
			{
				int* ptr3 = (int*)header3 + _nt.WordOffset;
				_nt._predictedSpawnPosTo = WriteReadVector3(_nt._predictedSpawnPosTo, positionWriteAccuracy, positionReadAccuracy);
				_nt._predictedSpawnRotTo = WriteReadQuaternion(_nt._predictedSpawnRotTo, rotationWriteAccuracy, rotationReadAccuracy);
				_accumulatedError.Position += _nt._predictedSpawnPosTo - _nt.ReadPosition(ptr3);
				_accumulatedError.Rotation = _nt._predictedSpawnRotTo * Quaternion.Inverse(_nt.ReadRotation(ptr3)) * _accumulatedError.Rotation;
			}
			UpdateInterpolatedErrorCorrection(ref _accumulatedError, ref param, _nt.InterpolatedErrorCorrectionSettings);
			_lastRenderFromTick = data.FromTick;
			_lastRenderToTick = data.ToTick;
			_lastRenderAlpha = data.Alpha;
			_lastRenderInterpolatedPos = WriteReadVector3(param.InterpolatedPosition, positionWriteAccuracy, positionReadAccuracy);
			_lastRenderInterpolatedRot = WriteReadQuaternion(param.InterpolatedRotation, rotationWriteAccuracy, rotationReadAccuracy);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void UpdateInterpolatedErrorCorrection(ref PositionRotationValues accumulatedError, ref InterpolatedTransformParameters param, InterpolatedErrorCorrectionSettings settings)
		{
			float num = settings.MinRate;
			float num2 = settings.MinRate;
			float magnitude = accumulatedError.Position.magnitude;
			if (magnitude > settings.PosTeleportDistance)
			{
				accumulatedError.Position = default;
			}
			else
			{
				float num3 = settings.PosBlendEnd - settings.PosBlendStart;
				float t = Mathf.Clamp01((magnitude - settings.PosBlendStart) / num3);
				num = Mathf.Lerp(settings.MinRate, settings.MaxRate, t);
			}
			float value = Quaternion.Dot(accumulatedError.Rotation, Quaternion.identity);
			value = Mathf.Clamp(value, -1f, 1f);
			float num4 = Mathf.Acos(value) * 2f;
			if (num4 > settings.RotTeleportRadians)
			{
				accumulatedError.Rotation = Quaternion.identity;
			}
			else
			{
				float num5 = settings.RotBlendEnd - settings.RotBlendStart;
				float t2 = Mathf.Clamp01((num4 - settings.RotBlendStart) / num5);
				num2 = Mathf.Lerp(settings.MinRate, settings.MaxRate, t2);
			}
			param.InterpolatedPositionErrorCorrection = accumulatedError.Position;
			param.InterpolatedRotationErrorCorrection = accumulatedError.Rotation;
			float num6 = 1f - Time.deltaTime * num;
			if ((accumulatedError.Position * num6).magnitude < settings.PosMinCorrection)
			{
				UpdateMinPositionCorrection(ref accumulatedError, settings);
			}
			else
			{
				accumulatedError.Position *= num6;
			}
			accumulatedError.Rotation = Quaternion.Slerp(accumulatedError.Rotation, Quaternion.identity, Time.deltaTime * num2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void UpdateMinPositionCorrection(ref PositionRotationValues accumulatedError, InterpolatedErrorCorrectionSettings settings)
		{
			if (accumulatedError.Position.x != 0f || accumulatedError.Position.y != 0f || accumulatedError.Position.z != 0f)
			{
				Vector3 normalized = accumulatedError.Position.normalized;
				bool flag = accumulatedError.Position.x >= 0f;
				bool flag2 = accumulatedError.Position.y >= 0f;
				bool flag3 = accumulatedError.Position.z >= 0f;
				accumulatedError.Position -= normalized * settings.PosMinCorrection;
				if (flag != accumulatedError.Position.x >= 0f)
				{
					accumulatedError.Position.x = 0f;
				}
				if (flag2 != accumulatedError.Position.y >= 0f)
				{
					accumulatedError.Position.y = 0f;
				}
				if (flag3 != accumulatedError.Position.z >= 0f)
				{
					accumulatedError.Position.z = 0f;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
	}

	private ConsecutiveStateInterp _consecutiveInterp;

	private IInterpolationImplementation _currInterpImpl;

	private bool _wasRenderedThisUpdate;

	[InlineHelp]
	public Spaces InterpolationSpace = Spaces.World;

	[InlineHelp]
	[FormerlySerializedAs("_interpolationTarget")]
	[WarnIf("InvalidInterpolationTarget", "If view interpolation is performed, an 'Interpolation Target' other than this object should be used. It is typically a child of this GameObject or a separate object without colliders and it must be a non-physics GameObject.")]
	public Transform InterpolationTarget;

	[InlineHelp]
	[ToggleLeft]
	[MultiPropertyDrawersFix]
	public bool InterpolateErrorCorrection = true;

	[InlineHelp]
	[DrawIf("InterpolateErrorCorrection", Hide = true)]
	[MultiPropertyDrawersFix]
	public InterpolatedErrorCorrectionSettings InterpolatedErrorCorrectionSettings;

	[InlineHelp]
	[ToggleLeft]
	[MultiPropertyDrawersFix]
	[Header("Shared Mode Interpolation")]
	public bool UseLegacySharedModeInterpolation = false;

	[InlineHelp]
	[DrawIf("UseLegacySharedModeInterpolation", 0.0, Hide = false)]
	[Range(0f, 0.2f)]
	[MultiPropertyDrawersFix]
	public float TargetInterpolationDelay = 0.03f;

	private IntermittentStateInterp _intermittentInterp;

	private LegacyIntermittentStateInterp _legacyIntermittentInterp;

	private SmoothErrorCorrectionInterp _smoothErrorCorrectedInterp;

	private EncodedPosition _prevEncodedPos;

	private EncodedRotation _prevEncodedRot;

	private Tick _predictedSpawnLatestToTick;

	private Vector3 _predictedSpawnPosFrom;

	private Vector3 _predictedSpawnPosTo;

	private Quaternion _predictedSpawnRotFrom = Quaternion.identity;

	private Quaternion _predictedSpawnRotTo = Quaternion.identity;

	private const int WORD_COUNT_TELEPORT_POS_TICK = 1;

	private const int WORD_COUNT_TELEPORT_ROT_TICK = 1;

	private const int WORD_COUNT_POS_CHANGED_TICK = 1;

	private const int WORD_COUNT_ROT_CHANGED_TICK = 1;

	private const int WORD_COUNT_POS_STOPPED_CHANGING_TICK = 1;

	private const int WORD_COUNT_ROT_STOPPED_CHANGING_TICK = 1;

	private const int INTERPOLATE_BACKWARDS_FLAG = int.MinValue;

	private const int WORD_COUNT_TELEPORT_INTERPOL_VEL = 3;

	private const int WORD_COUNT_TELEPORT_INTERPOL_ANG_VEL = 4;

	private const int OFFSET_BASE = 7;

	private const int OFFSET_TELEPORT_INTERPOL_VEL = 7;

	private const int OFFSET_TELEPORT_INTERPOL_ANG_VEL = 10;

	private const int OFFSET_TELEPORT_POS_TICK = 14;

	private const int OFFSET_TELEPORT_ROT_TICK = 15;

	private const int OFFSET_POS_CHANGED_TICK = 16;

	private const int OFFSET_ROT_CHANGED_TICK = 17;

	private const int OFFSET_POS_STOPPED_CHANGING_TICK = 18;

	private const int OFFSET_ROT_STOPPED_CHANGING_TICK = 19;

	protected const int WORD_COUNT_NT = 20;

	private bool InvalidInterpolationTarget
	{
		get
		{
			if (this == null)
			{
				return false;
			}
			return base.InterpolationDataSource != InterpolationDataSources.NoInterpolation && (InterpolationTarget == null || InterpolationTarget == base.transform);
		}
	}

	protected virtual Vector3 DefaultTeleportInterpolationVelocity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return default;
		}
	}

	protected virtual Vector3 DefaultTeleportInterpolationAngularVelocity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return default;
		}
	}

	protected override int BaseWordCount => 20;

	public override void Spawned()
	{
		base.Spawned();
		if (InterpolationTarget == null)
		{
			if (!Object.HasStateAuthority)
			{
				InterpolationTarget = base.Transform;
			}
			else if (Runner.IsClient && base.InterpolationDataSource != InterpolationDataSources.NoInterpolation)
			{
				Log.DebugWarn(Runner, "An interpolated Network Transform object was spawned without Interpolation Target on a client that has State Authority: the object will not be interpolated.");
			}
		}
		if (InterpolationTarget != null)
		{
			InterpolationTarget.SetPositionAndRotation(GetEnginePosition(), GetEngineRotation());
		}
	}

	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		base.Despawned(runner, hasState);
		_currInterpImpl = null;
	}

	public override void BeforeUpdate()
	{
		base.BeforeUpdate();
		_wasRenderedThisUpdate = false;
	}

	public override void Render()
	{
		if (base.InterpolationDataSource == InterpolationDataSources.NoInterpolation || _wasRenderedThisUpdate || InterpolationTarget == null)
		{
			_wasRenderedThisUpdate = true;
			return;
		}
		_wasRenderedThisUpdate = true;
		IInterpolationImplementation interpolationImplementation = GetInterpolationImplementation();
		if (interpolationImplementation.TryComputeInterpolatedTransform(out var param))
		{
			ApplyInterpolatedTransform(ref param);
			interpolationImplementation.AfterApplyInterpolatedTransform();
		}
	}

	public void AfterTick()
	{
		if (_currInterpImpl is IAfterTick afterTick)
		{
			afterTick.AfterTick();
		}
	}

	private IInterpolationImplementation GetInterpolationImplementation()
	{
		if (Runner.Topology == SimulationConfig.Topologies.Shared && !Object.HasStateAuthority)
		{
			if (UseLegacySharedModeInterpolation)
			{
				if (_currInterpImpl is LegacyIntermittentStateInterp)
				{
					return _currInterpImpl;
				}
				if (_legacyIntermittentInterp == null)
				{
					_legacyIntermittentInterp = new LegacyIntermittentStateInterp();
				}
				_currInterpImpl = _legacyIntermittentInterp;
			}
			else
			{
				if (_currInterpImpl is IntermittentStateInterp)
				{
					return _currInterpImpl;
				}
				if (_intermittentInterp == null)
				{
					_intermittentInterp = new IntermittentStateInterp();
				}
				_currInterpImpl = _intermittentInterp;
			}
			_currInterpImpl.Reset(this);
		}
		else if (Object.HasStateAuthority || !InterpolateErrorCorrection || !IsInterpolationDataPredicted())
		{
			if (_currInterpImpl is ConsecutiveStateInterp)
			{
				return _currInterpImpl;
			}
			if (_consecutiveInterp == null)
			{
				_consecutiveInterp = new ConsecutiveStateInterp();
			}
			_currInterpImpl = _consecutiveInterp;
			_currInterpImpl.Reset(this);
		}
		else
		{
			if (_currInterpImpl is SmoothErrorCorrectionInterp)
			{
				return _currInterpImpl;
			}
			if (_smoothErrorCorrectedInterp == null)
			{
				_smoothErrorCorrectedInterp = new SmoothErrorCorrectionInterp();
			}
			_currInterpImpl = _smoothErrorCorrectedInterp;
			_currInterpImpl.Reset(this);
		}
		return _currInterpImpl;
	}

	protected virtual void ApplyInterpolatedTransform(ref InterpolatedTransformParameters param)
	{
		Vector3 position = param.InterpolatedPosition + param.InterpolatedPositionErrorCorrection;
		Quaternion rotation = param.InterpolatedRotationErrorCorrection * param.InterpolatedRotation;
		InterpolationTarget.SetPositionAndRotation(position, rotation);
	}

	protected unsafe virtual void GetUninterpolatedWorldPositions(ref InterpolationData data, out Vector3 posFrom, out Vector3 posTo)
	{
		if (data.ToTick != ReadTeleportPositionTick(data.To))
		{
			posFrom = ReadPosition(data.From);
			posTo = ReadPosition(data.To);
			return;
		}
		Vector3 vector = ReadTeleportInterpolationVelocity(data.To) * Runner.DeltaTime;
		if (ReadTeleportPositionInterpolateBackwards(data.To))
		{
			posTo = ReadPosition(data.To);
			posFrom = posTo - vector;
		}
		else
		{
			posFrom = ReadPosition(data.From);
			posTo = posFrom + vector;
		}
	}

	protected unsafe virtual void GetUninterpolatedWorldRotations(ref InterpolationData data, out Quaternion rotFrom, out Quaternion rotTo)
	{
		if (data.ToTick != ReadTeleportRotationTick(data.To))
		{
			rotFrom = ReadRotation(data.From);
			rotTo = ReadRotation(data.To);
			return;
		}
		Vector3 vector = ReadTeleportInterpolationAngularVelocity(data.To);
		if (ReadTeleportRotationInterpolateBackwards(data.To))
		{
			rotTo = ReadRotation(data.To);
			rotFrom = Quaternion.Euler(vector * (0f - Runner.DeltaTime)) * rotTo;
		}
		else
		{
			rotFrom = ReadRotation(data.From);
			rotTo = Quaternion.Euler(vector * Runner.DeltaTime) * rotFrom;
		}
	}

	private unsafe void InterpolatePositionRotation(Vector3 fromPos, Quaternion fromRot, Vector3 toPos, Quaternion toRot, float alpha, out Vector3 pos, out Quaternion rot)
	{
		if (InterpolationSpace == Spaces.World || base.Transform.parent == null)
		{
			pos = Vector3.Lerp(fromPos, toPos, alpha);
			rot = Quaternion.Slerp(fromRot, toRot, alpha);
			return;
		}
		Assert.Check(InterpolationSpace == Spaces.Local);
		NetworkPositionRotation componentInParent = base.Transform.parent.GetComponentInParent<NetworkPositionRotation>();
		if (BehaviourUtils.IsNotAlive(componentInParent) || !componentInParent.GetInterpolationData(out var data))
		{
			pos = Vector3.Lerp(fromPos, toPos, alpha);
			rot = Quaternion.Slerp(fromRot, toRot, alpha);
			return;
		}
		Vector3 vector = ReadPosition(data.From);
		Quaternion quaternion = ReadRotation(data.From);
		Vector3 vector2 = ReadPosition(data.To);
		Quaternion quaternion2 = ReadRotation(data.To);
		Quaternion quaternion3 = Quaternion.Inverse(quaternion);
		Quaternion quaternion4 = Quaternion.Inverse(quaternion2);
		Vector3 a = quaternion3 * (fromPos - vector);
		Quaternion a2 = fromRot * quaternion3;
		Vector3 b = quaternion4 * (toPos - vector2);
		Quaternion b2 = toRot * quaternion4;
		pos = Vector3.Lerp(a, b, alpha);
		rot = Quaternion.Slerp(a2, b2, alpha);
		Vector3 vector3 = Vector3.Lerp(vector, vector2, alpha);
		Quaternion quaternion5 = Quaternion.Slerp(quaternion, quaternion2, alpha);
		pos = vector3 + quaternion5 * pos;
		rot *= quaternion5;
	}

	public override void AfterAllTicks(bool resimulation, int tickCount)
	{
		base.AfterAllTicks(resimulation, tickCount);
		if (Runner.Topology == SimulationConfig.Topologies.Shared)
		{
			ComputeStateChanged();
		}
	}

	public override void CopyBackingFieldsToState(bool firstTime)
	{
		base.CopyBackingFieldsToState(firstTime);
		if (Runner.Topology == SimulationConfig.Topologies.Shared)
		{
			ComputeStateChanged();
		}
	}

	private unsafe void ComputeStateChanged()
	{
		Assert.Check(sizeof(EncodedPosition) == 12);
		Assert.Check(sizeof(EncodedRotation) == 16);
		int num = ReadPositionChangedTick();
		int num2 = ReadRotationChangedTick();
		int num3 = ReadPositionStoppedChangingTick();
		int num4 = ReadRotationStoppedChangingTick();
		bool flag = (Runner.Tick.Raw > num) & (Runner.Tick.Raw > num3);
		bool flag2 = (Runner.Tick.Raw > num2) & (Runner.Tick.Raw > num4);
		bool flag3 = false;
		bool flag4 = false;
		if (flag)
		{
			EncodedPosition prevEncodedPos = *ReadEncodedPosition();
			flag3 = ((prevEncodedPos.Data[0] ^ ~_prevEncodedPos.Data[0]) & (prevEncodedPos.Data[1] ^ ~_prevEncodedPos.Data[1]) & (prevEncodedPos.Data[2] ^ ~_prevEncodedPos.Data[2])) != uint.MaxValue;
			_prevEncodedPos = prevEncodedPos;
			bool flag5 = num > num3;
			if (flag3)
			{
				WritePositionChangedTick(Runner.Tick);
			}
			else if (flag5)
			{
				WritePositionStoppedChangingTick(Runner.Tick);
			}
		}
		if (flag2)
		{
			EncodedRotation prevEncodedRot = *ReadEncodedRotation();
			flag4 = ((prevEncodedRot.Data[0] ^ ~_prevEncodedRot.Data[0]) & (prevEncodedRot.Data[1] ^ ~_prevEncodedRot.Data[1])) != ulong.MaxValue;
			_prevEncodedRot = prevEncodedRot;
			bool flag6 = num2 > num4;
			if (flag4)
			{
				WriteRotationChangedTick(Runner.Tick);
			}
			else if (flag6)
			{
				WriteRotationStoppedChangingTick(Runner.Tick);
			}
		}
		if (UseLegacySharedModeInterpolation)
		{
			if (flag3 | flag4)
			{
				WritePositionChangedTick(Runner.Tick);
				WriteRotationChangedTick(Runner.Tick);
			}
			int tick = (flag3 ? 1 : 0) | (flag4 ? 2 : 0);
			WritePositionStoppedChangingTick(tick);
		}
	}

	public void TeleportToPosition(Vector3 position, Vector3? interpolationVel = null, bool interpolateBackwards = true)
	{
		SetEnginePosition(position);
		WriteTeleportPositionTick(Runner._simulation.Tick);
		WriteTeleportPositionInterpolateBackwards(interpolateBackwards);
		WriteTeleportInterpolationVelocity(interpolationVel ?? DefaultTeleportInterpolationVelocity);
	}

	public void TeleportToRotation(Quaternion rotation, Vector3? interpolationAngularVel = null, bool interpolateBackwards = true)
	{
		SetEngineRotation(rotation);
		WriteTeleportRotationTick(Runner._simulation.Tick);
		WriteTeleportRotationInterpolateBackwards(interpolateBackwards);
		WriteTeleportInterpolationAngularVelocity(interpolationAngularVel ?? DefaultTeleportInterpolationAngularVelocity);
	}

	public void TeleportToPositionRotation(Vector3 position, Quaternion rotation, Vector3? interpolationVel = null, Vector3? interpolationAngularVel = null, bool interpolateBackwards = true)
	{
		SetEnginePosition(position);
		SetEngineRotation(rotation);
		WriteTeleportPositionTick(Runner._simulation.Tick);
		WriteTeleportRotationTick(Runner._simulation.Tick);
		WriteTeleportPositionInterpolateBackwards(interpolateBackwards);
		WriteTeleportRotationInterpolateBackwards(interpolateBackwards);
		WriteTeleportInterpolationVelocity(interpolationVel ?? DefaultTeleportInterpolationVelocity);
		WriteTeleportInterpolationAngularVelocity(interpolationAngularVel ?? DefaultTeleportInterpolationAngularVelocity);
	}

	public virtual void PredictedSpawnSpawned()
	{
		Spawned();
		PredictedSpawnCacheTransformState();
	}

	public virtual void PredictedSpawnUpdate()
	{
		PredictedSpawnCacheTransformState();
	}

	public virtual void PredictedSpawnRender()
	{
		if (base.InterpolationDataSource != InterpolationDataSources.NoInterpolation && !(InterpolationTarget == null))
		{
			InterpolatedTransformParameters param = PredictedSpawnGetInterpolationParameters();
			ApplyInterpolatedTransform(ref param);
		}
	}

	public virtual void PredictedSpawnFailed()
	{
	}

	public virtual void PredictedSpawnSuccess()
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void PredictedSpawnCacheTransformState()
	{
		if (Object.IsPredictedSpawn)
		{
			if (_predictedSpawnLatestToTick.Raw != Runner.Simulation.Tick.Raw)
			{
				_predictedSpawnPosFrom = _predictedSpawnPosTo;
				_predictedSpawnRotFrom = _predictedSpawnRotTo;
			}
			_predictedSpawnPosTo = GetEnginePosition();
			_predictedSpawnRotTo = GetEngineRotation();
			if (_predictedSpawnLatestToTick.Raw == 0)
			{
				_predictedSpawnPosFrom = _predictedSpawnPosTo;
				_predictedSpawnRotFrom = _predictedSpawnRotTo;
			}
			_predictedSpawnLatestToTick = Runner.Simulation.Tick;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private InterpolatedTransformParameters PredictedSpawnGetInterpolationParameters()
	{
		InterpolatedTransformParameters result = default;
		result.InterpolatedPositionErrorCorrection = default;
		result.InterpolatedRotationErrorCorrection = Quaternion.identity;
		result.UninterpolatedPositionFrom = _predictedSpawnPosFrom;
		result.UninterpolatedRotationFrom = _predictedSpawnRotFrom;
		result.UninterpolatedPositionTo = _predictedSpawnPosTo;
		result.UninterpolatedRotationTo = _predictedSpawnRotTo;
		result.InterpolationAlpha = Runner.Simulation.StateAlpha;
		InterpolatePositionRotation(result.UninterpolatedPositionFrom, result.UninterpolatedRotationFrom, result.UninterpolatedPositionTo, result.UninterpolatedRotationTo, result.InterpolationAlpha, out result.InterpolatedPosition, out result.InterpolatedRotation);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Vector3 ReadTeleportInterpolationVelocity()
	{
		return ReadWriteUtils.ReadVector3(Ptr + 7, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Vector3 ReadTeleportInterpolationVelocity(int* ptr)
	{
		return ReadWriteUtils.ReadVector3(ptr + 7, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Vector3 ReadTeleportInterpolationVelocity(int* ptr, ReadAccuracy readAccuracy)
	{
		return ReadWriteUtils.ReadVector3(ptr + 7, readAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Vector3 ReadTeleportInterpolationAngularVelocity()
	{
		return ReadWriteUtils.ReadVector3(Ptr + 10, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe Vector3 ReadTeleportInterpolationAngularVelocity(int* ptr)
	{
		return ReadWriteUtils.ReadVector3(ptr + 10, Runner._positionReadAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Vector3 ReadTeleportInterpolationAngularVelocity(int* ptr, ReadAccuracy readAccuracy)
	{
		return ReadWriteUtils.ReadVector3(ptr + 10, readAccuracy);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe int ReadTeleportPositionTick()
	{
		return Ptr[14] & 0x7FFFFFFF;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static int ReadTeleportPositionTick(int* ptr)
	{
		return ptr[14] & 0x7FFFFFFF;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe int ReadTeleportRotationTick()
	{
		return Ptr[15] & 0x7FFFFFFF;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static int ReadTeleportRotationTick(int* ptr)
	{
		return ptr[15] & 0x7FFFFFFF;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe bool ReadTeleportPositionInterpolateBackwards()
	{
		return (Ptr[14] & int.MinValue) == int.MinValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static bool ReadTeleportPositionInterpolateBackwards(int* ptr)
	{
		return (ptr[14] & int.MinValue) == int.MinValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe bool ReadTeleportRotationInterpolateBackwards()
	{
		return (Ptr[15] & int.MinValue) == int.MinValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static bool ReadTeleportRotationInterpolateBackwards(int* ptr)
	{
		return (ptr[15] & int.MinValue) == int.MinValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe int ReadPositionChangedTick()
	{
		return Ptr[16];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static int ReadPositionChangedTick(int* ptr)
	{
		return ptr[16];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe int ReadRotationChangedTick()
	{
		return Ptr[17];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static int ReadRotationChangedTick(int* ptr)
	{
		return ptr[17];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe int ReadPositionStoppedChangingTick()
	{
		return Ptr[18];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static int ReadPositionStoppedChangingTick(int* ptr)
	{
		return ptr[18];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe int ReadRotationStoppedChangingTick()
	{
		return Ptr[19];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static int ReadRotationStoppedChangingTick(int* ptr)
	{
		return ptr[19];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteTeleportInterpolationVelocity(Vector3 velocity)
	{
		ReadWriteUtils.WriteVector3(Ptr + 7, Runner._positionWriteAccuracy, velocity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteTeleportInterpolationVelocity(Vector3 velocity, int* ptr)
	{
		ReadWriteUtils.WriteVector3(ptr + 7, Runner._positionWriteAccuracy, velocity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteTeleportInterpolationVelocity(Vector3 velocity, int* ptr, WriteAccuracy writeAccuracy)
	{
		ReadWriteUtils.WriteVector3(ptr + 7, writeAccuracy, velocity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteTeleportInterpolationAngularVelocity(Vector3 angularVel)
	{
		ReadWriteUtils.WriteVector3(Ptr + 10, Runner._positionWriteAccuracy, angularVel);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteTeleportInterpolationAngularVelocity(Vector3 angularVel, int* ptr)
	{
		ReadWriteUtils.WriteVector3(ptr + 10, Runner._positionWriteAccuracy, angularVel);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteTeleportInterpolationAngularVelocity(Vector3 angularVel, int* ptr, WriteAccuracy writeAccuracy)
	{
		ReadWriteUtils.WriteVector3(ptr + 10, writeAccuracy, angularVel);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteTeleportPositionTick(int tick)
	{
		Ptr[14] = (Ptr[14] & int.MinValue) | tick;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteTeleportPositionTick(int tick, int* ptr)
	{
		ptr[14] = (ptr[14] & int.MinValue) | tick;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteTeleportRotationTick(int tick)
	{
		Ptr[15] = (Ptr[15] & int.MinValue) | tick;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteTeleportRotationTick(int tick, int* ptr)
	{
		ptr[15] = (ptr[15] & int.MinValue) | tick;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteTeleportPositionInterpolateBackwards(bool backwards)
	{
		if (backwards)
		{
			Ptr[14] |= int.MinValue;
		}
		else
		{
			Ptr[14] &= int.MaxValue;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteTeleportPositionInterpolateBackwards(int* ptr, bool backwards)
	{
		if (backwards)
		{
			ptr[14] |= int.MinValue;
		}
		else
		{
			ptr[14] &= int.MaxValue;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void WriteTeleportRotationInterpolateBackwards(bool backwards)
	{
		if (backwards)
		{
			Ptr[15] |= int.MinValue;
		}
		else
		{
			Ptr[15] &= int.MaxValue;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void WriteTeleportRotationInterpolateBackwards(int* ptr, bool backwards)
	{
		if (backwards)
		{
			ptr[15] |= int.MinValue;
		}
		else
		{
			ptr[15] &= int.MaxValue;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe void WritePositionChangedTick(int tick)
	{
		Ptr[16] = tick;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static void WritePositionChangedTick(int tick, int* ptr)
	{
		ptr[16] = tick;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe void WriteRotationChangedTick(int tick)
	{
		Ptr[17] = tick;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static void WriteRotationChangedTick(int tick, int* ptr)
	{
		ptr[17] = tick;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe void WritePositionStoppedChangingTick(int tick)
	{
		Ptr[18] = tick;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static void WritePositionStoppedChangingTick(int tick, int* ptr)
	{
		ptr[18] = tick;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe void WriteRotationStoppedChangingTick(int tick)
	{
		Ptr[19] = tick;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static void WriteRotationStoppedChangingTick(int tick, int* ptr)
	{
		ptr[19] = tick;
	}
}
