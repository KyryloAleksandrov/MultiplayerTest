using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : NetworkBehaviour
{
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private float deathDelay = 1f;

    [SerializeField] private NavMeshAgent agent;

    private NetworkVariable<bool> isDead = new NetworkVariable<bool>(false);

    private MeleeEnemyAI meleeAI;
    private RangedEnemyAI rangedAI;

    public override void OnNetworkSpawn()
    {
        meleeAI = GetComponent<MeleeEnemyAI>();
        rangedAI = GetComponent<RangedEnemyAI>();

        isDead.OnValueChanged += (oldValue, newValue) =>
        {
            if (newValue) StartCoroutine(DeathSequence());
        };
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MarkDead()
    {
        if (!IsServer || isDead.Value) return;
        isDead.Value = true;
    }
    
    private IEnumerator DeathSequence()
    {
        if (meleeAI  != null) meleeAI.enabled  = false;
        if (rangedAI != null) rangedAI.enabled = false;

        agent.updateRotation = false;
        agent.enabled = false;

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        transform.Rotate(0f, 0f, 90f, Space.Self);

        yield return new WaitForSeconds(deathDelay);

        if (IsServer)
        {
            GetComponent<NetworkObject>().Despawn(true);
        }
    }
}
