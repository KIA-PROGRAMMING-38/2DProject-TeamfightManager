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

			int loopCount = _gameManager.gameGlobalData.spawnObjectGolbalData.projectilePrefabContainer.Length;
			_projectilePoolerContainer = new Dictionary<string, ObjectPooler<Projectile>>(loopCount);

			for (int i = 0; i < loopCount; ++i)
			{
				Projectile prefab = _gameManager.gameGlobalData.spawnObjectGolbalData.projectilePrefabContainer[i].GetComponent<Projectile>();

				ObjectPooler<Projectile> pooler = new ObjectPooler<Projectile>(
					() => CreateProjectile(prefab),
					null, OnReleaseProjectile, null, 5, 2, 100);

				_projectilePoolerContainer.Add(prefab.projectileName, pooler);
			}
		}
	}

	private GameManager _gameManager;

	private Dictionary<string, ObjectPooler<Projectile>> _projectilePoolerContainer;

	private void Awake()
	{
		Champion.s_projectileManager = this;
	}

	public Projectile GetProjectile(string projectileName)
	{
		return _projectilePoolerContainer[projectileName].Get();
	}

	public void ReleaseProjectile(Projectile projectile)
	{
		_projectilePoolerContainer[projectile.projectileName].Release(projectile);
	}

	private Projectile CreateProjectile(Projectile prefab)
	{
		Projectile newProjectile = Instantiate<Projectile>(prefab);
		newProjectile.projectileManager = this;

		return newProjectile;
	}

	private void OnReleaseProjectile(Projectile projectile)
	{
		projectile.gameObject.SetActive(false);
	}
}