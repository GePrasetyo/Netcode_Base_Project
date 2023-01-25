using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class GameMenuWidget : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button findButton;
    [SerializeField] private Button stopButton;

    [SerializeField] private TMP_InputField roomNameField;
    [SerializeField] private TMP_InputField nameField;

    [SerializeField] private SessionItemWidget sessionItemPrefab;
    [SerializeField] private Transform contentListSession;

    private List<SessionItemWidget> sessionItems = new List<SessionItemWidget>();
    private Dictionary<System.Net.IPAddress, DiscoveryResponseData> discoveredServers = new Dictionary<System.Net.IPAddress, DiscoveryResponseData>();

    private void Start() {
        hostButton.onClick.AddListener(OnHostClicked);
        findButton.onClick.AddListener(OnFindClicked);
        stopButton.onClick.AddListener(OnStopFindClicked);

        nameField.onEndEdit.AddListener((string name) => GameInstance.myName = name);

        ConnectionHandler.ConnectionEstablished += GameConnected;
        ConnectionHandler.ConnectionShutdown += GameDisConnected;
    }

    private void OnDestroy()
    {
        ConnectionHandler.ConnectionEstablished -= GameConnected;
        ConnectionHandler.ConnectionShutdown -= GameDisConnected;
    }

    private void GameConnected()
    {
        gameObject.SetActive(false);
    }

    private void GameDisConnected()
    {
        gameObject.SetActive(true);
    }

    private void OnSessionFound(System.Net.IPEndPoint sender, DiscoveryResponseData response) {
        discoveredServers[sender.Address] = response;
        Debug.Log($"Name : {response.ServerName} - Address : {sender.Address} - Port : {response.Port}");

        foreach (var iSession in sessionItems) {
            Destroy(iSession.gameObject);
        }
        sessionItems.Clear();

        foreach(var discoveredServer in discoveredServers) {
            var i = Instantiate(sessionItemPrefab, contentListSession);
            sessionItems.Add(i);
            i.Setup(discoveredServer.Value.ServerName, discoveredServer.Key.ToString(), discoveredServer.Value.Port);
        }
    }

    private void OnHostClicked() {
        ServiceLocator.Resolve<GameNetworkManager>().StartAHost(roomNameField.text);
    }

    private void OnFindClicked() {
        findButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);
        discoveredServers.Clear();

        ConnectionHandler.OnServerFound += OnSessionFound;
        ServiceLocator.Resolve<GameNetworkManager>().FindLocalSession();
    }

    private void OnStopFindClicked() {
        findButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(false);

        ConnectionHandler.OnServerFound -= OnSessionFound;
        ServiceLocator.Resolve<GameNetworkManager>().StopSearchSession();
    }
}