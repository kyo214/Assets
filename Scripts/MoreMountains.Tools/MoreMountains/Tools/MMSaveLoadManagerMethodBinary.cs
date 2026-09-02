using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MoreMountains.Tools;

public class MMSaveLoadManagerMethodBinary : IMMSaveLoadManagerMethod
{
	public void Save(object objectToSave, FileStream saveFile)
	{
		new BinaryFormatter().Serialize(saveFile, objectToSave);
		saveFile.Close();
	}

	public object Load(Type objectType, FileStream saveFile)
	{
		object result = new BinaryFormatter().Deserialize(saveFile);
		saveFile.Close();
		return result;
	}
}
