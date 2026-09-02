using System;
using System.IO;

namespace MoreMountains.Tools;

public interface IMMSaveLoadManagerMethod
{
	void Save(object objectToSave, FileStream saveFile);

	object Load(Type objectType, FileStream saveFile);
}
