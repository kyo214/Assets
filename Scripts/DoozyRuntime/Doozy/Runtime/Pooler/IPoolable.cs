using System;

namespace Doozy.Runtime.Pooler;

public interface IPoolable : IDisposable
{
	bool inPool { get; set; }

	void Reset();

	void Recycle(bool debug = false);
}
