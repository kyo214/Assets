using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGDataBinderGoA : MonoBehaviour
{
	public enum SourceMethodEnum
	{
		Start = 0,
		Awake = 1,
		Manual = 2
	}

	[Serializable]
	public class PathItem
	{
		public string Field;

		public bool IsProperty;

		protected bool Equals(PathItem other)
		{
			if (string.Equals(Field, other.Field))
			{
				return IsProperty == other.IsProperty;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((PathItem)obj);
		}

		public override int GetHashCode()
		{
			return (((Field != null) ? Field.GetHashCode() : 0) * 397) ^ IsProperty.GetHashCode();
		}

		public override string ToString()
		{
			return Field;
		}
	}

	[SerializeField]
	[HideInInspector]
	private bool ignoreErrors;

	[SerializeField]
	[HideInInspector]
	private SourceMethodEnum sourceMethod;

	[NonSerialized]
	protected bool bindedOnce;

	public bool IgnoreErrors
	{
		get
		{
			return ignoreErrors;
		}
		set
		{
			ignoreErrors = value;
		}
	}

	public SourceMethodEnum SourceMethod
	{
		get
		{
			return sourceMethod;
		}
		set
		{
			sourceMethod = value;
		}
	}

	public abstract string Error { get; }

	public virtual bool SupportReverseBinding => true;

	private string Path
	{
		get
		{
			GameObject gameObject = base.gameObject;
			string text = "/" + gameObject.name;
			while (gameObject.transform.parent != null)
			{
				gameObject = gameObject.transform.parent.gameObject;
				text = "/" + gameObject.name + text;
			}
			return text;
		}
	}

	public event Action OnBind;

	private void Awake()
	{
		if (sourceMethod == SourceMethodEnum.Awake)
		{
			bindedOnce = true;
			FirstBind();
		}
	}

	private void Start()
	{
		if (sourceMethod == SourceMethodEnum.Start)
		{
			bindedOnce = true;
			FirstBind();
		}
	}

	protected abstract void OnDestroy();

	protected abstract void FirstBind();

	public abstract void Bind();

	public abstract void ReverseBind();

	protected void LogError(string error)
	{
		LogError(this, error);
	}

	public static void LogError(BGDataBinderGoA binder, string error)
	{
		if (string.IsNullOrEmpty(error))
		{
			return;
		}
		if (binder == null)
		{
			if (!string.IsNullOrEmpty(error))
			{
				Debug.LogError("BGDatabase.UnknownBinder error [" + error + "]");
			}
		}
		else if (!binder.IgnoreErrors)
		{
			Debug.LogError("BGDatabase." + binder.GetType().Name + " binder error at [" + binder.Path + "] GameObject: [" + error + "]. You can disable this message by 1) fixing the error or 2) toggling 'ignoreErrors' toggle on at target dataBinder");
		}
	}

	protected void FireOnBind()
	{
		OnBind?.Invoke();
	}

	private void Unused_DoNotCallIt_PreventIosStripping()
	{
		GetComponent<SpriteRenderer>().sprite = null;
		GetComponent<Material>().mainTexture = null;
		GetComponent<MeshRenderer>().sharedMaterial = null;
		GetComponent<MeshRenderer>().sharedMaterial.mainTexture = null;
		GetComponent<TextMesh>().text = null;
		GetComponent<AudioSource>().clip = null;
	}
}
