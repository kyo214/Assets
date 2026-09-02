using System;

namespace BansheeGz.BGDatabase;

public interface BGKeyStorageKeyI : IEquatable<BGKeyStorageKeyI>
{
	bool IsValueEquals(object value, int index);

	BGKeyStorageKeyI Clone();
}
