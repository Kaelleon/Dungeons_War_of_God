using UnityEngine;
using UnityEngine.SceneManagement;

public class ActionsMenu : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene("CharacterSelect");
    }

    public void Records()
    {
        Debug.Log("Abrir records");
    }

    public void Salir()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}