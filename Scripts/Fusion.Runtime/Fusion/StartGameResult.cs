using System;
using Fusion.Photon.Realtime.Async;

namespace Fusion;

public class StartGameResult
{
	public bool Ok => ShutdownReason == ShutdownReason.Ok;

	public ShutdownReason ShutdownReason { get; private set; }

	public string ErrorMessage { get; private set; }

	internal StartGameResult(ShutdownReason reason = ShutdownReason.Ok, string message = null)
	{
		ShutdownReason = reason;
		ErrorMessage = message ?? reason.ErrorDescription();
	}

	public override string ToString()
	{
		return string.Format("[{0}: {1}:{2}, {3}: {4}, {5}={6}]", "StartGameResult", "Ok", Ok, "ShutdownReason", ShutdownReason, "ErrorMessage", ErrorMessage);
	}

	internal static StartGameResult BuildStartGameResultFromException(Exception e)
	{
		ShutdownReason reason = ((e is StartGameException ex) ? ex.ShutdownReason : ((e is DisconnectException ex2) ? DisconnectCauseExt.ConvertToShutdownReason(ex2.Cause) : ((e is AuthenticationFailedException) ? ShutdownReason.CustomAuthenticationFailed : ((e is OperationException ex3) ? ErrorCodeExt.ConvertToShutdownReason(ex3.ErrorCode) : ((e is OperationStartException) ? ShutdownReason.Error : ((!(e is OperationTimeoutException) && !(e is TimeoutException)) ? ShutdownReason.Error : ShutdownReason.PhotonCloudTimeout))))));
		return new StartGameResult(reason, e.Message);
	}
}
