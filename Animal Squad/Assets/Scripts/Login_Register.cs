using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Login_Register : MonoBehaviour
{

    public TMP_InputField _password;
    public TMP_InputField _username;

    [SerializeField] private Button _showPasswordButton;
    [SerializeField] private Button _hidePasswordButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _password.contentType = TMP_InputField.ContentType.Password;
        _hidePasswordButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPassword()
    {
        if (_password.contentType == TMP_InputField.ContentType.Password)
        {
            _password.contentType = TMP_InputField.ContentType.Standard;
            _hidePasswordButton.gameObject.SetActive(true);
            _showPasswordButton.gameObject.SetActive(false);
        }
        else
        {
            _password.contentType = TMP_InputField.ContentType.Password;
            _hidePasswordButton.gameObject.SetActive(false);
            _showPasswordButton.gameObject.SetActive(true);
        }
        _password.ForceLabelUpdate();
    }
}
