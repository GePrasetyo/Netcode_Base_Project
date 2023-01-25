using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class PlayerState : NetworkBehaviour
{
    public NetworkVariable<float> hitPoint = new NetworkVariable<float>();
    public NetworkVariable<float> energy = new NetworkVariable<float>();
    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

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