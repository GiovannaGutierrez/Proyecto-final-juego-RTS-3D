using UnityEngine;
using TMPro;

public class ContadorUnidadesPorNombre : MonoBehaviour
{
    [Header("Textos UI")]
    public TextMeshProUGUI textoCaballerito;
    public TextMeshProUGUI textoArquera;
    public TextMeshProUGUI textoEscudero;

    void Update()
    {
        int countCaballerito = 0;
        int countArquera = 0;
        int countEscudero = 0;

        GameObject[] todos = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in todos)
        {
            string nombre = obj.name;

            if (nombre.Contains("Caballerito"))
                countCaballerito++;
            else if (nombre.Contains("Arquera"))
                countArquera++;
            else if (nombre.Contains("Escudero"))
                countEscudero++;
        }

        if (textoCaballerito != null)
            textoCaballerito.text = $"Caballeritos: {countCaballerito}";

        if (textoArquera != null)
            textoArquera.text = $"Lanzaderos: {countArquera}";

        if (textoEscudero != null)
            textoEscudero.text = $"Escuderos: {countEscudero}";
    }
}
