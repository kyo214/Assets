namespace Fusion.KCC;

public interface IKCCProcessor
{
	float Priority { get; }

	EKCCStages GetValidStages(KCC kcc, KCCData data);

	void SetInputProperties(KCC kcc, KCCData data);

	void SetDynamicVelocity(KCC kcc, KCCData data);

	void SetKinematicDirection(KCC kcc, KCCData data);

	void SetKinematicTangent(KCC kcc, KCCData data);

	void SetKinematicSpeed(KCC kcc, KCCData data);

	void SetKinematicVelocity(KCC kcc, KCCData data);

	void ProcessPhysicsQuery(KCC kcc, KCCData data);

	void OnEnter(KCC kcc, KCCData data);

	void OnExit(KCC kcc, KCCData data);

	void OnStay(KCC kcc, KCCData data);

	void OnInterpolate(KCC kcc, KCCData data);

	void ProcessUserLogic(KCC kcc, KCCData data, object userData);
}
