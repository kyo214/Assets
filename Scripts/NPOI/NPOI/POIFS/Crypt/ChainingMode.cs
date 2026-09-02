namespace NPOI.POIFS.Crypt;

public class ChainingMode
{
	public static readonly ChainingMode ecb = new ChainingMode("ECB", 1);

	public static readonly ChainingMode cbc = new ChainingMode("CBC", 2);

	public static readonly ChainingMode cfb = new ChainingMode("CFB8", 3);

	public string jceId { get; set; }

	public int ecmaId { get; set; }

	public ChainingMode(string jceId, int ecmaId)
	{
		this.jceId = jceId;
		this.ecmaId = ecmaId;
	}
}
