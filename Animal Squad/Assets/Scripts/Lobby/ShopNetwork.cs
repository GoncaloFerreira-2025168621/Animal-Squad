using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class ShopNetwork : NetworkBehaviour
{
    public static ShopNetwork Instance;

    [Header("Servidor Node.js")]
    [SerializeField] private string _serverURL = "http://localhost:3000";

    // Isto so existe no Server.
    // Guarda os animais comprados por cada client.
    private Dictionary<ulong, HashSet<int>> _ownedAnimalsByClient = new Dictionary<ulong, HashSet<int>>();

    // Guarda as moedas de cada client no Server.
    private Dictionary<ulong, int> _coinsByClient = new Dictionary<ulong, int>();

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
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
        // Quando o client sai, limpamos a cache dele
        if (_ownedAnimalsByClient.ContainsKey(clientID))
        {
            _ownedAnimalsByClient.Remove(clientID);
        }

        if (_coinsByClient.ContainsKey(clientID))
        {
            _coinsByClient.Remove(clientID);
        }
    }

    // Usado pelo LobbyRoomManager para confirmar se o jogador pode usar o animal
    public bool ClientOwnsAnimal(ulong clientID, int animalID)
    {
        if (!_ownedAnimalsByClient.ContainsKey(clientID))
            return false;

        return _ownedAnimalsByClient[clientID].Contains(animalID);
    }

    [ServerRpc(RequireOwnership = false)]
    public void LoadShopServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientID = rpcParams.Receive.SenderClientId;

        // Confirma se o client fez login
        if (LoginNetwork.Instance == null || !LoginNetwork.Instance.IsClientLoggedIn(clientID))
        {
            SendShopErrorToClient(clientID, "Tens de fazer login primeiro.");
            return;
        }

        int userID = LoginNetwork.Instance.GetUserIDFromClient(clientID);

        // So o Server fala com o Node.js
        StartCoroutine(LoadShopFromNode(userID, clientID));
    }

    private IEnumerator LoadShopFromNode(int userID, ulong clientID)
    {
        string url = _serverURL + "/shop/" + userID;

        Debug.Log("Server a chamar Shop: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Erro ao carregar shop: " + request.error);
            SendShopErrorToClient(clientID, "Erro ao carregar shop no servidor.");
            yield break;
        }

        string json = request.downloadHandler.text;
        Debug.Log("Resposta Shop: " + json);

        ShopResponse response = JsonUtility.FromJson<ShopResponse>(json);

        if (response.success)
        {
            SaveShopInServerCache(clientID, response);
        }

        SendShopToClient(clientID, json);
    }

    private void SaveShopInServerCache(ulong clientID, ShopResponse response)
    {
        HashSet<int> ownedAnimals = new HashSet<int>();

        if (response.animals != null)
        {
            for (int i = 0; i < response.animals.Length; i++)
            {
                if (response.animals[i].owned == 1)
                {
                    ownedAnimals.Add(response.animals[i].id_animal);
                }
            }
        }

        _ownedAnimalsByClient[clientID] = ownedAnimals;
        _coinsByClient[clientID] = response.coins;
    }

    private void SendShopErrorToClient(ulong clientID, string message)
    {
        ShopResponse response = new ShopResponse();
        response.success = false;
        response.message = message;
        response.coins = 0;
        response.animals = new AnimalShopData[0];

        string json = JsonUtility.ToJson(response);
        SendShopToClient(clientID, json);
    }

    private void SendShopToClient(ulong clientID, string json)
    {
        ClientRpcParams targetClient = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };

        ReceiveShopClientRpc(json, targetClient);
    }

    [ClientRpc]
    private void ReceiveShopClientRpc(string json, ClientRpcParams clientRpcParams = default)
    {
        Shop_Manager shopManager = FindFirstObjectByType<Shop_Manager>();

        if (shopManager != null)
        {
            shopManager.ReceiveShopFromServer(json);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void BuyAnimalServerRpc(int animalID, ServerRpcParams rpcParams = default)
    {
        ulong clientID = rpcParams.Receive.SenderClientId;

        // Confirma se o client fez login
        if (LoginNetwork.Instance == null || !LoginNetwork.Instance.IsClientLoggedIn(clientID))
        {
            SendBuyResultToClient(clientID, false, "Tens de fazer login primeiro.", 0, animalID);
            return;
        }

        int userID = LoginNetwork.Instance.GetUserIDFromClient(clientID);

        // So o Server compra no Node.js
        StartCoroutine(BuyAnimalInNode(userID, animalID, clientID));
    }

    private IEnumerator BuyAnimalInNode(int userID, int animalID, ulong clientID)
    {
        BuyAnimalRequest buyData = new BuyAnimalRequest();
        buyData.userID = userID;
        buyData.animalID = animalID;

        string json = JsonUtility.ToJson(buyData);

        UnityWebRequest request = new UnityWebRequest(_serverURL + "/shop/buy", "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Erro ao comprar animal: " + request.error);
            SendBuyResultToClient(clientID, false, "Erro ao comprar animal no servidor.", 0, animalID);
            yield break;
        }

        Debug.Log("Resposta Compra: " + request.downloadHandler.text);

        BuyAnimalResponse response = JsonUtility.FromJson<BuyAnimalResponse>(request.downloadHandler.text);

        if (response.success)
        {
            // Atualiza cache no Server
            if (!_ownedAnimalsByClient.ContainsKey(clientID))
            {
                _ownedAnimalsByClient[clientID] = new HashSet<int>();
            }

            _ownedAnimalsByClient[clientID].Add(animalID);
            _coinsByClient[clientID] = response.newCoins;
        }

        SendBuyResultToClient(clientID, response.success, response.message, response.newCoins, animalID);
    }

    private void SendBuyResultToClient(ulong clientID, bool success, string message, int newCoins, int animalID)
    {
        ClientRpcParams targetClient = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };

        ReceiveBuyResultClientRpc(success, message, newCoins, animalID, targetClient);
    }

    [ClientRpc]
    private void ReceiveBuyResultClientRpc(bool success, string message, int newCoins, int animalID, ClientRpcParams clientRpcParams = default)
    {
        Shop_Manager shopManager = FindFirstObjectByType<Shop_Manager>();

        if (shopManager != null)
        {
            shopManager.ReceiveBuyResultFromServer(success, message, newCoins, animalID);
        }
    }
}
