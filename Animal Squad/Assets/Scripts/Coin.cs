using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Guarda todas as moedas que existem neste client
    public static Dictionary<int, Coin> Coins = new Dictionary<int, Coin>();

    [Header("Dados da Moeda")]
    [SerializeField] private int _coinID;

    [Header("Objeto Visual")]
    [SerializeField] private GameObject _visualObject;

    [Header("Trigger")]
    [SerializeField] private Collider _triggerCollider;

    private bool _waitingServer;
    private bool _collectedLocal;

    private void Awake()
    {
        // Se não arrastares o visual, usa o próprio objeto
        if (_visualObject == null)
        {
            _visualObject = gameObject;
        }

        // Se não arrastares o collider, tenta encontrar no objeto
        if (_triggerCollider == null)
        {
            _triggerCollider = GetComponent<Collider>();
        }
    }

    private void OnEnable()
    {
        // Guarda esta moeda na lista pelo ID dela
        Coins[_coinID] = this;

        _waitingServer = false;
        _collectedLocal = false;
    }

    private void OnDisable()
    {
        // Remove a moeda da lista se ela for desativada
        if (Coins.ContainsKey(_coinID) && Coins[_coinID] == this)
        {
            Coins.Remove(_coinID);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Se já estamos à espera do server, não faz nada
        if (_waitingServer)
        {
            return;
        }

        // Se este client já apanhou esta moeda, não faz nada
        if (_collectedLocal)
        {
            return;
        }

        // Procura o NetworkObject do player que tocou na moeda
        NetworkObject playerNetworkObject = other.GetComponentInParent<NetworkObject>();

        if (playerNetworkObject == null)
        {
            return;
        }

        // Só o dono desse player pode pedir para apanhar a moeda
        if (playerNetworkObject.IsOwner == false)
        {
            return;
        }

        // Confirma se existe CoinNetwork
        if (CoinNetwork.Instance == null)
        {
            Debug.Log("CoinNetwork não encontrado na cena.");
            return;
        }

        _waitingServer = true;

        // O client só envia o ID da moeda
        // O valor da moeda é decidido no Server
        CoinNetwork.Instance.CollectCoinServerRpc(_coinID);
    }

    public void ConfirmCollectedLocal(int newCoins)
    {
        _waitingServer = false;
        _collectedLocal = true;

        // Atualiza moedas locais
        PlayerSession.Coins = newCoins;

        // Esconde a moeda só neste client
        HideCoinLocal();
    }

    public void CancelCollectedLocal(string message)
    {
        _waitingServer = false;

        Debug.Log(message);
    }

    private void HideCoinLocal()
    {
        // Desliga o trigger só neste client
        if (_triggerCollider != null)
        {
            _triggerCollider.enabled = false;
        }

        // Esconde o visual só neste client
        if (_visualObject != null)
        {
            _visualObject.SetActive(false);
        }
    }

    public static Coin GetCoinByID(int coinID)
    {
        if (Coins.ContainsKey(coinID))
        {
            return Coins[coinID];
        }

        return null;
    }
}
