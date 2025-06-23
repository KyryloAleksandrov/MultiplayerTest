using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float shootCooldown = 0.5f;

    private float nextShootTime;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time < nextShootTime) return;

        if (Input.GetButtonDown("Fire1"))
        {
            nextShootTime = Time.time + shootCooldown;

            Vector3 spawnPosition = muzzlePoint.position;
            Vector3 direction = muzzlePoint.forward;

            ShootServerRpc(spawnPosition, direction);
        }
    }

    [ServerRpc]
    void ShootServerRpc(Vector3 spawnPosition, Vector3 direction)
    {
        GameObject instance = Instantiate(bulletPrefab, spawnPosition, Quaternion.LookRotation(direction));
        instance.GetComponent<NetworkObject>().Spawn();

        Rigidbody rigidbody = instance.GetComponent<Rigidbody>();
        rigidbody.velocity = direction * bulletSpeed;
    }
}
