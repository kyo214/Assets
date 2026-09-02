namespace Fusion;

public enum ShutdownReason
{
	Ok = 0,
	Error = 1,
	IncompatibleConfiguration = 2,
	ServerInRoom = 3,
	DisconnectedByPluginLogic = 4,
	GameClosed = 5,
	GameNotFound = 6,
	MaxCcuReached = 7,
	InvalidRegion = 8,
	GameIdAlreadyExists = 9,
	GameIsFull = 10,
	InvalidAuthentication = 11,
	CustomAuthenticationFailed = 12,
	AuthenticationTicketExpired = 13,
	PhotonCloudTimeout = 14,
	AlreadyRunning = 15,
	InvalidArguments = 16,
	HostMigration = 17,
	ConnectionTimeout = 18,
	ConnectionRefused = 19
}
