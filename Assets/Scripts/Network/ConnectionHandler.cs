using System;
using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
public class ConnectionHandler : NetworkDiscovery<DiscoveryBroadcastData, DiscoveryResponseData>
{
    public static event Action ConnectionEstablished;
    public static event Action ConnectionShutdown;
    private NetworkManager netManager;
    public static event Action<IPEndPoint, DiscoveryResponseData> OnServerFound;

    void Awake() {
        netManager = GetComponent<NetworkManager>();
        netManager.OnServerStarted += OnServerStart;
        netManager.OnClientConnectedCallback += ClientNetworkReadyWrapper;
        netManager.OnClientDisconnectCallback += OnClientDisconnected;
    }

    void OnDestroy() {
        netManager.OnServerStarted -= OnServerStart;
        netManager.OnClientConnectedCallback -= ClientNetworkReadyWrapper;
        netManager.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (clientId == netManager.LocalClientId)
        {
            Debug.Log("I'm disconnected");
            ConnectionShutdown?.Invoke();
            StopDiscovery();
        }
    }

    private void OnServerStart()
    {
        ConnectionEstablished?.Invoke();
        StartServer();
    }

    private void ClientNetworkReadyWrapper(ulong clientId)
    {
        if (netManager.IsServer)
            return;

        if (clientId == netManager.LocalClientId)
        {
            Debug.Log("I'm(client) success connect to Server");
            ConnectionEstablished?.Invoke();
        }
    }

    protected override bool ProcessBroadcast(IPEndPoint sender, DiscoveryBroadcastData broadCast, out DiscoveryResponseData response) {

        Debug.Log($"Broadcast my Session {sender.Address} -- {sender.Port}");
        var sessionProp = ServiceLocator.Resolve<SessionState>();
        response = new DiscoveryResponseData() {
            ServerName = sessionProp.sessionName,
            Port = ((UnityTransport)netManager.NetworkConfig.NetworkTransport).ConnectionData.Port,
        };
        return true;
    }

    protected override void ResponseReceived(IPEndPoint sender, DiscoveryResponseData response) {
        OnServerFound?.Invoke(sender, response);
    }
}

[Serializable]
public class ConnectionPayload
{
    //public string clientGUID;
    //public string clientScene = "";
    public string playerName;
}

//public class DisconnectStatus
//{
//    /// <summary>
//    /// When a disconnect is detected (or expected), set this to provide some context for why it occurred.
//    /// </summary>
//    public void SetDisconnectReason(ConnectStatus reason)
//    {
//        //using an explicit setter here rather than the auto-property, to make the code locations where disconnect information is set more obvious.
//        Debug.Assert(reason != ConnectStatus.Success);
//        Reason = reason;
//    }

//    /// <summary>
//    /// The reason why a disconnect occurred, or Undefined if not set.
//    /// </summary>
//    public ConnectStatus Reason { get; private set; } = ConnectStatus.Undefined;

//    /// <summary>
//    /// Clear the DisconnectReason, returning it to Undefined.
//    /// </summary>
//    public void Clear()
//    {
//        Reason = ConnectStatus.Undefined;
//    }

//    /// <summary>
//    /// Has a TransitionReason already be set? (The TransitionReason provides context for why someone transition back to the MainMenu, and is a one-use item
//    /// that is unset as soon as it is read).
//    /// </summary>
//    public bool HasTransitionReason => Reason != ConnectStatus.Undefined;
//}