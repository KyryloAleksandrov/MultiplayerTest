using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System;

public class NetworkMenuUI : MonoBehaviour
{
    public Button hostButton;
    public Button clientButton;
    public Button shutdownButton;
    public GameObject menuPanel;
    [SerializeField] private string defaultServerIP = "127.0.0.1";

    // Start is called before the first frame update
    void Start()
    {
        hostButton.onClick.AddListener(OnStartHost);
        clientButton.onClick.AddListener(OnStartClient);
        shutdownButton.onClick.AddListener(OnShutdown);

        shutdownButton.gameObject.SetActive(false);

        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnStartHost()
    {
        NetworkManager.Singleton.StartHost();
        menuPanel.SetActive(false);
        shutdownButton.gameObject.SetActive(true);
    }
    private void OnStartClient()
    {
        Unity.Netcode.Transports.UTP.UnityTransport transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        transport.ConnectionData.Address = defaultServerIP;
        NetworkManager.Singleton.StartClient();
        menuPanel.SetActive(false);
        shutdownButton.gameObject.SetActive(true);
    }
    private void OnShutdown()
    {
        NetworkManager.Singleton.Shutdown();
        menuPanel.SetActive(true);
        shutdownButton.gameObject.SetActive(false);
    }
}
