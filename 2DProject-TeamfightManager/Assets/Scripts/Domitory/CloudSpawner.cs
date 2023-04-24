using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using Util.Pool;

public class CloudSpawner : MonoBehaviour
{
	[SerializeField] private Sprite[] _cloudSpriteContainer;
	[SerializeField] private Cloud _cloudPrefab;
	[SerializeField] private Transform _spawnMinPoint;
	[SerializeField] private Transform _spawnMaxPoint;

	private ObjectPooler<Cloud> _cloudPooler;
	private int _cloudSpriteCount = 0;

	public Vector3 randomSpawnPoint
	{
		get
		{
			return new Vector3(
				UnityEngine.Random.Range(_spawnMinPoint.position.x, _spawnMaxPoint.position.x),
				UnityEngine.Random.Range(_spawnMinPoint.position.y, _spawnMaxPoint.position.y),
				0f
				);
		}
	}

	private void Awake()
	{
		_cloudPooler = new ObjectPooler<Cloud>(CreateCloudInstance, null, null, null, 20, 20, 100);

		_cloudSpriteCount = _cloudSpriteContainer.Length;
	}

	private void Start()
	{
		StartCoroutine(StartSpawnCloud());
	}

	private Cloud CreateCloudInstance()
	{
		Cloud cloud = Instantiate<Cloud>(_cloudPrefab, transform);
		cloud.cloudSpawner = this;
		cloud.gameObject.SetActive(false);

		return cloud;
	}

	public void ReleaseCloud(Cloud cloud)
	{
		cloud.gameObject.SetActive(false);
		_cloudPooler.Release(cloud);
	}

	IEnumerator StartSpawnCloud()
	{
		WaitForSeconds waitSecInst = YieldInstructionStore.GetWaitForSec(1.2f);

		while(true)
		{
			yield return waitSecInst;

			Cloud cloud = _cloudPooler.Get();

			int cloudSpriteIndex = UnityEngine.Random.Range(0, _cloudSpriteCount);
			cloud.sprite = _cloudSpriteContainer[cloudSpriteIndex];
			cloud.transform.position = randomSpawnPoint;
			cloud.moveSpeed = UnityEngine.Random.Range(0.5f, 1.5f);
			cloud.gameObject.SetActive(true);
		}
	}
}
