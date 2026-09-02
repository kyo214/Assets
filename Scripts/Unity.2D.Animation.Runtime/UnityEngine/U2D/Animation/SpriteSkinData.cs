namespace UnityEngine.U2D.Animation;

internal struct SpriteSkinData
{
	public NativeCustomSlice<Vector3> vertices;

	public NativeCustomSlice<BoneWeight> boneWeights;

	public NativeCustomSlice<Matrix4x4> bindPoses;

	public NativeCustomSlice<Vector4> tangents;

	public bool hasTangents;

	public int spriteVertexStreamSize;

	public int spriteVertexCount;

	public int tangentVertexOffset;

	public int deformVerticesStartPos;

	public int transformId;

	public NativeCustomSlice<int> boneTransformId;
}
