using UnityEngine;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Starting Server...");
            StartServer();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Starting Host...");
            StartHost();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Starting Client...");
            StartClient();
        }
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
