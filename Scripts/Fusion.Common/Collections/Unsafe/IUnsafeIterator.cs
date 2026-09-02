using System;
using System.Collections;
using System.Collections.Generic;

namespace Collections.Unsafe;

public interface IUnsafeIterator<T> : IEnumerator<T>, IEnumerator, IDisposable, IEnumerable<T>, IEnumerable where T : struct
{
}
