using UnityEngine;

public class Options : MonoBehaviour
{
    [SerializeField] private GameObject _AudioLinha;
    [SerializeField] private GameObject _VideoLinha;
    [SerializeField] private GameObject _GameplayLinha;
    [SerializeField] private GameObject _OptionsMenu;
    [SerializeField] private GameObject _AudioMenu;
    [SerializeField] private GameObject _VideoMenu;
    [SerializeField] private GameObject _GameplayMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Screen.fullScreen = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackOptions()//Voltar
    {
        _OptionsMenu.SetActive(false);
    }

    public void OptionsMenu()//Opções
    {
        _OptionsMenu.SetActive(true);
    }

    public void Audio()//Audio
    {
        _AudioLinha.SetActive(true);
        _AudioMenu.SetActive(true);
        _VideoLinha.SetActive(false);
        _VideoMenu.SetActive(false);
        _GameplayLinha.SetActive(false);
        _GameplayMenu.SetActive(false);
        //_AudioMenu.SetActive(true);
    }

    public void Video()//Video
    {
        _VideoLinha.SetActive(true);
        _VideoMenu.SetActive(true);
        _AudioLinha.SetActive(false);
        _AudioMenu.SetActive(false);
        _GameplayLinha.SetActive(false);
        _GameplayMenu.SetActive(false);
        //_VideoMenu.SetActive(true);
    }
    
    public void Gameplay()//Gameplay
    {
        _GameplayLinha.SetActive(true);
        _GameplayMenu.SetActive(true);
        _AudioLinha.SetActive(false);
        _AudioMenu.SetActive(false);
        _VideoLinha.SetActive(false);
        _VideoMenu.SetActive(false);
        //_GameplayMenu.SetActive(true);
    }

   

public class VideoSettings : MonoBehaviour
    {
        public int resolutionIndex;
        public int qualityIndex;
        public bool isFullScreen;

        public void WindowScreen()//Janela Normal
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Debug.Log("Windowed");
        }

        public void MaxWindowScreen()//Janela Maximizada
        {
            Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
            Debug.Log("Maximized Window");
        }

        public void ExFullScreen()//Tela Cheia Exclusiva
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            Debug.Log("Exclusive Full Screen");
        }
        public void FullScreen()//Tela Cheia
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Debug.Log("Full Screen");
        }
    }



}
