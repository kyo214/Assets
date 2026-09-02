namespace UnityEngine.UIElements.StyleSheets;

internal struct MatchResultInfo(bool success, PseudoStates triggerPseudoMask, PseudoStates dependencyPseudoMask)
{
	public readonly bool success = success;

	public readonly PseudoStates triggerPseudoMask = triggerPseudoMask;

	public readonly PseudoStates dependencyPseudoMask = dependencyPseudoMask;
}
