using UnityEngine;
using UnityEngine.UI;

public class RawImageAddOffset : MonoBehaviour
{
	private RawImage img;

	[SerializeField]
	private float incrementOffset;

	private float increment;

	private void Start()
	{
		img = GetComponent<RawImage>();
		increment = 0f;
	}

	private void FixedUpdate()
	{
		increment += incrementOffset * Time.deltaTime;
		img.uvRect = new Rect(increment, 0f, 1f, 1f);
		if (increment >= 1f)
		{
			increment--;
		}
		else if (increment < 0f)
		{
			increment++;
		}
	}
}
