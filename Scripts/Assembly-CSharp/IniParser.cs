using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IniParser
{
	private List<string> keys = new List<string>();

	private List<string> vals = new List<string>();

	private List<string> comments = new List<string>();

	private List<string> subSections = new List<string>();

	private int commentMargin = 60;

	public IniParser()
	{
	}

	public IniParser(IniFiles file)
	{
		Load(file);
	}

	public bool DoesExist(IniFiles file)
	{
		if (!File.Exists(Application.dataPath + "/" + file.ToString() + ".ini"))
		{
			return false;
		}
		return true;
	}

	public bool DoesExist(string path, string fileName)
	{
		if (!File.Exists(path + "/" + fileName + ".ini"))
		{
			return false;
		}
		return true;
	}

	public void Set(string subSection, string key, string value)
	{
		for (int i = 0; i < keys.Count; i++)
		{
			if (keys[i].Equals(key))
			{
				vals[i] = value;
				subSections[i] = subSection;
				return;
			}
		}
		subSections.Add(subSection);
		keys.Add(key);
		vals.Add(value);
		comments.Add("");
	}

	public void Set(string subSection, string key, string value, string comment)
	{
		for (int i = 0; i < keys.Count; i++)
		{
			if (keys[i].Equals(key))
			{
				vals[i] = value;
				subSections[i] = subSection;
				comments[i] = comment;
				return;
			}
		}
		subSections.Add(subSection);
		keys.Add(key);
		vals.Add(value);
		comments.Add(comment);
	}

	public string Get(string key)
	{
		for (int i = 0; i < keys.Count; i++)
		{
			if (keys[i].Equals(key))
			{
				return vals[i];
			}
		}
		return "";
	}

	public string Get(string subSection, string key)
	{
		for (int i = 0; i < keys.Count; i++)
		{
			if (keys[i].Equals(key) && subSections[i].Equals(subSection))
			{
				return vals[i];
			}
		}
		return "";
	}

	public string[] GetLine(string key)
	{
		string[] array = new string[4];
		for (int i = 0; i < keys.Count; i++)
		{
			if (keys[i].Equals(key))
			{
				array[0] = subSections[i];
				array[1] = keys[i];
				array[2] = vals[i];
				array[3] = comments[i];
				return array;
			}
		}
		return array;
	}

	public void Remove(string key)
	{
		for (int i = 0; i < keys.Count; i++)
		{
			if (keys[i].Equals(key))
			{
				subSections.RemoveAt(i);
				keys.RemoveAt(i);
				vals.RemoveAt(i);
				comments.RemoveAt(i);
				return;
			}
		}
		Debug.LogError("Key not found");
	}

	public void Remove(string subSection, string key)
	{
		for (int i = 0; i < keys.Count; i++)
		{
			if (keys[i].Equals(key) && subSections[i].Equals(subSection))
			{
				subSections.RemoveAt(i);
				keys.RemoveAt(i);
				vals.RemoveAt(i);
				comments.RemoveAt(i);
				return;
			}
		}
		Debug.LogError("Key not found");
	}

	public void Save(string file)
	{
		using (StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/" + file + ".ini"))
		{
			List<string> list = new List<string>();
			for (int i = 0; i < subSections.Count; i++)
			{
				if (!list.Contains(subSections[i]))
				{
					list.Add(subSections[i]);
				}
			}
			list.Sort();
			List<string> list2 = keys;
			List<string> list3 = vals;
			List<string> list4 = comments;
			List<string> list5 = subSections;
			for (int j = 0; j < list.Count; j++)
			{
				int num = 0;
				while (list5.Contains(list[j]))
				{
					int index = list5.IndexOf(list[j]);
					if (num == 0 && !list[j].Equals(""))
					{
						streamWriter.WriteLine("\n[" + list[j] + "]\n");
					}
					if (!list4[index].Equals(""))
					{
						string text = list2[index] + "=" + list3[index];
						int count = (commentMargin - text.Length) / 4;
						streamWriter.WriteLine(text + new string('\t', count) + "; " + list4[index]);
					}
					else
					{
						streamWriter.WriteLine(list2[index] + "=" + list3[index]);
					}
					list5.RemoveAt(index);
					list2.RemoveAt(index);
					list4.RemoveAt(index);
					list3.RemoveAt(index);
					num++;
				}
			}
		}
		Debug.Log(file + ".ini Saved");
	}

	public void Save(string file, string path)
	{
		using (StreamWriter streamWriter = new StreamWriter(path + "/" + file + ".ini"))
		{
			List<string> list = new List<string>();
			for (int i = 0; i < subSections.Count; i++)
			{
				if (!list.Contains(subSections[i]))
				{
					list.Add(subSections[i]);
				}
			}
			list.Sort();
			List<string> list2 = keys;
			List<string> list3 = vals;
			List<string> list4 = comments;
			List<string> list5 = subSections;
			for (int j = 0; j < list.Count; j++)
			{
				int num = 0;
				while (list5.Contains(list[j]))
				{
					int index = list5.IndexOf(list[j]);
					if (num == 0 && !list[j].Equals(""))
					{
						streamWriter.WriteLine("\n[" + list[j] + "]\n");
					}
					if (!list4[index].Equals(""))
					{
						string text = list2[index] + "=" + list3[index];
						int count = (commentMargin - text.Length) / 4;
						streamWriter.WriteLine(text + new string('\t', count) + "; " + list4[index]);
					}
					else
					{
						streamWriter.WriteLine(list2[index] + "=" + list3[index]);
					}
					list5.RemoveAt(index);
					list2.RemoveAt(index);
					list4.RemoveAt(index);
					list3.RemoveAt(index);
					num++;
				}
			}
		}
		Debug.Log(file + ".ini Saved");
	}

	public void Load(IniFiles file)
	{
		Clear();
		string text = "";
		string path = Application.dataPath + "/" + file.ToString() + ".ini";
		string subSection = "";
		int num = 0;
		int num2 = 0;
		try
		{
			using StreamReader streamReader = new StreamReader(path);
			while ((text = streamReader.ReadLine()) != null)
			{
				num = text.IndexOf("=");
				num2 = text.IndexOf(";");
				if (text.IndexOf("[") == 0)
				{
					subSection = text.Substring(1, text.Length - 2);
				}
				if (num > 0)
				{
					if (num2 != -1)
					{
						string text2 = text.Substring(num + 1, num2 - (num + 1));
						text2 = text2.Replace("\t", "");
						Set(subSection, text.Substring(0, num), text2, text.Substring(num2 + 1).TrimStart(' '));
					}
					else
					{
						Set(subSection, text.Substring(0, num), text.Substring(num + 1));
					}
				}
			}
			Debug.Log(file.ToString() + " Loaded");
		}
		catch (IOException message)
		{
			Debug.Log("Error opening " + file.ToString() + ".ini");
			Debug.LogWarning(message);
		}
	}

	public void Load(string path, string fileName)
	{
		Clear();
		string text = "";
		string path2 = path + "/" + fileName + ".ini";
		string subSection = "";
		int num = 0;
		int num2 = 0;
		try
		{
			using StreamReader streamReader = new StreamReader(path2);
			while ((text = streamReader.ReadLine()) != null)
			{
				num = text.IndexOf("=");
				num2 = text.IndexOf(";");
				if (text.IndexOf("[") == 0)
				{
					subSection = text.Substring(1, text.Length - 2);
				}
				if (num > 0)
				{
					if (num2 != -1)
					{
						string text2 = text.Substring(num + 1, num2 - (num + 1));
						text2 = text2.Replace("\t", "");
						Set(subSection, text.Substring(0, num), text2, text.Substring(num2 + 1).TrimStart(' '));
					}
					else
					{
						Set(subSection, text.Substring(0, num), text.Substring(num + 1));
					}
				}
			}
		}
		catch (IOException message)
		{
			Debug.LogWarning(message);
		}
	}

	public void Clear()
	{
		keys = new List<string>();
		vals = new List<string>();
		comments = new List<string>();
		subSections = new List<string>();
	}

	public int Count()
	{
		return keys.Count;
	}
}
