using Unity.Netcode;
using UnityEngine;

public class SnowBall : NetworkBehaviour
{
    public float growthRate = 0.1f;
    public float maxScale = 3.0f;

    private Rigidbody rb;
    private Vector3 initialScale;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialScale = transform.localScale;
    }

    void FixedUpdate()
    {
        float movementMagnitude = rb.velocity.magnitude;

        if (movementMagnitude > 0)
        {
            Grow(movementMagnitude);
        }
    }

    private void Grow(float movementMagnitude)
    {
        // Calculamos el nuevo tamaño basándonos en la magnitud del movimiento.
        float growth = movementMagnitude * growthRate * Time.fixedDeltaTime;
        Vector3 newScale = transform.localScale + Vector3.one * growth;

        // Limitamos el tamaño al máximo definido.
        if (newScale.x <= maxScale && newScale.y <= maxScale && newScale.z <= maxScale)
        {
            transform.localScale = newScale;
        }
        else
        {
            transform.localScale = Vector3.one * maxScale;
        }
    }

    public void ResetSize()
    {
        // Método para reiniciar el tamaño de la bola de nieve.
        transform.localScale = initialScale;
    }
}
