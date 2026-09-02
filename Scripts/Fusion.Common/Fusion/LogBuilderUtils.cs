using System.Text;

namespace Fusion;

public static class LogBuilderUtils
{
	internal interface ICombinedLogBuilder
	{
	}

	internal unsafe readonly struct CombinedLogBuilder_Value_Ptr<T0, T1>(T0 builder0, T1* builder1) : ILogBuilder, ICombinedLogBuilder where T0 : class, ILogBuilder where T1 : unmanaged, ILogBuilder
	{
		internal readonly T0 _builder0 = builder0;

		internal unsafe readonly T1* _builder1 = builder1;

		public unsafe void BuildLogMessage(StringBuilder builder, string message, in LogOptions options)
		{
			if (_builder0 != null)
			{
				_builder0.BuildLogMessage(builder, "", in options);
			}
			if (_builder1 != null)
			{
				_builder1->BuildLogMessage(builder, message, in options);
			}
		}

		void ILogBuilder.BuildLogMessage(StringBuilder builder, string message, in LogOptions options)
		{
			BuildLogMessage(builder, message, in options);
		}
	}

	internal unsafe static CombinedLogBuilder_Value_Ptr<T0, T1> Combine<T0, T1>(T0 builder0, T1* builder1) where T0 : class, ILogBuilder where T1 : unmanaged, ILogBuilder
	{
		return new CombinedLogBuilder_Value_Ptr<T0, T1>(builder0, builder1);
	}
}
