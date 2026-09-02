using System;

namespace BansheeGz.BGDatabase;

public interface BGFieldEnumI
{
	Type UnderlyingType { get; }

	Type EnumType { get; set; }
}
