namespace Unity.Properties;

internal readonly struct IndexedCollectionPropertyBagEnumerable<TContainer>(IIndexedCollectionPropertyBagEnumerator<TContainer> impl, TContainer container)
{
	private readonly IIndexedCollectionPropertyBagEnumerator<TContainer> m_Impl = impl;

	private readonly TContainer m_Container = container;

	public IndexedCollectionPropertyBagEnumerator<TContainer> GetEnumerator()
	{
		return new IndexedCollectionPropertyBagEnumerator<TContainer>(m_Impl, m_Container);
	}
}
