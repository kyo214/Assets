using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomizeImage : MonoBehaviour
{
	[SerializeField]
	private List<Sprite> arrSprites = new List<Sprite>();

	[SerializeField]
	public Image image;

	private void Awake()
	{
		if ((object)image == null)
		{
			image = GetComponent<Image>();
		}
		image.sprite = arrSprites[Random.Range(0, arrSprites.Count)];
	}

	public void RandomizeSprite()
	{
		image.sprite = arrSprites[Random.Range(0, arrSprites.Count)];
	}
}
