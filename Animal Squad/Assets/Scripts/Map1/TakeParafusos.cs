using Unity.Netcode;
using UnityEngine;

public class TakeParafusos : NetworkBehaviour
{
    [SerializeField] private Mission4 _Mission;

    [SerializeField] private GameObject _Parafuso;


    private Vector3 _PsitionParafusoAtual;
    private Vector3 _PsitionParafusoInicial;

    private bool _RemoveParafusoAtivada;

    [SerializeField] private float _DistanciaParaAtivar = 0.1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _PsitionParafusoInicial = _Parafuso.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        _PsitionParafusoAtual = _Parafuso.transform.position;

        float distancia1 = Vector3.Distance(_PsitionParafusoAtual, _PsitionParafusoInicial);

        if (distancia1 > _DistanciaParaAtivar && _RemoveParafusoAtivada == false)//Verifica se a bateria moveu - Usando a posição atual com a posição Inicial
        {
            ReduzirPregosServerRpc();
            _RemoveParafusoAtivada = true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReduzirPregosServerRpc()
    {
        _Mission._Pregos--;
        ReduzirPregosClientRpc();
    }

    [ClientRpc]
    private void ReduzirPregosClientRpc()
    {
        _Mission._Pregos--;
    }
}
