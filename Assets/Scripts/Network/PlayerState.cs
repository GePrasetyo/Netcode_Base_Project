using UnityEngine;
using Unity.Netcode;

public class PlayerState : NetworkBehaviour
{
    public NetworkVariable<float> hitPoint = new NetworkVariable<float>();
    public NetworkVariable<float> energy = new NetworkVariable<float>();
    public NetworkVariable<string> playerName = new NetworkVariable<string>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }
}

public struct PlayerData
{
    public string PlayerName;  //name of the player
    public ulong ClientID; //the identifying id of the client

    public PlayerData(string playerName, ulong clientId)
    {
        PlayerName = playerName;
        ClientID = clientId;
    }
}