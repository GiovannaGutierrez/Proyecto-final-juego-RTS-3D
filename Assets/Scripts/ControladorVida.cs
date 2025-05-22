using UnityEngine;
using UnityEngine.SceneManagement;

public class Vida : MonoBehaviour
{
    public float vidaMaxima = 100f;
    public float vidaActual;

    [Header("Rectángulo visual")]
    public Transform barraVisual; 

    private Vector3 escalaInicial;

    void Start()
    {
        vidaActual = vidaMaxima;

        if (barraVisual != null)
            escalaInicial = barraVisual.localScale;
    }

    public void RecibirDaño(float cantidad)
    {
        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        if (barraVisual != null)
        {
            float porcentaje = Mathf.Clamp01(vidaActual / vidaMaxima);
            barraVisual.localScale = new Vector3(escalaInicial.x * porcentaje, escalaInicial.y, escalaInicial.z);
        }

        if (vidaActual <= 0)
        {
            VerificarFinDeJuego();
            if (barraVisual != null)
                Destroy(barraVisual.gameObject);
            Destroy(gameObject);
        }
    }

    void VerificarFinDeJuego()
    {
        if (GetComponent<BaseEnemiga>() != null)
        {
            SceneManager.LoadScene("Win");
        }
        else if (GetComponent<Edificio>() != null)
        {
            SceneManager.LoadScene("Lose");
        }
    }
}
