namespace DestroyIt;

public static class StringExtensions
{
	public static string SceneFolder(this string scenePath)
	{
		string[] array = scenePath.Split('/');
		if (array.Length > 1)
		{
			string[] array2 = new string[array.Length - 1];
			for (int i = 0; i < array.Length - 1; i++)
			{
				array2[i] = array[i];
			}
			return string.Join("/", array2);
		}
		return scenePath;
	}
}
