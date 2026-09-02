using System;

namespace BansheeGz.BGDatabase;

public interface BGConfigurableBinaryI
{
	byte[] ConfigToBytes();

	void ConfigFromBytes(ArraySegment<byte> config);
}
