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
        Debug.Log("Saliendo...");
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}