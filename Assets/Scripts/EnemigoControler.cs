using UnityEngine;
using UnityEngine.AI;

public class EnemigoControler : MonoBehaviour
{
    [Header("Stats")]
    public float daño = 10f;
    public float rangoAtaque = 2f;
    public float velocidadAtaque = 1f;

    [Header("Detección")]
    public float radioDeteccion = 6f;
    public LayerMask capaBichos;
    public LayerMask capaEstructuras;

    private NavMeshAgent agente;
    private GameObject objetivo;
    private float tiempoUltimoAtaque;
    private float tiempoUltimaBusqueda;
    public float intervaloBusqueda = 1f;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (objetivo == null || objetivo.GetComponent<Vida>() == null)
        {
            objetivo = null;
        }

        if (objetivo == null && Time.time - tiempoUltimaBusqueda >= intervaloBusqueda)
        {
            BuscarObjetivo();
            tiempoUltimaBusqueda = Time.time;
        }

        if (objetivo != null)
        {
            Collider col = objetivo.GetComponent<Collider>();
            if (col == null) return;

            Vector3 puntoCercano = col.ClosestPoint(transform.position);
            float distancia = Vector3.Distance(transform.position, puntoCercano);

            if (distancia <= rangoAtaque)
            {
                if (Time.time - tiempoUltimoAtaque >= velocidadAtaque)
                {
                    Vida vida = objetivo.GetComponent<Vida>();
                    if (vida != null)
                    {
                        vida.RecibirDaño(daño);
                        Debug.Log($"{gameObject.name} atacó a {objetivo.name} con {daño} de daño.");
                        tiempoUltimoAtaque = Time.time;
                    }
                }
            }

            if (distancia > 0.5f)
            {
                agente.SetDestination(objetivo.transform.position);
            }
        }
    }

    void BuscarObjetivo()
    {
        GameObject mejorObjetivo = null;
        float distanciaMin = float.MaxValue;

        Collider[] bichos = Physics.OverlapSphere(transform.position, radioDeteccion, capaBichos);
        foreach (Collider col in bichos)
        {
            float dist = Vector3.Distance(transform.position, col.ClosestPoint(transform.position));
            if (dist < distanciaMin)
            {
                mejorObjetivo = col.gameObject;
                distanciaMin = dist;
            }
        }

        if (mejorObjetivo == null)
        {
            Collider[] estructuras = Physics.OverlapSphere(transform.position, radioDeteccion, capaEstructuras);
            foreach (Collider col in estructuras)
            {
                float dist = Vector3.Distance(transform.position, col.ClosestPoint(transform.position));
                if (dist < distanciaMin)
                {
                    mejorObjetivo = col.gameObject;
                    distanciaMin = dist;
                }
            }
        }

        objetivo = mejorObjetivo;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}
