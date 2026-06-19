using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class Capom : NetworkBehaviour
{
    [SerializeField] private Mission1 _Mission;

    [Header("Pedras dentro do rio")]
    [SerializeField] private int _Objetos;

    [Header("Peças visuais da Fogo, por ordem")]
    [SerializeField] private GameObject[] _Fire;

    [Header("Efeitos de luz para desativar no final")]
    [SerializeField] private GameObject[] _Luzes;

    [Header("Tempo entre cada peça aparecer")]
    [SerializeField] private float _DelayBetweenPieces = 0.25f;
    [SerializeField] private float _DelayFireEffects = 0.10f;

    [Header("DesativarLuz só no fim?")]
    [SerializeField] private bool _DesactivateCurrentOnlyAtEnd = false;//Serve para defenir se queremos ativar as correntes junto com as peças de água ou só no fim, após todas as peças estarem ativas

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
        if (other.gameObject.CompareTag("Object_Bird"))
        {
            FlowRoutineServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void FlowRoutineServerRpc()
    {
        _Objetos++;

        if (_Objetos == 4)
        {
            _Mission._Carrinha--;
            StartCoroutine(FlowRoutine());
            FlowRoutineClientRpc();
        }
    }

    [ClientRpc]
    private void FlowRoutineClientRpc()
    {
        _Mission._Carrinha--;
        StartCoroutine(FlowRoutine());
    }

    private IEnumerator FlowRoutine()// Ativa as peças de água e as correntes com um delay entre cada uma
    {
        for (int i = 0; i < _Fire.Length; i++)// Ativa cada peça de água e, se necessário, a corrente correspondente, com um delay entre cada uma
        {
            if (_Fire[i] != null)
                _Fire[i].SetActive(true);

            yield return new WaitForSeconds(_DelayBetweenPieces);
        }

        if (_DesactivateCurrentOnlyAtEnd)// Se a opção de ativar a corrente só no fim estiver marcada, ativa todas as correntes após ativar todas as peças de água
        {
            for (int i = 0; i < _Luzes.Length; i++)// Desativa os efeitos de fogo um por um, com um delay entre cada um, para criar um efeito visual de "apagamento" do fogo
            {
                if (_Luzes[i] != null)
                    _Luzes[i].SetActive(false);

                yield return new WaitForSeconds(_DelayBetweenPieces);
            }
        }
    }
}
