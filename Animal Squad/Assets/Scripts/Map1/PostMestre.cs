using Unity.Netcode;
using UnityEngine;

public class PostMestre : NetworkBehaviour
{
    [SerializeField] private Mission1 _Mission;

    [Header("Barris")]
    [SerializeField] private GameObject _Pilar;
    [SerializeField] private bool _PilarDestroy = false;

    [Header("Casa")]
    [SerializeField] private GameObject _Home;
    [SerializeField] private GameObject _HomeDestroy;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        VerificationBarrilServerRpc();
    }


    [ServerRpc(RequireOwnership = false)]
    private void VerificationBarrilServerRpc()
    {
        if (_Pilar == null && _PilarDestroy == false)
        {
            _Mission._Luzes = _Mission._Luzes - 3;
            _Mission._Carrinha--;
            _Home.SetActive(false);
            _HomeDestroy.SetActive(true);
            _PilarDestroy = true;
        }
        VerificationBarrrilClientRpc();
    }

    [ClientRpc]
    private void VerificationBarrrilClientRpc()
    {
        if (_Pilar == null && _PilarDestroy == false)
        {
            _Mission._Luzes = _Mission._Luzes - 3;
            _Mission._Carrinha--;
            _Home.SetActive(false);
            _HomeDestroy.SetActive(true);
            _PilarDestroy = true;
        }
    }
}
