using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Controlador_Interaccion : MonoBehaviour
{
    [Header("Configuración de Selección")]
    public LayerMask capaSeleccionable;

    [Header("Click")]
    public float delayClick = 0.1f;

    [Header("Selección por Caja")]
    public RectTransform cajaSeleccionUI;
    private Vector2 inicioCaja;
    private Vector2 finCaja;
    private bool seleccionando = false;

    [Header("Debug")]
    public List<GameObject> unidadesSeleccionadas = new List<GameObject>();
    public List<GameObject> edificiosSeleccionados = new List<GameObject>();

    private float tiempoUltimoClick = 0f;

    void Start()
    {
        if (cajaSeleccionUI != null)
            cajaSeleccionUI.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                SeleccionarUnidadesVisibles();
                return;
            }

            DetectarClickSimple();

            seleccionando = true;
            inicioCaja = Input.mousePosition;
            if (cajaSeleccionUI != null) cajaSeleccionUI.gameObject.SetActive(true);
        }

        if (seleccionando)
        {
            finCaja = Input.mousePosition;
            ActualizarCajaUI();
        }

        if (Input.GetMouseButtonUp(0) && seleccionando)
        {
            seleccionando = false;
            if (cajaSeleccionUI != null) cajaSeleccionUI.gameObject.SetActive(false);

            if (Vector2.Distance(inicioCaja, finCaja) > 10f)
                SeleccionarUnidadesEnCaja();
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                GameObject objetivo = hit.collider.gameObject;

                if (LayerMask.LayerToName(objetivo.layer) == "Enemigo")
                {
                    foreach (GameObject unidad in unidadesSeleccionadas)
                    {
                        UnidadJugador control = unidad.GetComponent<UnidadJugador>();
                        if (control != null)
                        {
                            control.Atacar(objetivo);
                        }
                        else
                        {
                            ArqueroJugador arquero = unidad.GetComponent<ArqueroJugador>();
                            if (arquero != null)
                                arquero.Atacar(objetivo);
                        }
                    }
                }
                else
                {
                    Vector3 centro = hit.point;
                    float separacion = 1.5f;
                    int cantidad = unidadesSeleccionadas.Count;
                    int filas = Mathf.CeilToInt(Mathf.Sqrt(cantidad));

                    for (int i = 0; i < cantidad; i++)
                    {
                        int fila = i / filas;
                        int columna = i % filas;
                        Vector3 offset = new Vector3((columna - filas / 2f) * separacion, 0, (fila - filas / 2f) * separacion);
                        Vector3 destino = centro + offset;

                        var control = unidadesSeleccionadas[i].GetComponent<UnidadJugador>();
                        if (control != null)
                        {
                            control.MoverA(destino);
                        }
                        else
                        {
                            var arquero = unidadesSeleccionadas[i].GetComponent<ArqueroJugador>();
                            if (arquero != null)
                                arquero.MoverA(destino);
                        }
                    }
                }
            }
        }
    }

    void DetectarClickSimple()
    {
        if (Time.time - tiempoUltimoClick < delayClick)
            return;

        tiempoUltimoClick = Time.time;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            GameObject clicado = hit.collider.gameObject;
            GameObject unidad = ObtenerUnidadRaiz(clicado);

            if (unidad != null && ((1 << unidad.layer) & capaSeleccionable) != 0)
            {
                DeseleccionarTodo();
                SeleccionarUnidad(unidad);
            }
            else
            {
                Edificio edificio = clicado.GetComponentInParent<Edificio>();
                if (edificio != null)
                {
                    DeseleccionarTodo();
                    edificiosSeleccionados.Add(edificio.gameObject);
                    edificio.ActivarSelector(true);
                }
                else
                {
                    DeseleccionarTodo();
                }
            }
        }
    }

    void SeleccionarUnidad(GameObject unidad)
    {
        if (!unidadesSeleccionadas.Contains(unidad))
        {
            unidadesSeleccionadas.Add(unidad);
            ActivarSelector(unidad, true);
        }
    }

    void DeseleccionarTodo()
    {
        foreach (GameObject unidad in unidadesSeleccionadas)
            ActivarSelector(unidad, false);
        unidadesSeleccionadas.Clear();

        foreach (GameObject edificio in edificiosSeleccionados)
        {
            Edificio e = edificio.GetComponent<Edificio>();
            if (e != null)
                e.ActivarSelector(false);
        }
        edificiosSeleccionados.Clear();
    }

    void ActivarSelector(GameObject objeto, bool activar)
    {
        Transform selector = objeto.transform.Find("Selector");
        if (selector != null)
            selector.gameObject.SetActive(activar);
    }

    void SeleccionarUnidadesVisibles()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        GameObject[] todos = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in todos)
        {
            if (((1 << obj.layer) & capaSeleccionable) == 0) continue;

            Vector3 viewPos = cam.WorldToViewportPoint(obj.transform.position);
            if (viewPos.z > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            {
                GameObject unidad = ObtenerUnidadRaiz(obj);
                if (unidad != null && !unidadesSeleccionadas.Contains(unidad))
                    SeleccionarUnidad(unidad);
            }
        }
    }

    void ActualizarCajaUI()
    {
        if (!seleccionando || cajaSeleccionUI == null) return;

        Vector2 puntoInicio = inicioCaja;
        Vector2 puntoActual = Input.mousePosition;

        Vector2 posicion = (puntoInicio + puntoActual) / 2f;
        Vector2 tamaño = new Vector2(Mathf.Abs(puntoActual.x - puntoInicio.x), Mathf.Abs(puntoActual.y - puntoInicio.y));

        cajaSeleccionUI.position = posicion;
        cajaSeleccionUI.sizeDelta = tamaño;
    }

    void SeleccionarUnidadesEnCaja()
    {
        bool mantenerSeleccion = Input.GetKey(KeyCode.LeftShift);
        if (!mantenerSeleccion)
            DeseleccionarTodo();

        Camera cam = Camera.main;
        GameObject[] todos = GameObject.FindObjectsOfType<GameObject>();

        Rect caja = new Rect(
            Mathf.Min(inicioCaja.x, finCaja.x),
            Mathf.Min(inicioCaja.y, finCaja.y),
            Mathf.Abs(inicioCaja.x - finCaja.x),
            Mathf.Abs(inicioCaja.y - finCaja.y)
        );

        foreach (GameObject obj in todos)
        {
            if (((1 << obj.layer) & capaSeleccionable) == 0) continue;

            Vector3 screenPos = cam.WorldToScreenPoint(obj.transform.position);
            if (screenPos.z > 0 && caja.Contains(screenPos))
            {
                GameObject unidad = ObtenerUnidadRaiz(obj);
                if (unidad != null && !unidadesSeleccionadas.Contains(unidad))
                    SeleccionarUnidad(unidad);
            }
        }
    }

    GameObject ObtenerUnidadRaiz(GameObject objeto)
    {
        Transform actual = objeto.transform;
        while (actual != null)
        {
            if (actual.GetComponent<NavMeshAgent>() != null || actual.CompareTag("Unidad"))
                return actual.gameObject;

            actual = actual.parent;
        }
        return null;
    }
}
