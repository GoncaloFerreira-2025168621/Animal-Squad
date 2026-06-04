using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class to_push : NetworkBehaviour
{
    [SerializeField] private GameObject _Point_Push; // Ponto onde detecta o objeto para empurrar

    void Start()
    {
        // Começa desligado em todas as máquinas
        _Point_Push.SetActive(false);
    }

    void Update()
    {
        // Só o dono deste player pode controlar este script
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            PushObjectServerRpc(true);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            PushObjectServerRpc(false);
        }
    }

    [ServerRpc]
    private void PushObjectServerRpc(bool active)
    {
        // Atualiza no servidor
        _Point_Push.SetActive(active);

        // Manda atualizar em todos os clients
        PushObjectClientRpc(active);
    }

    [ClientRpc]
    private void PushObjectClientRpc(bool active)
    {
        // Atualiza nos clients
        _Point_Push.SetActive(active);
    }
}
