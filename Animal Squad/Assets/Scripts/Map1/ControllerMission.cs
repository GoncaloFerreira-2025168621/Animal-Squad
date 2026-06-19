using UnityEngine;
using Unity.Netcode;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class ControllerMission : NetworkBehaviour
{
    [SerializeField] private GameObject _Fim;

    [Header("Mission 1")]
    [SerializeField] public bool _CompletMission1 = false;
    [SerializeField] private bool _MoedaCheck1 = false;
    [SerializeField] public GameObject _Mission1Object;
    [SerializeField] private GameObject _Mission1Text;
    [SerializeField] private GameObject _Moeda1;

    [Header("Mission 2")]
    [SerializeField] public bool _CompletMission2 = false;
    [SerializeField] public GameObject _Mission2Object;
    [SerializeField] private GameObject _Mission2Text;
    [SerializeField] private GameObject _Moeda2;
    [SerializeField] private bool _MoedaCheck2 = false;

    [Header("Mission 4")]
    [SerializeField] public bool _CompletMission4 = false;
    [SerializeField] public GameObject _Mission4Object;
    [SerializeField] private GameObject _Mission4Text;
    [SerializeField] private GameObject _Moeda3;
    [SerializeField] private bool _MoedaCheck3 = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(_CompletMission2 == true && _MoedaCheck2 == false)
        {
            VerificationMission2ServerRpc();
            _MoedaCheck2 = true;
        }

        if(_CompletMission4 == true && _MoedaCheck3 == false)
        {
            VerificationMission4ServerRpc();
            _MoedaCheck3 = true;
        }

        if (_CompletMission1 == true && _MoedaCheck1 == false)
        {
            VerificationMission1ServerRpc();
            _MoedaCheck1 = true;
        }

        if (_CompletMission2 == true && _CompletMission4 == true && _CompletMission1 == true)
        {
            _Fim.SetActive(true);
        }
    }

    //Mission 1
    [ServerRpc(RequireOwnership = false)]
    private void VerificationMission1ServerRpc()
    {
        _Mission1Object.SetActive(false);
        _Mission1Text.SetActive(false);
        VerificationMission1ClientRpc();
    }

    [ClientRpc]
    private void VerificationMission1ClientRpc()
    {
        _Mission1Object.SetActive(false);
        _Mission1Text.SetActive(false);
        _Moeda1.SetActive(true);
    }


    //Mission 2
    [ServerRpc(RequireOwnership = false)]
    private void VerificationMission2ServerRpc()
    {
        _Mission2Object.SetActive(false);
        _Mission2Text.SetActive(false);
        VerificationMission2ClientRpc();
    }

    [ClientRpc]
    private void VerificationMission2ClientRpc()
    {
        _Mission2Object.SetActive(false);
        _Mission2Text.SetActive(false);
        _Moeda2.SetActive(true);
    }

    //Mission 4
    [ServerRpc(RequireOwnership = false)]
    private void VerificationMission4ServerRpc()
    {
        _Mission4Object.SetActive(false);
        _Mission4Text.SetActive(false);
        VerificationMission4ClientRpc();
    }

    [ClientRpc]
    private void VerificationMission4ClientRpc()
    {
        _Mission4Object.SetActive(false);
        _Mission4Text.SetActive(false);
        _Moeda3.SetActive(true);
    }
}
