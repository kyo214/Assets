using System.Text;

namespace Fusion;

public interface ILogBuilder
{
	void BuildLogMessage(StringBuilder builder, string message, in LogOptions options);
}
