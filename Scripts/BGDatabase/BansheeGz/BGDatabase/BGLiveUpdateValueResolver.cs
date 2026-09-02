namespace BansheeGz.BGDatabase;

public interface BGLiveUpdateValueResolver
{
	string Resolve(BGField field, string value);
}
