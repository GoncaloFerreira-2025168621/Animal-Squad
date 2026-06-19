using Unity.Netcode;
using UnityEngine;

public class TorresVigia : NetworkBehaviour
{

    [SerializeField] private Mission4 _Mission;

    [SerializeField] private bool _Torre2Destroy = false;
    [SerializeField] private bool _Torre1Destroy = false;


    [Header("Corda e Contrapeso")]
    [SerializeField] private GameObject _ContraPesoCaido;
    [SerializeField] private GameObject _ContraPeso;

    [SerializeField] private GameObject _Torre2;
    [SerializeField] private GameObject _TorreCaida2;
    [SerializeField] private GameObject _Pilar2;

    [SerializeField] private GameObject _Torre1;
    [SerializeField] private GameObject _TorreCaida1;
    [SerializeField] private GameObject _Pilar1;

    [SerializeField] private int _TorresCaidas;

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
        VerificationTorre2ServerRpc();
        VerificationTorre1ServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void VerificationTorre2ServerRpc()
    {
        if (_Pilar2 == null && _Torre2Destroy == false)
        {
            _Mission._Torres--;
            _TorreCaida2.SetActive(true);
            _Torre2.SetActive(false);
            _PortaoFechado.SetActive(true);
            _Portao.SetActive(false);
            _ContraPeso.SetActive(false);
            _ContraPesoCaido.SetActive(true);
            _Torre2Destroy = true;
        }
        VerificationTorre2ClientRpc();
    }

    [ClientRpc]
    private void VerificationTorre2ClientRpc()
    {
        if (_Pilar2 == null && _Torre2Destroy == false)
        {
            _Mission._Torres--;
            _TorreCaida2.SetActive(true);
            _Torre2.SetActive(false);
            _ContraPeso.SetActive(false);
            _Portao.SetActive(false);
            _PortaoFechado.SetActive(true);
            _ContraPesoCaido.SetActive(true);
            _Torre2Destroy = true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void VerificationTorre1ServerRpc()
    {
        if (_Pilar1 == null && _Torre1Destroy == false)
        {
            _Mission._Torres--;
            _TorreCaida1.SetActive(true);
            _Torre1.SetActive(false);
            _Torre1Destroy = true;
        }
        VerificationTorre1ClientRpc();
    }

    [ClientRpc]
    private void VerificationTorre1ClientRpc()
    {
        if (_Pilar1 == null && _Torre1Destroy == false)
        {
            _Mission._Torres--;
            _TorreCaida1.SetActive(true);
            _Torre1.SetActive(false);
            _Torre1Destroy = true;
        }
    }
}
