using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class RomeveBate : NetworkBehaviour
{
    [SerializeField] private Mission1 _Mission;

    [SerializeField] private GameObject _Bat1;
    [SerializeField] private GameObject _Bat2;

    [Header("Efeitos de luz para desativar no final")]
    [SerializeField] private GameObject[] _Luzes;

    [Header("Tempo entre cada peça aparecer")]
    [SerializeField] private float _DelayBetweenPieces = 0.25f;

    private Vector3 _PsitionBat1Atual;
    private Vector3 _PsitionBat2Atual;

    private Vector3 _PsitionBat1Inicial;
    private Vector3 _PsitionBat2Inicial;

    private bool _RemoveBat1Ativada;
    private bool _RemoveBat2Ativada;

    [SerializeField] private int _BatRemoved;

    [SerializeField] private float _DistanciaParaAtivar = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _PsitionBat1Inicial = _Bat1.transform.position;
        _PsitionBat2Inicial = _Bat2.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        _PsitionBat1Atual = _Bat1.transform.position;
        _PsitionBat2Atual = _Bat2.transform.position;

        float distancia1 = Vector3.Distance(_PsitionBat1Atual, _PsitionBat1Inicial);
        float distancia2 = Vector3.Distance(_PsitionBat2Atual, _PsitionBat2Inicial);

        if (distancia1 > _DistanciaParaAtivar && _RemoveBat1Ativada == false)//Verifica se a bateria moveu - Usando a posição atual com a posição Inicial
        {
            Debug.Log("Pilha mexeu, vou ativar água");
            PilhaFoiPega();
            _RemoveBat1Ativada = true;
        }

        if (distancia2 > _DistanciaParaAtivar && _RemoveBat2Ativada == false)//Verifica se a bateria moveu - Usando a posição atual com a posição Inicial
        {
            Debug.Log("Pilha mexeu, vou ativar água");
            PilhaFoiPega();
            _RemoveBat2Ativada = true;
        }
    }

    public void PilhaFoiPega()
    {
        DesligarLuzesServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DesligarLuzesServerRpc()
    {
        _BatRemoved--;

        if (_BatRemoved <= 0)
        {
            _Mission._Luzes--;

            //if (_RemoveBat1Ativada) return;
            Debug.Log("Pilha quase Ativada");
            StartCoroutine(FlowRoutine());
        }
        DesligarLuzesClientRpc();
    }

    [ClientRpc]
    private void DesligarLuzesClientRpc()
    {
        _BatRemoved--;

        if (_BatRemoved <= 0)
        {
            _Mission._Luzes--;

            //if (_RemoveBat1Ativada) return;
            Debug.Log("Pilha quase Ativada");
            StartCoroutine(FlowRoutine());
        }
    }


    private IEnumerator FlowRoutine()// Ativa as peças de água e as correntes com um delay entre cada uma
    {

        for (int i = 0; i < _Luzes.Length; i++)// Desativa as luzaes uma por uma, com um delay entre cada um, para criar um efeito visual de "apagamento" do fogo
        {
            if (_Luzes[i] != null)
                _Luzes[i].SetActive(false);

            yield return new WaitForSeconds(_DelayBetweenPieces);
        }
    }
}
