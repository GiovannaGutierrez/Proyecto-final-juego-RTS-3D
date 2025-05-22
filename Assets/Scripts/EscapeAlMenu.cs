using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeAlMenu : MonoBehaviour
{
    [Header("Nombre de la escena del menú")]
    public string nombreEscenaMenu = "Menu";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(nombreEscenaMenu);
        }
    }
}
