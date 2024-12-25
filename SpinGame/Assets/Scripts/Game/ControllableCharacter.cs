using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ControllableCharacter : NetworkBehaviour
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

    protected NetworkVariable<bool> isGrounded = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Owner);
    protected bool isGroundedSingle = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (GameManager.instance.isSinglePlayer)
        {
            rb.isKinematic = false;
        }
    }

    private void FixedUpdate()
    {
        if (IsServer ||  GameManager.instance.isSinglePlayer)
        {
            CheckGrounded(); // Alternativa para detección precisa
        }
    }

    protected void StartCharging()
    {
        isCharging = true;
        currentJumpForce = 0f; // Resetea la fuerza al iniciar la carga
    }

    protected void ChargeJumpForce()
    {
        if (isCharging)
        {
            currentJumpForce += chargeRate * Time.deltaTime;
            currentJumpForce = Mathf.Clamp(currentJumpForce, 0f, maxJumpForce); // Limitar al máximo
        }
    }

    protected void ReleaseJumpForce()
    {
        if (isCharging)
        {
            isCharging = false;

            
            if (GameManager.instance.isSinglePlayer)
            {
                ApplyJumpForce(currentJumpForce);
            }else{
                ApplyJumpForceServerRpc(currentJumpForce);
            }
        
            // Reset force
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
        Debug.Log("Apply force 1");
        if (!IsServer && !GameManager.instance.isSinglePlayer) return;
        Debug.Log("Apply force 2");
        // La dirección hacia donde apunta la cabeza
        Vector3 jumpDirection = headTransform.forward;

        // Agregar componente vertical al salto
        Vector3 forceDirection = jumpDirection + Vector3.up;

        // Aplicar fuerza con la magnitud cargada
        rb.AddForce(forceDirection.normalized * force, ForceMode.Impulse);

        // Salto realizado, ya no está tocando el suelo
        if (GameManager.instance.isSinglePlayer)
        {
            isGroundedSingle = false;
        }else{
            isGrounded.Value = false;    
        }
        
    }

    private void CheckGrounded()
    {
        // Raycast hacia abajo desde la posición del jugador para detectar el suelo
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1.1f, groundLayer))
        {
            if (GameManager.instance.isSinglePlayer)
            {
                isGroundedSingle = true;
            }else{
                isGrounded.Value = true;    
            }
            
        }
        else
        {
            if (GameManager.instance.isSinglePlayer)
            {
                isGroundedSingle = false;
            }else{
                isGrounded.Value = false;    
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (IsServer || GameManager.instance.isSinglePlayer)
        {
            // Verificar si está tocando el suelo usando el LayerMask
            if (((1 << collision.gameObject.layer) & groundLayer) != 0)
            {
                if (GameManager.instance.isSinglePlayer)
                {
                    isGroundedSingle = true;
                }else{
                    isGrounded.Value = true;    
                }
                
            }
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        if (IsServer || GameManager.instance.isSinglePlayer)
        {
            // Si deja de tocar el suelo, actualizamos el estado
            if (((1 << collision.gameObject.layer) & groundLayer) != 0)
            {
                if (GameManager.instance.isSinglePlayer)
                {
                    isGroundedSingle = false;
                }else{
                    isGrounded.Value = false;    
                }
            }
        }
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer || GameManager.instance.isSinglePlayer)
        {
            if (collision.gameObject.layer == 6)
            {
                ApplyRepulsionForce(collision.gameObject);
            }
        }
    }

    private void ApplyRepulsionForce(GameObject otherPlayer)
    {
        Debug.Log("REPULSION FORCE");
        // Obtener la dirección de la fuerza de repulsión
        Vector3 repulsionDirection = (otherPlayer.transform.position - transform.position).normalized;
        //repulsionDirection.y = Mathf.Max(repulsionDirection.y, 0);
        // Aplicar la fuerza al jugador actual en dirección opuesta
        rb.AddForce(-repulsionDirection * collisionForce, ForceMode.Impulse); // Ajusta la magnitud de la fuerza
        // Aplicar la fuerza al otro jugador
        otherPlayer.GetComponent<Rigidbody>().AddForce(repulsionDirection * collisionForce, ForceMode.Impulse); // Ajusta la magnitud de la fuerza
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
