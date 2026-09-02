using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserMirror : MonoBehaviour
{
	public float maxLaserDistance = 50f;

	public LayerMask collisionMask;

	private LineRenderer lineRenderer;

	private bool isHitThisFrame;

	private void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		lineRenderer.positionCount = 0;
	}

	public void ReflectLaser(Vector3 hitPoint, Vector3 incomingDirection, Vector3 surfaceNormal, Color laserColor)
	{
		isHitThisFrame = true;
		Vector3 direction = Vector3.Reflect(incomingDirection, surfaceNormal);
		CastReflectedLaser(hitPoint, direction, laserColor);
	}

	private void CastReflectedLaser(Vector3 startPos, Vector3 direction, Color laserColor)
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

	private void LateUpdate()
	{
		if (!isHitThisFrame)
		{
			lineRenderer.positionCount = 0;
		}
		isHitThisFrame = false;
	}
}
