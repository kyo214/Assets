#define DEBUG
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion;

[OrderAfter(new Type[] { typeof(NetworkBehaviour) })]
[HelpURL("https://doc.photonengine.com/fusion/current/manual/prebuilt-components#networkmechanimanimator")]
[OrderBefore(new Type[]
{
	typeof(NetworkTransform),
	typeof(NetworkRigidbody)
})]
[NetworkBehaviourWeaved(-1)]
[DisallowMultipleComponent]
[AddComponentMenu("Fusion/Network Mecanim Animator")]
public sealed class NetworkMecanimAnimator : NetworkBehaviour, IAfterAllTicks
{
	[InlineHelp]
	public Animator Animator;

	[InlineHelp]
	[MultiPropertyDrawersFix]
	[EditorDisabled(false)]
	[SerializeField]
	internal int TotalWords = -1;

	[SerializeField]
	[InlineHelp]
	[VersaMask(true, null)]
	[MultiPropertyDrawersFix]
	internal AnimatorSyncSettings SyncSettings = AnimatorSyncSettings.ParameterInts | AnimatorSyncSettings.ParameterFloats | AnimatorSyncSettings.ParameterBools | AnimatorSyncSettings.ParameterTriggers | AnimatorSyncSettings.LayerWeights;

	[InlineHelp]
	public Accuracy FloatAccuracy = new Accuracy("Default");

	[InlineHelp]
	public Accuracy TimeAccuracy = new Accuracy("NormalizedTime");

	[InlineHelp]
	public Accuracy WeightAccuracy = new Accuracy("Default");

	[InlineHelp]
	[SerializeField]
	internal int[] StateHashes;

	internal Dictionary<int, int> StateIndexLookup = new Dictionary<int, int>();

	[InlineHelp]
	[SerializeField]
	internal int[] TriggerHashes;

	internal Dictionary<int, int> TriggerIndexLookup = new Dictionary<int, int>();

	private int _param32Count;

	private int _paramBoolCount;

	private int _paramBoolsWordCount;

	private int _paramBoolsPtrOffset;

	private int _syncedLayerCount;

	private int[] _prevBoolsBitmask;

	private const int BITS_PER_BOOL = 4;

	private AnimatorControllerParameter[] _cachedParameters;

	private int[] _cachedParameterHashes;

	private int _cachedParameterCount;

	private int _cachedLayerCount;

	private WriteAccuracy _floatWriteAccuracy;

	private WriteAccuracy _timeWriteAccuracy;

	private WriteAccuracy _weightWriteAccuracy;

	private ReadAccuracy _floatReadAccuracy;

	private ReadAccuracy _timeReadAccuracy;

	private ReadAccuracy _weightReadAccuracy;

	private HashSet<int> pendingTriggers = new HashSet<int>();

	public override int? DynamicWordCount
	{
		get
		{
			if (Application.isPlaying)
			{
				TotalWords = Math.Max(TotalWords, GetRuntimeCounts());
				return TotalWords;
			}
			throw new InvalidOperationException("DynamicWordCount should not be called outside of playing.");
		}
	}

	public override void Spawned()
	{
		if (Animator == null)
		{
			Animator = GetComponent<Animator>();
		}
		NetworkProjectConfig config = Runner.Config;
		_floatWriteAccuracy = FloatAccuracy.GetWriteAccuracy(config);
		_timeWriteAccuracy = TimeAccuracy.GetWriteAccuracy(config);
		_weightWriteAccuracy = WeightAccuracy.GetWriteAccuracy(config);
		_floatReadAccuracy = FloatAccuracy.GetReadAccuracy(config);
		_timeReadAccuracy = TimeAccuracy.GetReadAccuracy(config);
		_weightReadAccuracy = WeightAccuracy.GetReadAccuracy(config);
	}

	internal int GetRuntimeCounts()
	{
		bool flag = (SyncSettings & AnimatorSyncSettings.ParameterFloats) == AnimatorSyncSettings.ParameterFloats;
		bool flag2 = (SyncSettings & AnimatorSyncSettings.ParameterInts) == AnimatorSyncSettings.ParameterInts;
		bool flag3 = (SyncSettings & AnimatorSyncSettings.ParameterBools) == AnimatorSyncSettings.ParameterBools;
		bool flag4 = (SyncSettings & AnimatorSyncSettings.ParameterTriggers) == AnimatorSyncSettings.ParameterTriggers;
		bool flag5 = (SyncSettings & AnimatorSyncSettings.StateRoot) == AnimatorSyncSettings.StateRoot;
		bool flag6 = (SyncSettings & AnimatorSyncSettings.LayerWeights) == AnimatorSyncSettings.LayerWeights;
		bool flag7 = (SyncSettings & AnimatorSyncSettings.StateLayers) == AnimatorSyncSettings.StateLayers;
		_param32Count = 0;
		_paramBoolCount = 0;
		_cachedParameters = Animator.parameters;
		_cachedParameterCount = Animator.parameterCount;
		_cachedParameterHashes = new int[_cachedParameterCount];
		_cachedLayerCount = Animator.layerCount;
		int i = 0;
		for (int num = _cachedParameters.Length; i < num; i++)
		{
			AnimatorControllerParameter animatorControllerParameter = _cachedParameters[i];
			_cachedParameterHashes[i] = animatorControllerParameter.nameHash;
			switch (animatorControllerParameter.type)
			{
			case AnimatorControllerParameterType.Float:
				if (flag && !Animator.IsParameterControlledByCurve(_cachedParameterHashes[i]))
				{
					_param32Count++;
				}
				break;
			case AnimatorControllerParameterType.Int:
				if (flag2)
				{
					_param32Count++;
				}
				break;
			case AnimatorControllerParameterType.Bool:
				if (flag3)
				{
					_paramBoolCount++;
				}
				break;
			case AnimatorControllerParameterType.Trigger:
				if (flag4)
				{
					_paramBoolCount++;
				}
				break;
			}
		}
		_syncedLayerCount = ((!flag7) ? 1 : Animator.layerCount);
		int param32Count = _param32Count;
		int num2 = (flag5 ? (2 * _syncedLayerCount) : 0);
		int num3 = ((flag6 && Animator.layerCount > 0) ? (Animator.layerCount - 1) : 0);
		_paramBoolsWordCount = _paramBoolCount * 4 + 31 >> 5;
		_paramBoolsPtrOffset = _param32Count;
		_prevBoolsBitmask = new int[_paramBoolsWordCount];
		return param32Count + _paramBoolsWordCount + num2 + num3;
	}

	private void Awake()
	{
		if (Animator == null)
		{
			Animator = GetComponent<Animator>();
		}
		if (Animator == null)
		{
			Debug.LogWarning("NetworkMecanimAnimator found no associated Unity Animator component. Removing.");
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (TotalWords == -1)
		{
			TotalWords = GetRuntimeCounts();
		}
		for (int i = 0; i < StateHashes.Length; i++)
		{
			StateIndexLookup.Add(StateHashes[i], i);
		}
		for (int j = 0; j < TriggerHashes.Length; j++)
		{
			TriggerIndexLookup.Add(TriggerHashes[j], j);
		}
	}

	public void SetTrigger(int triggerHash, bool passThroughOnInputAuthority = false)
	{
		if (Object.HasStateAuthority)
		{
			pendingTriggers.Add(triggerHash);
		}
		else if (passThroughOnInputAuthority && Object.HasInputAuthority)
		{
			Animator.SetTrigger(triggerHash);
		}
	}

	public void SetTrigger(string trigger, bool passThroughOnInputAuthority = false)
	{
		if (Object.HasStateAuthority)
		{
			int item = Animator.StringToHash(trigger);
			pendingTriggers.Add(item);
		}
		else if (passThroughOnInputAuthority && Object.HasInputAuthority)
		{
			Animator.SetTrigger(trigger);
		}
	}

	public unsafe override void FixedUpdateNetwork()
	{
		if (Object.HasStateAuthority)
		{
			CaptureAnimatorData(Ptr);
		}
	}

	unsafe void IAfterAllTicks.AfterAllTicks(bool resimulation, int tickCount)
	{
		if (Object.IsProxy)
		{
			ApplyAnimatorData(Ptr);
		}
	}

	internal unsafe void CaptureAnimatorData(int* ptr)
	{
		CaptureParameters(ref ptr);
		if ((SyncSettings & AnimatorSyncSettings.StateRoot) == AnimatorSyncSettings.StateRoot)
		{
			CaptureStates(ref ptr);
		}
		if ((SyncSettings & AnimatorSyncSettings.LayerWeights) == AnimatorSyncSettings.LayerWeights)
		{
			CaptureLayerWeights(ref ptr);
		}
	}

	internal unsafe void ApplyAnimatorData(int* ptr)
	{
		ApplyParameters(ref ptr);
		if ((SyncSettings & AnimatorSyncSettings.StateRoot) == AnimatorSyncSettings.StateRoot)
		{
			ApplyStates(ref ptr);
		}
		if ((SyncSettings & AnimatorSyncSettings.LayerWeights) == AnimatorSyncSettings.LayerWeights)
		{
			ApplyLayerWeights(ref ptr);
		}
	}

	private unsafe void CaptureStates(ref int* ptr)
	{
		for (int i = 0; i < _syncedLayerCount; i++)
		{
			if (Animator.IsInTransition(i))
			{
				*ptr = 0;
				ptr++;
				*(float*)ptr = 0f;
				ptr++;
				continue;
			}
			AnimatorStateInfo currentAnimatorStateInfo = Animator.GetCurrentAnimatorStateInfo(i);
			int num = currentAnimatorStateInfo.fullPathHash;
			if (StateIndexLookup.TryGetValue(num, out var value))
			{
				num = value;
			}
			else
			{
				Log.DebugWarn(base.name + ":" + GetType().Name + " cannot find hash in indexes. Inspect the component to refresh the controller hash lookup. Sending full hash instead of index as fallback.");
			}
			*ptr = num;
			ptr++;
			ReadWriteUtils.WriteFloat(ptr, _timeWriteAccuracy, currentAnimatorStateInfo.normalizedTime);
			ptr++;
		}
	}

	private unsafe void ApplyStates(ref int* ptr)
	{
		for (int i = 0; i < _syncedLayerCount; i++)
		{
			int num = *ptr;
			ptr++;
			float normalizedTime = ReadWriteUtils.ReadFloat(ptr, _timeReadAccuracy);
			ptr++;
			if (num == 0)
			{
				break;
			}
			if (num > 0 && num < StateHashes.Length)
			{
				num = StateHashes[num];
			}
			else
			{
				Log.DebugWarn(base.name + ":" + GetType().Name + " cannot find hash in indexes. Inspect the component to refresh the controller hash lookup. Sending full hash instead of index as fallback.");
			}
			Animator.Play(num, i, normalizedTime);
		}
	}

	private unsafe void CaptureParameters(ref int* ptr)
	{
		bool flag = (SyncSettings & AnimatorSyncSettings.ParameterFloats) == AnimatorSyncSettings.ParameterFloats;
		bool flag2 = (SyncSettings & AnimatorSyncSettings.ParameterInts) == AnimatorSyncSettings.ParameterInts;
		bool flag3 = (SyncSettings & AnimatorSyncSettings.ParameterBools) == AnimatorSyncSettings.ParameterBools;
		bool flag4 = (SyncSettings & AnimatorSyncSettings.ParameterTriggers) == AnimatorSyncSettings.ParameterTriggers;
		bool flag5 = true;
		int* ptr2 = ptr + _paramBoolsPtrOffset;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		AnimatorControllerParameter[] cachedParameters = _cachedParameters;
		int[] cachedParameterHashes = _cachedParameterHashes;
		int i = 0;
		for (int cachedParameterCount = _cachedParameterCount; i < cachedParameterCount; i++)
		{
			int num4 = cachedParameterHashes[i];
			switch (cachedParameters[i].type)
			{
			case AnimatorControllerParameterType.Float:
				if (flag && !Animator.IsParameterControlledByCurve(num4))
				{
					float value = Animator.GetFloat(num4);
					ReadWriteUtils.WriteFloat(ptr, _floatWriteAccuracy, value);
					ptr++;
				}
				break;
			case AnimatorControllerParameterType.Int:
				if (flag2)
				{
					*ptr = Animator.GetInteger(num4);
					ptr++;
				}
				break;
			case AnimatorControllerParameterType.Bool:
			{
				if (!flag3)
				{
					break;
				}
				if (flag5)
				{
					num3 = *ptr2;
					flag5 = false;
				}
				int num9 = 4 * num;
				int num10 = 15 << num9;
				num3 &= ~num10;
				if (Animator.GetBool(num4))
				{
					num3 |= 1 << num9;
				}
				num++;
				if (num == 8)
				{
					num = 0;
					ptr2[num2++] = num3;
					if (num2 < _paramBoolsWordCount)
					{
						num3 = ptr2[num2];
					}
				}
				break;
			}
			case AnimatorControllerParameterType.Trigger:
			{
				bool flag6 = pendingTriggers.Contains(num4);
				if (flag6)
				{
					Animator.SetTrigger(num4);
				}
				if (!flag4)
				{
					break;
				}
				if (flag5)
				{
					num3 = *ptr2;
					flag5 = false;
				}
				int num5 = 4 * num;
				int num6 = 15 << num5;
				int num7 = (num3 & num6) >> num5;
				int num8 = num7 >> 1;
				bool flag7 = (num7 & 1) != 0;
				if (flag6 | flag7)
				{
					num7 = num8 + 1 << 1;
					if (flag6)
					{
						num7 |= 1;
					}
					num7 <<= num5;
					num3 &= ~num6;
					num3 |= num7 & num6;
				}
				num++;
				if (num == 8)
				{
					num = 0;
					ptr2[num2++] = num3;
					if (num2 < _paramBoolsWordCount)
					{
						num3 = ptr2[num2];
					}
				}
				break;
			}
			}
		}
		if (num > 0)
		{
			ptr2[num2] = num3;
		}
		ptr += _paramBoolsWordCount;
		pendingTriggers.Clear();
	}

	private unsafe void ApplyParameters(ref int* ptr)
	{
		bool flag = (SyncSettings & AnimatorSyncSettings.ParameterFloats) == AnimatorSyncSettings.ParameterFloats;
		bool flag2 = (SyncSettings & AnimatorSyncSettings.ParameterInts) == AnimatorSyncSettings.ParameterInts;
		bool flag3 = (SyncSettings & AnimatorSyncSettings.ParameterBools) == AnimatorSyncSettings.ParameterBools;
		bool flag4 = (SyncSettings & AnimatorSyncSettings.ParameterTriggers) == AnimatorSyncSettings.ParameterTriggers;
		bool flag5 = true;
		int* ptr2 = ptr + _paramBoolsPtrOffset;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		AnimatorControllerParameter[] cachedParameters = _cachedParameters;
		int[] cachedParameterHashes = _cachedParameterHashes;
		int i = 0;
		for (int cachedParameterCount = _cachedParameterCount; i < cachedParameterCount; i++)
		{
			int num5 = cachedParameterHashes[i];
			switch (cachedParameters[i].type)
			{
			case AnimatorControllerParameterType.Float:
				if (flag && !Animator.IsParameterControlledByCurve(num5))
				{
					float value = ReadWriteUtils.ReadFloat(ptr, _floatReadAccuracy);
					Animator.SetFloat(num5, value);
					ptr++;
				}
				break;
			case AnimatorControllerParameterType.Int:
				if (flag2)
				{
					Animator.SetInteger(num5, *ptr);
					ptr++;
				}
				break;
			case AnimatorControllerParameterType.Bool:
			{
				if (!flag3)
				{
					break;
				}
				if (flag5)
				{
					num3 = _prevBoolsBitmask[0];
					num4 = *ptr2;
					flag5 = false;
				}
				int num10 = 4 * num;
				bool value2 = (num4 & (1 << num10)) != 0;
				Animator.SetBool(num5, value2);
				num++;
				if (num == 8)
				{
					_prevBoolsBitmask[num2] = num4;
					num = 0;
					num2++;
					if (num2 < _paramBoolsWordCount)
					{
						num3 = _prevBoolsBitmask[num2];
						num4 = ptr2[num2];
					}
				}
				break;
			}
			case AnimatorControllerParameterType.Trigger:
			{
				if (!flag4)
				{
					break;
				}
				if (flag5)
				{
					num3 = _prevBoolsBitmask[0];
					num4 = *ptr2;
					flag5 = false;
				}
				int num6 = 4 * num;
				int num7 = 15 << num6;
				int num8 = (num3 & num7) >> num6;
				int num9 = (num4 & num7) >> num6;
				if (num8 != num9)
				{
					bool flag6 = (num8 & 1) != 0;
					if ((num9 & 1) != 0 || !flag6)
					{
						Animator.SetTrigger(num5);
					}
				}
				num++;
				if (num == 8)
				{
					num = 0;
					num2++;
					if (num2 < _paramBoolsWordCount)
					{
						num3 = _prevBoolsBitmask[num2];
						num4 = ptr2[num2];
					}
				}
				break;
			}
			}
		}
		if (num > 0)
		{
			_prevBoolsBitmask[num2] = num4;
		}
		ptr += _paramBoolsWordCount;
	}

	private unsafe void CaptureLayerWeights(ref int* ptr)
	{
		int num = 1;
		int cachedLayerCount = _cachedLayerCount;
		while (num < cachedLayerCount)
		{
			ReadWriteUtils.WriteFloat(ptr, _weightWriteAccuracy, Animator.GetLayerWeight(num));
			num++;
			ptr++;
		}
	}

	private unsafe void ApplyLayerWeights(ref int* ptr)
	{
		int num = 1;
		int cachedLayerCount = _cachedLayerCount;
		while (num < cachedLayerCount)
		{
			Animator.SetLayerWeight(num, ReadWriteUtils.ReadFloat(ptr, _weightReadAccuracy));
			num++;
			ptr++;
		}
	}
}
