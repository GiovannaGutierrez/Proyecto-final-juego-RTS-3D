using UnityEngine;
using UnityEngine.AI;

public class UnidadJugador : MonoBehaviour
{
    [Header("Stats")]
    public float daño = 10f;
    public float rangoAtaque = 2f;
    public float velocidadAtaque = 1f;

    [Header("Detección Automática")]
    public float radioDeteccion = 6f;
    public LayerMask capaEnemigos;
    public float intervaloDeteccion = 1f;

    private NavMeshAgent agente;
    private GameObject objetivo;
    private float tiempoUltimoAtaque;
    private float tiempoUltimaBusqueda;
    private bool modoManual = false;

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

        if (!modoManual && objetivo == null && Time.time - tiempoUltimaBusqueda >= intervaloDeteccion)
        {
            BuscarEnemigoCercano();
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

        if (modoManual && objetivo == null && !agente.hasPath)
        {
            modoManual = false;
        }
    }

    public void MoverA(Vector3 destino)
    {
        objetivo = null;
        modoManual = true;
        agente.SetDestination(destino);
    }

    public void Atacar(GameObject nuevoObjetivo)
    {
        objetivo = nuevoObjetivo;
        modoManual = true;
    }

    private void BuscarEnemigoCercano()
    {
        Collider[] enemigos = Physics.OverlapSphere(transform.position, radioDeteccion, capaEnemigos);
        float distanciaMasCercana = float.MaxValue;
        GameObject mejorObjetivo = null;

        foreach (Collider col in enemigos)
        {
            float dist = Vector3.Distance(transform.position, col.ClosestPoint(transform.position));
            if (dist < distanciaMasCercana)
            {
                mejorObjetivo = col.gameObject;
                distanciaMasCercana = dist;
            }
        }

        if (mejorObjetivo != null)
        {
            objetivo = mejorObjetivo;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}
