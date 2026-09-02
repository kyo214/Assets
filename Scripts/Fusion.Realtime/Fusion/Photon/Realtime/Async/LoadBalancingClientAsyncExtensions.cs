using System;
using System.Threading;
using System.Threading.Tasks;
using Fusion.Async;

namespace Fusion.Photon.Realtime.Async;

internal static class LoadBalancingClientAsyncExtensions
{
	private const int SERVICE_INTERVAL_MS = 10;

	public static Task ConnectToMasterAsync(this LoadBalancingClient client, AppSettings appSettings, bool createServiceTask = true)
	{
		if (client.State != ClientState.Disconnected && client.State != ClientState.PeerCreated)
		{
			return Task.FromException(new OperationStartException("Client still connected"));
		}
		if (!client.ConnectUsingSettings(appSettings))
		{
			return Task.FromException(new OperationStartException("Failed to start connecting"));
		}
		return client.CreateOpHandler(throwOnErrors: true, createServiceTask).Task;
	}

	public static Task ReconnectAndRejoinAsync(this LoadBalancingClient client, bool throwOnError = true, bool createServiceTask = true)
	{
		if (client.State != ClientState.Disconnected)
		{
			return Task.FromException(new OperationStartException("Client still connected"));
		}
		if (!client.ReconnectAndRejoin())
		{
			return Task.FromException(new OperationStartException("Failed to start reconnecting"));
		}
		return client.CreateOpHandler(throwOnError, createServiceTask).Task;
	}

	public static Task DisconnectAsync(this LoadBalancingClient client, bool createServiceTask = true)
	{
		if (client == null)
		{
			return Task.CompletedTask;
		}
		if (client.State == ClientState.Disconnected)
		{
			return Task.CompletedTask;
		}
		OperationHandler handler = client.CreateOpHandler(throwOnErrors: true, createServiceTask);
		PhotonConnectionCallbacks connectionCallbacks = handler.ConnectionCallbacks;
		connectionCallbacks.Disconnected = (Action<DisconnectCause>)Delegate.Combine(connectionCallbacks.Disconnected, (Action<DisconnectCause>)((DisconnectCause cause) =>
		{
			Log.Info($"Disconnected: {cause}");
			handler.SetResult(0);
		}));
		client.Disconnect();
		return handler.Task;
	}

	public static Task<short> CreateRoomAsync(this LoadBalancingClient client, EnterRoomParams enterRoomParams, bool throwOnError = true, bool createServiceTask = true)
	{
		if (!client.OpCreateRoom(enterRoomParams))
		{
			return Task.FromException<short>(new OperationStartException("Failed to send CreateRoom operation"));
		}
		return client.CreateOpHandler(throwOnError, createServiceTask).Task;
	}

	public static Task<short> CreateOrJoinRoomAsync(this LoadBalancingClient client, EnterRoomParams enterRoomParams, bool throwOnError = true, bool createServiceTask = true)
	{
		if (!client.OpJoinOrCreateRoom(enterRoomParams))
		{
			return Task.FromException<short>(new OperationStartException("Failed to send CreateRoom operation"));
		}
		return client.CreateOpHandler(throwOnError, createServiceTask).Task;
	}

	public static Task<short> JoinRoomAsync(this LoadBalancingClient client, EnterRoomParams enterRoomParams, bool throwOnError = true, bool createServiceTask = true)
	{
		if (!client.OpJoinRoom(enterRoomParams))
		{
			return Task.FromException<short>(new OperationStartException("Failed to send JoinRoom operation"));
		}
		return client.CreateOpHandler(throwOnError, createServiceTask).Task;
	}

	public static Task<short> JoinRandomOrCreateRoomAsync(this LoadBalancingClient client, OpJoinRandomRoomParams joinRandomRoomParams, EnterRoomParams enterRoomParams, bool throwOnError = true, bool createServiceTask = true)
	{
		if (!client.OpJoinRandomOrCreateRoom(joinRandomRoomParams, enterRoomParams))
		{
			return Task.FromException<short>(new OperationStartException("Failed to send JoinRandomOrCreateRoom operation"));
		}
		return client.CreateOpHandler(throwOnError, createServiceTask).Task;
	}

	public static Task<short> JoinRandomRoomAsync(this LoadBalancingClient client, OpJoinRandomRoomParams joinRandomRoomParams, bool throwOnError = true, bool createServiceTask = true)
	{
		if (!client.OpJoinRandomRoom(joinRandomRoomParams))
		{
			return Task.FromException<short>(new OperationStartException("Failed to send JoinRandomRoom operation"));
		}
		return client.CreateOpHandler(throwOnError, createServiceTask).Task;
	}

	public static Task<short> JoinLobbyAsync(this LoadBalancingClient client, TypedLobby lobby, bool throwOnError = true, bool createServiceTask = true)
	{
		if (!client.OpJoinLobby(lobby))
		{
			return Task.FromException<short>(new OperationStartException("Failed to send JoinLobby operation"));
		}
		return client.CreateOpHandler(throwOnError, createServiceTask).Task;
	}

	public static OperationHandler CreateOpHandler(this LoadBalancingClient client, bool throwOnErrors = true, bool createServiceTask = true)
	{
		OperationHandler handler = new OperationHandler(throwOnErrors);
		client.AddCallbackTarget(handler);
		TaskManager.ContinueWhenAll(new Task[1] { handler.Task }, (CancellationToken token) =>
		{
			client.RemoveCallbackTarget(handler);
			return Task.CompletedTask;
		}, handler.Token);
		if (createServiceTask)
		{
			client.Service_ClientUpdate(handler.Token, handler.CompletionSource);
		}
		return handler;
	}

	public static void Service_ClientUpdate(this LoadBalancingClient client, CancellationToken token, TaskCompletionSource<short> completionSource)
	{
		TaskManager.Service(() =>
		{
			try
			{
				if (!token.IsCancellationRequested)
				{
					client.Service();
				}
			}
			catch (Exception exception)
			{
				completionSource.TrySetException(exception);
			}
			return Task.FromResult(client.IsConnected);
		}, token, 10, "AsyncClientUpdate");
	}
}
