using UnityEngine;
using UnityEngine.UI;

namespace Chronos.Example;

[RequireComponent(typeof(Slider))]
public class ExampleSlider : MonoBehaviour
{
	public GlobalClock clock;

	public Text text;

	private Slider slider => GetComponent<Slider>();

	private void Start()
	{
		slider.onValueChanged.AddListener(OnValueChanged);
	}

	public void OnValueChanged(float value)
	{
		clock.localTimeScale = value;
	}

	private void Update()
	{
		slider.value = clock.localTimeScale;
		float num = clock.localTimeScale;
		string text;
		if (clock.parent == null)
		{
			text = "=";
		}
		else if (clock.parentBlend == ClockBlend.Multiplicative)
		{
			text = "x";
		}
		else
		{
			text = ((clock.localTimeScale >= 0f) ? "+" : "-");
			num = Mathf.Abs(num);
		}
		this.text.text = $"{clock.key} ({text} {num:0.0} = {clock.timeScale:0.0})";
	}
}
