using Unity.Services.Core.Internal;

namespace Unity.Services.Core.Environments.Internal;

internal class Environments : IEnvironments, IServiceComponent
{
	private string m_Current;

	public string Current
	{
		get
		{
			return m_Current;
		}
		internal set
		{
			m_Current = value;
		}
	}
}
