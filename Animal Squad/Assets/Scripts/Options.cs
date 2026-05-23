using UnityEngine;

public class Options : MonoBehaviour
{


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Screen.fullScreen = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WindowScreen()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Debug.Log("Windowed");
    }

    public void FullScreen()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Debug.Log("Full Screen");
    }


}
