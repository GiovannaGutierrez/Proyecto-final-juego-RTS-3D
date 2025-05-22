using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float daño = 10f;
    public float tiempoVida = 0.1f;

    void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    void OnTriggerEnter(Collider other)
    {
        Vida vida = other.GetComponent<Vida>();
        if (vida != null)
        {
            vida.RecibirDaño(daño);
        }
    }
}
