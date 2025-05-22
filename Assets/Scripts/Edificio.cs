using UnityEngine;
using UnityEngine.UI;

public class Edificio : MonoBehaviour
{
    [Header("Unidades y Spawn")]
    public GameObject prefabCaballerito;
    public GameObject prefabEscudero;
    public GameObject prefabArquero;
    public Transform[] puntosDeSpawn;

    [Header("Cooldown y Recursos")]
    public float cooldown = 3f;
    private float tiempoUltimoSpawn = -999f;
    public int costoCaballerito = 30;
    public int costoEscudero = 50;
    public int costoArquero = 80;

    [Header("Límite de Unidades")]
    public int limiteUnidades = 30;
    public string tagUnidadesJugador = "Unidad";

    [Header("UI y Selección")]
    public GameObject selectorVisual;
    public GameObject panelBotones;
    public Button boton1;
    public Button boton2;
    public Button boton3;

    private int siguientePunto = 0;

    private void Start()
    {
        if (panelBotones != null)
            panelBotones.SetActive(false);

        if (boton1 != null) boton1.onClick.AddListener(() => IntentarCrear(prefabCaballerito, costoCaballerito));
        if (boton2 != null) boton2.onClick.AddListener(() => IntentarCrear(prefabEscudero, costoEscudero));
        if (boton3 != null) boton3.onClick.AddListener(() => IntentarCrear(prefabArquero, costoArquero));
    }

    public void ActivarSelector(bool activar)
    {
        if (selectorVisual != null)
            selectorVisual.SetActive(activar);

        if (panelBotones != null)
            panelBotones.SetActive(activar);
    }

    void IntentarCrear(GameObject prefab, int costo)
    {
        if (Time.time - tiempoUltimoSpawn < cooldown)
        {
            Debug.Log("Espera el cooldown para crear otra unidad.");
            return;
        }

        if (ContarUnidadesJugador() >= limiteUnidades)
        {
            Debug.Log("Límite de unidades alcanzado.");
            return;
        }

        if (prefab == null || puntosDeSpawn.Length == 0)
        {
            Debug.LogWarning("Faltan prefabs o puntos de spawn.");
            return;
        }

        if (RecursosJugador.Instancia != null && RecursosJugador.Instancia.ConsumirRecursos(costo))
        {
            Transform punto = puntosDeSpawn[siguientePunto];
            Instantiate(prefab, punto.position, Quaternion.identity);
            siguientePunto = (siguientePunto + 1) % puntosDeSpawn.Length;

            tiempoUltimoSpawn = Time.time;
        }
        else
        {
            Debug.Log("No hay recursos suficientes.");
        }
    }

    int ContarUnidadesJugador()
    {
        return GameObject.FindGameObjectsWithTag(tagUnidadesJugador).Length;
    }
}
