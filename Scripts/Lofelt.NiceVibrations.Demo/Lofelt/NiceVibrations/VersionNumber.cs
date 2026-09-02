using UnityEngine;
using UnityEngine.UI;

namespace Lofelt.NiceVibrations;

[RequireComponent(typeof(Text))]
public class VersionNumber : MonoBehaviour
{
	public string Version = "v3.3";

	protected Text _text;

	protected virtual void Awake()
	{
		_text = base.gameObject.GetComponent<Text>();
	}

	protected virtual void Start()
	{
		_text.text = Version.Replace("-alpha.", "a").Replace("-beta.", "b");
	}
}
