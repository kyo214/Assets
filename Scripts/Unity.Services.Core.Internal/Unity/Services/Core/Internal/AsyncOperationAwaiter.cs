using System;
using System.Runtime.CompilerServices;

namespace Unity.Services.Core.Internal;

internal struct AsyncOperationAwaiter(IAsyncOperation asyncOperation) : IAsyncOperationAwaiter, ICriticalNotifyCompletion, INotifyCompletion
{
	private IAsyncOperation m_Operation = asyncOperation;

	public bool IsCompleted => m_Operation.IsDone;

	public void OnCompleted(Action continuation)
	{
		m_Operation.Completed += (IAsyncOperation operation) =>
		{
			continuation();
		};
	}

	public void UnsafeOnCompleted(Action continuation)
	{
		m_Operation.Completed += (IAsyncOperation operation) =>
		{
			continuation();
		};
	}

	public void GetResult()
	{
		if (m_Operation.Status == AsyncOperationStatus.Failed || m_Operation.Status == AsyncOperationStatus.Cancelled)
		{
			throw m_Operation.Exception;
		}
	}
}
internal struct AsyncOperationAwaiter<T>(IAsyncOperation<T> asyncOperation) : IAsyncOperationAwaiter<T>, ICriticalNotifyCompletion, INotifyCompletion
{
	private IAsyncOperation<T> m_Operation = asyncOperation;

	public bool IsCompleted => m_Operation.IsDone;

	public void OnCompleted(Action continuation)
	{
		m_Operation.Completed += (IAsyncOperation<T> obj) =>
		{
			continuation();
		};
	}

	public void UnsafeOnCompleted(Action continuation)
	{
		m_Operation.Completed += (IAsyncOperation<T> obj) =>
		{
			continuation();
		};
	}

	public T GetResult()
	{
		if (m_Operation.Status == AsyncOperationStatus.Failed || m_Operation.Status == AsyncOperationStatus.Cancelled)
		{
			throw m_Operation.Exception;
		}
		return m_Operation.Result;
	}
}
