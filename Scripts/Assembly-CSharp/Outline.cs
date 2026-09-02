using HighlightPlus;
using UnityEngine;

public class Outline : MonoBehaviour
{
	public HighlightEffect highlight;

	private void Awake()
	{
		if (highlight == null)
		{
			highlight = GetComponent<HighlightEffect>();
		}
	}
}
