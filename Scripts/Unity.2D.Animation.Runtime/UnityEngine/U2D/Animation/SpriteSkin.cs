using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.U2D.Common;

namespace UnityEngine.U2D.Animation;

[Preserve]
[ExecuteInEditMode]
[DefaultExecutionOrder(-1)]
[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
[AddComponentMenu("2D Animation/Sprite Skin")]
[MovedFrom("UnityEngine.U2D.Experimental.Animation")]
[HelpURL("https://docs.unity3d.com/Packages/com.unity.2d.animation@9.0/manual/SpriteSkin.html")]
public sealed class SpriteSkin : MonoBehaviour, IPreviewable, ISerializationCallbackReceiver
{
	private static class Profiling
	{
		public static readonly ProfilerMarker cacheCurrentSprite = new ProfilerMarker("SpriteSkin.CacheCurrentSprite");

		public static readonly ProfilerMarker cacheHierarchy = new ProfilerMarker("SpriteSkin.CacheHierarchy");

		public static readonly ProfilerMarker getSpriteBonesTransformFromGuid = new ProfilerMarker("SpriteSkin.GetSpriteBoneTransformsFromGuid");

		public static readonly ProfilerMarker getSpriteBonesTransformFromPath = new ProfilerMarker("SpriteSkin.GetSpriteBoneTransformsFromPath");
	}

	private struct TransformData
	{
		public string fullName;

		public Transform transform;
	}

	[SerializeField]
	private Transform m_RootBone;

	[SerializeField]
	private Transform[] m_BoneTransforms = new Transform[0];

	[SerializeField]
	private Bounds m_Bounds;

	[SerializeField]
	private bool m_AlwaysUpdate = true;

	[SerializeField]
	private bool m_AutoRebind;

	private NativeByteArray m_DeformedVertices;

	private int m_CurrentDeformVerticesLength;

	private SpriteRenderer m_SpriteRenderer;

	private int m_CurrentDeformSprite;

	private bool m_ForceSkinning;

	private bool m_IsValid;

	private int m_TransformsHash;

	private int m_VertexDeformationHash;

	private int m_TransformId;

	private NativeArray<int> m_BoneTransformId;

	private int m_RootBoneTransformId;

	private NativeCustomSlice<Vector2> m_SpriteUVs;

	private NativeCustomSlice<Vector3> m_SpriteVertices;

	private NativeCustomSlice<Vector4> m_SpriteTangents;

	private NativeCustomSlice<BoneWeight> m_SpriteBoneWeights;

	private NativeCustomSlice<Matrix4x4> m_SpriteBindPoses;

	private NativeCustomSlice<int> m_BoneTransformIdNativeSlice;

	private bool m_SpriteHasTangents;

	private int m_SpriteVertexStreamSize;

	private int m_SpriteVertexCount;

	private int m_SpriteTangentVertexOffset;

	private int m_DataIndex = -1;

	private bool m_BoneCacheUpdateToDate;

	private Dictionary<int, List<TransformData>> m_HierarchyCache = new Dictionary<int, List<TransformData>>();

	internal Sprite sprite
	{
		get
		{
			if (!(m_SpriteRenderer != null))
			{
				return null;
			}
			return m_SpriteRenderer.sprite;
		}
	}

	internal SpriteRenderer spriteRenderer => m_SpriteRenderer;

	internal NativeCustomSlice<BoneWeight> spriteBoneWeights => m_SpriteBoneWeights;

	internal int dataIndex => m_DataIndex;

	public bool autoRebind
	{
		get
		{
			return m_AutoRebind;
		}
		set
		{
			m_AutoRebind = value;
			if (base.isActiveAndEnabled)
			{
				CacheHierarchy();
				CacheCurrentSprite(m_AutoRebind);
			}
		}
	}

	public Transform[] boneTransforms
	{
		get
		{
			return m_BoneTransforms;
		}
		internal set
		{
			m_BoneTransforms = value;
			CacheValidFlag();
			if (base.isActiveAndEnabled)
			{
				OnBoneTransformChanged();
			}
		}
	}

	public Transform rootBone
	{
		get
		{
			return m_RootBone;
		}
		internal set
		{
			m_RootBone = value;
			CacheValidFlag();
			if (base.isActiveAndEnabled)
			{
				CacheHierarchy();
				OnRootBoneTransformChanged();
			}
		}
	}

	internal Bounds bounds
	{
		get
		{
			return m_Bounds;
		}
		set
		{
			m_Bounds = value;
		}
	}

	public bool alwaysUpdate
	{
		get
		{
			return m_AlwaysUpdate;
		}
		set
		{
			m_AlwaysUpdate = value;
		}
	}

	internal bool isValid => this.Validate() == SpriteSkinValidationResult.Ready;

	private int GetSpriteInstanceID()
	{
		if (!(sprite != null))
		{
			return 0;
		}
		return sprite.GetInstanceID();
	}

	internal void Awake()
	{
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void OnEnable()
	{
		Awake();
		m_TransformsHash = 0;
		CacheCurrentSprite(rebind: false);
		if (m_HierarchyCache.Count == 0)
		{
			CacheHierarchy();
		}
		OnEnableBatch();
	}

	private void OnEnableBatch()
	{
		m_TransformId = base.gameObject.transform.GetInstanceID();
		UpdateSpriteDeform();
		CacheBoneTransformIds(forceUpdate: true);
		SpriteSkinComposite.instance.AddSpriteSkin(this);
	}

	private void OnResetBatch()
	{
		CacheBoneTransformIds(forceUpdate: true);
		SpriteSkinComposite.instance.CopyToSpriteSkinData(this);
	}

	private void OnDisableBatch()
	{
		RemoveTransformFromSpriteSkinComposite();
		SpriteSkinComposite.instance.RemoveSpriteSkin(this);
	}

	private void OnBoneTransformChanged()
	{
		CacheBoneTransformIds(forceUpdate: true);
	}

	private void OnRootBoneTransformChanged()
	{
		CacheBoneTransformIds(forceUpdate: true);
	}

	public void OnBeforeSerialize()
	{
		OnBeforeSerializeBatch();
	}

	public void OnAfterDeserialize()
	{
		OnAfterSerializeBatch();
	}

	private void OnBeforeSerializeBatch()
	{
	}

	private void OnAfterSerializeBatch()
	{
	}

	internal void OnEditorEnable()
	{
		Awake();
	}

	private void CacheValidFlag()
	{
		m_IsValid = isValid;
		if (!m_IsValid)
		{
			DeactivateSkinning();
		}
	}

	internal bool BatchValidate()
	{
		CacheBoneTransformIds();
		CacheCurrentSprite(m_AutoRebind);
		bool flag = m_CurrentDeformSprite != 0;
		if ((m_IsValid & flag) && spriteRenderer.enabled)
		{
			if (!alwaysUpdate)
			{
				return spriteRenderer.isVisible;
			}
			return true;
		}
		return false;
	}

	private void Reset()
	{
		Awake();
		if (base.isActiveAndEnabled)
		{
			CacheValidFlag();
			OnResetBatch();
		}
	}

	private void CacheBoneTransformIds(bool forceUpdate = false)
	{
		if (!(!m_BoneCacheUpdateToDate | forceUpdate))
		{
			return;
		}
		SpriteSkinComposite.instance.RemoveTransformById(m_RootBoneTransformId);
		if (rootBone != null)
		{
			m_RootBoneTransformId = rootBone.GetInstanceID();
			if (base.enabled)
			{
				SpriteSkinComposite.instance.AddSpriteSkinRootBoneTransform(this);
			}
		}
		else
		{
			m_RootBoneTransformId = 0;
		}
		if (boneTransforms != null)
		{
			int num = 0;
			for (int i = 0; i < boneTransforms.Length; i++)
			{
				if (boneTransforms[i] != null)
				{
					num++;
				}
			}
			if (m_BoneTransformId.IsCreated)
			{
				for (int j = 0; j < m_BoneTransformId.Length; j++)
				{
					SpriteSkinComposite.instance.RemoveTransformById(m_BoneTransformId[j]);
				}
				NativeArrayHelpers.ResizeIfNeeded(ref m_BoneTransformId, num);
			}
			else
			{
				m_BoneTransformId = new NativeArray<int>(num, Allocator.Persistent);
			}
			m_BoneTransformIdNativeSlice = new NativeCustomSlice<int>(m_BoneTransformId);
			int k = 0;
			int num2 = 0;
			for (; k < boneTransforms.Length; k++)
			{
				if (boneTransforms[k] != null)
				{
					m_BoneTransformId[num2] = boneTransforms[k].GetInstanceID();
					num2++;
				}
			}
			if (base.enabled)
			{
				SpriteSkinComposite.instance.AddSpriteSkinBoneTransform(this);
			}
		}
		else if (m_BoneTransformId.IsCreated)
		{
			NativeArrayHelpers.ResizeIfNeeded(ref m_BoneTransformId, 0);
		}
		else
		{
			m_BoneTransformId = new NativeArray<int>(0, Allocator.Persistent);
		}
		CacheValidFlag();
		m_BoneCacheUpdateToDate = true;
		SpriteSkinComposite.instance.CopyToSpriteSkinData(this);
	}

	private void RemoveTransformFromSpriteSkinComposite()
	{
		if (m_BoneTransformId.IsCreated)
		{
			for (int i = 0; i < m_BoneTransformId.Length; i++)
			{
				SpriteSkinComposite.instance.RemoveTransformById(m_BoneTransformId[i]);
			}
			m_BoneTransformId.Dispose();
		}
		SpriteSkinComposite.instance.RemoveTransformById(m_RootBoneTransformId);
		m_RootBoneTransformId = -1;
		m_BoneCacheUpdateToDate = false;
	}

	internal NativeByteArray GetDeformedVertices(int spriteVertexCount)
	{
		if (sprite != null)
		{
			if (m_CurrentDeformVerticesLength != spriteVertexCount)
			{
				m_TransformsHash = 0;
				m_CurrentDeformVerticesLength = spriteVertexCount;
			}
		}
		else
		{
			m_CurrentDeformVerticesLength = 0;
		}
		m_DeformedVertices = BufferManager.instance.GetBuffer(GetInstanceID(), m_CurrentDeformVerticesLength);
		return m_DeformedVertices;
	}

	public bool HasCurrentDeformedVertices()
	{
		if (!m_IsValid)
		{
			return false;
		}
		if (m_DataIndex >= 0)
		{
			return SpriteSkinComposite.instance.HasDeformableBufferForSprite(m_DataIndex);
		}
		return false;
	}

	internal NativeArray<byte> GetCurrentDeformedVertices()
	{
		if (!m_IsValid)
		{
			throw new InvalidOperationException("The SpriteSkin deformation is not valid.");
		}
		if (m_DataIndex < 0)
		{
			throw new InvalidOperationException("There are no currently deformed vertices.");
		}
		return SpriteSkinComposite.instance.GetDeformableBufferForSprite(m_DataIndex);
	}

	internal NativeSlice<PositionVertex> GetCurrentDeformedVertexPositions()
	{
		if (!m_IsValid)
		{
			throw new InvalidOperationException("The SpriteSkin deformation is not valid.");
		}
		if (sprite.HasVertexAttribute(VertexAttribute.Tangent))
		{
			throw new InvalidOperationException("This SpriteSkin has deformed tangents");
		}
		if (!sprite.HasVertexAttribute(VertexAttribute.Position))
		{
			throw new InvalidOperationException("This SpriteSkin does not have deformed positions.");
		}
		return GetCurrentDeformedVertices().Slice().SliceConvert<PositionVertex>();
	}

	internal NativeSlice<PositionTangentVertex> GetCurrentDeformedVertexPositionsAndTangents()
	{
		if (!m_IsValid)
		{
			throw new InvalidOperationException("The SpriteSkin deformation is not valid.");
		}
		if (!sprite.HasVertexAttribute(VertexAttribute.Tangent))
		{
			throw new InvalidOperationException("This SpriteSkin does not have deformed tangents");
		}
		if (!sprite.HasVertexAttribute(VertexAttribute.Position))
		{
			throw new InvalidOperationException("This SpriteSkin does not have deformed positions.");
		}
		return GetCurrentDeformedVertices().Slice().SliceConvert<PositionTangentVertex>();
	}

	public IEnumerable<Vector3> GetDeformedVertexPositionData()
	{
		if (!m_IsValid)
		{
			throw new InvalidOperationException("The SpriteSkin deformation is not valid.");
		}
		if (!sprite.HasVertexAttribute(VertexAttribute.Position))
		{
			throw new InvalidOperationException("Sprite does not have vertex position data.");
		}
		return new NativeCustomSliceEnumerator<Vector3>(GetCurrentDeformedVertices().Slice(sprite.GetVertexStreamOffset(VertexAttribute.Position)), sprite.GetVertexCount(), sprite.GetVertexStreamSize());
	}

	public IEnumerable<Vector4> GetDeformedVertexTangentData()
	{
		if (!m_IsValid)
		{
			throw new InvalidOperationException("The SpriteSkin deformation is not valid.");
		}
		if (!sprite.HasVertexAttribute(VertexAttribute.Tangent))
		{
			throw new InvalidOperationException("Sprite does not have vertex tangent data.");
		}
		return new NativeCustomSliceEnumerator<Vector4>(GetCurrentDeformedVertices().Slice(sprite.GetVertexStreamOffset(VertexAttribute.Tangent)), sprite.GetVertexCount(), sprite.GetVertexStreamSize());
	}

	private void OnDisable()
	{
		DeactivateSkinning();
		BufferManager.instance.ReturnBuffer(GetInstanceID());
		OnDisableBatch();
	}

	public void OnPreviewUpdate()
	{
	}

	private static bool IsInGUIUpdateLoop()
	{
		return Event.current != null;
	}

	private void Deform()
	{
		CacheCurrentSprite(m_AutoRebind);
		if (isValid && base.isActiveAndEnabled && (alwaysUpdate || spriteRenderer.isVisible))
		{
			int num = this.CalculateTransformHash();
			int num2 = sprite.GetVertexStreamSize() * sprite.GetVertexCount();
			int newVertexDeformationHash = GetNewVertexDeformationHash();
			if (num2 > 0 && (m_TransformsHash != num || m_VertexDeformationHash != newVertexDeformationHash))
			{
				NativeByteArray deformedVertices = GetDeformedVertices(num2);
				SpriteSkinUtility.Deform(sprite, base.gameObject.transform.worldToLocalMatrix, boneTransforms, deformedVertices.array);
				this.UpdateBounds(deformedVertices.array);
				InternalEngineBridge.SetDeformableBuffer(spriteRenderer, deformedVertices.array);
				m_TransformsHash = num;
				m_CurrentDeformSprite = GetSpriteInstanceID();
				m_VertexDeformationHash = newVertexDeformationHash;
			}
		}
		else if (!InternalEngineBridge.IsUsingDeformableBuffer(spriteRenderer, IntPtr.Zero))
		{
			DeactivateSkinning();
		}
	}

	private void CacheCurrentSprite(bool rebind)
	{
		if (m_CurrentDeformSprite == GetSpriteInstanceID())
		{
			return;
		}
		using (Profiling.cacheCurrentSprite.Auto())
		{
			DeactivateSkinning();
			m_CurrentDeformSprite = GetSpriteInstanceID();
			if (rebind && m_CurrentDeformSprite > 0 && rootBone != null)
			{
				if (!GetSpriteBonesTransforms(this, out var outTransform))
				{
					Debug.LogWarning("Rebind failed for " + base.name + ". Could not find all bones required by the Sprite: " + sprite.name + ".");
				}
				boneTransforms = outTransform;
			}
			UpdateSpriteDeform();
			CacheValidFlag();
			m_TransformsHash = 0;
		}
	}

	private void UpdateSpriteDeform()
	{
		if (sprite == null)
		{
			m_SpriteUVs = NativeCustomSlice<Vector2>.Default();
			m_SpriteVertices = NativeCustomSlice<Vector3>.Default();
			m_SpriteTangents = NativeCustomSlice<Vector4>.Default();
			m_SpriteBoneWeights = NativeCustomSlice<BoneWeight>.Default();
			m_SpriteBindPoses = NativeCustomSlice<Matrix4x4>.Default();
			m_SpriteHasTangents = false;
			m_SpriteVertexStreamSize = 0;
			m_SpriteVertexCount = 0;
			m_SpriteTangentVertexOffset = 0;
		}
		else
		{
			m_SpriteUVs = new NativeCustomSlice<Vector2>(sprite.GetVertexAttribute<Vector2>(VertexAttribute.TexCoord0));
			m_SpriteVertices = new NativeCustomSlice<Vector3>(sprite.GetVertexAttribute<Vector3>(VertexAttribute.Position));
			m_SpriteTangents = new NativeCustomSlice<Vector4>(sprite.GetVertexAttribute<Vector4>(VertexAttribute.Tangent));
			m_SpriteBoneWeights = new NativeCustomSlice<BoneWeight>(sprite.GetVertexAttribute<BoneWeight>(VertexAttribute.BlendWeight));
			m_SpriteBindPoses = new NativeCustomSlice<Matrix4x4>(sprite.GetBindPoses());
			m_SpriteHasTangents = sprite.HasVertexAttribute(VertexAttribute.Tangent);
			m_SpriteVertexStreamSize = sprite.GetVertexStreamSize();
			m_SpriteVertexCount = sprite.GetVertexCount();
			m_SpriteTangentVertexOffset = sprite.GetVertexStreamOffset(VertexAttribute.Tangent);
		}
		SpriteSkinComposite.instance.CopyToSpriteSkinData(this);
	}

	internal void CopyToSpriteSkinData(ref SpriteSkinData data, int spriteSkinIndex)
	{
		CacheBoneTransformIds();
		CacheCurrentSprite(m_AutoRebind);
		data.vertices = m_SpriteVertices;
		data.boneWeights = m_SpriteBoneWeights;
		data.bindPoses = m_SpriteBindPoses;
		data.tangents = m_SpriteTangents;
		data.hasTangents = m_SpriteHasTangents;
		data.spriteVertexStreamSize = m_SpriteVertexStreamSize;
		data.spriteVertexCount = m_SpriteVertexCount;
		data.tangentVertexOffset = m_SpriteTangentVertexOffset;
		data.transformId = m_TransformId;
		data.boneTransformId = m_BoneTransformIdNativeSlice;
		m_DataIndex = spriteSkinIndex;
	}

	internal unsafe bool NeedUpdateCompositeCache()
	{
		IntPtr intPtr = new IntPtr(sprite.GetVertexAttribute<Vector2>(VertexAttribute.TexCoord0).GetUnsafeReadOnlyPtr());
		bool num = m_SpriteUVs.data != intPtr;
		if (num)
		{
			UpdateSpriteDeform();
		}
		return num;
	}

	private void CacheHierarchy()
	{
		using (Profiling.cacheHierarchy.Auto())
		{
			m_HierarchyCache.Clear();
			if (rootBone == null || !m_AutoRebind)
			{
				return;
			}
			int num = CountChildren(rootBone);
			m_HierarchyCache.EnsureCapacity(num + 1);
			CacheChildren(rootBone, m_HierarchyCache);
			foreach (KeyValuePair<int, List<TransformData>> item in m_HierarchyCache)
			{
				if (item.Value.Count != 1)
				{
					int count = item.Value.Count;
					for (int i = 0; i < count; i++)
					{
						TransformData value = item.Value[i];
						value.fullName = GenerateTransformPath(rootBone, value.transform);
						item.Value[i] = value;
					}
				}
			}
		}
	}

	private static void CacheChildren(Transform current, Dictionary<int, List<TransformData>> cache)
	{
		int hashCode = current.name.GetHashCode();
		TransformData item = new TransformData
		{
			fullName = string.Empty,
			transform = current
		};
		if (cache.ContainsKey(hashCode))
		{
			cache[hashCode].Add(item);
		}
		else
		{
			cache.Add(hashCode, new List<TransformData>(1) { item });
		}
		for (int i = 0; i < current.childCount; i++)
		{
			CacheChildren(current.GetChild(i), cache);
		}
	}

	private static string GenerateTransformPath(Transform rootBone, Transform child)
	{
		string text = child.name;
		if (child == rootBone)
		{
			return text;
		}
		Transform parent = child.parent;
		do
		{
			text = parent.name + "/" + text;
			parent = parent.parent;
		}
		while (parent != rootBone && parent != null);
		return text;
	}

	internal static bool GetSpriteBonesTransforms(SpriteSkin spriteSkin, out Transform[] outTransform)
	{
		Transform obj = spriteSkin.rootBone;
		SpriteBone[] bones = spriteSkin.sprite.GetBones();
		if (obj == null)
		{
			throw new ArgumentException("rootBone parameter cannot be null");
		}
		if (bones == null)
		{
			throw new ArgumentException("spriteBones parameter cannot be null");
		}
		outTransform = new Transform[bones.Length];
		Bone[] componentsInChildren = obj.GetComponentsInChildren<Bone>();
		if (componentsInChildren != null && componentsInChildren.Length >= bones.Length)
		{
			using (Profiling.getSpriteBonesTransformFromGuid.Auto())
			{
				int i;
				for (i = 0; i < bones.Length; i++)
				{
					string boneHash = bones[i].guid;
					Bone bone = Array.Find(componentsInChildren, (Bone x) => x.guid == boneHash);
					if (bone == null)
					{
						break;
					}
					outTransform[i] = bone.transform;
				}
				if (i >= bones.Length)
				{
					return true;
				}
			}
		}
		Dictionary<int, List<TransformData>> hierarchyCache = spriteSkin.m_HierarchyCache;
		if (hierarchyCache.Count == 0)
		{
			spriteSkin.CacheHierarchy();
		}
		return GetSpriteBonesTransformFromPath(bones, hierarchyCache, outTransform);
	}

	private static bool GetSpriteBonesTransformFromPath(SpriteBone[] spriteBones, Dictionary<int, List<TransformData>> hierarchyCache, Transform[] outNewBoneTransform)
	{
		using (Profiling.getSpriteBonesTransformFromPath.Auto())
		{
			string[] array = null;
			bool result = true;
			for (int i = 0; i < spriteBones.Length; i++)
			{
				int hashCode = spriteBones[i].name.GetHashCode();
				if (!hierarchyCache.TryGetValue(hashCode, out var value))
				{
					outNewBoneTransform[i] = null;
					result = false;
					continue;
				}
				if (value.Count == 1)
				{
					outNewBoneTransform[i] = value[0].transform;
					continue;
				}
				if (array == null)
				{
					array = new string[spriteBones.Length];
				}
				if (array[i] == null)
				{
					CalculateBoneTransformsPath(i, spriteBones, array);
				}
				int j;
				for (j = 0; j < value.Count; j++)
				{
					if (value[j].fullName.Contains(array[i]))
					{
						outNewBoneTransform[i] = value[j].transform;
						break;
					}
				}
				if (j >= value.Count)
				{
					outNewBoneTransform[i] = null;
					result = false;
				}
			}
			return result;
		}
	}

	private static void CalculateBoneTransformsPath(int index, SpriteBone[] spriteBones, string[] paths)
	{
		SpriteBone spriteBone = spriteBones[index];
		int parentId = spriteBone.parentId;
		string text = spriteBone.name;
		if (parentId != -1)
		{
			if (paths[parentId] == null)
			{
				CalculateBoneTransformsPath(spriteBone.parentId, spriteBones, paths);
			}
			paths[index] = paths[parentId] + "/" + text;
		}
		else
		{
			paths[index] = text;
		}
	}

	internal void DeactivateSkinning()
	{
		if (m_SpriteRenderer != null)
		{
			Sprite sprite = m_SpriteRenderer.sprite;
			if (sprite != null)
			{
				InternalEngineBridge.SetLocalAABB(m_SpriteRenderer, sprite.bounds);
			}
			m_SpriteRenderer.DeactivateDeformableBuffer();
		}
		m_TransformsHash = 0;
	}

	internal void ResetSprite()
	{
		m_CurrentDeformSprite = 0;
		CacheValidFlag();
	}

	private static int CountChildren(Transform transform)
	{
		int childCount = transform.childCount;
		int num = childCount;
		for (int i = 0; i < childCount; i++)
		{
			num += CountChildren(transform.GetChild(i));
		}
		return num;
	}

	private static int GetNewVertexDeformationHash()
	{
		return Time.frameCount;
	}
}
