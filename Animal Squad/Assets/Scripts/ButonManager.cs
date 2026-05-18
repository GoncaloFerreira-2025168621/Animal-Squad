using UnityEngine;

public class ButonManager : MonoBehaviour
{
    [SerializeField] private Login_Register _loginRegister;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPassword()
    {
        _loginRegister.ShowPassword();
    }
}
