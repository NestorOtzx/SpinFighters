using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NetworkPlayer : NetworkBehaviour
{
    [HideInInspector]
    public Rigidbody rb;

    [SerializeField]
    private Transform headTransform; // Transform de la "cabeza" del jugador

    [SerializeField]
    private float maxJumpForce = 20f; // Fuerza máxima de salto
    [SerializeField]
    private float collisionForce = 5f; // Fuerza máxima de salto
    [SerializeField]
    private float chargeRate = 10f; // Velocidad de carga de la fuerza
    [SerializeField]
    private LayerMask groundLayer; // Capa del suelo

    private float currentJumpForce = 0f; // Fuerza actual cargada
    private bool isCharging = false; // Indicador de si está cargando el salto

    private NetworkVariable<bool> isGrounded = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Owner);

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (IsOwner)
        {
            HandleJumpInput();
        }
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            CheckGrounded(); // Alternativa para detección precisa
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded.Value)
        {
            StartCharging();
        }

        if (Input.GetKey(KeyCode.Space) && isGrounded.Value)
        {
            ChargeJumpForce();
        }

        if (Input.GetKeyUp(KeyCode.Space) && isGrounded.Value)
        {
            ReleaseJumpForce();
        }
    }

    private void StartCharging()
    {
        isCharging = true;
        currentJumpForce = 0f; // Resetea la fuerza al iniciar la carga
    }

    private void ChargeJumpForce()
    {
        if (isCharging)
        {
            currentJumpForce += chargeRate * Time.deltaTime;
            currentJumpForce = Mathf.Clamp(currentJumpForce, 0f, maxJumpForce); // Limitar al máximo
        }
    }

    private void ReleaseJumpForce()
    {
        if (isCharging)
        {
            isCharging = false;

            // Llamar al servidor para aplicar la fuerza
            ApplyJumpForceServerRpc(currentJumpForce);

            // Resetea la fuerza tras el salto
            currentJumpForce = 0f;
        }
    }

    [ServerRpc]
    private void ApplyJumpForceServerRpc(float force)
    {
        ApplyJumpForce(force);
    }

    private void ApplyJumpForce(float force)
    {
        if (!IsServer) return;

        // La dirección hacia donde apunta la cabeza
        Vector3 jumpDirection = headTransform.forward;

        // Agregar componente vertical al salto
        Vector3 forceDirection = jumpDirection + Vector3.up;

        // Aplicar fuerza con la magnitud cargada
        rb.AddForce(forceDirection.normalized * force, ForceMode.Impulse);

        // Salto realizado, ya no está tocando el suelo
        isGrounded.Value = false;
    }

    private void CheckGrounded()
    {
        // Raycast hacia abajo desde la posición del jugador para detectar el suelo
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1.1f, groundLayer))
        {
            isGrounded.Value = true;
        }
        else
        {
            isGrounded.Value = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (IsServer)
        {
            // Verificar si está tocando el suelo usando el LayerMask
            if (((1 << collision.gameObject.layer) & groundLayer) != 0)
            {
                isGrounded.Value = true;
            }
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        if (IsServer)
        {
            // Si deja de tocar el suelo, actualizamos el estado
            if (((1 << collision.gameObject.layer) & groundLayer) != 0)
            {
                isGrounded.Value = false;
            }
        }
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            // Verificar si el objeto con el que colisionó es otro jugador
            NetworkPlayer otherPlayer = collision.gameObject.GetComponent<NetworkPlayer>();
            if (otherPlayer != null)
            {
                ApplyRepulsionForce(otherPlayer);
            }
        }
    }

    private void ApplyRepulsionForce(NetworkPlayer otherPlayer)
    {
        // Obtener la dirección de la fuerza de repulsión
        Vector3 repulsionDirection = (otherPlayer.transform.position - transform.position).normalized;
        repulsionDirection.y = Mathf.Min(repulsionDirection.y, 0);
        // Aplicar la fuerza al jugador actual en dirección opuesta
        rb.AddForce(-repulsionDirection * collisionForce, ForceMode.Impulse); // Ajusta la magnitud de la fuerza

        // Aplicar la fuerza al otro jugador
        otherPlayer.rb.AddForce(repulsionDirection * collisionForce, ForceMode.Impulse); // Ajusta la magnitud de la fuerza
    }

    private void OnDrawGizmos()
    {
        if (headTransform != null)
        {
            Gizmos.color = Color.red;

            // Dibujar la línea de dirección del salto
            Vector3 start = headTransform.position;
            Vector3 end = start + headTransform.forward * 4.0f; // Largo de la línea
            Gizmos.DrawLine(start, end);

            // Dibujar una pequeña esfera en la punta
            Gizmos.DrawSphere(end, 0.1f);
        }
    }
}
