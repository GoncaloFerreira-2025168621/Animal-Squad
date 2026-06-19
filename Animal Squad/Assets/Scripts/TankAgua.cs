using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class TankAgua : NetworkBehaviour
{
    [SerializeField] private ControllerMission _Mission;

    [Header("Agua")]
    [SerializeField] private GameObject _Agua;
    [SerializeField] private float _TimeMax = 5f;
    [SerializeField] private float _TimeWater;

    [Header("Tampa")]
    [SerializeField] private GameObject _Tampa;
    [SerializeField] private float _DistanciaParaAtivar = 0.1f;

    [Header("Efeitos de fogo para desativar no final")]
    [SerializeField] private GameObject[] _FireEffects;

    [Header("Tempo entre cada peça aparecer")]
    [SerializeField] private float _DelayBetweenPieces = 0.25f;
    [SerializeField] private float _DelayFireEffects = 0.10f;

    private bool _AguaAtivada;

    private Vector3 _PositionTampaInicial;
    private Vector3 _PositionTampaAtual;

    void Start()
    {
        _PositionTampaInicial = _Tampa.transform.position;//da ao vector "_PositionTampaInicial" a posição da _tampa Inicial
    }

    void Update()
    {
        _PositionTampaAtual = _Tampa.transform.position;//da ao vector "_PositionTampaInicial" a posição da _tampa Atual

        float distancia = Vector3.Distance(_PositionTampaAtual, _PositionTampaInicial);

        if (distancia > _DistanciaParaAtivar && _AguaAtivada == false)//Verifica se a tampa moveu - Usando a posição atual com a posição Inicial
        {
            Debug.Log("Tampa mexeu, vou ativar água");
            TampaFoiPega();
        }

        if (!IsServer) return;

        if (_AguaAtivada == true)
        {
            _TimeWater += Time.deltaTime;

            if (_TimeWater > _TimeMax)
            {
                DesativarAgua();
            }
        }
    }

    public void TampaFoiPega()
    {
        AtivarAguaServerRpc();
        
    }

    public void DesativarAgua()
    {
        _AguaAtivada = false;
        _Agua.SetActive(false);
        DesativarAguaClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void AtivarAguaServerRpc()
    {
        _Mission._CompletMission2 = true;

        if (_AguaAtivada) return;
        Debug.Log("Agua quase Ativada");
        _AguaAtivada = true;
        _TimeWater = 0f;


        _Agua.SetActive(true);
        Debug.Log("Agua quase Ativada");
        StartCoroutine(FlowRoutine());
        AtivarAguaClientRpc();
    }

    [ClientRpc]
    private void AtivarAguaClientRpc()
    {
        _Mission._CompletMission2 = true;

        _AguaAtivada = true;
        _Agua.SetActive(true);
        StartCoroutine(FlowRoutine());
    }

    [ServerRpc(RequireOwnership = false)]
    private void DesativarAguaServerRpc()
    {
        _Agua.SetActive(false);
        //_AguaAtivada = false;
        DesativarAguaClientRpc();
    }

    [ClientRpc]
    private void DesativarAguaClientRpc()
    {
        _Agua.SetActive(false);
        //_AguaAtivada = false;
    }

    private IEnumerator FlowRoutine()// Ativa as peças de água e as correntes com um delay entre cada uma
    {

        for (int i = 0; i < _FireEffects.Length; i++)// Desativa os efeitos de fogo um por um, com um delay entre cada um, para criar um efeito visual de "apagamento" do fogo
        {
            if (_FireEffects[i] != null)
                _FireEffects[i].SetActive(false);

            yield return new WaitForSeconds(_DelayBetweenPieces);
        }
    }
}
