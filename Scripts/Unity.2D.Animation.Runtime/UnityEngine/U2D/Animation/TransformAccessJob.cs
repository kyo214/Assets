using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Jobs;

namespace UnityEngine.U2D.Animation;

internal class TransformAccessJob
{
	internal struct TransformData(int index)
	{
		public int transformIndex = index;

		public int refCount = 1;
	}

	private Transform[] m_Transform;

	private TransformAccessArray m_TransformAccessArray;

	private NativeHashMap<int, TransformData> m_TransformData;

	private NativeArray<float4x4> m_TransformMatrix;

	private bool m_Dirty;

	private JobHandle m_JobHandle;

	public NativeHashMap<int, TransformData> transformData => m_TransformData;

	public NativeArray<float4x4> transformMatrix => m_TransformMatrix;

	public TransformAccessJob()
	{
		InitializeDataStructures();
		m_Dirty = false;
		m_JobHandle = default;
	}

	public void ResetCache()
	{
		ClearDataStructures();
		InitializeDataStructures();
	}

	private void InitializeDataStructures()
	{
		m_TransformMatrix = new NativeArray<float4x4>(1, Allocator.Persistent);
		m_TransformData = new NativeHashMap<int, TransformData>(1, Allocator.Persistent);
		m_Transform = Array.Empty<Transform>();
	}

	public void Destroy()
	{
		m_JobHandle.Complete();
		ClearDataStructures();
	}

	private void ClearDataStructures()
	{
		if (m_TransformMatrix.IsCreated)
		{
			m_TransformMatrix.Dispose();
		}
		if (m_TransformAccessArray.isCreated)
		{
			m_TransformAccessArray.Dispose();
		}
		if (m_TransformData.IsCreated)
		{
			m_TransformData.Dispose();
		}
		m_Transform = null;
	}

	public void AddTransform(Transform t)
	{
		if (!(t == null) && m_TransformData.IsCreated)
		{
			m_JobHandle.Complete();
			int instanceID = t.GetInstanceID();
			if (m_TransformData.ContainsKey(instanceID))
			{
				TransformData value = m_TransformData[instanceID];
				value.refCount++;
				m_TransformData[instanceID] = value;
			}
			else
			{
				m_TransformData.TryAdd(instanceID, new TransformData(-1));
				ArrayAdd(ref m_Transform, t);
				m_Dirty = true;
			}
		}
	}

	private static void ArrayAdd<T>(ref T[] array, T item)
	{
		int num = array.Length;
		Array.Resize(ref array, num + 1);
		array[num] = item;
	}

	private static void ArrayRemoveAt<T>(ref T[] array, int index)
	{
		List<T> list = new List<T>(array);
		list.RemoveAt(index);
		array = list.ToArray();
	}

	private void UpdateTransformIndex()
	{
		if (!m_Dirty)
		{
			return;
		}
		m_Dirty = false;
		NativeArrayHelpers.ResizeIfNeeded(ref m_TransformMatrix, m_Transform.Length);
		if (!m_TransformAccessArray.isCreated)
		{
			TransformAccessArray.Allocate(m_Transform.Length, -1, out m_TransformAccessArray);
		}
		else if (m_TransformAccessArray.capacity != m_Transform.Length)
		{
			m_TransformAccessArray.capacity = m_Transform.Length;
		}
		m_TransformAccessArray.SetTransforms(m_Transform);
		for (int i = 0; i < m_Transform.Length; i++)
		{
			if (m_Transform[i] != null)
			{
				int instanceID = m_Transform[i].GetInstanceID();
				TransformData value = m_TransformData[instanceID];
				value.transformIndex = i;
				m_TransformData[instanceID] = value;
			}
		}
	}

	public JobHandle StartLocalToWorldJob()
	{
		if (m_Transform.Length != 0)
		{
			m_JobHandle.Complete();
			UpdateTransformIndex();
			LocalToWorldTransformAccessJob jobData = new LocalToWorldTransformAccessJob
			{
				outMatrix = transformMatrix
			};
			m_JobHandle = jobData.Schedule(m_TransformAccessArray);
			return m_JobHandle;
		}
		return default;
	}

	public JobHandle StartWorldToLocalJob()
	{
		if (m_Transform.Length != 0)
		{
			m_JobHandle.Complete();
			UpdateTransformIndex();
			WorldToLocalTransformAccessJob jobData = new WorldToLocalTransformAccessJob
			{
				outMatrix = transformMatrix
			};
			m_JobHandle = jobData.Schedule(m_TransformAccessArray);
			return m_JobHandle;
		}
		return default;
	}

	internal string GetDebugLog()
	{
		string text = "";
		text = text + "TransformData Count: " + m_TransformData.Count() + "\n";
		text = text + "Transform Count: " + m_Transform.Length + "\n";
		Transform[] transform = m_Transform;
		foreach (Transform transform2 in transform)
		{
			text += ((transform2 == null) ? "null" : (transform2.name + " " + transform2.GetInstanceID()));
			text += "\n";
			if (transform2 != null)
			{
				text = text + "RefCount: " + m_TransformData[transform2.GetInstanceID()].refCount + "\n";
			}
			text += "\n";
		}
		return text;
	}

	internal int RemoveTransformsIfNull()
	{
		if (!Array.Exists(m_Transform, (Transform t) => t == null))
		{
			return 0;
		}
		List<Transform> list = new List<Transform>(m_Transform);
		int result = list.RemoveAll((Transform t) => t == null);
		if (m_Transform.Length != list.Count)
		{
			m_Transform = list.ToArray();
		}
		return result;
	}

	internal void RemoveTransformsByIds(IList<int> idsToRemove)
	{
		if (!m_TransformData.IsCreated)
		{
			return;
		}
		m_JobHandle.Complete();
		for (int num = idsToRemove.Count - 1; num >= 0; num--)
		{
			int num2 = idsToRemove[num];
			if (!m_TransformData.ContainsKey(num2))
			{
				idsToRemove.Remove(num2);
			}
			else
			{
				TransformData value = m_TransformData[num2];
				if (value.refCount > 1)
				{
					value.refCount--;
					m_TransformData[num2] = value;
					idsToRemove.Remove(num2);
				}
			}
		}
		if (idsToRemove.Count == 0)
		{
			return;
		}
		List<Transform> list = new List<Transform>(m_Transform);
		foreach (int id in idsToRemove)
		{
			m_TransformData.Remove(id);
			int num3 = list.FindIndex((Transform t) => t.GetInstanceID() == id);
			if (num3 >= 0)
			{
				list.RemoveAt(num3);
			}
		}
		m_Transform = list.ToArray();
	}

	internal void RemoveTransformById(int transformId)
	{
		if (!m_TransformData.IsCreated)
		{
			return;
		}
		m_JobHandle.Complete();
		if (!m_TransformData.ContainsKey(transformId))
		{
			return;
		}
		TransformData value = m_TransformData[transformId];
		if (value.refCount == 1)
		{
			m_TransformData.Remove(transformId);
			int num = Array.FindIndex(m_Transform, (Transform t) => t.GetInstanceID() == transformId);
			if (num >= 0)
			{
				ArrayRemoveAt(ref m_Transform, num);
			}
			m_Dirty = true;
		}
		else
		{
			value.refCount--;
			m_TransformData[transformId] = value;
		}
	}
}
