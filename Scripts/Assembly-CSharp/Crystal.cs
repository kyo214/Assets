using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Crystal : MonoBehaviour
{
	public Color crystalColor = Color.red;

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

	public void ReceiveLaser(Vector3 hitPoint, Vector3 incomingDirection, Color incomingColor)
	{
		isHitThisFrame = true;
		Color laserColor = CombineColors(incomingColor, crystalColor);
		CastOutgoingLaser(hitPoint, incomingDirection, laserColor);
	}

	private void CastOutgoingLaser(Vector3 startPos, Vector3 direction, Color laserColor)
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

	private Color CombineColors(Color colorA, Color colorB)
	{
		float r = Mathf.Clamp01(colorA.r + colorB.r);
		float g = Mathf.Clamp01(colorA.g + colorB.g);
		float b = Mathf.Clamp01(colorA.b + colorB.b);
		return new Color(r, g, b, 1f);
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
