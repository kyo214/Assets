using UnityEngine;

namespace Dissonance.Demo;

public class AudioProcessingGainsDisplay : MonoBehaviour
{
	private readonly float[] _gains = new float[22];

	private AudioProcessingTestSetup _processor;

	public RectTransform[] Bars;

	private RectTransform _self;

	private void Start()
	{
		_processor = GetComponentInParent<AudioProcessingTestSetup>();
		_self = GetComponent<RectTransform>();
	}

	private void Update()
	{
		int gains = _processor.GetGains(_gains);
		for (int i = 0; i < Bars.Length; i++)
		{
			float num = ((i >= gains) ? 0f : _gains[i]);
			Vector2 sizeDelta = Bars[i].sizeDelta;
			sizeDelta.y = _self.rect.height * num;
			Bars[i].sizeDelta = sizeDelta;
		}
	}
}
