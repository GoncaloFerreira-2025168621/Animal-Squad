using Unity.Netcode;
using UnityEngine;

public class ActivateWaterPump : NetworkBehaviour
{
    [Header("Tocas")]
    public int _NTocas = 2;
    [SerializeField] private GameObject _Toca1;
    [SerializeField] private bool _Toca1Destroy = false;
    [SerializeField] private GameObject _Toca2;
    [SerializeField] private bool _Toca2Destroy = false;
    [SerializeField] private bool _WaterPumpActive = false;

    [SerializeField] WaterMouse _waterMouse;

    [SerializeField] private GameObject _Agua;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        VerificationTocasServerRpc();
        if (_NTocas <= 0)
        {
            ActivateWaterServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void VerificationTocasServerRpc()
    {
        if (_Toca1 == null && _Toca1Destroy == false)
        {
            _NTocas--;
            _Toca1Destroy = true;
        }
        if (_Toca2 == null && _Toca2Destroy == false)
        {
            _NTocas--;
            _Toca2Destroy = true;
        }
        VerificationTocasClientRpc();
    }

    [ClientRpc]
    private void VerificationTocasClientRpc()
    {
        if (_Toca1 == null && _Toca1Destroy == false)
        {
            _NTocas--;
            _Toca1Destroy = true;
        }
        if (_Toca2 == null && _Toca2Destroy == false)
        {
            _NTocas--;
            _Toca2Destroy = true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ActivateWaterServerRpc()
    {
        _Agua.SetActive(true);
        if (_WaterPumpActive == false)
        {
            _WaterPumpActive = true;
            _waterMouse._NumberWater++;
        }
        ActivateWaterClientRpc();
    }

    [ClientRpc]
    private void ActivateWaterClientRpc()
    {
        _Agua.SetActive(true);

        if (_WaterPumpActive == false)
        {
            _WaterPumpActive = true;
            _waterMouse._NumberWater++;
        }
    }
}
