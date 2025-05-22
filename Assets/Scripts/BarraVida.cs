using UnityEngine;

public class BarraVidaSimple : MonoBehaviour
{
    public Transform objetivo;
    public Vector3 offset = new Vector3(0, 2f, 0);

    private Vector3 escalaInicial;
    private Vida vidaRef;

    void Start()
    {
        escalaInicial = transform.localScale;

        if (objetivo == null)
            objetivo = transform.parent;

        if (objetivo != null)
            vidaRef = objetivo.GetComponent<Vida>();
    }

    void LateUpdate()
    {
        if (objetivo != null)
        {
            transform.position = objetivo.position + offset;

            if (vidaRef != null)
            {
                ActualizarVida(vidaRef.vidaActual, vidaRef.vidaMaxima);
            }
        }
    }

    public void ActualizarVida(float vidaActual, float vidaMaxima)
    {
        float porcentaje = Mathf.Clamp01(vidaActual / vidaMaxima);
        transform.localScale = new Vector3(escalaInicial.x * porcentaje, escalaInicial.y, escalaInicial.z);
    }
}
