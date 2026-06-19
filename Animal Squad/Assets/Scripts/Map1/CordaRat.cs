using Unity.Netcode;
using UnityEngine;

public class CordaRat : NetworkBehaviour
{
    [SerializeField] private Mission4 _Mission;

    [SerializeField] private bool _CableDestroy = false;

    [Header("Corda e Contrapeso")]
    [SerializeField] private GameObject _ContraPesoCaido;
    [SerializeField] private GameObject _ContraPeso;
    [SerializeField] private GameObject _Corda;
    [SerializeField] private GameObject _CordaCaida;

    [Header("Portão")]
    [SerializeField] private GameObject _Portao;
    [SerializeField] private GameObject _PortaoFechado;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        VerificationCordaServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void VerificationCordaServerRpc()
    {
        if (_Corda == null && _CableDestroy == false)
        {
            _Mission._Corda = true;
            _CordaCaida.SetActive(true);
            _PortaoFechado.SetActive(true);
            _Portao.SetActive(false);
            _ContraPeso.SetActive(false);
            _ContraPesoCaido.SetActive(true);
            _CableDestroy = true;
        }
        VerificationCordaClientRpc();
    }

    [ClientRpc]
    private void VerificationCordaClientRpc()
    {
        if (_Corda == null && _CableDestroy == false)
        {
            _Mission._Corda = true;
            _CordaCaida.SetActive(true);
            _ContraPeso.SetActive(false);
            _Portao.SetActive(false);
            _PortaoFechado.SetActive(true);
            _ContraPesoCaido.SetActive(true);
            _CableDestroy = true;
        }
    }
}
