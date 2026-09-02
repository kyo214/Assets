using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMSaveLoadManagerMethodBinaryEncrypted : MMSaveLoadManagerEncrypter, IMMSaveLoadManagerMethod
{
	public void Save(object objectToSave, FileStream saveFile)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		binaryFormatter.Serialize(memoryStream, objectToSave);
		memoryStream.Position = 0L;
		Encrypt(memoryStream, saveFile, base.Key);
		saveFile.Flush();
		memoryStream.Close();
		saveFile.Close();
	}

	public object Load(Type objectType, FileStream saveFile)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		try
		{
			Decrypt(saveFile, memoryStream, base.Key);
		}
		catch (CryptographicException ex)
		{
			Debug.LogError("[MMSaveLoadManager] Encryption key error: " + ex.Message);
			return null;
		}
		memoryStream.Position = 0L;
		object result = binaryFormatter.Deserialize(memoryStream);
		memoryStream.Close();
		saveFile.Close();
		return result;
	}
}
