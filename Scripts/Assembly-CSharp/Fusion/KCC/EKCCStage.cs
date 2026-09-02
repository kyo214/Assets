namespace Fusion.KCC;

public enum EKCCStage
{
	None = 0,
	SetInputProperties = 1,
	SetDynamicVelocity = 2,
	SetKinematicDirection = 3,
	SetKinematicTangent = 4,
	SetKinematicSpeed = 5,
	SetKinematicVelocity = 6,
	ProcessPhysicsQuery = 7,
	OnStay = 8,
	OnInterpolate = 9,
	ProcessUserLogic = 10
}
