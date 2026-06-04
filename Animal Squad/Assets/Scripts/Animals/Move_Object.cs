using UnityEngine;
using Unity.Netcode;

public class Move_Object : MonoBehaviour
{
    private Rigidbody rb;
    public bool _Moved = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_Moved == true)
        {
            MoveObjectServerRPC(true);
        }
        else if (_Moved == false)
        {
            MoveObjectServerRPC(false);
        }
    }

    [ServerRpc]
    public void MoveObjectServerRPC(bool canMove)
    {
        MoveObjectClientRPC(canMove); // Chama o ClientRpc para atualizar o estado em todos os clientes

        if (canMove)
        {
            rb.isKinematic = false; // Permite que o objeto seja afetado pela física
        }
        else
        {
            rb.isKinematic = true; // Impede que o objeto seja afetado pela física
        }
    }

    [ClientRpc]
    public void MoveObjectClientRPC(bool canMove)
    {
        if (canMove)
        {
            rb.isKinematic = false; // Permite que o objeto seja afetado pela física
        }
        else
        {
            rb.isKinematic = true; // Impede que o objeto seja afetado pela física
        }
    }
}
