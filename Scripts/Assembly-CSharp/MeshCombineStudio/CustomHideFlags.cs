namespace MeshCombineStudio;

public enum CustomHideFlags
{
	HideInHierarchy = 1,
	HideInInspector = 2,
	DontSaveInEditor = 4,
	NotEditable = 8,
	DontSaveInBuild = 0x10,
	DontUnloadUnusedAsset = 0x20
}
