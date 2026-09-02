using System;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;

namespace Doozy.Runtime.Mody;

[Serializable]
public class ModyActionRunner
{
	public ModyModule Module;

	public string ActionName;

	public bool BoolValue;

	public Color ColorValue;

	public double DoubleValue;

	public float FloatValue;

	public GameObject GameObjectValue;

	public int IntValue;

	public long LongValue;

	public MonoBehaviour MonoBehaviourValue;

	public UnityEngine.Object GenericValue;

	public ScriptableObject ScriptableObjectValue;

	public Sprite SpriteValue;

	public string StringValue;

	public Texture TextureValue;

	public Texture2D Texture2DValue;

	public Vector2 Vector2Value;

	public Vector3 Vector3Value;

	public Vector4 Vector4Value;

	public RunAction Run;

	public bool IgnoreCooldown;

	public ModyActionRunner()
	{
		Reset();
	}

	public void Reset()
	{
		Module = null;
		ActionName = string.Empty;
		Run = RunAction.Start;
		IgnoreCooldown = false;
		BoolValue = false;
		ColorValue = default;
		DoubleValue = 0.0;
		FloatValue = 0f;
		GameObjectValue = null;
		GenericValue = null;
		IntValue = 0;
		LongValue = 0L;
		MonoBehaviourValue = null;
		ScriptableObjectValue = null;
		SpriteValue = null;
		StringValue = null;
		Texture2DValue = null;
		TextureValue = null;
		Vector2Value = default;
		Vector3Value = default;
		Vector4Value = default;
	}

	public void Execute(bool debug = false)
	{
		var (flag, message) = CanExecute();
		if (debug)
		{
			Debug.Log(message);
		}
		if (!flag)
		{
			return;
		}
		ModyAction action = Module.GetAction(ActionName);
		bool reactToAnySignal = action.ReactToAnySignal;
		action.ReactToAnySignal = true;
		bool ignoreSignalValue = action.IgnoreSignalValue;
		action.IgnoreSignalValue = true;
		if (action.HasValue)
		{
			if (action.ValueType == typeof(int))
			{
				action.SetValue(IntValue);
			}
			else if (action.ValueType == typeof(float))
			{
				action.SetValue(FloatValue);
			}
			else if (action.ValueType == typeof(double))
			{
				action.SetValue(DoubleValue);
			}
			else if (action.ValueType == typeof(long))
			{
				action.SetValue(LongValue);
			}
			else if (action.ValueType == typeof(string))
			{
				action.SetValue(StringValue);
			}
			else if (action.ValueType == typeof(bool))
			{
				action.SetValue(BoolValue);
			}
			else if (action.ValueType == typeof(Color) || action.ValueType == typeof(Color32))
			{
				action.SetValue(ColorValue);
			}
			else if (action.ValueType == typeof(Vector2))
			{
				action.SetValue(Vector2Value);
			}
			else if (action.ValueType == typeof(Vector3))
			{
				action.SetValue(Vector3Value);
			}
			else if (action.ValueType == typeof(Vector4))
			{
				action.SetValue(Vector4Value);
			}
			else if (action.ValueType == typeof(GameObject))
			{
				action.SetValue(GameObjectValue);
			}
			else if (action.ValueType == typeof(MonoBehaviour))
			{
				action.SetValue(MonoBehaviourValue);
			}
			else if (action.ValueType == typeof(Sprite))
			{
				action.SetValue(SpriteValue);
			}
			else if (action.ValueType == typeof(Texture))
			{
				action.SetValue(TextureValue);
			}
			else if (action.ValueType == typeof(Texture2D))
			{
				action.SetValue(Texture2DValue);
			}
			else if (action.ValueType == typeof(ScriptableObject))
			{
				action.SetValue(ScriptableObjectValue);
			}
			else
			{
				action.SetValue(GenericValue);
			}
		}
		action.ExecuteMethod(Run, IgnoreCooldown, forced: true);
		action.ReactToAnySignal = reactToAnySignal;
		action.IgnoreSignalValue = ignoreSignalValue;
	}

	public (bool, string) CanExecute()
	{
		if (!(Module == null))
		{
			if (!ActionName.IsNullOrEmpty())
			{
				if (Module.ContainsAction(ActionName))
				{
					return (true, "Success! Action " + ActionName + " can be executed!");
				}
				return (false, "The Module " + Module.moduleName + " does not contain an Action named " + ActionName + ". Cannot Execute!");
			}
			return (false, "ActionName cannot be null or empty! Cannot Execute!");
		}
		return (false, "Module reference is null! Cannot Execute!");
	}
}
