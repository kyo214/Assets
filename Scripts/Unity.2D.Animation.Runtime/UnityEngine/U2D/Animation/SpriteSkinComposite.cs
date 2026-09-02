using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine.U2D.Common;

namespace UnityEngine.U2D.Animation;

internal class SpriteSkinComposite : ScriptableObject
{
	private static class Profiling
	{
		public static readonly ProfilerMarker batchAddSpriteSkin = new ProfilerMarker("SpriteSkinComposite.BatchAddSpriteSkin");

		public static readonly ProfilerMarker batchRemoveSpriteSkin = new ProfilerMarker("SpriteSkinComposite.BatchRemoveSpriteSkin");

		public static readonly ProfilerMarker prepareData = new ProfilerMarker("SpriteSkinComposite.PrepareData");

		public static readonly ProfilerMarker validateSpriteSkinData = new ProfilerMarker("SpriteSkinComposite.ValidateSpriteSkinData");

		public static readonly ProfilerMarker transformAccessJob = new ProfilerMarker("SpriteSkinComposite.TransformAccessJob");

		public static readonly ProfilerMarker getSpriteSkinBatchData = new ProfilerMarker("SpriteSkinComposite.GetSpriteSkinBatchData");

		public static readonly ProfilerMarker resizeBuffers = new ProfilerMarker("SpriteSkinComposite.ResizeBuffers");

		public static readonly ProfilerMarker prepare = new ProfilerMarker("SpriteSkinComposite.Prepare");

		public static readonly ProfilerMarker scheduleJobs = new ProfilerMarker("SpriteSkinComposite.ScheduleJobs");

		public static readonly ProfilerMarker setBatchDeformableBufferAndLocalAABB = new ProfilerMarker("SpriteSkinComposite.SetBatchDeformableBufferAndLocalAABB");

		public static readonly ProfilerMarker deactivateDeformableBuffer = new ProfilerMarker("SpriteSkinComposite.DeactivateDeformableBuffer");
	}

	private static SpriteSkinComposite s_Instance;

	private List<SpriteSkin> m_SpriteSkinsToAdd = new List<SpriteSkin>();

	private List<SpriteSkin> m_SpriteSkinsToRemove = new List<SpriteSkin>();

	private List<int> m_TransformIdsToRemove = new List<int>();

	private List<SpriteSkin> m_SpriteSkins = new List<SpriteSkin>();

	private SpriteRenderer[] m_SpriteRenderers = new SpriteRenderer[0];

	private NativeByteArray m_DeformedVerticesBuffer;

	private NativeArray<float4x4> m_FinalBoneTransforms;

	private NativeArray<bool> m_IsSpriteSkinActiveForDeform;

	private NativeArray<SpriteSkinData> m_SpriteSkinData;

	private NativeArray<PerSkinJobData> m_PerSkinJobData;

	private NativeArray<Bounds> m_BoundsData;

	private NativeArray<IntPtr> m_Buffers;

	private NativeArray<int> m_BufferSizes;

	private NativeArray<int2> m_BoneLookupData;

	private NativeArray<int2> m_VertexLookupData;

	private NativeArray<PerSkinJobData> m_SkinBatchArray;

	private TransformAccessJob m_LocalToWorldTransformAccessJob;

	private TransformAccessJob m_WorldToLocalTransformAccessJob;

	private JobHandle m_BoundJobHandle;

	private JobHandle m_DeformJobHandle;

	private JobHandle m_CopyJobHandle;

	[SerializeField]
	private GameObject m_Helper;

	public static SpriteSkinComposite instance
	{
		get
		{
			if (s_Instance == null)
			{
				SpriteSkinComposite[] array = Resources.FindObjectsOfTypeAll<SpriteSkinComposite>();
				if (array.Length != 0)
				{
					s_Instance = array[0];
				}
				else
				{
					s_Instance = ScriptableObject.CreateInstance<SpriteSkinComposite>();
				}
				s_Instance.hideFlags = HideFlags.HideAndDontSave;
				s_Instance.Init();
			}
			return s_Instance;
		}
	}

	internal GameObject helperGameObject => m_Helper;

	internal void RemoveTransformById(int transformId)
	{
		m_LocalToWorldTransformAccessJob.RemoveTransformById(transformId);
	}

	internal void AddSpriteSkinBoneTransform(SpriteSkin spriteSkin)
	{
		if (spriteSkin == null || spriteSkin.boneTransforms == null)
		{
			return;
		}
		Transform[] boneTransforms = spriteSkin.boneTransforms;
		foreach (Transform transform in boneTransforms)
		{
			if (transform != null)
			{
				m_LocalToWorldTransformAccessJob.AddTransform(transform);
			}
		}
	}

	internal void AddSpriteSkinRootBoneTransform(SpriteSkin spriteSkin)
	{
		if (!(spriteSkin == null) && !(spriteSkin.rootBone == null))
		{
			m_LocalToWorldTransformAccessJob.AddTransform(spriteSkin.rootBone);
		}
	}

	internal void AddSpriteSkin(SpriteSkin spriteSkin)
	{
		if (!(spriteSkin == null))
		{
			if (!DoesContainSpriteSkin(in m_SpriteSkins, spriteSkin) && !DoesContainSpriteSkin(in m_SpriteSkinsToAdd, spriteSkin))
			{
				m_SpriteSkinsToAdd.Add(spriteSkin);
			}
			if (DoesContainSpriteSkin(in m_SpriteSkinsToRemove, spriteSkin))
			{
				m_SpriteSkinsToRemove.Remove(spriteSkin);
				m_TransformIdsToRemove.Remove(spriteSkin.transform.GetInstanceID());
			}
		}
	}

	internal void CopyToSpriteSkinData(SpriteSkin spriteSkin)
	{
		if (!(spriteSkin == null))
		{
			int num = m_SpriteSkins.IndexOf(spriteSkin);
			if (num >= 0)
			{
				CopyToSpriteSkinData(num);
			}
		}
	}

	private void CopyToSpriteSkinData(int index)
	{
		if (index >= 0 && index < m_SpriteSkins.Count && !(m_SpriteSkins[index] == null) && m_SpriteSkinData.IsCreated)
		{
			SpriteSkinData data = default;
			SpriteSkin spriteSkin = m_SpriteSkins[index];
			spriteSkin.CopyToSpriteSkinData(ref data, index);
			m_SpriteSkinData[index] = data;
			m_SpriteRenderers[index] = spriteSkin.spriteRenderer;
		}
	}

	internal void RemoveSpriteSkin(SpriteSkin spriteSkin)
	{
		if (!(spriteSkin == null))
		{
			if (DoesContainSpriteSkin(in m_SpriteSkins, spriteSkin) && !DoesContainSpriteSkin(in m_SpriteSkinsToRemove, spriteSkin))
			{
				m_SpriteSkinsToRemove.Add(spriteSkin);
				m_TransformIdsToRemove.Add(spriteSkin.transform.GetInstanceID());
			}
			if (DoesContainSpriteSkin(in m_SpriteSkinsToAdd, spriteSkin))
			{
				m_SpriteSkinsToAdd.Remove(spriteSkin);
			}
		}
	}

	private static bool DoesContainSpriteSkin(in List<SpriteSkin> collection, SpriteSkin skin)
	{
		int num = collection.IndexOf(skin);
		if (num < 0)
		{
			return false;
		}
		return collection[num] != null;
	}

	private void Init()
	{
		if (m_LocalToWorldTransformAccessJob == null)
		{
			m_LocalToWorldTransformAccessJob = new TransformAccessJob();
		}
		if (m_WorldToLocalTransformAccessJob == null)
		{
			m_WorldToLocalTransformAccessJob = new TransformAccessJob();
		}
		CreateHelper();
	}

	private void CreateHelper()
	{
		if (!(m_Helper != null))
		{
			m_Helper = new GameObject("SpriteSkinUpdateHelper");
			m_Helper.hideFlags = HideFlags.HideAndDontSave;
			SpriteSkinUpdateHelper spriteSkinUpdateHelper = m_Helper.AddComponent<SpriteSkinUpdateHelper>();
			spriteSkinUpdateHelper.onDestroyingComponent = (Action<GameObject>)Delegate.Combine(spriteSkinUpdateHelper.onDestroyingComponent, new Action<GameObject>(OnHelperDestroyed));
			Object.DontDestroyOnLoad(m_Helper);
		}
	}

	private void OnHelperDestroyed(GameObject helperGo)
	{
		if (!(m_Helper != helperGo))
		{
			m_Helper = null;
			CreateHelper();
		}
	}

	internal void ResetComposite()
	{
		m_SpriteSkins.Clear();
		m_LocalToWorldTransformAccessJob.Destroy();
		m_WorldToLocalTransformAccessJob.Destroy();
		m_LocalToWorldTransformAccessJob = new TransformAccessJob();
		m_WorldToLocalTransformAccessJob = new TransformAccessJob();
	}

	public void OnEnable()
	{
		s_Instance = this;
		m_FinalBoneTransforms = new NativeArray<float4x4>(1, Allocator.Persistent);
		m_BoneLookupData = new NativeArray<int2>(1, Allocator.Persistent);
		m_VertexLookupData = new NativeArray<int2>(1, Allocator.Persistent);
		m_SkinBatchArray = new NativeArray<PerSkinJobData>(1, Allocator.Persistent);
		Init();
		InitializeArrays();
		BatchRemoveSpriteSkins();
		BatchAddSpriteSkins();
		for (int i = 0; i < m_SpriteSkins.Count; i++)
		{
			CopyToSpriteSkinData(i);
		}
	}

	private void InitializeArrays()
	{
		m_IsSpriteSkinActiveForDeform = new NativeArray<bool>(0, Allocator.Persistent);
		m_PerSkinJobData = new NativeArray<PerSkinJobData>(0, Allocator.Persistent);
		m_SpriteSkinData = new NativeArray<SpriteSkinData>(0, Allocator.Persistent);
		m_BoundsData = new NativeArray<Bounds>(0, Allocator.Persistent);
		m_Buffers = new NativeArray<IntPtr>(0, Allocator.Persistent);
		m_BufferSizes = new NativeArray<int>(0, Allocator.Persistent);
	}

	private void OnDisable()
	{
		m_DeformJobHandle.Complete();
		m_BoundJobHandle.Complete();
		m_CopyJobHandle.Complete();
		m_SpriteSkins.Clear();
		m_SpriteRenderers = new SpriteRenderer[0];
		BufferManager.instance.ReturnBuffer(GetInstanceID());
		m_IsSpriteSkinActiveForDeform.DisposeIfCreated();
		m_PerSkinJobData.DisposeIfCreated();
		m_SpriteSkinData.DisposeIfCreated();
		m_Buffers.DisposeIfCreated();
		m_BufferSizes.DisposeIfCreated();
		m_BoneLookupData.DisposeIfCreated();
		m_VertexLookupData.DisposeIfCreated();
		m_SkinBatchArray.DisposeIfCreated();
		m_FinalBoneTransforms.DisposeIfCreated();
		m_BoundsData.DisposeIfCreated();
		if (m_Helper != null)
		{
			SpriteSkinUpdateHelper component = m_Helper.GetComponent<SpriteSkinUpdateHelper>();
			component.onDestroyingComponent = (Action<GameObject>)Delegate.Remove(component.onDestroyingComponent, new Action<GameObject>(OnHelperDestroyed));
			Object.DestroyImmediate(m_Helper);
		}
		m_LocalToWorldTransformAccessJob.Destroy();
		m_WorldToLocalTransformAccessJob.Destroy();
	}

	internal unsafe void LateUpdate()
	{
		BatchRemoveSpriteSkins();
		BatchAddSpriteSkins();
		if (m_SpriteSkins.Count == 0)
		{
			m_LocalToWorldTransformAccessJob.ResetCache();
			m_WorldToLocalTransformAccessJob.ResetCache();
			return;
		}
		using (Profiling.validateSpriteSkinData.Auto())
		{
			for (int i = 0; i < m_SpriteSkins.Count; i++)
			{
				SpriteSkin spriteSkin = m_SpriteSkins[i];
				m_IsSpriteSkinActiveForDeform[i] = spriteSkin.BatchValidate();
				if (m_IsSpriteSkinActiveForDeform[i] && spriteSkin.NeedUpdateCompositeCache())
				{
					CopyToSpriteSkinData(i);
				}
			}
		}
		JobHandle job = m_LocalToWorldTransformAccessJob.StartLocalToWorldJob();
		JobHandle job2 = m_WorldToLocalTransformAccessJob.StartWorldToLocalJob();
		using (Profiling.getSpriteSkinBatchData.Auto())
		{
			NativeArrayHelpers.ResizeIfNeeded(ref m_SkinBatchArray, 1);
			new FillPerSkinJobSingleThread
			{
				isSpriteSkinValidForDeformArray = m_IsSpriteSkinActiveForDeform,
				combinedSkinBatchArray = m_SkinBatchArray,
				spriteSkinDataArray = m_SpriteSkinData,
				perSkinJobDataArray = m_PerSkinJobData
			}.Run();
		}
		PerSkinJobData perSkinJobData = m_SkinBatchArray[0];
		int length = m_SpriteSkinData.Length;
		int deformVerticesStartPos = perSkinJobData.deformVerticesStartPos;
		if (deformVerticesStartPos <= 0)
		{
			job.Complete();
			job2.Complete();
			DeactivateDeformableBuffers();
			return;
		}
		using (Profiling.resizeBuffers.Auto())
		{
			m_DeformedVerticesBuffer = BufferManager.instance.GetBuffer(GetInstanceID(), deformVerticesStartPos);
			NativeArrayHelpers.ResizeIfNeeded(ref m_FinalBoneTransforms, perSkinJobData.bindPosesIndex.y);
			NativeArrayHelpers.ResizeIfNeeded(ref m_BoneLookupData, perSkinJobData.bindPosesIndex.y);
			NativeArrayHelpers.ResizeIfNeeded(ref m_VertexLookupData, perSkinJobData.verticesIndex.y);
		}
		JobHandle job3 = new PrepareDeformJob
		{
			batchDataSize = length,
			perSkinJobData = m_PerSkinJobData,
			boneLookupData = m_BoneLookupData,
			vertexLookupData = m_VertexLookupData
		}.Schedule();
		job3 = new BoneDeformBatchedJob
		{
			boneTransform = m_LocalToWorldTransformAccessJob.transformMatrix,
			rootTransform = m_WorldToLocalTransformAccessJob.transformMatrix,
			spriteSkinData = m_SpriteSkinData,
			boneLookupData = m_BoneLookupData,
			finalBoneTransforms = m_FinalBoneTransforms,
			rootTransformIndex = m_WorldToLocalTransformAccessJob.transformData,
			boneTransformIndex = m_LocalToWorldTransformAccessJob.transformData
		}.Schedule(dependsOn: JobHandle.CombineDependencies(job, job2, job3), arrayLength: perSkinJobData.bindPosesIndex.y, innerloopBatchCount: 8);
		SkinDeformBatchedJob jobData = new SkinDeformBatchedJob
		{
			vertices = m_DeformedVerticesBuffer.array,
			vertexLookupData = m_VertexLookupData,
			spriteSkinData = m_SpriteSkinData,
			perSkinJobData = m_PerSkinJobData,
			finalBoneTransforms = m_FinalBoneTransforms
		};
		m_DeformJobHandle = jobData.Schedule(perSkinJobData.verticesIndex.y, 16, job3);
		CopySpriteRendererBuffersJob jobData2 = new CopySpriteRendererBuffersJob
		{
			isSpriteSkinValidForDeformArray = m_IsSpriteSkinActiveForDeform,
			spriteSkinData = m_SpriteSkinData,
			ptrVertices = (IntPtr)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(m_DeformedVerticesBuffer.array),
			buffers = m_Buffers,
			bufferSizes = m_BufferSizes
		};
		m_CopyJobHandle = jobData2.Schedule(length, 16, job3);
		CalculateSpriteSkinAABBJob jobData3 = new CalculateSpriteSkinAABBJob
		{
			vertices = m_DeformedVerticesBuffer.array,
			isSpriteSkinValidForDeformArray = m_IsSpriteSkinActiveForDeform,
			spriteSkinData = m_SpriteSkinData,
			bounds = m_BoundsData
		};
		m_BoundJobHandle = jobData3.Schedule(length, 4, m_DeformJobHandle);
		JobHandle.ScheduleBatchedJobs();
		JobHandle.CombineDependencies(m_BoundJobHandle, m_CopyJobHandle).Complete();
		using (Profiling.setBatchDeformableBufferAndLocalAABB.Auto())
		{
			InternalEngineBridge.SetBatchDeformableBufferAndLocalAABBArray(m_SpriteRenderers, m_Buffers, m_BufferSizes, m_BoundsData);
		}
		DeactivateDeformableBuffers();
	}

	private void BatchRemoveSpriteSkins()
	{
		m_WorldToLocalTransformAccessJob.RemoveTransformsIfNull();
		if (m_SpriteSkinsToRemove.Count == 0)
		{
			return;
		}
		using (Profiling.batchRemoveSpriteSkin.Auto())
		{
			m_WorldToLocalTransformAccessJob.RemoveTransformsByIds(m_TransformIdsToRemove);
			int num = Mathf.Max(m_SpriteSkins.Count - m_SpriteSkinsToRemove.Count, 0);
			if (num == 0)
			{
				m_SpriteSkins.Clear();
			}
			else
			{
				foreach (SpriteSkin item in m_SpriteSkinsToRemove)
				{
					int num2 = m_SpriteSkins.IndexOf(item);
					if (num2 >= 0)
					{
						if (num2 < m_SpriteSkins.Count - 1)
						{
							m_SpriteSkins.RemoveAtSwapBack(num2);
						}
						else
						{
							m_SpriteSkins.RemoveAt(num2);
						}
					}
				}
			}
			for (int i = 0; i < m_SpriteSkins.Count; i++)
			{
				if (i != m_SpriteSkins[i].dataIndex)
				{
					CopyToSpriteSkinData(i);
				}
			}
			Array.Resize(ref m_SpriteRenderers, num);
			ResizeAndCopyArrays(num);
			m_TransformIdsToRemove.Clear();
			m_SpriteSkinsToRemove.Clear();
		}
	}

	private void BatchAddSpriteSkins()
	{
		if (m_SpriteSkinsToAdd.Count == 0)
		{
			return;
		}
		using (Profiling.batchAddSpriteSkin.Auto())
		{
			int num = m_SpriteSkins.Count + m_SpriteSkinsToAdd.Count;
			Array.Resize(ref m_SpriteRenderers, num);
			if (m_IsSpriteSkinActiveForDeform.IsCreated)
			{
				ResizeAndCopyArrays(num);
			}
			foreach (SpriteSkin item in m_SpriteSkinsToAdd)
			{
				if (DoesContainSpriteSkin(in m_SpriteSkins, item))
				{
					Debug.LogError("Skin already exists! Name=" + item.name);
					continue;
				}
				m_SpriteSkins.Add(item);
				int count = m_SpriteSkins.Count;
				m_SpriteRenderers[count - 1] = item.spriteRenderer;
				m_WorldToLocalTransformAccessJob.AddTransform(item.transform);
				if (m_IsSpriteSkinActiveForDeform.IsCreated)
				{
					CopyToSpriteSkinData(count - 1);
				}
			}
			m_SpriteSkinsToAdd.Clear();
		}
	}

	private void ResizeAndCopyArrays(int updatedCount)
	{
		NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_IsSpriteSkinActiveForDeform, updatedCount);
		NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_PerSkinJobData, updatedCount);
		NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_SpriteSkinData, updatedCount);
		NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_BoundsData, updatedCount);
		NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_Buffers, updatedCount);
		NativeArrayHelpers.ResizeAndCopyIfNeeded(ref m_BufferSizes, updatedCount);
	}

	private void DeactivateDeformableBuffers()
	{
		using (Profiling.deactivateDeformableBuffer.Auto())
		{
			for (int i = 0; i < m_IsSpriteSkinActiveForDeform.Length; i++)
			{
				if (!m_IsSpriteSkinActiveForDeform[i] && !InternalEngineBridge.IsUsingDeformableBuffer(m_SpriteRenderers[i], IntPtr.Zero))
				{
					m_SpriteRenderers[i].DeactivateDeformableBuffer();
				}
			}
		}
	}

	internal bool HasDeformableBufferForSprite(int dataIndex)
	{
		if (dataIndex < 0 && m_IsSpriteSkinActiveForDeform.Length >= dataIndex)
		{
			throw new InvalidOperationException("Invalid index for deformable buffer");
		}
		return m_IsSpriteSkinActiveForDeform[dataIndex];
	}

	internal unsafe NativeArray<byte> GetDeformableBufferForSprite(int dataIndex)
	{
		if (dataIndex < 0 && m_SpriteSkinData.Length >= dataIndex)
		{
			throw new InvalidOperationException("Invalid index for deformable buffer");
		}
		if (!m_DeformJobHandle.IsCompleted)
		{
			m_DeformJobHandle.Complete();
		}
		SpriteSkinData spriteSkinData = m_SpriteSkinData[dataIndex];
		if (spriteSkinData.deformVerticesStartPos < 0)
		{
			throw new InvalidOperationException("There are no currently deformed vertices.");
		}
		int length = spriteSkinData.spriteVertexCount * spriteSkinData.spriteVertexStreamSize;
		byte* unsafeReadOnlyPtr = (byte*)m_DeformedVerticesBuffer.array.GetUnsafeReadOnlyPtr();
		unsafeReadOnlyPtr += spriteSkinData.deformVerticesStartPos;
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(unsafeReadOnlyPtr, length, Allocator.None);
	}

	internal string GetDebugLog()
	{
		string text = "";
		text += "===SpriteSkinBatch===\n";
		text = text + "Count: " + m_SpriteSkins.Count + "\n";
		foreach (SpriteSkin spriteSkin in m_SpriteSkins)
		{
			text += ((spriteSkin == null) ? "null" : spriteSkin.name);
			text += "\n";
		}
		text += "===LocalToWorldTransformAccessJob===\n";
		text += m_LocalToWorldTransformAccessJob.GetDebugLog();
		text += "\n";
		text += "===WorldToLocalTransformAccessJob===\n";
		text += "\n";
		return text + m_WorldToLocalTransformAccessJob.GetDebugLog();
	}

	internal SpriteSkin[] GetSpriteSkins()
	{
		return m_SpriteSkins.ToArray();
	}

	internal TransformAccessJob GetWorldToLocalTransformAccessJob()
	{
		return m_WorldToLocalTransformAccessJob;
	}

	internal TransformAccessJob GetLocalToWorldTransformAccessJob()
	{
		return m_LocalToWorldTransformAccessJob;
	}
}
