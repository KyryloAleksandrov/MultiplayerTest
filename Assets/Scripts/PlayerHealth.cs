using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private NetworkVariable<int> health = new NetworkVariable<int>(100);

    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private float deathDelay = 1f;

    public bool isAlive => health.Value > 0;

    private TMP_Text healthText;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            health.Value = maxHealth;
        }

        if (IsOwner)
        {
            GameObject text = GameObject.Find("HealthText");
            healthText = text.GetComponent<TMPro.TMP_Text>();
            UpdateHUD(health.Value);
        }

        health.OnValueChanged += OnHealthChanged;
        OnHealthChanged(0, health.Value);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner && Input.GetKeyDown(KeyCode.T))
        {
            ToggleHealthServerRpc();
        }
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        if (IsOwner)
        {
            UpdateHUD(newValue);
        }

        if (oldValue > 0 && newValue <= 0)
        {
            StartCoroutine(DeathSequence());
        }
    }

    private void UpdateHUD(int value)
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {value}";
        }
    }
    public void ApplyDamage(int amount)
    {
        if (!IsServer || health.Value <= 0) return;
        health.Value = Mathf.Max(health.Value - amount, 0);
    }
    private IEnumerator DeathSequence()
    {
        if (IsOwner)
        {
            PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            playerMovement.enabled = false;
            PlayerLook playerLook = GetComponent<PlayerLook>();
            playerLook.enabled = false;
        }

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
        else
        {
            foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.enabled = false;
            }
        }

        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void ToggleHealthServerRpc(ServerRpcParams rpcParams = default)
    {
        if (!IsServer) return;

        if (health.Value <= 100)
            health.Value = 1000;
        else
            health.Value = 100;
    }
}
