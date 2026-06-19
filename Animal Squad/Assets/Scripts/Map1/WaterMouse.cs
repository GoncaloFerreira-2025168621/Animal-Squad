using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class WaterMouse : NetworkBehaviour
{
    [SerializeField] public int _NumberWater;
    private bool _WaterActivate = false;
    [SerializeField] private ControllerMission _Mission;

    [Header("Peças visuais da água, por ordem")]
    [SerializeField] private GameObject[] _WaterPieces;

    [Header("Efeitos de fogo para desativar no final")]
    [SerializeField] private GameObject[] _FireEffects;

    [Header("Triggers da corrente")]
    [SerializeField] private RiverCurrent[] _RiverCurrents;

    [Header("Ativar corrente só no fim?")]
    [SerializeField] private bool _ActivateCurrentOnlyAtEnd = false;//Serve para defenir se queremos ativar as correntes junto com as peças de água ou só no fim, após todas as peças estarem ativas

    [Header("Tempo entre cada peça aparecer")]
    [SerializeField] private float _DelayBetweenPieces = 0.25f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ActivateWaterServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ActivateWaterServerRpc()
    {
        if (_NumberWater >= 2 && _WaterActivate == false)
        {
            StartCoroutine(FlowRoutine());
            _WaterActivate = true;
            _Mission._CompletMission2 = true;
            ActivateWaterClientRpc();
        }
    }

    [ClientRpc]
    private void ActivateWaterClientRpc()
    {
        _WaterActivate = true;
        _Mission._CompletMission2 = true;
        StartCoroutine(FlowRoutine());
    }

    private IEnumerator FlowRoutine()// Ativa as peças de água e as correntes com um delay entre cada uma
    {
        for (int i = 0; i < _WaterPieces.Length; i++)// Ativa cada peça de água e, se necessário, a corrente correspondente, com um delay entre cada uma
        {
            if (_WaterPieces[i] != null)
                _WaterPieces[i].SetActive(true);

            yield return new WaitForSeconds(_DelayBetweenPieces);
        }

        if (_ActivateCurrentOnlyAtEnd)// Se a opção de ativar a corrente só no fim estiver marcada, ativa todas as correntes após ativar todas as peças de água
        {

            for (int i = 0; i < _FireEffects.Length; i++)// Desativa os efeitos de fogo um por um, com um delay entre cada um, para criar um efeito visual de "apagamento" do fogo
            {
                if (_FireEffects[i] != null)
                    _FireEffects[i].SetActive(false);

                yield return new WaitForSeconds(_DelayBetweenPieces);
            }
        }
    }
}
