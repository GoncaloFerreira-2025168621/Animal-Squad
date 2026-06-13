using System.Collections;
using UnityEngine;

public class RiverFLow : MonoBehaviour
{
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

    private bool _Started = false;

    private void Start()
    {
        foreach (GameObject waterPiece in _WaterPieces)// Desativa todas as peças de água no início do jogo para que elas possam ser ativadas posteriormente
        {
            if (waterPiece != null)
                waterPiece.SetActive(false);
        }

        foreach (RiverCurrent current in _RiverCurrents)// Desativa todas as correntes no início do jogo para que elas possam ser ativadas posteriormente
        {
            if (current != null)
                current.DeactivateCurrent();
        }
    }

    public void StartRiverFlow()
    {
        if (_Started) return;

        _Started = true;
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
         
