using UnityEngine;

[ExecuteInEditMode]
public class LavaShaderController : MonoBehaviour
{
    public Material lavaMaterial;
    public float speed = 1.0f;

    private float offset;

    void Update()
    {
        if (Application.isPlaying)
        {
            offset += Time.deltaTime * speed;
        }
        else
        {
            offset = 0;
        }

        if (lavaMaterial != null)
        {
            lavaMaterial.SetFloat("_Offset", offset);
        }
    }
}