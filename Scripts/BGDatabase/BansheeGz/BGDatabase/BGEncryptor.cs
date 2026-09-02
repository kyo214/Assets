using System;

namespace BansheeGz.BGDatabase;

public interface BGEncryptor
{
	ArraySegment<byte> Encrypt(ArraySegment<byte> data, string config);

	ArraySegment<byte> Decrypt(ArraySegment<byte> data, string config);
}
