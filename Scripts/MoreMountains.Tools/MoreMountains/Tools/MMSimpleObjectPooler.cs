using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Object Pool/MMSimpleObjectPooler")]
public class MMSimpleObjectPooler : MMObjectPooler
{
	public GameObject GameObjectToPool;

	public int PoolSize = 20;

	public bool PoolCanExpand = true;

	protected List<GameObject> _pooledGameObjects;

	public List<MMSimpleObjectPooler> Owner { get; set; }

	private void OnDestroy()
	{
		Owner?.Remove(this);
	}

	public override void FillObjectPool()
	{
		if (!(GameObjectToPool == null) && (!(_objectPool != null) || _objectPool.PooledGameObjects.Count <= PoolSize))
		{
			CreateWaitingPool();
			_pooledGameObjects = new List<GameObject>();
			int num = PoolSize;
			if (_objectPool != null)
			{
				num -= _objectPool.PooledGameObjects.Count;
				_pooledGameObjects = new List<GameObject>(_objectPool.PooledGameObjects);
			}
			for (int i = 0; i < num; i++)
			{
				AddOneObjectToThePool();
			}
		}
	}

	protected override string DetermineObjectPoolName()
	{
		return "[SimpleObjectPooler] " + GameObjectToPool.name;
	}

	public override GameObject GetPooledGameObject()
	{
		for (int i = 0; i < _pooledGameObjects.Count; i++)
		{
			if (!_pooledGameObjects[i].gameObject.activeInHierarchy)
			{
				return _pooledGameObjects[i];
			}
		}
		if (PoolCanExpand)
		{
			return AddOneObjectToThePool();
		}
		return null;
	}

	protected virtual GameObject AddOneObjectToThePool()
	{
		if (GameObjectToPool == null)
		{
			Debug.LogWarning("The " + base.gameObject.name + " ObjectPooler doesn't have any GameObjectToPool defined.", base.gameObject);
			return null;
		}
		GameObjectToPool.SetActive(value: false);
		GameObject gameObject = Object.Instantiate(GameObjectToPool);
		SceneManager.MoveGameObjectToScene(gameObject, base.gameObject.scene);
		if (NestWaitingPool)
		{
			gameObject.transform.SetParent(_waitingPool.transform);
		}
		gameObject.name = GameObjectToPool.name + "-" + _pooledGameObjects.Count;
		_pooledGameObjects.Add(gameObject);
		_objectPool.PooledGameObjects.Add(gameObject);
		return gameObject;
	}
}
