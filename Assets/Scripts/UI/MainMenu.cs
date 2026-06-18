using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button LevelSelect;
    public Button Quit;
    public void QuitApplication()
    {
        Application.Quit();
    }
}


