using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RangedSpawner : MonoBehaviour
{
    public GameObject rangedEnemyPrefab;
    public Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 5f;
    private bool _spawningStarted = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitializeSpawner());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator InitializeSpawner()
    {
        while (NetworkManager.Singleton == null)
        {
            yield return null;
        }

        NetworkManager.Singleton.OnServerStarted += OnServerStarted;

        if (NetworkManager.Singleton.IsServer)
            OnServerStarted();
    }

    private void OnServerStarted()
    {
        if (_spawningStarted) return;
        _spawningStarted = true;

        if (!NetworkManager.Singleton.IsServer) return;

        StartCoroutine(SpawnLoop());
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(spawnInterval);

        while (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    private void SpawnEnemy()
    {
        if (rangedEnemyPrefab == null || spawnPoints.Length == 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject instance = Instantiate(rangedEnemyPrefab, spawnPoint.position, Quaternion.identity);
        instance.GetComponent<NetworkObject>().Spawn();
    }
}
