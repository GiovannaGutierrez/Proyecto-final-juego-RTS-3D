using UnityEngine;
using TMPro;

public class RecursosJugador : MonoBehaviour
{
    public static RecursosJugador Instancia;

    [Header("Recursos")]
    public int recursos = 100;
    public TextMeshProUGUI textoRecursos;

    [Header("Generaci�n autom�tica")]
    public bool generarRecursos = true;
    public float intervalo = 5f;
    public int minimo = 100;
    public int maximo = 250;
    private float tiempoSiguienteGeneracion = 0f;

    void Awake()
    {
        Instancia = this;
    }

    void Start()
    {
        ActualizarUI();
        tiempoSiguienteGeneracion = Time.time + intervalo;
    }

    void Update()
    {
        if (generarRecursos && Time.time >= tiempoSiguienteGeneracion)
        {
            int cantidad = Random.Range(minimo, maximo + 1);
            A�adirRecursos(cantidad);
            Debug.Log($"+{cantidad} recursos generados autom�ticamente.");
            tiempoSiguienteGeneracion = Time.time + intervalo;
        }
    }

    public bool ConsumirRecursos(int cantidad)
    {
        if (recursos >= cantidad)
        {
            recursos -= cantidad;
            ActualizarUI();
            return true;
        }
        return false;
    }

    public void A�adirRecursos(int cantidad)
    {
        recursos += cantidad;
        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (textoRecursos != null)
            textoRecursos.text = $"ORO: {recursos}";
    }
}
