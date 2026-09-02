using System;
using UnityEngine;

namespace MoreMountains.Tools;

[Serializable]
public class MMPropertyPicker
{
	public UnityEngine.Object TargetObject;

	public Component TargetComponent;

	public ScriptableObject TargetScriptableObject;

	public string TargetPropertyName;

	protected MMProperty _targetMMProperty;

	protected bool _initialized;

	protected MMPropertyLink _propertySetter;

	public bool PropertyFound { get; protected set; }

	public virtual void Initialization(GameObject source)
	{
		if (TargetComponent == null && TargetScriptableObject == null)
		{
			PropertyFound = false;
			return;
		}
		_targetMMProperty = MMProperty.FindProperty(TargetPropertyName, TargetComponent, source, TargetScriptableObject);
		if (_targetMMProperty == null)
		{
			PropertyFound = false;
			return;
		}
		if (_targetMMProperty.TargetComponent == null && _targetMMProperty.TargetScriptableObject == null)
		{
			PropertyFound = false;
			return;
		}
		if (_targetMMProperty.MemberPropertyInfo == null && _targetMMProperty.MemberFieldInfo == null)
		{
			PropertyFound = false;
			return;
		}
		PropertyFound = true;
		_initialized = true;
		if (_targetMMProperty.PropertyType == typeof(string))
		{
			_propertySetter = new MMPropertyLinkString();
			_propertySetter.Initialization(_targetMMProperty);
		}
		else if (_targetMMProperty.PropertyType == typeof(float))
		{
			_propertySetter = new MMPropertyLinkFloat();
			_propertySetter.Initialization(_targetMMProperty);
		}
		else if (_targetMMProperty.PropertyType == typeof(Vector2))
		{
			_propertySetter = new MMPropertyLinkVector2();
			_propertySetter.Initialization(_targetMMProperty);
		}
		else if (_targetMMProperty.PropertyType == typeof(Vector3))
		{
			_propertySetter = new MMPropertyLinkVector3();
			_propertySetter.Initialization(_targetMMProperty);
		}
		else if (_targetMMProperty.PropertyType == typeof(Vector4))
		{
			_propertySetter = new MMPropertyLinkVector4();
			_propertySetter.Initialization(_targetMMProperty);
		}
		else if (_targetMMProperty.PropertyType == typeof(Quaternion))
		{
			_propertySetter = new MMPropertyLinkQuaternion();
			_propertySetter.Initialization(_targetMMProperty);
		}
		else if (_targetMMProperty.PropertyType == typeof(int))
		{
			_propertySetter = new MMPropertyLinkInt();
			_propertySetter.Initialization(_targetMMProperty);
		}
		else if (_targetMMProperty.PropertyType == typeof(bool))
		{
			_propertySetter = new MMPropertyLinkBool();
			_propertySetter.Initialization(_targetMMProperty);
		}
		else if (_targetMMProperty.PropertyType == typeof(Color))
		{
			_propertySetter = new MMPropertyLinkColor();
			_propertySetter.Initialization(_targetMMProperty);
		}
	}

	public virtual object GetRawValue()
	{
		return _propertySetter.GetPropertyValue(_targetMMProperty);
	}
}
