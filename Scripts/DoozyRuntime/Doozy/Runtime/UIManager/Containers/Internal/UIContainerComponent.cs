using System.Collections.Generic;
using Doozy.Runtime.Common.Attributes;

namespace Doozy.Runtime.UIManager.Containers.Internal;

public abstract class UIContainerComponent<T> : UIContainer where T : UIContainer
{
	[ClearOnReload]
	public static HashSet<T> database { get; } = new HashSet<T>();

	public T component { get; private set; }

	protected override void Awake()
	{
		HashSet<T> hashSet = database;
		T item = (component = GetComponent<T>());
		hashSet.Add(item);
		base.Awake();
	}

	protected override void OnEnable()
	{
		database.Remove(null);
		base.OnEnable();
	}

	protected override void OnDestroy()
	{
		database.Remove(component);
		database.Remove(null);
		base.OnDestroy();
	}
}
