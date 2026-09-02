using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMSaveLoadManagerMethodJson : IMMSaveLoadManagerMethod
{
	public void Save(object objectToSave, FileStream saveFile)
	{
		string value = JsonUtility.ToJson(objectToSave);
		StreamWriter streamWriter = new StreamWriter(saveFile);
		streamWriter.Write(value);
		streamWriter.Close();
		saveFile.Close();
	}

	public object Load(Type objectType, FileStream saveFile)
	{
		StreamReader streamReader = new StreamReader(saveFile, Encoding.UTF8);
		object result = JsonUtility.FromJson(streamReader.ReadToEnd(), objectType);
		streamReader.Close();
		saveFile.Close();
		return result;
	}
}
