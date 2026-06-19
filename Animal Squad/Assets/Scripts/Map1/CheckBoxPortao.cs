using Unity.Netcode;
using UnityEngine;

public class CheckBoxPortao : NetworkBehaviour
{
    [SerializeField] private Mission4 _Mission;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pedra"))
        {
            RocksServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RocksServerRpc()
    {
        _Mission._Rocks--;
        RocksClientRpc();
    }

    [ClientRpc]
    private void RocksClientRpc()
    {
        _Mission._Rocks--;
    }
}
