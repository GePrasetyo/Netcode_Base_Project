using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkManager))]
[RequireComponent(typeof(ConnectionHandler))]
public class GameNetworkManager : MonoBehaviour, IGameService {
    // used in ApprovalCheck. This is intended as a bit of light protection against DOS attacks that rely on sending silly big buffers of garbage.
    private const int maxConnectPayload = 256;
    private const int _timeoutDuration = 30;

    private ConnectionHandler connectionHandler;
    private NetworkManager netManager;

    public event System.Action connectingSession;
    public event System.Action connectingFailed;

    private void Awake() {
        ServiceLocator.Register(typeof(GameNetworkManager), this);
        netManager = GetComponent<NetworkManager>();
        connectionHandler = GetComponent<ConnectionHandler>();
    }

    private void Start()
    {
        ConnectionHandler.ConnectionEstablished += OnGameConnected;
        ConnectionHandler.ConnectionShutdown += OnShutdown;
        netManager.ConnectionApprovalCallback += ApprovalCheck;

        Invoke("StartAHost", 5f);
    }

    private void OnDestroy()
    {
        ServiceLocator.RemoveService(this.GetType());
        ConnectionHandler.ConnectionEstablished -= OnGameConnected;
        ConnectionHandler.ConnectionShutdown -= OnShutdown;
        netManager.ConnectionApprovalCallback -= ApprovalCheck;
    }

    public void StartAHost(string roomName = "aswdewas")
    {
        new SessionState(roomName);

        var payload = JsonUtility.ToJson(new ConnectionPayload()
        {
            playerName = GameInstance.myName,
        });
        var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);
        netManager.NetworkConfig.ConnectionData = payloadBytes;
        netManager.NetworkConfig.ClientConnectionBufferTimeout = _timeoutDuration;

        netManager.StartHost();
        connectingSession?.Invoke();
    }

    public void StartJoin(string ip, ushort port)
    {
        connectingSession?.Invoke();

        var chosenTransport = netManager.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        chosenTransport.SetConnectionData(ip, port, "0.0.0.0");

        Debug.Log($"Connecting to{ip}:{port}");
        var payload = JsonUtility.ToJson(new ConnectionPayload() {
            playerName = GameInstance.myName,
        });

        var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);
        netManager.NetworkConfig.ConnectionData = payloadBytes;
        netManager.NetworkConfig.ClientConnectionBufferTimeout = _timeoutDuration;
        netManager.StartClient();
    }

    public void FindLocalSession() {
        connectionHandler.StartClient();
        connectionHandler.ClientBroadcast(new DiscoveryBroadcastData());
    }

    public void StopSearchSession() {
        connectionHandler.StopDiscovery();
    }

    private void OnShutdown()
    {
        Debug.Log("Connection Shutdown");
        NetworkManager.Singleton.ConnectionApprovalCallback = null;
        ServiceLocator.RemoveService(typeof(SessionState));
        connectingFailed?.Invoke();
    }

    private void OnGameConnected()
    {
        Debug.Log("Connection Established");
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        var clientID = request.ClientNetworkId;
        var connectionData = request.Payload;

        var approvalMaxPlayer = request.Payload.Length <= maxConnectPayload;
        Debug.Log($"Someone wants approval : {approvalMaxPlayer}");
        if (!approvalMaxPlayer)
        {
            response.CreatePlayerObject = false;
            response.Approved = false;
            return;
        }

        response.Approved = approvalMaxPlayer;
        response.CreatePlayerObject = true;

        var payload = System.Text.Encoding.UTF8.GetString(connectionData);
        var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);
        //var clientScene = connectionPayload.clientScene;
        //var playerSessionID = connectionPayload.clientGUID;

        Debug.Log("Host ApprovalCheck: connecting client Name: " + connectionPayload.playerName);
        //TODO: Saving the GUID to fully support a reconnect flow (where you get your same character back after disconnect/reconnect).

        //ConnectStatus gameReturnStatus = ConnectStatus.Success;

        //response.PlayerPrefabHash = null;
        //_clientToScene[clientID] = clientScene;
        //_clientIDToGuid[clientID] = connectionPayload.clientGUID;
        //_guidToClientData[connectionPayload.clientGUID] = new PlayerData(connectionPayload.playerName, clientID);

        //ClientRpcParams clientRpcParams = new ClientRpcParams
        //{
        //    Send = new ClientRpcSendParams
        //    {
        //        TargetClientIds = new ulong[] { clientID }
        //    }
        //};
        //TargetRPC[clientID] = clientRpcParams;
        //_portal.ServerToClientConnectResult(clientID, gameReturnStatus);

        //in case useful in the future
        //if (gameReturnStatus != ConnectStatus.Success)
        //    StartCoroutine(WaitToDisconnectClient(clientID, gameReturnStatus));
    }
}