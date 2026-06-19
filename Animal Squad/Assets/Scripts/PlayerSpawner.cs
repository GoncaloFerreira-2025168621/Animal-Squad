using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : NetworkBehaviour
{
    [Header("Animal Prefabs")]
    [SerializeField] private GameObject _bearPrefab;
    [SerializeField] private GameObject _beaverPrefab;
    [SerializeField] private GameObject _mousePrefab;
    [SerializeField] private GameObject _birdPrefab;

    [SerializeField] private GameObject _spawnPoint; // Ponto de spawn para os jogadores

    [SerializeField] private SaveAnimal _animalSelection;
    [SerializeField] private AnimalSelection _animalSelection1;

    public override void OnNetworkSpawn()// Este método é chamado quando o objeto de rede é ativado
    {
        if (!IsClient) return;

        RequestSpawnServerRpc(_animalSelection._AnimalSelect);
        //RequestSpawnServerRpc(_animalSelection1._selectedAnimal);
    }

    [ServerRpc(RequireOwnership = false)]
    // Este método é chamado pelos clientes para solicitar o spawn do jogador no servidor
    private void RequestSpawnServerRpc(int animalIndex, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;// Obtém o ID do cliente que fez a solicitação

        GameObject prefabToSpawn = GetAnimalPrefab(animalIndex);// Obtém o prefab do animal com base no índice selecionado

        if (prefabToSpawn == null)
        {
            Debug.LogError("Prefab do animal está vazio no Inspector!");
            return;
        }

        // Calcula a posição de spawn para o jogador, deslocando-a com base no ID do cliente para evitar que os jogadores spawnem no mesmo local
        Vector3 spawnPosition = _spawnPoint.transform.position + new Vector3(clientId * 2f, 0f, 0f);

        GameObject player = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);// Instancia o prefab do animal na posição de spawn

        NetworkObject networkObject = player.GetComponent<NetworkObject>();// Obtém o componente NetworkObject do jogador instanciado

        if (networkObject == null)
        {
            Debug.LogError("O prefab do animal não tem NetworkObject!");
            return;
        }

        networkObject.SpawnAsPlayerObject(clientId, true);// Spawna o objeto de rede como um objeto de jogador para o cliente que fez a solicitação
    }

    private GameObject GetAnimalPrefab(int animalIndex)
    {
        switch (animalIndex)
        {
            case 0:
                return _bearPrefab;

            case 1:
                return _beaverPrefab;

            case 2:
                return _mousePrefab;

            case 3:
                return _birdPrefab;

            default:
                return _bearPrefab;
        }
    }
}