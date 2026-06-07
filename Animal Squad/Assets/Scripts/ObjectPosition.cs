using Unity.Netcode;
using UnityEngine;

public class ObjectPosition : NetworkBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc]
    public void UpdateObjectPositionServerRpc(Vector3 newPosition)
    {
        // Lógica para atualizar a posição do objeto, como mover o objeto para a nova posição
        transform.position = newPosition;// Atualiza a posição do objeto para a nova posição recebida
        Debug.Log("Posição do objeto atualizada para: " + newPosition);
        UpdateObjectPositionClientRpc(newPosition); // Chama o método para atualizar a posição do objeto em todos os clientes
    }

    [ClientRpc]
    public void UpdateObjectPositionClientRpc(Vector3 newPosition)
    {
        // Lógica para atualizar a posição do objeto em todos os clientes, como mover o objeto para a nova posição
        transform.position = newPosition;// Atualiza a posição do objeto para a nova posição recebida
        Debug.Log("Posição do objeto atualizada para: " + newPosition);
    }
}
