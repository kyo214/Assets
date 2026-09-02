namespace BansheeGz.BGDatabase;

public interface BGStorable<T>
{
	void SetStoredValue(int entityIndex, T value);

	T GetStoredValue(int entityIndex);
}
