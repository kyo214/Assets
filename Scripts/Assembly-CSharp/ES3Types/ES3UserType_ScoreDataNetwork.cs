using UnityEngine.Scripting;

namespace ES3Types;

[Preserve]
[ES3Properties(new string[] { "KillZombieCount", "KillEliteCount", "PuzzleSolved", "DeathCount", "Life", "ReviveOtherPlayer" })]
public class ES3UserType_ScoreDataNetwork : ES3Type
{
	public static ES3Type Instance;

	public ES3UserType_ScoreDataNetwork()
		: base(typeof(ScoreDataNetwork))
	{
		Instance = this;
		priority = 1;
	}

	public override void Write(object obj, ES3Writer writer)
	{
		ScoreDataNetwork scoreDataNetwork = (ScoreDataNetwork)obj;
		writer.WriteProperty("KillZombieCount", scoreDataNetwork.KillZombieCount, ES3Type_short.Instance);
		writer.WriteProperty("KillEliteCount", scoreDataNetwork.KillEliteCount, ES3Type_byte.Instance);
		writer.WriteProperty("PuzzleSolved", scoreDataNetwork.PuzzleSolved, ES3Type_byte.Instance);
		writer.WriteProperty("DeathCount", scoreDataNetwork.DeathCount, ES3Type_byte.Instance);
		writer.WriteProperty("Life", scoreDataNetwork.Life, ES3Type_byte.Instance);
		writer.WriteProperty("ReviveOtherPlayer", scoreDataNetwork.ReviveOtherPlayer, ES3Type_byte.Instance);
	}

	public override object Read<T>(ES3Reader reader)
	{
		ScoreDataNetwork scoreDataNetwork = default;
		string text;
		while ((text = reader.ReadPropertyName()) != null)
		{
			switch (text)
			{
			case "KillZombieCount":
				scoreDataNetwork.KillZombieCount = reader.Read<short>(ES3Type_short.Instance);
				break;
			case "KillEliteCount":
				scoreDataNetwork.KillEliteCount = reader.Read<byte>(ES3Type_byte.Instance);
				break;
			case "PuzzleSolved":
				scoreDataNetwork.PuzzleSolved = reader.Read<byte>(ES3Type_byte.Instance);
				break;
			case "DeathCount":
				scoreDataNetwork.DeathCount = reader.Read<byte>(ES3Type_byte.Instance);
				break;
			case "Life":
				scoreDataNetwork.Life = reader.Read<byte>(ES3Type_byte.Instance);
				break;
			case "ReviveOtherPlayer":
				scoreDataNetwork.ReviveOtherPlayer = reader.Read<byte>(ES3Type_byte.Instance);
				break;
			default:
				reader.Skip();
				break;
			}
		}
		return scoreDataNetwork;
	}
}
