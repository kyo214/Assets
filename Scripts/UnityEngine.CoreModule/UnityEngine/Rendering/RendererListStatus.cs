using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering;

[MovedFrom("UnityEngine.Rendering.RendererUtils")]
public enum RendererListStatus
{
	kRendererListInvalid = -2,
	kRendererListProcessing = -1,
	kRendererListEmpty = 0,
	kRendererListPopulated = 1
}
