using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorDeCamara : MonoBehaviour
{
    private InputAction accionMovimiento;
    private InputAction accionRotacion;
    private InputAction accionZoom;

    private Transform ejeYaw;
    private Transform ejePitch;
    private Transform camara;

    [Header("Parámetros de Movimiento")]
    public float rapidezMovimiento = 5f;
    public float rapidezRotacion = 100f;

    [Header("Parámetros de Zoom")]
    public float rapidezZoom = 10f;
    public float zoomMinimo = 5f;
    public float zoomMaximo = 50f;

    [Header("Límites de Movimiento")]
    public float minX = 170f;
    public float maxX = 630f;
    public float minZ = 0f;
    public float maxZ = 460f;

    private void Awake()
    {
        ejeYaw = transform.Find("Yaw");
        if (ejeYaw == null) Debug.LogError("No se encontró el hijo 'Yaw'.");

        ejePitch = ejeYaw?.Find("Pitch");
        if (ejePitch == null) Debug.LogError("No se encontró el hijo 'Pitch' dentro de 'Yaw'.");

        camara = ejePitch?.Find("Camera");
        if (camara == null) Debug.LogError("No se encontró la cámara dentro de 'Pitch'.");
    }

    private void OnEnable()
    {
        accionMovimiento = InputSystem.actions?.FindAction("Movimiento");
        accionRotacion = InputSystem.actions?.FindAction("Rotacion");
        accionZoom = InputSystem.actions?.FindAction("Zoom");

        if (accionMovimiento == null || accionRotacion == null || accionZoom == null)
        {
            Debug.LogError("Alguna acción de entrada no fue encontrada.");
        }
    }

    private void Update()
    {
        if (!AccionesValidas()) return;

        Vector2 inputMovimiento = accionMovimiento.ReadValue<Vector2>();
        float inputRotacion = accionRotacion.ReadValue<float>();
        float inputZoom = accionZoom.ReadValue<float>();

        MoverCamara(inputMovimiento);
        RotarCamara(inputRotacion);
        AplicarZoom(inputZoom);
    }

    private void MoverCamara(Vector2 input)
    {
        Vector3 direccion = ejeYaw.rotation * new Vector3(input.x, 0f, input.y);
        Vector3 nuevaPos = transform.position + direccion * rapidezMovimiento * Time.deltaTime;

        // Aplicar límites en X y Z
        nuevaPos.x = Mathf.Clamp(nuevaPos.x, minX, maxX);
        nuevaPos.z = Mathf.Clamp(nuevaPos.z, minZ, maxZ);

        transform.position = nuevaPos;
    }

    private void RotarCamara(float input)
    {
        ejeYaw.Rotate(0f, input * rapidezRotacion * Time.deltaTime, 0f);
    }

    private void AplicarZoom(float input)
    {
        float zoomActual = camara.localPosition.z + input * rapidezZoom * Time.deltaTime;
        zoomActual = Mathf.Clamp(zoomActual, -zoomMaximo, -zoomMinimo);
        camara.localPosition = new Vector3(0f, 0f, zoomActual);
    }

    private bool AccionesValidas()
    {
        return accionMovimiento != null && accionRotacion != null && accionZoom != null &&
               ejeYaw != null && ejePitch != null && camara != null;
    }
}
