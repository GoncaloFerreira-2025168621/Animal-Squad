using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyNetworkManager : NetworkBehaviour

{
    private ulong _RoomLeaderClientId;// Armazena o ClientId do jogador que criou a sala
    private bool _HasRoomLeader;// Indica se a sala já tem um líder

    [ServerRpc(RequireOwnership = false)]
    public void CreateRoomServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        _RoomLeaderClientId = clientId;
        _HasRoomLeader = true;

        Debug.Log($"O Client {clientId} criou a sala.");
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc(int mapIndex, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        // Apenas o jogador que criou a sala pode iniciar
        if (!_HasRoomLeader || clientId != _RoomLeaderClientId)
        {
            Debug.Log("Este Client não é o líder da sala.");
            return;
        }

        if (!AllPlayersReady())
        {
            Debug.Log("Nem todos os jogadores estão prontos.");
            return;
        }

        string sceneName;

        switch (mapIndex)
        {
            case 1:
                sceneName = "Mapa_1";
                break;

            case 2:
                sceneName = "Mapa_2";
                break;

            default:
                Debug.Log("Mapa inválido.");
                return;
        }

        NetworkManager.SceneManager.LoadScene(sceneName,LoadSceneMode.Single);
    }

    private bool AllPlayersReady()
    {
        // Mais tarde vais verificar aqui o Ready de cada jogador
        return true;
    }
}
