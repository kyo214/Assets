namespace System.Threading.Tasks;

internal sealed class BeginEndAwaitableAdapter : RendezvousAwaitable<IAsyncResult>
{
	public static readonly AsyncCallback Callback = (IAsyncResult asyncResult) =>
	{
		((BeginEndAwaitableAdapter)asyncResult.AsyncState).SetResult(asyncResult);
	};

	public BeginEndAwaitableAdapter()
	{
		base.RunContinuationsAsynchronously = false;
	}
}
