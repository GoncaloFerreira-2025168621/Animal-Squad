using GLTFast.Schema;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyRoomManager : NetworkBehaviour
{
    [Header("Quamtidade de jogadores prontos")]
    [SerializeField] private int _NPlayers;
    [SerializeField] private TMP_Text _Pronto;

    [Header("UI Mapas")]
    [SerializeField] private GameObject _Mapas; // Painel dos mapas
    [SerializeField] private GameObject[] _Maps; //icons Maps 
    [SerializeField] private int _NMap; //1 - Mapa 1; 2 - Mapa 2 etc...

    [Header("Mensagem")]
    [SerializeField] private TMP_Text _Message; // Mensagem para avisar se não for líder

    [Header("Referência ao visual das plataformas")]
    [SerializeField] private AnimalShowLobby _animalShowLobby;// Script que mostra os animais nas plataformas

    [Header("Inputs da Sala")]
    [SerializeField] private TMP_InputField _codeRoomInput;//Campo onde o jogador escreve o codigo da sala
    [SerializeField] private TMP_InputField _passRoomInput;//campo onde o player escreve a password da sala

    [Header("Mensagem da Sala")]
    [SerializeField] private TMP_Text _roomMessageText;//

    // Diz se ESTE jogador já está dentro de uma sala.
    // Isto é local em cada build.
    private bool _isInsideRoom = false;

    // DADOS QUE SÓ O SERVER USa
    public Dictionary<string, RoomData> _rooms = new Dictionary<string, RoomData>(); // Guarda todas as salas criadas
    public Dictionary<ulong, string> _clientRooms = new Dictionary<ulong, string>(); // Guarda em que sala está cada jogador
    public Dictionary<ulong, int> _clientAnimals = new Dictionary<ulong, int>(); // Guarda o animal escolhido por cada jogador

    public override void OnNetworkSpawn()
    {
        if (_animalShowLobby != null)
        {
            _animalShowLobby.HideAllSlots();

            if (IsClient && PlayerSession.SelectedAnimalID > 0)
            {
                _animalShowLobby.ShowLocalPreview(PlayerSession.SelectedAnimalID);
            }
        }

        if (IsServer)
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

    // Chamado pelo Shop_Manager quando o jogador seleciona um animal
    public void UpdateSelectedAnimal()
    {
        if (PlayerSession.SelectedAnimalID <= 0)
            return;

        // Se ainda não está dentro de uma sala, mostra só localmente
        if (!_isInsideRoom)
        {
            if (_animalShowLobby != null)
            {
                _animalShowLobby.ShowLocalPreview(PlayerSession.SelectedAnimalID);
            }

            return;
        }

        // Se já está numa sala, avisa o servidor para atualizar os outros jogadores
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsClient && NetworkManager.Singleton.IsConnectedClient)
        {
            UpdateAnimalServerRpc(PlayerSession.SelectedAnimalID);
        }
    }

    // Botão Criar
    public void CreateRoom()// Chamado pelo botão Criar Sala
    {
        // Confirma se este jogador já está ligado ao servidor Netcode.
        if (!NetworkManager.Singleton.IsClient || !NetworkManager.Singleton.IsConnectedClient)// Verifica se o jogador está ligado ao servidor
        {
            SetLocalMessage("Ainda não estás ligado ao servidor.");
            return;
        }

        // Para criar sala, o jogador tem de ter escolhido animal.
        if (PlayerSession.SelectedAnimalID <= 0)// Verifica se escolheu animal
        {
            SetLocalMessage("Escolhe um animal primeiro.");
            return;
        }
        // Vai buscar o texto escrito no input do código da sala.Trim remove espaços no início/fim.ToUpper transforma em maiúsculas.
        string roomCode = _codeRoomInput.text.Trim().ToUpper();
        // Vai buscar a password escrita.
        string roomPass = _passRoomInput.text.Trim();

        if (string.IsNullOrWhiteSpace(roomCode))// Não deixa criar sala sem código.
        {
            SetLocalMessage("Escreve um código para a sala.");
            return;
        }

        if (roomCode.Length > 30)// FixedString32Bytes tem limite de tamanho.
            roomCode = roomCode.Substring(0, 30);

        if (roomPass.Length > 60)// A password usa FixedString64Bytes.
            roomPass = roomPass.Substring(0, 60);

        // Envia pedido para o Server criar a sala.
        CreateRoomServerRpc(new FixedString32Bytes(roomCode), new FixedString64Bytes(roomPass), PlayerSession.SelectedAnimalID);
    }

    // Botão Entrar
    public void JoinRoom()
    {
        // Confirma se este jogador já está ligado ao servidor Netcode.
        if (!NetworkManager.Singleton.IsClient || !NetworkManager.Singleton.IsConnectedClient)
        {
            SetLocalMessage("Ainda não estás ligado ao servidor.");
            return;
        }

        // Para entrar numa sala, também precisa de ter animal escolhido.
        if (PlayerSession.SelectedAnimalID <= 0)
        {
            SetLocalMessage("Escolhe um animal primeiro.");
            return;
        }

        // Vai buscar código e password escritos pelo jogador.
        string roomCode = _codeRoomInput.text.Trim().ToUpper();
        string roomPass = _passRoomInput.text.Trim();

        // Não deixa entrar sem código.
        if (string.IsNullOrWhiteSpace(roomCode))
        {
            SetLocalMessage("Escreve o código da sala.");
            return;
        }

        if (roomCode.Length > 30)
            roomCode = roomCode.Substring(0, 30);

        if (roomPass.Length > 60)
            roomPass = roomPass.Substring(0, 60);

        // Envia pedido para o Server meter este jogador na sala.
        JoinRoomServerRpc(new FixedString32Bytes(roomCode), new FixedString64Bytes(roomPass), PlayerSession.SelectedAnimalID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CreateRoomServerRpc(FixedString32Bytes roomCode, FixedString64Bytes roomPass, int animalID, ServerRpcParams rpcParams = default)
    {
        // Descobre qual Client chamou este ServerRpc.
        ulong clientID = rpcParams.Receive.SenderClientId;

        string code = roomCode.ToString();
        string pass = roomPass.ToString();

        // O Server confirma se o jogador pode usar este animal
        if (!CanClientUseAnimal(clientID, animalID))
            return;

        // Se já existe uma sala com esse código,não deixa criar outra igual.
        if (_rooms.ContainsKey(code))
        {
            SendMessageToClient(clientID, "Já existe uma sala com esse código.");
            return;
        }
        // Se este jogador já estava noutra sala, removemos ele da sala antiga.
        string oldRoom = RemoveClientFromCurrentRoom(clientID);

        // Se saiu de uma sala antiga, atualizamos essa sala antiga.
        if (!string.IsNullOrEmpty(oldRoom) && _rooms.ContainsKey(oldRoom))
        {
            RefreshRoom(oldRoom);
        }

        RoomData newRoom = new RoomData();// Cria os dados da nova sala
        newRoom.Code = code;// Código da sala.
        newRoom.Password = pass;// Password da sala.
        newRoom.LeaderClientID = clientID;// Quem criou a sala vira líder.
        newRoom.Clients.Add(clientID);// Mete o criador dentro da sala.

        _rooms.Add(code, newRoom); // Guarda a sala no dicionário de salas.

        _clientRooms[clientID] = code;// Guarda que este client está nesta sala.
        _clientAnimals[clientID] = animalID; // Guarda o animal escolhido por este client.

        SendMessageToClient(clientID, "Sala criada: " + code);// Envia mensagem apenas para este client.

        RefreshRoom(code);// Atualiza os visuais dos jogadores dentro desta sala.

    }

    [ServerRpc(RequireOwnership = false)]
    private void JoinRoomServerRpc(FixedString32Bytes roomCode, FixedString64Bytes roomPass, int animalID, ServerRpcParams rpcParams = default)
    {
        ulong clientID = rpcParams.Receive.SenderClientId;

        string code = roomCode.ToString();
        string pass = roomPass.ToString();

        // O Server confirma se o jogador pode usar este animal
        if (!CanClientUseAnimal(clientID, animalID))
            return;

        // Verifica se a sala existe
        if (!_rooms.ContainsKey(code))
        {
            SendMessageToClient(clientID, "Essa sala não existe.");
            return;
        }

        RoomData room = _rooms[code]; // Vai buscar os dados dessa sala.

        // Verifica se a password está certa.
        if (room.Password != pass)
        {
            SendMessageToClient(clientID, "Password errada.");
            return;
        }

        // Limite de 4 jogadores por sala.
        if (room.Clients.Count >= 4)
        {
            SendMessageToClient(clientID, "A sala está cheia.");
            return;
        }



        // Se este client já estava noutra sala, removemos primeiro da sala antiga.
        string oldRoom = RemoveClientFromCurrentRoom(clientID);

        // Atualiza a sala antiga, caso ainda exista.
        if (!string.IsNullOrEmpty(oldRoom) && _rooms.ContainsKey(oldRoom))
        {
            RefreshRoom(oldRoom);
        }

        // Mete este client na nova sala.
        room.Clients.Add(clientID);

        _clientRooms[clientID] = code;// Guarda que este client está nesta sala.
        _clientAnimals[clientID] = animalID;// Guarda o animal escolhido por este client.

        SendMessageToClient(clientID, "Entraste na sala: " + code);

        RefreshRoom(code);// Atualiza todos os jogadores desta sala.
        RefreshReadyText(code);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateAnimalServerRpc(int animalID, ServerRpcParams rpcParams = default)
    {
        ulong clientID = rpcParams.Receive.SenderClientId;

        // O Server confirma se o jogador pode usar este animal
        if (!CanClientUseAnimal(clientID, animalID))
            return;

        _clientAnimals[clientID] = animalID;// Guarda o novo animal escolhido.

        // Se o jogador ainda não está numa sala, o Server não precisa avisar ninguém.
        if (!_clientRooms.ContainsKey(clientID))
            return;

        // Descobre em que sala este client está.
        string roomCode = _clientRooms[clientID];

        RoomData room = _rooms[roomCode];

        // Se mudou de animal, deixa de estar pronto
        if (room.ReadyClients.Contains(clientID))
        {
            room.ReadyClients.Remove(clientID);
        }

        // Atualiza os jogadores dessa sala.
        RefreshRoom(roomCode);
        RefreshReadyText(roomCode);
    }

    private bool CanClientUseAnimal(ulong clientID, int animalID)
    {
        // Nao deixa usar animal invalido
        if (animalID <= 0)
        {
            SendMessageToClient(clientID, "Escolhe um animal primeiro.");
            return false;
        }

        // O Server confirma se existe ShopNetwork
        if (ShopNetwork.Instance == null)
        {
            SendMessageToClient(clientID, "ShopNetwork nao encontrado no servidor.");
            return false;
        }

        // O Server confirma se o animal esta comprado por este client
        if (!ShopNetwork.Instance.ClientOwnsAnimal(clientID, animalID))
        {
            SendMessageToClient(clientID, "Nao tens esse animal comprado.");
            return false;
        }

        return true;
    }

    //ATUALIZAR VISUAL DE UMA SALA
    private void RefreshRoom(string roomCode)
    {
        if (!_rooms.ContainsKey(roomCode))// Se a sala não existe, não faz nada.
            return;

        RoomData room = _rooms[roomCode];

        // Descobre quais animais devem aparecer nas 4 plataformas.
        int animal0 = GetAnimalInRoomSlot(room, 0);
        int animal1 = GetAnimalInRoomSlot(room, 1);
        int animal2 = GetAnimalInRoomSlot(room, 2);
        int animal3 = GetAnimalInRoomSlot(room, 3);

        // Este ClientRpc vai ser enviado apenas para os jogadores desta sala.
        ClientRpcParams targetClients = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = room.Clients.ToArray()
            }
        };

        // Manda os clients desta sala atualizarem o visual.
        UpdateRoomVisualClientRpc(animal0, animal1, animal2, animal3, targetClients);
    }

    private int GetAnimalInRoomSlot(RoomData room, int slot)
    {
        // Se não existe jogador nesta posição, devolve -1, que significa "sem animal".
        if (slot >= room.Clients.Count)
            return -1;

        ulong clientID = room.Clients[slot];// Vai buscar o client que está nesse slot.

        // Se por algum motivo não houver animal guardado, também devolve -1.
        if (!_clientAnimals.ContainsKey(clientID))
            return -1;

        return _clientAnimals[clientID];// Devolve o animal desse client.
    }

    [ClientRpc]
    private void UpdateRoomVisualClientRpc(int animalSlot0, int animalSlot1, int animalSlot2, int animalSlot3, ClientRpcParams clientRpcParams = default)
    {
        _isInsideRoom = true;

        if (_animalShowLobby != null)
        {
            _animalShowLobby.ShowRoomAnimals(animalSlot0, animalSlot1, animalSlot2, animalSlot3);
        }
    }

    private string RemoveClientFromCurrentRoom(ulong clientID)
    {
        // Se este client não está em nenhuma sala, devolve vazio.
        if (!_clientRooms.ContainsKey(clientID))
            return "";

        string oldRoomCode = _clientRooms[clientID];// Guarda o código da sala antiga.

        if (_rooms.ContainsKey(oldRoomCode))
        {
            RoomData oldRoom = _rooms[oldRoomCode];
            oldRoom.ReadyClients.Remove(clientID); // Remove o ready do jogador que saiu

            oldRoom.Clients.Remove(clientID);// Remove este client da lista de jogadores dessa sala.

            // Se o jogador que saiu era líder, passa a liderança para o primeiro jogador que sobrou.
            if (oldRoom.LeaderClientID == clientID && oldRoom.Clients.Count > 0)
            {
                oldRoom.LeaderClientID = oldRoom.Clients[0];
            }

            // Se a sala ficou vazia, apaga a sala do servidor.
            if (oldRoom.Clients.Count == 0)
            {
                _rooms.Remove(oldRoomCode);
            }
        }

        // Remove a informação de que este client estava numa sala.
        _clientRooms.Remove(clientID);

        // Devolve o código da sala antiga, para podermos atualizar essa sala se necessário.
        return oldRoomCode;
    }

    // Chamado automaticamente quando um client se desconecta.
    private void OnClientDisconnected(ulong clientID)
    {
        string oldRoom = RemoveClientFromCurrentRoom(clientID);// Remove o client da sala onde estava.

        // Remove o animal guardado desse client.
        if (_clientAnimals.ContainsKey(clientID))
        {
            _clientAnimals.Remove(clientID);
        }

        // Atualiza a sala antiga caso ela ainda exista.
        if (!string.IsNullOrEmpty(oldRoom) && _rooms.ContainsKey(oldRoom))
        {
            RefreshRoom(oldRoom);
        }
    }

    private void SendMessageToClient(ulong clientID, string message)
    {
        // Prepara um ClientRpc para ir apenas para 1 client.
        ClientRpcParams targetClient = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };

        // Envia a mensagem para esse client.
        RoomMessageClientRpc(new FixedString128Bytes(message), targetClient);
    }

    [ClientRpc]
    private void RoomMessageClientRpc(FixedString128Bytes message, ClientRpcParams clientRpcParams = default)
    {
        SetLocalMessage(message.ToString()); // Mostra a mensagem localmente no client que recebeu.
    }

    private void SetLocalMessage(string message)
    {
        Debug.Log(message);

        if (_roomMessageText != null)
        {
            _roomMessageText.text = message;
        }
    }

    private void RefreshReadyText(string roomCode)
    {
        if (!_rooms.ContainsKey(roomCode))
            return;

        RoomData room = _rooms[roomCode];

        string readyText = room.ReadyClients.Count + "/" + room.Clients.Count + " prontos";

        ClientRpcParams targetClients = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = room.Clients.ToArray()
            }
        };

        UpdateReadyTextClientRpc(new FixedString32Bytes(readyText), targetClients);
    }

    [ClientRpc]
    private void UpdateReadyTextClientRpc(FixedString32Bytes text, ClientRpcParams clientRpcParams = default)
    {
        if (_Pronto != null)
        {
            _Pronto.text = text.ToString();
        }
    }


    //Mostrar painel de mapa

    private void LoadSelectedMap(int mapNumber)
    {
        string sceneName = "";

        if (mapNumber == 1)
        {
            sceneName = "Mapa_1";
        }
        else if (mapNumber == 2)
        {
            sceneName = "Mapa_2";
        }
        else if (mapNumber == 3)
        {
            sceneName = "Mapa_3";
        }
        else if (mapNumber == 4)
        {
            sceneName = "Mapa_4";
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.Log("Mapa inválido.");
            return;
        }

        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void ShowMaps() // Chamado pelo botão Maps
    {
        ShowMapsServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShowMapsServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientID = rpcParams.Receive.SenderClientId; // Quem carregou no botão Maps

        // Verifica se este jogador está dentro de alguma sala
        if (!_clientRooms.ContainsKey(clientID))
        {
            SendMessageToClient(clientID, "Ainda não estás numa sala.");
            return;
        }

        string roomCode = _clientRooms[clientID]; // Sala onde o jogador está

        if (!_rooms.ContainsKey(roomCode))
        {
            SendMessageToClient(clientID, "Sala não encontrada.");
            return;
        }

        RoomData room = _rooms[roomCode];

        // Verifica se quem carregou é o líder da sala
        if (room.LeaderClientID == clientID)
        {
            OpenMapsClientRpc(GetTargetClient(clientID));
        }
        else
        {
            SendMessageToClient(clientID, "Não és o líder da sala.");
        }
    }

    [ClientRpc]
    private void OpenMapsClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (_Mapas != null)
        {
            _Mapas.SetActive(true);
        }
    }

    private ClientRpcParams GetTargetClient(ulong clientID)
    {
        ClientRpcParams targetClient = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };

        return targetClient;
    }

    public void Mapa1()
    {
        SelectMapServerRpc(1);
    }

    public void Mapa2()
    {
        SelectMapServerRpc(2);
    }

    public void Mapa3()
    {
        SelectMapServerRpc(3);
    }

    public void Mapa4()
    {
        SelectMapServerRpc(4);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectMapServerRpc(int mapNumber, ServerRpcParams rpcParams = default)
    {

        ulong clientID = rpcParams.Receive.SenderClientId; // Quem escolheu o mapa

        if (!_clientRooms.ContainsKey(clientID))
        {
            SendMessageToClient(clientID, "Ainda não estás numa sala.");
            return;
        }

        string roomCode = _clientRooms[clientID];
        RoomData room = _rooms[roomCode];

        if (room.LeaderClientID != clientID)
        {
            SendMessageToClient(clientID, "Só o líder pode escolher o mapa.");
            return;
        }

        room.SelectedMap = mapNumber; // Guarda o mapa no Server

        room.ReadyClients.Clear(); // Quando muda o mapa, todos deixam de estar prontos

        // Atualiza o ícone apenas para os jogadores desta sala
        ClientRpcParams targetClients = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = room.Clients.ToArray()
            }
        };


        SendMessageToClient(clientID, "Mapa " + mapNumber + " selecionado.");
        IconeMapLobbyClientRpc(mapNumber, targetClients);


        RefreshReadyText(roomCode);
    }

    [ClientRpc]
    private void IconeMapLobbyClientRpc(int map, ClientRpcParams clientRpcParams = default)
    {
        // Primeiro esconde todos os ícones
        for (int i = 0; i < _Maps.Length; i++)
        {
            if (_Maps[i] != null)
                _Maps[i].SetActive(false);
        }

        // Como map começa em 1, o índice do array é map - 1
        int mapIndex = map - 1;

        if (mapIndex >= 0 && mapIndex < _Maps.Length)
        {
            if (_Maps[mapIndex] != null)
                _Maps[mapIndex].SetActive(true);
        }
    }

    //Colocar todos os players prontos

    public void ReadyGame()
    {
        if (PlayerSession.SelectedAnimalID <= 0)
        {
            SetLocalMessage("Escolhe um animal primeiro.");
            return;
        }

        ReadyGameServerRpc(PlayerSession.SelectedAnimalID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReadyGameServerRpc(int animalID, ServerRpcParams rpcParams = default)
    {
        ulong clientID = rpcParams.Receive.SenderClientId; // Quem carregou Ready

        if (!_clientRooms.ContainsKey(clientID))
        {
            SendMessageToClient(clientID, "Ainda não estás numa sala.");
            return;
        }

        string roomCode = _clientRooms[clientID];
        RoomData room = _rooms[roomCode];

        if (!room.Clients.Contains(clientID))
        {
            SendMessageToClient(clientID, "Não pertences a esta sala.");
            return;
        }

        // O Server confirma se o jogador pode usar este animal
        if (!CanClientUseAnimal(clientID, animalID))
            return;

        _clientAnimals[clientID] = animalID; // Atualiza o animal escolhido

        if (room.ReadyClients.Contains(clientID))
        {
            room.ReadyClients.Remove(clientID); // Se já estava pronto, deixa de estar pronto
            SendMessageToClient(clientID, "Já não estás pronto.");
        }
        else
        {
            room.ReadyClients.Add(clientID); // Marca este jogador como pronto
            SendMessageToClient(clientID, "Estás pronto.");
        }

        RefreshReadyText(roomCode);

        if (room.ReadyClients.Count == room.Clients.Count && room.Clients.Count > 0)
        {
            LoadSelectedMap(room.SelectedMap);
        }
    }


}

// Esta classe representa uma sala. Ela só guarda dados simples da sala.
public class RoomData
{
    public string Code;// Código da sala.
    public string Password;// Password da sala.
    public ulong LeaderClientID; // Client que criou a sala. Mais tarde podes usar isto para deixar só o líder escolher o mapa.
    public List<ulong> Clients = new List<ulong>(); // Lista dos clients que estão dentro desta sala.

    public int SelectedMap = 1; // Mapa escolhido pela sala
    public HashSet<ulong> ReadyClients = new HashSet<ulong>(); // Jogadores prontos
}

