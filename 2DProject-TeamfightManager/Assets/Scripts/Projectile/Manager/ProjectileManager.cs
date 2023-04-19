using System.Collections.Generic;
using UnityEngine;
using Util.Pool;

public class ProjectileManager : MonoBehaviour
{
	public GameManager gameManager
	{
		set 
		{
			_gameManager = value;
		}
	}

	private GameManager _gameManager;

	private Dictionary<string, ObjectPooler<Projectile>> _projectilePoolerContainer;

	private void Awake()
	{
		int loopCount = _gameManager.gameGlobalData.spawnObjectGolbalData.projectilePrefabContainer.Length;
		_projectilePoolerContainer = new Dictionary<string, ObjectPooler<Projectile>>(loopCount);

		for ( int i = 0; i < loopCount; ++i)
		{
			Projectile prefab = _gameManager.gameGlobalData.spawnObjectGolbalData.projectilePrefabContainer[i].GetComponent<Projectile>();

			ObjectPooler<Projectile> pooler = new ObjectPooler<Projectile>(
				() => CreateProjectile(prefab),
				null, ReleaseProjectile, null, 5, 2, 100);

			_projectilePoolerContainer.Add(prefab.projectileName, pooler);
		}
	}

	private Projectile CreateProjectile(Projectile prefab)
	{
		Projectile newProjectile = Instantiate<Projectile>(prefab);

		return newProjectile;
	}

	private void ReleaseProjectile(Projectile projectile)
	{
		projectile.gameObject.SetActive(false);
	}
}