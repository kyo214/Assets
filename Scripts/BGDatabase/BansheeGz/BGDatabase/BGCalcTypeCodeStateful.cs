namespace BansheeGz.BGDatabase;

public interface BGCalcTypeCodeStateful
{
	void ReadState(BGBinaryReader reader);

	void WriteState(BGBinaryWriter writer);

	void ReadState(string state);

	string WriteState();
}
