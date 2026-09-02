using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Object Pool/MMMultipleObjectPooler")]
public class MMMultipleObjectPooler : MMObjectPooler
{
	public List<MMMultipleObjectPoolerObject> Pool;

	[MMInformation("A MultipleObjectPooler is a reserve of objects, to be used by a Spawner. When asked, it will return an object from the pool (ideally an inactive one) chosen based on the pooling method you've chosen.\n- OriginalOrder will spawn objects in the order you've set them in the inspector (from top to bottom)\n- OriginalOrderSequential will do the same, but will empty each pool before moving to the next object\n- RandomBetweenObjects will pick one object from the pool, at random, but ignoring its pool size, each object has equal chances to get picked\n- PoolSizeBased randomly choses one object from the pool, based on its pool size probability (the larger the pool size, the higher the chances it'll get picked)'...", MMInformationAttribute.InformationType.Info, false)]
	public MMPoolingMethods PoolingMethod = MMPoolingMethods.RandomPoolSizeBased;

	[MMInformation("If you set CanPoolSameObjectTwice to false, the Pooler will try to prevent the same object from being pooled twice to avoid repetition. This will only affect random pooling methods, not ordered pooling.", MMInformationAttribute.InformationType.Info, false)]
	public bool CanPoolSameObjectTwice = true;

	[MMCondition("MutualizeWaitingPools", true)]
	public string MutualizedPoolName = "";

	protected GameObject _lastPooledObject;

	protected int _currentIndex;

	protected int _currentIndexCounter;

	public List<MMMultipleObjectPooler> Owner { get; set; }

	private void OnDestroy()
	{
		Owner?.Remove(this);
	}

	protected override string DetermineObjectPoolName()
	{
		if (MutualizedPoolName == null || MutualizedPoolName == "")
		{
			return "[MultipleObjectPooler] " + base.name;
		}
		return "[MultipleObjectPooler] " + MutualizedPoolName;
	}

	public override void FillObjectPool()
	{
		if (Pool == null || Pool.Count == 0 || !CreateWaitingPool())
		{
			return;
		}
		if (Pool.Count <= 1)
		{
			CanPoolSameObjectTwice = true;
		}
		switch (PoolingMethod)
		{
		case MMPoolingMethods.OriginalOrder:
		{
			bool flag = true;
			int[] array = new int[Pool.Count];
			for (int j = 0; j < Pool.Count; j++)
			{
				array[j] = Pool[j].PoolSize;
			}
			while (flag)
			{
				flag = false;
				for (int k = 0; k < Pool.Count; k++)
				{
					if (array[k] > 0)
					{
						AddOneObjectToThePool(Pool[k].GameObjectToPool);
						array[k]--;
						flag = true;
					}
				}
			}
			return;
		}
		case MMPoolingMethods.OriginalOrderSequential:
		{
			foreach (MMMultipleObjectPoolerObject item in Pool)
			{
				for (int i = 0; i < item.PoolSize; i++)
				{
					AddOneObjectToThePool(item.GameObjectToPool);
				}
			}
			return;
		}
		}
		int num = 0;
		foreach (MMMultipleObjectPoolerObject item2 in Pool)
		{
			if (num > Pool.Count)
			{
				break;
			}
			for (int l = 0; l < Pool[num].PoolSize; l++)
			{
				AddOneObjectToThePool(item2.GameObjectToPool);
			}
			num++;
		}
	}

	protected virtual GameObject AddOneObjectToThePool(GameObject typeOfObject)
	{
		if (typeOfObject == null)
		{
			return null;
		}
		typeOfObject.SetActive(value: false);
		GameObject gameObject = Object.Instantiate(typeOfObject);
		SceneManager.MoveGameObjectToScene(gameObject, base.gameObject.scene);
		if (NestWaitingPool)
		{
			gameObject.transform.SetParent(_waitingPool.transform);
		}
		gameObject.name = typeOfObject.name;
		_objectPool.PooledGameObjects.Add(gameObject);
		return gameObject;
	}

	public override GameObject GetPooledGameObject()
	{
		GameObject gameObject = PoolingMethod switch
		{
			MMPoolingMethods.OriginalOrder => GetPooledGameObjectOriginalOrder(), 
			MMPoolingMethods.RandomPoolSizeBased => GetPooledGameObjectPoolSizeBased(), 
			MMPoolingMethods.RandomBetweenObjects => GetPooledGameObjectRandomBetweenObjects(), 
			MMPoolingMethods.OriginalOrderSequential => GetPooledGameObjectOriginalOrderSequential(), 
			_ => null, 
		};
		if (gameObject != null)
		{
			_lastPooledObject = gameObject;
		}
		else
		{
			_lastPooledObject = null;
		}
		return gameObject;
	}

	protected virtual GameObject GetPooledGameObjectOriginalOrder()
	{
		if (_currentIndexCounter >= Pool[_currentIndex].PoolSize)
		{
			_currentIndexCounter = 0;
			_currentIndex++;
		}
		if (_currentIndex >= Pool.Count)
		{
			ResetCurrentIndex();
		}
		MMMultipleObjectPoolerObject poolObject = GetPoolObject(Pool[_currentIndex].GameObjectToPool);
		if (_currentIndex >= _objectPool.PooledGameObjects.Count)
		{
			return null;
		}
		if (!poolObject.Enabled)
		{
			_currentIndex++;
			return null;
		}
		if (_objectPool.PooledGameObjects[_currentIndex].gameObject.activeInHierarchy)
		{
			GameObject gameObject = FindInactiveObject(_objectPool.PooledGameObjects[_currentIndex].gameObject.name, _objectPool.PooledGameObjects);
			if (gameObject != null)
			{
				_currentIndexCounter++;
				return gameObject;
			}
			if (poolObject.PoolCanExpand)
			{
				_currentIndexCounter++;
				return AddOneObjectToThePool(poolObject.GameObjectToPool);
			}
			return null;
		}
		int currentIndex = _currentIndex;
		_currentIndexCounter++;
		return _objectPool.PooledGameObjects[currentIndex];
	}

	protected virtual GameObject GetPooledGameObjectOriginalOrderSequential()
	{
		if (_currentIndex >= Pool.Count)
		{
			ResetCurrentIndex();
		}
		MMMultipleObjectPoolerObject poolObject = GetPoolObject(Pool[_currentIndex].GameObjectToPool);
		if (_currentIndex >= _objectPool.PooledGameObjects.Count)
		{
			return null;
		}
		if (!poolObject.Enabled)
		{
			_currentIndex++;
			return null;
		}
		if (_objectPool.PooledGameObjects[_currentIndex].gameObject.activeInHierarchy)
		{
			GameObject gameObject = FindInactiveObject(_objectPool.PooledGameObjects[_currentIndex].gameObject.name, _objectPool.PooledGameObjects);
			if (gameObject != null)
			{
				_currentIndex++;
				return gameObject;
			}
			if (poolObject.PoolCanExpand)
			{
				_currentIndex++;
				return AddOneObjectToThePool(poolObject.GameObjectToPool);
			}
			return null;
		}
		int currentIndex = _currentIndex;
		_currentIndex++;
		return _objectPool.PooledGameObjects[currentIndex];
	}

	protected virtual GameObject GetPooledGameObjectPoolSizeBased()
	{
		int index = Random.Range(0, _objectPool.PooledGameObjects.Count);
		int num = 0;
		while (!PoolObjectEnabled(_objectPool.PooledGameObjects[index]) && num < _objectPool.PooledGameObjects.Count)
		{
			index = Random.Range(0, _objectPool.PooledGameObjects.Count);
			num++;
		}
		if (!PoolObjectEnabled(_objectPool.PooledGameObjects[index]))
		{
			return null;
		}
		num = 0;
		while (!CanPoolSameObjectTwice && _objectPool.PooledGameObjects[index] == _lastPooledObject && num < _objectPool.PooledGameObjects.Count)
		{
			index = Random.Range(0, _objectPool.PooledGameObjects.Count);
			num++;
		}
		if (_objectPool.PooledGameObjects[index].gameObject.activeInHierarchy)
		{
			GameObject gameObject = FindInactiveObject(_objectPool.PooledGameObjects[index].gameObject.name, _objectPool.PooledGameObjects);
			if (gameObject != null)
			{
				return gameObject;
			}
			MMMultipleObjectPoolerObject poolObject = GetPoolObject(_objectPool.PooledGameObjects[index].gameObject);
			if (poolObject == null)
			{
				return null;
			}
			if (poolObject.PoolCanExpand)
			{
				return AddOneObjectToThePool(poolObject.GameObjectToPool);
			}
			return null;
		}
		return _objectPool.PooledGameObjects[index];
	}

	protected virtual GameObject GetPooledGameObjectRandomBetweenObjects()
	{
		int num = Random.Range(0, Pool.Count);
		int num2 = 0;
		while (!CanPoolSameObjectTwice && Pool[num].GameObjectToPool == _lastPooledObject && num2 < _objectPool.PooledGameObjects.Count)
		{
			num = Random.Range(0, Pool.Count);
			num2++;
		}
		int num3 = num + 1;
		bool flag = false;
		num2 = 0;
		while (!flag && num != num3 && num2 < _objectPool.PooledGameObjects.Count)
		{
			if (num >= Pool.Count)
			{
				num = 0;
			}
			if (!Pool[num].Enabled)
			{
				num++;
				num2++;
				continue;
			}
			GameObject gameObject = FindInactiveObject(Pool[num].GameObjectToPool.name, _objectPool.PooledGameObjects);
			if (gameObject != null)
			{
				flag = true;
				return gameObject;
			}
			if (Pool[num].PoolCanExpand)
			{
				return AddOneObjectToThePool(Pool[num].GameObjectToPool);
			}
			num++;
			num2++;
		}
		return null;
	}

	protected virtual GameObject GetPooledGameObjectOfType(string searchedName)
	{
		GameObject gameObject = FindInactiveObject(searchedName, _objectPool.PooledGameObjects);
		if (gameObject != null)
		{
			return gameObject;
		}
		GameObject gameObject2 = FindObject(searchedName, _objectPool.PooledGameObjects);
		if (gameObject2 == null)
		{
			return null;
		}
		if (GetPoolObject(FindObject(searchedName, _objectPool.PooledGameObjects)).PoolCanExpand)
		{
			GameObject gameObject3 = Object.Instantiate(gameObject2);
			SceneManager.MoveGameObjectToScene(gameObject3, base.gameObject.scene);
			_objectPool.PooledGameObjects.Add(gameObject3);
			return gameObject3;
		}
		return null;
	}

	protected virtual GameObject FindInactiveObject(string searchedName, List<GameObject> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].name.Equals(searchedName) && !list[i].gameObject.activeInHierarchy)
			{
				return list[i];
			}
		}
		return null;
	}

	protected virtual GameObject FindAnyInactiveObject(List<GameObject> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (!list[i].gameObject.activeInHierarchy)
			{
				return list[i];
			}
		}
		return null;
	}

	protected virtual GameObject FindObject(string searchedName, List<GameObject> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].name.Equals(searchedName))
			{
				return list[i];
			}
		}
		return null;
	}

	protected virtual MMMultipleObjectPoolerObject GetPoolObject(GameObject testedObject)
	{
		if (testedObject == null)
		{
			return null;
		}
		int num = 0;
		foreach (MMMultipleObjectPoolerObject item in Pool)
		{
			if (testedObject.name.Equals(item.GameObjectToPool.name))
			{
				return item;
			}
			num++;
		}
		return null;
	}

	protected virtual bool PoolObjectEnabled(GameObject testedObject)
	{
		return GetPoolObject(testedObject)?.Enabled ?? false;
	}

	public virtual void EnableObjects(string name, bool newStatus)
	{
		foreach (MMMultipleObjectPoolerObject item in Pool)
		{
			if (name.Equals(item.GameObjectToPool.name))
			{
				item.Enabled = newStatus;
			}
		}
	}

	public virtual void ResetCurrentIndex()
	{
		_currentIndex = 0;
		_currentIndexCounter = 0;
	}
}
