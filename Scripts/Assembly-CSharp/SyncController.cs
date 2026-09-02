using UnityEngine;

public class SyncController : MonoBehaviour
{
	public bool isSync;

	public int ctrSyncyTolerance;

	public int maxSyncyTolerance;

	public int ctrAFK;

	public int maxAFKTime;

	public Vector3 pos;

	public int angle;

	private float interval;

	public float intervalSync = 1f;

	[SerializeField]
	private PlayerNetwork playerNetwork;

	[SerializeField]
	private EnemyNetwork enemyNetwork;

	public void Update()
	{
		interval -= Time.deltaTime;
		if (interval < 0f)
		{
			interval = intervalSync;
			if (playerNetwork != null && !GameManager.Instance.gameOver)
			{
				CheckSyncPlayer(base.transform.position, playerNetwork.GetAngleInputNetwork());
			}
			else if (enemyNetwork != null)
			{
				CheckSyncEnemy(base.transform.position, enemyNetwork.GetAngleDirection());
			}
		}
	}

	public void SetSync(bool _isSync)
	{
		if (_isSync)
		{
			isSync = true;
			ctrSyncyTolerance = 0;
		}
	}

	public void CheckSyncPlayer(Vector3 _pos, int _angle)
	{
		if (pos == _pos && angle == _angle && playerNetwork.GetHealth() > 0f)
		{
			if (!playerNetwork.GetAFKPlayer())
			{
				ctrSyncyTolerance++;
				if (playerNetwork.isLocalPlayer)
				{
					ctrAFK++;
				}
			}
		}
		else
		{
			if (playerNetwork.isLocalPlayer)
			{
				ctrAFK = 0;
				if (playerNetwork.GetAFKPlayer())
				{
					playerNetwork.SetPlayerAFK(value: false);
				}
			}
			ctrSyncyTolerance = 0;
			isSync = true;
			pos = _pos;
			angle = _angle;
		}
		if (ctrSyncyTolerance > maxSyncyTolerance)
		{
			isSync = false;
		}
		if (playerNetwork.isLocalPlayer && ctrAFK > maxAFKTime)
		{
			isSync = true;
			pos = _pos;
			angle = _angle;
			if (!playerNetwork.GetAFKPlayer())
			{
				playerNetwork.SetPlayerAFK(value: true);
			}
		}
	}

	public void CheckSyncEnemy(Vector3 _pos, int _angle)
	{
		if (pos == _pos && angle == _angle)
		{
			ctrSyncyTolerance++;
		}
		else
		{
			ctrSyncyTolerance = 0;
			isSync = true;
			pos = _pos;
			angle = _angle;
		}
		if (ctrSyncyTolerance > maxSyncyTolerance)
		{
			isSync = false;
		}
	}
}
