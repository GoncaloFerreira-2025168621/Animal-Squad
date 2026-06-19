using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class RiverParalelo : MonoBehaviour
{
    [SerializeField] private ControllerMission _Mission;

    [Header("Pedras dentro do rio")]
    [SerializeField] private int _QuantidadePedras;

    [Header("Peças visuais da água, por ordem")]
    [SerializeField] private GameObject[] _WaterPieces;

    [Header("Efeitos de fogo para desativar no final")]
    [SerializeField] private GameObject[] _FireEffects;

    [Header("Triggers da corrente")]
    [SerializeField] private RiverCurrent[] _RiverCurrents;

    [Header("Tempo entre cada peça aparecer")]
    [SerializeField] private float _DelayBetweenPieces = 0.25f;
    [SerializeField] private float _DelayFireEffects = 0.10f;

    [Header("Ativar corrente só no fim?")]
    [SerializeField] private bool _ActivateCurrentOnlyAtEnd = false;//Serve para defenir se queremos ativar as correntes junto com as peças de água ou só no fim, após todas as peças estarem ativas

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
            FlowRoutineServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void FlowRoutineServerRpc()
    {
        _QuantidadePedras++;

        if(_QuantidadePedras == 4)
        {
            _Mission._CompletMission2 = true;
            StartCoroutine(FlowRoutine());
            FlowRoutineClientRpc();
        }
    }

    [ClientRpc]
    private void FlowRoutineClientRpc()
    {
        _Mission._CompletMission2 = true;
        StartCoroutine(FlowRoutine());
    }

    private IEnumerator FlowRoutine()// Ativa as peças de água e as correntes com um delay entre cada uma
    {
        for (int i = 0; i < _WaterPieces.Length; i++)// Ativa cada peça de água e, se necessário, a corrente correspondente, com um delay entre cada uma
        {
            if (_WaterPieces[i] != null)
                _WaterPieces[i].SetActive(true);

            if (!_ActivateCurrentOnlyAtEnd)// Se a opção de ativar a corrente só no fim não estiver marcada, ativa a corrente correspondente à peça de água atual
            {
                if (i < _RiverCurrents.Length && _RiverCurrents[i] != null)
                    _RiverCurrents[i].ActivateCurrent();
            }

            yield return new WaitForSeconds(_DelayBetweenPieces);
        }

        if (_ActivateCurrentOnlyAtEnd)// Se a opção de ativar a corrente só no fim estiver marcada, ativa todas as correntes após ativar todas as peças de água
        {
            foreach (RiverCurrent current in _RiverCurrents)//
            {
                if (current != null)
                    current.ActivateCurrent();
            }

            for (int i = 0; i < _FireEffects.Length; i++)// Desativa os efeitos de fogo um por um, com um delay entre cada um, para criar um efeito visual de "apagamento" do fogo
            {
                if (_FireEffects[i] != null)
                    _FireEffects[i].SetActive(false);

                yield return new WaitForSeconds(_DelayBetweenPieces);
            }
        }
    }
}
