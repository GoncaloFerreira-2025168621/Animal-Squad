using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class CoinNetwork : NetworkBehaviour
{
    public static CoinNetwork Instance;

    [Header("Servidor Node.js")]
    [SerializeField] private string _serverURL = "http://localhost:3000";

    [Header("Valor das moedas")]
    [SerializeField] private int _defaultCoinValue = 20;

    // Guarda moedas apanhadas nesta sessão por cada client
    private Dictionary<ulong, HashSet<int>> _coinsCollectedByClient = new Dictionary<ulong, HashSet<int>>();

    // Guarda as moedas atuais de cada client no server
    private Dictionary<ulong, int> _coinsByClient = new Dictionary<ulong, int>();

    [Header("UI Moedas")]
    [SerializeField] private TMP_Text _coinsText;

    private void Awake()
    {
        Instance = this;
        _coinsText.text = PlayerSession.Coins.ToString();
    }

    public void Update()
    {
        _coinsText.text = PlayerSession.Coins.ToString();
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
        // Limpa dados quando o client sai
        if (_coinsCollectedByClient.ContainsKey(clientID))
        {
            _coinsCollectedByClient.Remove(clientID);
        }

        if (_coinsByClient.ContainsKey(clientID))
        {
            _coinsByClient.Remove(clientID);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CollectCoinServerRpc(int coinID, ServerRpcParams rpcParams = default)
    {
        // Descobre qual client pediu para apanhar a moeda
        ulong clientID = rpcParams.Receive.SenderClientId;

        // Confirma se o client fez login no Server
        if (LoginNetwork.IsClientLoggedInStatic(clientID) == false)
        {
            Debug.Log("Client tentou apanhar moeda sem login no Server | ClientID: " + clientID);

            SendCoinResultToClient(clientID, false, "Tens de fazer login primeiro.", 0, coinID);
            return;
        }

        // Impede apanhar a mesma moeda duas vezes na mesma sessão
        if (_coinsCollectedByClient.ContainsKey(clientID) &&
            _coinsCollectedByClient[clientID].Contains(coinID))
        {
            int coins = 0;

            if (_coinsByClient.ContainsKey(clientID))
            {
                coins = _coinsByClient[clientID];
            }

            SendCoinResultToClient(clientID, true, "Moeda já apanhada.", coins, coinID);
            return;
        }

        // O Server vai buscar o UserID real
        int userID = LoginNetwork.GetUserIDFromClientStatic(clientID);

        // O valor da moeda é decidido pelo Server
        int coinValue = _defaultCoinValue;

        // Só o Server fala com Node.js
        StartCoroutine(SendCoinToNode(userID, clientID, coinID, coinValue));
    }

    private IEnumerator SendCoinToNode(int userID, ulong clientID, int coinID, int coinValue)
    {
        // Dados enviados para o Node.js
        CoinCollectRequest data = new CoinCollectRequest();
        data.userID = userID;
        data.coinID = coinID;
        data.value = coinValue;

        string json = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(_serverURL + "/coins/collect", "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Erro ao guardar moeda: " + request.error);

            SendCoinResultToClient(clientID, false, "Erro ao guardar moeda no servidor.", 0, coinID);
            yield break;
        }

        Debug.Log("Resposta moeda: " + request.downloadHandler.text);

        CoinCollectResponse response = JsonUtility.FromJson<CoinCollectResponse>(request.downloadHandler.text);

        if (response.success)
        {
            // Guarda no Server que este client já apanhou esta moeda
            if (_coinsCollectedByClient.ContainsKey(clientID) == false)
            {
                _coinsCollectedByClient[clientID] = new HashSet<int>();
            }

            _coinsCollectedByClient[clientID].Add(coinID);

            // Atualiza moedas guardadas no Server
            _coinsByClient[clientID] = response.newCoins;
        }

        // Responde só ao client que apanhou a moeda
        SendCoinResultToClient(clientID, response.success, response.message, response.newCoins, coinID);
    }

    private void SendCoinResultToClient(ulong clientID, bool success, string message, int newCoins, int coinID)
    {

        ClientRpcParams targetClient = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };

        ReceiveCoinResultClientRpc(success, message, newCoins, coinID, targetClient);
    }

    [ClientRpc]
    private void ReceiveCoinResultClientRpc(bool success, string message, int newCoins, int coinID, ClientRpcParams clientRpcParams = default)
    {
        // Procura a moeda neste client
        Coin coin = Coin.GetCoinByID(coinID);

        if (coin == null)
        {
            return;
        }

        if (success)
        {
            // Esconde só neste client
            coin.ConfirmCollectedLocal(newCoins);
        }
        else
        {
            coin.CancelCollectedLocal(message);
        }
    }


}

[System.Serializable]
public class CoinCollectRequest
{
    public int userID;
    public int coinID;
    public int value;
}

[System.Serializable]
public class CoinCollectResponse
{
    public bool success;
    public string message;
    public int newCoins;
}

