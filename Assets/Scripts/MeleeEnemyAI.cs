using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyAI : NetworkBehaviour
{

    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;

    [SerializeField] private NavMeshAgent agent;
    private float lastAttackTime;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
        }
    }

    void Awake()
    {
        agent.stoppingDistance = attackRange;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;

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
        if (nearestPlayer == null) return;

        //Attack
        if (minimumDistance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(nearestPlayer.transform.position);
        }
        else
        {
            agent.isStopped = true;

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;

                PlayerHealth playerHealth = nearestPlayer.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.ApplyDamage(attackDamage);
                }
            }
        }
    }
}
