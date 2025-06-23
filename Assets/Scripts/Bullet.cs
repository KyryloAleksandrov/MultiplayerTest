using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 5f;

    private Rigidbody rigidBody;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        rigidBody.useGravity = false; 
        Invoke(nameof(DestroySelf), lifetime);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        if (collision.collider.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            playerHealth.ApplyDamage(damage);
        }
        else if (collision.collider.TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth))
        {
            enemyHealth.MarkDead();
        }

        DestroySelf();
    }

    private void DestroySelf()
    {
        if (NetworkObject.IsSpawned)
            NetworkObject.Despawn(true);
    }

}
