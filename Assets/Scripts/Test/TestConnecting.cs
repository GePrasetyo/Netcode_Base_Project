using UnityEngine;
using Unity.Netcode;

public class TestConnecting : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("----------------");
            NetworkManager.Singleton.StartClient();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("sRAAERWASXDAW");
            NetworkManager.Singleton.StartHost();
        }
    }
}
