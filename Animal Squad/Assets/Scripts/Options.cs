using UnityEngine;

public class Options : MonoBehaviour
{


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Screen.fullScreen = !Screen.fullScreen;
            Debug.Log("Modo de tela cheia: " + Screen.fullScreen);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Screen.SetResolution(1920, 1080, true);
        }
    }
}
