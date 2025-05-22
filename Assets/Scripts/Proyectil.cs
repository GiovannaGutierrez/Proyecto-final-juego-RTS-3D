using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public float velocidad = 12f;
    private Vector3 destino;
    private float da�o;
    private bool activo = false;

    public void Inicializar(Vector3 posicionObjetivo, float da�oAsignado)
    {
        destino = posicionObjetivo;
        da�o = da�oAsignado;
        activo = true;
    }

    void Update()
    {
        if (!activo) return;

        Vector3 direccion = (destino - transform.position).normalized;
        transform.position += direccion * velocidad * Time.deltaTime;

        if (Vector3.Distance(transform.position, destino) < 0.3f)
        {
            Collider[] colisiones = Physics.OverlapSphere(transform.position, 0.5f);
            foreach (var col in colisiones)
            {
                Vida vida = col.GetComponent<Vida>();
                if (vida != null)
                {
                    vida.RecibirDa�o(da�o);
                    break;
                }
            }

            Destroy(gameObject);
        }
    }
}
