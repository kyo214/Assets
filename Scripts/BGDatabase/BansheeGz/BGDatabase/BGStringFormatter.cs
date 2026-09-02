namespace BansheeGz.BGDatabase;

public interface BGStringFormatter<T>
{
	T FromString(string value);

	string ToString(T value);
}
