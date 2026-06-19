using UnityEngine;
using Unity.Netcode;

public class Barragem_Partida : NetworkBehaviour
{
    [Header("Barragem")]
    [SerializeField] private GameObject _BarragemNormal;
    [SerializeField] private GameObject _BarragemPartida;

    [Header("Rio")]
    [SerializeField] private RiverFLow _RiverFlowController;

    [Header("Efeitos")]
    [SerializeField] private GameObject _BreakEffect;
    [SerializeField] private AudioSource _WaterSound;

    private bool _IsBroken = false;

    public void BreakDam()
    {
        if (_IsBroken) return;

        _IsBroken = true;

        if (_BarragemNormal != null)
            _BarragemNormal.SetActive(false);//Desativa a barragem normal

        if (_BarragemPartida != null)
            _BarragemPartida.SetActive(true);//Ativa a barragem partida

        if (_BreakEffect != null)
            Instantiate(_BreakEffect, transform.position, Quaternion.identity);//Instancia o efeito de quebra da barragem

        if (_WaterSound != null)
            _WaterSound.Play();//Toca o som da água

        if (_RiverFlowController != null)
            _RiverFlowController.FlowRoutineServerRpc();//Começa a aumentar o fluxo do rio
    }
}
