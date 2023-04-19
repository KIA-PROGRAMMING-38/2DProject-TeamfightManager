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

	private List<ObjectPooler<Projectile>> _projectilePoolerContainer;

	private void Awake()
	{
		_projectilePoolerContainer = new List<ObjectPooler<Projectile>>();
	}
}