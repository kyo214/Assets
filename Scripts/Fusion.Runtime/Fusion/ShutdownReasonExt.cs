using System.Collections.Generic;

namespace Fusion;

internal static class ShutdownReasonExt
{
	private static readonly Dictionary<ShutdownReason, string> ShutdownReasonDescription = new Dictionary<ShutdownReason, string>
	{
		{
			ShutdownReason.Ok,
			"Success"
		},
		{
			ShutdownReason.Error,
			"Unspecified Error"
		},
		{
			ShutdownReason.IncompatibleConfiguration,
			"Make sure to use a Fusion-type Photon Application ID (Fusion plug-in not found)"
		},
		{
			ShutdownReason.ServerInRoom,
			"A Fusion Server/Host is already in the Game Session"
		},
		{
			ShutdownReason.DisconnectedByPluginLogic,
			"Fusion Plugin has disconnected the local peer"
		},
		{
			ShutdownReason.GameClosed,
			"Game Session is Closed"
		},
		{
			ShutdownReason.GameNotFound,
			"No Game Session was found"
		},
		{
			ShutdownReason.MaxCcuReached,
			"Max CCU Reached for the used Photon Application Id"
		},
		{
			ShutdownReason.InvalidRegion,
			"Invalid Region Id"
		},
		{
			ShutdownReason.GameIdAlreadyExists,
			"A Game Session already exists with the especified Session Name"
		},
		{
			ShutdownReason.GameIsFull,
			"Game Session is Full"
		},
		{
			ShutdownReason.InvalidAuthentication,
			"Invalid Authentication Credentials"
		},
		{
			ShutdownReason.CustomAuthenticationFailed,
			"Custom Authentication has failed or denied"
		},
		{
			ShutdownReason.AuthenticationTicketExpired,
			"Authentication Ticket has expired"
		},
		{
			ShutdownReason.PhotonCloudTimeout,
			"Connection with the Photon Cloud has timed out"
		},
		{
			ShutdownReason.AlreadyRunning,
			"Fusion Simulation is already Running."
		},
		{
			ShutdownReason.InvalidArguments,
			"Invalid StartGame Arguments"
		},
		{
			ShutdownReason.HostMigration,
			"Host Migration"
		},
		{
			ShutdownReason.ConnectionTimeout,
			"Connection with the Fusion Server has timed out"
		},
		{
			ShutdownReason.ConnectionRefused,
			"Connection with the Fusion Server has been refused"
		}
	};

	public static string ErrorDescription(this ShutdownReason shutdownReason)
	{
		string value;
		return ShutdownReasonDescription.TryGetValue(shutdownReason, out value) ? value : shutdownReason.ToString();
	}
}
