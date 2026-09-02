namespace NPOI.POIFS.Properties;

public interface Child
{
	Child PreviousChild { get; set; }

	Child NextChild { get; set; }
}
