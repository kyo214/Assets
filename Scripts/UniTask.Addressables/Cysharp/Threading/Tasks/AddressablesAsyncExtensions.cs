using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks.Sources;
using Cysharp.Threading.Tasks.Internal;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Cysharp.Threading.Tasks;

public static class AddressablesAsyncExtensions
{
	public struct AsyncOperationHandleAwaiter(AsyncOperationHandle handle) : ICriticalNotifyCompletion, INotifyCompletion
	{
		private AsyncOperationHandle handle = handle;

		private Action<AsyncOperationHandle> continuationAction = null;

		public bool IsCompleted => handle.IsDone;

		public void GetResult()
		{
			if (continuationAction != null)
			{
				handle.Completed -= continuationAction;
				continuationAction = null;
			}
			if (handle.Status == AsyncOperationStatus.Failed)
			{
				Exception operationException = handle.OperationException;
				handle = default;
				ExceptionDispatchInfo.Capture(operationException).Throw();
			}
			_ = handle.Result;
			handle = default;
		}

		public void OnCompleted(Action continuation)
		{
			UnsafeOnCompleted(continuation);
		}

		public void UnsafeOnCompleted(Action continuation)
		{
			Error.ThrowWhenContinuationIsAlreadyRegistered(continuationAction);
			continuationAction = PooledDelegate<AsyncOperationHandle>.Create(continuation);
			handle.Completed += continuationAction;
		}
	}

	private sealed class AsyncOperationHandleConfiguredSource : IUniTaskSource, IValueTaskSource, IPlayerLoopItem, ITaskPoolNode<AsyncOperationHandleConfiguredSource>
	{
		private static TaskPool<AsyncOperationHandleConfiguredSource> pool;

		private AsyncOperationHandleConfiguredSource nextNode;

		private readonly Action<AsyncOperationHandle> completedCallback;

		private AsyncOperationHandle handle;

		private CancellationToken cancellationToken;

		private CancellationTokenRegistration cancellationTokenRegistration;

		private IProgress<float> progress;

		private bool autoReleaseWhenCanceled;

		private bool cancelImmediately;

		private bool completed;

		private UniTaskCompletionSourceCore<AsyncUnit> core;

		public ref AsyncOperationHandleConfiguredSource NextNode => ref nextNode;

		static AsyncOperationHandleConfiguredSource()
		{
			TaskPool.RegisterSizeGetter(typeof(AsyncOperationHandleConfiguredSource), () => pool.Size);
		}

		private AsyncOperationHandleConfiguredSource()
		{
			completedCallback = HandleCompleted;
		}

		public static IUniTaskSource Create(AsyncOperationHandle handle, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, bool cancelImmediately, bool autoReleaseWhenCanceled, out short token)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
			}
			if (!pool.TryPop(out var result))
			{
				result = new AsyncOperationHandleConfiguredSource();
			}
			result.handle = handle;
			result.progress = progress;
			result.cancellationToken = cancellationToken;
			result.cancelImmediately = cancelImmediately;
			result.autoReleaseWhenCanceled = autoReleaseWhenCanceled;
			result.completed = false;
			if (cancelImmediately && cancellationToken.CanBeCanceled)
			{
				result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext((object state) =>
				{
					AsyncOperationHandleConfiguredSource asyncOperationHandleConfiguredSource = (AsyncOperationHandleConfiguredSource)state;
					if (asyncOperationHandleConfiguredSource.autoReleaseWhenCanceled && asyncOperationHandleConfiguredSource.handle.IsValid())
					{
						Addressables.Release(asyncOperationHandleConfiguredSource.handle);
					}
					asyncOperationHandleConfiguredSource.core.TrySetCanceled(asyncOperationHandleConfiguredSource.cancellationToken);
				}, result);
			}
			PlayerLoopHelper.AddAction(timing, result);
			handle.Completed += result.completedCallback;
			token = result.core.Version;
			return result;
		}

		private void HandleCompleted(AsyncOperationHandle _)
		{
			if (handle.IsValid())
			{
				handle.Completed -= completedCallback;
			}
			if (completed)
			{
				return;
			}
			completed = true;
			if (cancellationToken.IsCancellationRequested)
			{
				if (autoReleaseWhenCanceled && handle.IsValid())
				{
					Addressables.Release(handle);
				}
				core.TrySetCanceled(cancellationToken);
			}
			else if (handle.Status == AsyncOperationStatus.Failed)
			{
				core.TrySetException(handle.OperationException);
			}
			else
			{
				core.TrySetResult(AsyncUnit.Default);
			}
		}

		public void GetResult(short token)
		{
			try
			{
				core.GetResult(token);
			}
			finally
			{
				if (!cancelImmediately || !cancellationToken.IsCancellationRequested)
				{
					TryReturn();
				}
			}
		}

		public UniTaskStatus GetStatus(short token)
		{
			return core.GetStatus(token);
		}

		public UniTaskStatus UnsafeGetStatus()
		{
			return core.UnsafeGetStatus();
		}

		public void OnCompleted(Action<object> continuation, object state, short token)
		{
			core.OnCompleted(continuation, state, token);
		}

		public bool MoveNext()
		{
			if (completed)
			{
				return false;
			}
			if (cancellationToken.IsCancellationRequested)
			{
				completed = true;
				if (autoReleaseWhenCanceled && handle.IsValid())
				{
					Addressables.Release(handle);
				}
				core.TrySetCanceled(cancellationToken);
				return false;
			}
			if (progress != null && handle.IsValid())
			{
				progress.Report(handle.GetDownloadStatus().Percent);
			}
			return true;
		}

		private bool TryReturn()
		{
			core.Reset();
			handle = default;
			progress = null;
			cancellationToken = default;
			cancellationTokenRegistration.Dispose();
			return pool.TryPush(this);
		}
	}

	private sealed class AsyncOperationHandleConfiguredSource<T> : IUniTaskSource<T>, IUniTaskSource, IValueTaskSource, IValueTaskSource<T>, IPlayerLoopItem, ITaskPoolNode<AsyncOperationHandleConfiguredSource<T>>
	{
		private static TaskPool<AsyncOperationHandleConfiguredSource<T>> pool;

		private AsyncOperationHandleConfiguredSource<T> nextNode;

		private readonly Action<AsyncOperationHandle<T>> completedCallback;

		private AsyncOperationHandle<T> handle;

		private CancellationToken cancellationToken;

		private CancellationTokenRegistration cancellationTokenRegistration;

		private IProgress<float> progress;

		private bool autoReleaseWhenCanceled;

		private bool cancelImmediately;

		private bool completed;

		private UniTaskCompletionSourceCore<T> core;

		public ref AsyncOperationHandleConfiguredSource<T> NextNode => ref nextNode;

		static AsyncOperationHandleConfiguredSource()
		{
			TaskPool.RegisterSizeGetter(typeof(AsyncOperationHandleConfiguredSource<T>), () => pool.Size);
		}

		private AsyncOperationHandleConfiguredSource()
		{
			completedCallback = HandleCompleted;
		}

		public static IUniTaskSource<T> Create(AsyncOperationHandle<T> handle, PlayerLoopTiming timing, IProgress<float> progress, CancellationToken cancellationToken, bool cancelImmediately, bool autoReleaseWhenCanceled, out short token)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return AutoResetUniTaskCompletionSource<T>.CreateFromCanceled(cancellationToken, out token);
			}
			if (!pool.TryPop(out var result))
			{
				result = new AsyncOperationHandleConfiguredSource<T>();
			}
			result.handle = handle;
			result.cancellationToken = cancellationToken;
			result.completed = false;
			result.progress = progress;
			result.autoReleaseWhenCanceled = autoReleaseWhenCanceled;
			result.cancelImmediately = cancelImmediately;
			if (cancelImmediately && cancellationToken.CanBeCanceled)
			{
				result.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext((object state) =>
				{
					AsyncOperationHandleConfiguredSource<T> asyncOperationHandleConfiguredSource = (AsyncOperationHandleConfiguredSource<T>)state;
					if (asyncOperationHandleConfiguredSource.autoReleaseWhenCanceled && asyncOperationHandleConfiguredSource.handle.IsValid())
					{
						Addressables.Release(asyncOperationHandleConfiguredSource.handle);
					}
					asyncOperationHandleConfiguredSource.core.TrySetCanceled(asyncOperationHandleConfiguredSource.cancellationToken);
				}, result);
			}
			PlayerLoopHelper.AddAction(timing, result);
			handle.Completed += result.completedCallback;
			token = result.core.Version;
			return result;
		}

		private void HandleCompleted(AsyncOperationHandle<T> argHandle)
		{
			if (handle.IsValid())
			{
				handle.Completed -= completedCallback;
			}
			if (completed)
			{
				return;
			}
			completed = true;
			if (cancellationToken.IsCancellationRequested)
			{
				if (autoReleaseWhenCanceled && handle.IsValid())
				{
					Addressables.Release(handle);
				}
				core.TrySetCanceled(cancellationToken);
			}
			else if (argHandle.Status == AsyncOperationStatus.Failed)
			{
				core.TrySetException(argHandle.OperationException);
			}
			else
			{
				core.TrySetResult(argHandle.Result);
			}
		}

		public T GetResult(short token)
		{
			try
			{
				return core.GetResult(token);
			}
			finally
			{
				if (!cancelImmediately || !cancellationToken.IsCancellationRequested)
				{
					TryReturn();
				}
			}
		}

		void IUniTaskSource.GetResult(short token)
		{
			GetResult(token);
		}

		public UniTaskStatus GetStatus(short token)
		{
			return core.GetStatus(token);
		}

		public UniTaskStatus UnsafeGetStatus()
		{
			return core.UnsafeGetStatus();
		}

		public void OnCompleted(Action<object> continuation, object state, short token)
		{
			core.OnCompleted(continuation, state, token);
		}

		public bool MoveNext()
		{
			if (completed)
			{
				return false;
			}
			if (cancellationToken.IsCancellationRequested)
			{
				completed = true;
				if (autoReleaseWhenCanceled && handle.IsValid())
				{
					Addressables.Release(handle);
				}
				core.TrySetCanceled(cancellationToken);
				return false;
			}
			if (progress != null && handle.IsValid())
			{
				progress.Report(handle.GetDownloadStatus().Percent);
			}
			return true;
		}

		private bool TryReturn()
		{
			core.Reset();
			handle = default;
			progress = null;
			cancellationToken = default;
			cancellationTokenRegistration.Dispose();
			return pool.TryPush(this);
		}
	}

	public static UniTask.Awaiter GetAwaiter(this AsyncOperationHandle handle)
	{
		return handle.ToUniTask().GetAwaiter();
	}

	public static UniTask WithCancellation(this AsyncOperationHandle handle, CancellationToken cancellationToken, bool cancelImmediately = false, bool autoReleaseWhenCanceled = false)
	{
		return handle.ToUniTask(null, PlayerLoopTiming.Update, cancellationToken, cancelImmediately, autoReleaseWhenCanceled);
	}

	public static UniTask ToUniTask(this AsyncOperationHandle handle, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false, bool autoReleaseWhenCanceled = false)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return UniTask.FromCanceled(cancellationToken);
		}
		if (!handle.IsValid())
		{
			return UniTask.CompletedTask;
		}
		if (handle.IsDone)
		{
			if (handle.Status == AsyncOperationStatus.Failed)
			{
				return UniTask.FromException(handle.OperationException);
			}
			return UniTask.CompletedTask;
		}
		short token;
		return new UniTask(AsyncOperationHandleConfiguredSource.Create(handle, timing, progress, cancellationToken, cancelImmediately, autoReleaseWhenCanceled, out token), token);
	}

	public static UniTask<T>.Awaiter GetAwaiter<T>(this AsyncOperationHandle<T> handle)
	{
		return handle.ToUniTask().GetAwaiter();
	}

	public static UniTask<T> WithCancellation<T>(this AsyncOperationHandle<T> handle, CancellationToken cancellationToken, bool cancelImmediately = false, bool autoReleaseWhenCanceled = false)
	{
		return handle.ToUniTask(null, PlayerLoopTiming.Update, cancellationToken, cancelImmediately, autoReleaseWhenCanceled);
	}

	public static UniTask<T> ToUniTask<T>(this AsyncOperationHandle<T> handle, IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false, bool autoReleaseWhenCanceled = false)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return UniTask.FromCanceled<T>(cancellationToken);
		}
		if (!handle.IsValid())
		{
			throw new Exception("Attempting to use an invalid operation handle");
		}
		if (handle.IsDone)
		{
			if (handle.Status == AsyncOperationStatus.Failed)
			{
				return UniTask.FromException<T>(handle.OperationException);
			}
			return UniTask.FromResult(handle.Result);
		}
		short token;
		return new UniTask<T>(AsyncOperationHandleConfiguredSource<T>.Create(handle, timing, progress, cancellationToken, cancelImmediately, autoReleaseWhenCanceled, out token), token);
	}
}
