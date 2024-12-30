using UnityEngine;

[ExecuteInEditMode]
public class LavaShaderController : MonoBehaviour
{
    public Material lavaMaterial; // Asigna el material del shader aquí.
    public float speed = 1.0f;

    private float offset;

    void Update()
    {
        if (Application.isPlaying)
        {
            // Incrementa el desplazamiento solo en modo de juego
            offset += Time.deltaTime * speed;
        }
        else
        {
            // Mantén el desplazamiento estático en el editor
            offset = 0;
        }

        // Actualiza la propiedad del material
        if (lavaMaterial != null)
        {
            lavaMaterial.SetFloat("_Offset", offset);
        }
    }
}