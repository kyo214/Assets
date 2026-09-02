using Sirenix.OdinInspector;

public abstract class IScriptableObjectLibrary : SerializedScriptableObject
{
	public enum Update_Type
	{
		FINDASSETS = 0,
		DATABASE = 1
	}

	protected abstract string GetFilterString();

	public abstract void RefreshLibrary(Update_Type updateType = Update_Type.FINDASSETS);

	public abstract void UpdateLibrary();
}
