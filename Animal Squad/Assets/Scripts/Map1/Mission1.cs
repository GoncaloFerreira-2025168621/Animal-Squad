using Unity.Netcode;
using UnityEngine;

public class Mission1 : MonoBehaviour
{
    [SerializeField] private ControllerMission _ControllerMission;

    [Header("Rato")]
    [SerializeField] public int _Luzes;
    [SerializeField] public int _Carrinha;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        VerificationMouseBirdBearServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void VerificationMouseBirdBearServerRpc()
    {
        if (_Luzes <= 0 && _Carrinha <= 0)
        {
            _ControllerMission._CompletMission1 = true;
        }
        VerificationMouseBirdBearClientRpc();
    }

    [ClientRpc]
    private void VerificationMouseBirdBearClientRpc()
    {
        if (_Luzes <= 0 && _Carrinha <= 0)
        {
            _ControllerMission._CompletMission1 = true;
        }
    }


}
