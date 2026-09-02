namespace BansheeGz.BGDatabase;

public interface BGStorageI<T> : BGStorable<T>
{
	T[] CopyRawValues();
}
