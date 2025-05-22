using UnityEngine;

public class BaseEnemiga : MonoBehaviour
{
    [Header("Unidades")]
    public GameObject[] prefabsUnidades;
    public Transform[] puntosDeSpawn;

    [Header("Configuración")]
    public float tiempoEntreUnidades = 5f;
    public bool iniciarAutomaticamente = true;
    public int limiteUnidades = 20; 
    public string tagUnidadesEnemigas = "UnidadEnemiga";

    private float tiempoSiguienteSpawn = 0f;
    private int siguientePunto = 0;

    void Start()
    {
        if (iniciarAutomaticamente)
            tiempoSiguienteSpawn = Time.time + tiempoEntreUnidades;
    }

    void Update()
    {
        if (Time.time >= tiempoSiguienteSpawn)
        {
            if (ContarUnidadesEnemigas() < limiteUnidades)
            {
                GenerarUnidad();
                tiempoSiguienteSpawn = Time.time + tiempoEntreUnidades;
            }
            else
            {
                tiempoSiguienteSpawn = Time.time + 1f;
            }
        }
    }

    void GenerarUnidad()
    {
        if (prefabsUnidades.Length == 0 || puntosDeSpawn.Length == 0) return;

        GameObject prefab = prefabsUnidades[Random.Range(0, prefabsUnidades.Length)];

        Transform punto = puntosDeSpawn[siguientePunto];
        siguientePunto = (siguientePunto + 1) % puntosDeSpawn.Length;

        Instantiate(prefab, punto.position, Quaternion.identity);
    }

    int ContarUnidadesEnemigas()
    {
        return GameObject.FindGameObjectsWithTag(tagUnidadesEnemigas).Length;
    }
}
