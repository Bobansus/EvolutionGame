using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public string startGameScene;

    public void OnPressStartGame()
    {
        SceneManager.LoadScene(startGameScene);
    }
}