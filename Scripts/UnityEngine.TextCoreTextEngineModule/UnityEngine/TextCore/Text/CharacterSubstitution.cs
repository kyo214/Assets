namespace UnityEngine.TextCore.Text;

internal struct CharacterSubstitution(int index, uint unicode)
{
	public int index = index;

	public uint unicode = unicode;
}
