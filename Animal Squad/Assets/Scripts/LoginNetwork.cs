using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class LoginNetwork : NetworkBehaviour
{
    public static LoginNetwork Instance;

    [Header("Servidor Node.js")]
    [SerializeField] private string _serverURL = "http://localhost:3000";

    // Isto so existe no Server.
    // Guarda os logins no Server mesmo quando muda de cena
    private static Dictionary<ulong, int> _clientUserIDs = new Dictionary<ulong, int>();
    private static Dictionary<ulong, string> _clientUsernames = new Dictionary<ulong, string>();

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        // Quando um client sai, o Server esquece o login dele
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnClientDisconnected(ulong clientID)
    {
        // Remove dados guardados deste client
        if (_clientUserIDs.ContainsKey(clientID))
        {
            _clientUserIDs.Remove(clientID);
        }

        if (_clientUsernames.ContainsKey(clientID))
        {
            _clientUsernames.Remove(clientID);
        }
    }

    // Usado por outros scripts do Server para saber se o client fez login
    public bool IsClientLoggedIn(ulong clientID)
    {
        return IsClientLoggedInStatic(clientID);
    }

    // Usado por outros scripts do Server para ir buscar o UserID real da base de dados
    public int GetUserIDFromClient(ulong clientID)
    {
        return GetUserIDFromClientStatic(clientID);
    }

    public string GetUsernameFromClient(ulong clientID)
    {
        return GetUsernameFromClientStatic(clientID);
    }

    // Versão static para funcionar mesmo se o LoginNetwork mudar de cena
    public static bool IsClientLoggedInStatic(ulong clientID)
    {
        return _clientUserIDs.ContainsKey(clientID);
    }

    // Versão static para ir buscar o UserID
    public static int GetUserIDFromClientStatic(ulong clientID)
    {
        if (_clientUserIDs.ContainsKey(clientID))
        {
            return _clientUserIDs[clientID];
        }

        return -1;
    }

    // Versão static para ir buscar o username
    public static string GetUsernameFromClientStatic(ulong clientID)
    {
        if (_clientUsernames.ContainsKey(clientID))
        {
            return _clientUsernames[clientID];
        }

        return "";
    }

    [ServerRpc(RequireOwnership = false)]
    public void LoginServerRpc(string username, string password, ServerRpcParams rpcParams = default)
    {
        // Guarda quem pediu o login
        ulong clientId = rpcParams.Receive.SenderClientId;

        // So o servidor faz o pedido ao Node.js
        StartCoroutine(SendRequestToNode("/login", username, password, clientId, true));
    }

    [ServerRpc(RequireOwnership = false)]
    public void RegisterServerRpc(string username, string password, ServerRpcParams rpcParams = default)
    {
        // Guarda quem pediu o registo
        ulong clientId = rpcParams.Receive.SenderClientId;

        // So o servidor faz o pedido ao Node.js
        StartCoroutine(SendRequestToNode("/register", username, password, clientId, false));
    }

    private IEnumerator SendRequestToNode(string endpoint, string username, string password, ulong clientId, bool isLogin)
    {
        // Dados enviados para o Node.js
        UserData data = new UserData();
        data.username = username;
        data.password = password;

        // Converte para JSON
        string json = JsonUtility.ToJson(data);

        // Cria o pedido POST
        UnityWebRequest request = new UnityWebRequest(_serverURL + endpoint, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Diz ao Node.js que estamos a enviar JSON
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        // Se o servidor Unity nao conseguir falar com o Node.js
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Erro Node.js: " + request.error);

            SendResultToClient(clientId, false, "Erro ao ligar ao Node.js no servidor", -1, username, isLogin);
            yield break;
        }

        Debug.Log("Resposta Node.js: " + request.downloadHandler.text);

        // Converte resposta do Node.js
        ResponseData response = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);

        // Se for login com sucesso, o Server guarda qual UserID pertence a este client
        if (response.success && isLogin)
        {
            _clientUserIDs[clientId] = response.userID;
            _clientUsernames[clientId] = username;

            Debug.Log("SERVER guardou login | ClientID: " + clientId + " | UserID: " + response.userID);
        }

        // Envia a resposta so para o client que pediu
        SendResultToClient(clientId, response.success, response.message, response.userID, username, isLogin);
    }

    private void SendResultToClient(ulong clientId, bool success, string message, int userID, string username, bool isLogin)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        ReceiveLoginResultClientRpc(success, message, userID, username, isLogin, clientRpcParams);
    }

    [ClientRpc]
    private void ReceiveLoginResultClientRpc(bool success, string message, int userID, string username, bool isLogin, ClientRpcParams clientRpcParams = default)
    {
        // Procura o script do login no client
        Login_Register loginRegister = FindFirstObjectByType<Login_Register>();

        if (loginRegister != null)
        {
            loginRegister.ReceiveLoginResult(success, message, userID, username, isLogin);
        }

        // Se o login deu certo, tenta carregar a shop deste jogador
        if (success && isLogin)
        {
            Shop_Manager shopManager = FindFirstObjectByType<Shop_Manager>();

            if (shopManager != null)
            {
                shopManager.LoadShopFromServer();
            }
        }
    }
}

[System.Serializable]
public class UserData
{
    public string username;
    public string password;
}

[System.Serializable]
public class ResponseData
{
    public bool success;
    public string message;
    public int userID;
}

