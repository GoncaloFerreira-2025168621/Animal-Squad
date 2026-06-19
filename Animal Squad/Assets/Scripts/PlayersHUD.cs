using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayersHUD : NetworkBehaviour
{
    [Header("Caixas dos outros Players")]
    [SerializeField] private GameObject[] _otherPlayerBoxes;

    [Header("Textos dos outros Players")]
    [SerializeField] private TMP_Text[] _otherPlayerNames;

    [Header("Caixa do Player Atual")]
    [SerializeField] private GameObject _currentPlayerBox;

    [Header("Texto do Player Atual")]
    [SerializeField] private TMP_Text _currentPlayerName;

    [Header("Atualização")]
    [SerializeField] private float _refreshTime = 2f;

    private float _timer;

    public override void OnNetworkSpawn()
    {
        // Esconde as caixas dos outros players
        HideOtherPlayerBoxes();

        // Mostra o nome do jogador atual
        UpdateCurrentPlayerBox();

        // Pede ao Server a lista dos outros players
        if (IsClient)
        {
            RequestPlayersHUDServerRpc();
        }
    }

    private void Update()
    {
        // Só os clients atualizam a UI
        if (IsClient == false)
        {
            return;
        }

        _timer += Time.deltaTime;

        if (_timer >= _refreshTime)
        {
            _timer = 0f;

            // Pede novamente a lista dos players
            RequestPlayersHUDServerRpc();
        }

        // Atualiza sempre o nome do jogador atual
        UpdateCurrentPlayerBox();
    }

    private void UpdateCurrentPlayerBox()
    {
        // Se não tiver caixa, não faz nada
        if (_currentPlayerBox == null || _currentPlayerName == null)
        {
            return;
        }

        // Se ainda não tiver username, esconde
        if (string.IsNullOrWhiteSpace(PlayerSession.Username))
        {
            _currentPlayerBox.SetActive(false);
            _currentPlayerName.text = "";
            return;
        }

        // Mostra a caixa do player atual
        _currentPlayerBox.SetActive(true);
        _currentPlayerName.text = PlayerSession.Username;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestPlayersHUDServerRpc(ServerRpcParams rpcParams = default)
    {
        // Client que pediu a lista
        ulong clientID = rpcParams.Receive.SenderClientId;

        // Lista dos outros jogadores
        List<string> names = new List<string>();

        foreach (ulong connectedClientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            // Não mostra o próprio jogador nas caixas de cima
            if (connectedClientID == clientID)
            {
                continue;
            }

            // Só mostra jogadores com login feito
            if (LoginNetwork.IsClientLoggedInStatic(connectedClientID))
            {
                string username = LoginNetwork.GetUsernameFromClientStatic(connectedClientID);
                names.Add(username);
            }
        }

        // Prepara os 3 nomes possíveis
        FixedString64Bytes name1 = "";
        FixedString64Bytes name2 = "";
        FixedString64Bytes name3 = "";

        int count = names.Count;

        if (count > 3)
        {
            count = 3;
        }

        if (count >= 1)
        {
            name1 = names[0];
        }

        if (count >= 2)
        {
            name2 = names[1];
        }

        if (count >= 3)
        {
            name3 = names[2];
        }

        // Envia só para o client que pediu
        SendPlayersHUDToClient(clientID, count, name1, name2, name3);
    }

    private void SendPlayersHUDToClient(ulong clientID, int count, FixedString64Bytes name1, FixedString64Bytes name2, FixedString64Bytes name3)
    {
        ClientRpcParams targetClient = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };

        UpdateOtherPlayersHUDClientRpc(count, name1, name2, name3, targetClient);
    }

    [ClientRpc]
    private void UpdateOtherPlayersHUDClientRpc(int count, FixedString64Bytes name1, FixedString64Bytes name2, FixedString64Bytes name3, ClientRpcParams clientRpcParams = default)
    {
        // Primeiro esconde todas as caixas
        HideOtherPlayerBoxes();

        // Caixa 1
        if (count >= 1 && _otherPlayerBoxes.Length > 0 && _otherPlayerNames.Length > 0)
        {
            _otherPlayerBoxes[0].SetActive(true);
            _otherPlayerNames[0].text = name1.ToString();
        }

        // Caixa 2
        if (count >= 2 && _otherPlayerBoxes.Length > 1 && _otherPlayerNames.Length > 1)
        {
            _otherPlayerBoxes[1].SetActive(true);
            _otherPlayerNames[1].text = name2.ToString();
        }

        // Caixa 3
        if (count >= 3 && _otherPlayerBoxes.Length > 2 && _otherPlayerNames.Length > 2)
        {
            _otherPlayerBoxes[2].SetActive(true);
            _otherPlayerNames[2].text = name3.ToString();
        }
    }

    private void HideOtherPlayerBoxes()
    {
        // Esconde caixas dos outros players
        for (int i = 0; i < _otherPlayerBoxes.Length; i++)
        {
            if (_otherPlayerBoxes[i] != null)
            {
                _otherPlayerBoxes[i].SetActive(false);
            }
        }

        // Limpa textos dos outros players
        for (int i = 0; i < _otherPlayerNames.Length; i++)
        {
            if (_otherPlayerNames[i] != null)
            {
                _otherPlayerNames[i].text = "";
            }
        }
    }
}