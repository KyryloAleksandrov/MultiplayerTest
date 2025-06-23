using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyAI : NetworkBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolThreshold = 0.5f;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private float bulletSpeed = 10f;

    [SerializeField] private float shootInterval = 3f;

    [SerializeField] private NavMeshAgent agent;

    private int patrolIndex;
    private float nextShootTime;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        patrolPoints = GameObject.FindGameObjectsWithTag("PatrolPoint").Select(point => point.transform).ToArray();
        nextShootTime = Time.time + shootInterval;
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[0].position);
        }

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Patrol
        if (patrolPoints.Length > 0 && !agent.pathPending && agent.remainingDistance < patrolThreshold)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }

        //Find Nearest Player
        NetworkObject nearestPlayer = null;
        float minimumDistance = float.MaxValue;
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(client.ClientId);
            if (playerObject == null) continue;

            PlayerHealth playerHealth = playerObject.GetComponent<PlayerHealth>();
            if (!playerHealth.isAlive) continue;

            float distance = Vector3.Distance(transform.position, playerObject.transform.position);
            if (distance < minimumDistance)
            {
                minimumDistance = distance;
                nearestPlayer = playerObject;
            }
        }

        //Face and Shoot
        if (nearestPlayer != null)
        {
            Vector3 direction = (nearestPlayer.transform.position - transform.position).normalized;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);

            if (Time.time >= nextShootTime)
            {
                Shoot(direction);
                nextShootTime = Time.time + shootInterval;
            }
        }
    }

    private void Shoot(Vector3 direction)
    {
        Vector3 spawnPosition = muzzlePoint.position;
        Quaternion spawnRotation = muzzlePoint.rotation;
        GameObject instance = Instantiate(bulletPrefab, spawnPosition, spawnRotation);

        NetworkObject networkObject = instance.GetComponent<NetworkObject>();
        networkObject.Spawn();

        Rigidbody rigidbody = instance.GetComponent<Rigidbody>();
        rigidbody.velocity = direction * bulletSpeed;
    }
}
