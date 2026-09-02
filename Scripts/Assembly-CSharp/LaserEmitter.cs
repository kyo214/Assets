using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserEmitter : MonoBehaviour
{
	public float maxLaserDistance = 50f;

	public LayerMask collisionMask;

	public Color currentLaserColor = Color.white;

	private LineRenderer lineRenderer;

	private void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
	}

	private void Update()
	{
		CastLaser(base.transform.position, base.transform.forward, currentLaserColor);
	}

	private void CastLaser(Vector3 startPos, Vector3 direction, Color laserColor)
	{
		lineRenderer.positionCount = 2;
		lineRenderer.SetPosition(0, startPos);
		lineRenderer.startColor = laserColor;
		lineRenderer.endColor = laserColor;
		if (Physics.Raycast(startPos, direction, out var hitInfo, maxLaserDistance, collisionMask))
		{
			lineRenderer.SetPosition(1, hitInfo.point);
			LaserMirror component = hitInfo.collider.GetComponent<LaserMirror>();
			if (component != null)
			{
				component.ReflectLaser(hitInfo.point, direction, hitInfo.normal, laserColor);
				return;
			}
			Crystal component2 = hitInfo.collider.GetComponent<Crystal>();
			if (component2 != null)
			{
				component2.ReceiveLaser(hitInfo.point, direction, laserColor);
				return;
			}
			LaserReceiver component3 = hitInfo.collider.GetComponent<LaserReceiver>();
			if (component3 != null)
			{
				component3.CheckColor(laserColor);
			}
		}
		else
		{
			lineRenderer.SetPosition(1, startPos + direction * maxLaserDistance);
		}
	}
}
