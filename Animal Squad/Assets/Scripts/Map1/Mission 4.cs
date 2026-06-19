using Unity.Netcode;
using UnityEngine;

public class Mission4 : MonoBehaviour
{
    [SerializeField] public bool _Corda = false;
    [SerializeField] public int _Pregos;
    [SerializeField] public int _Torres;
    [SerializeField] public int _Rocks;
    [SerializeField] private ControllerMission _ControllerMission;

    [SerializeField] private GameObject _PortaoInicial;
    [SerializeField] private GameObject _PortaoDestruido;
    [SerializeField] private GameObject _ContraPeso;
    [SerializeField] private GameObject _CordaObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        VerificationMouseBirdBearServerRpc();

        if (_Pregos <= 0)
        {
            ReduzirPregosServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReduzirPregosServerRpc()
    {
        _PortaoInicial.SetActive(false);
        _PortaoDestruido.SetActive(true);
        _ContraPeso.SetActive(false);
        _CordaObject.SetActive(false);
        
        ReduzirPregosClientRpc();
    }

    [ClientRpc]
    private void ReduzirPregosClientRpc()
    {
        _PortaoInicial.SetActive(false);
        _PortaoDestruido.SetActive(true);
        _ContraPeso.SetActive(false);
        _CordaObject.SetActive(false);
    }



    [ServerRpc(RequireOwnership = false)]
    private void VerificationMouseBirdBearServerRpc()
    {
        if (_Pregos <= 0)
        {
            _ControllerMission._CompletMission4 = true;
        }
        else if (_Torres <= 0)
        {
            _ControllerMission._CompletMission4 = true;
        }
        else if (_Rocks <= 0)
        {
            _ControllerMission._CompletMission4 = true;
        }
        else if (_Corda == true)
        {
            _ControllerMission._CompletMission4 = true;
        }
        VerificationMouseBirdBearClientRpc();
    }

    [ClientRpc]
    private void VerificationMouseBirdBearClientRpc()
    {
        if (_Pregos <= 0)
        {
            _ControllerMission._CompletMission4 = true;
        }
        else if (_Torres <= 0)
        {
            _ControllerMission._CompletMission4 = true;
        }
        else if (_Rocks <= 0)
        {
            _ControllerMission._CompletMission4 = true;
        }
        else if (_Corda == true)
        {
            _ControllerMission._CompletMission4 = true;
        }
    }
}
