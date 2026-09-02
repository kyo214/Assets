namespace UnityEngine.U2D.Common.UTess;

internal sealed class ArraySliceDebugView<T> where T : struct
{
	private ArraySlice<T> m_Slice;

	public T[] Items => m_Slice.ToArray();

	public ArraySliceDebugView(ArraySlice<T> slice)
	{
		m_Slice = slice;
	}
}
