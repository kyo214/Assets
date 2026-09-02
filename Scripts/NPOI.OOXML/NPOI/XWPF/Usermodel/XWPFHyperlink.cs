namespace NPOI.XWPF.UserModel;

public class XWPFHyperlink
{
	private string id;

	private string url;

	public string Id => id;

	public string URL => url;

	public XWPFHyperlink(string id, string url)
	{
		this.id = id;
		this.url = url;
	}
}
