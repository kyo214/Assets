using UnityEngine;

public class RocketLoading : MonoBehaviour
{
	public bool isLoaded = true;

	private void OnEnable()
	{
		if (isLoaded)
		{
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, 0.74f);
			return;
		}
		Animation component = base.gameObject.GetComponent<Animation>();
		if (component != null)
		{
			component.Play("Rocket Loading");
		}
		isLoaded = true;
	}
}
