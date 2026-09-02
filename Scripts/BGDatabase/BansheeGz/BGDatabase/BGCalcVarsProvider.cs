namespace BansheeGz.BGDatabase;

public interface BGCalcVarsProvider
{
	bool TryGet(BGId variableId, out object value);
}
