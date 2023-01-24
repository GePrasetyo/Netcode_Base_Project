using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SessionItemWidget : MonoBehaviour
{
    [SerializeField] private Button joinButton;
    [SerializeField] private TextMeshProUGUI roomNameText;
    
    private string ipLocation;
    private ushort portLocation;

    public void Setup(string roomName, string ip, ushort port) {
        roomNameText.text = roomName;
        ipLocation = ip;
        portLocation = port;

        joinButton.onClick.AddListener(OnJoinClicked);
    }

    private void OnJoinClicked() {
        ServiceLocator.Resolve<GameNetworkManager>().StartJoin(ipLocation, portLocation);
    }
}