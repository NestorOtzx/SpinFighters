using UnityEngine;

public class UIFollowTarget : MonoBehaviour
{
    public Transform target;       // Transform a seguir
    public Vector3 offset;         // Desplazamiento opcional
    public float scaleMultiplier = 1f; // Factor de escala para ajustar el tamaño
    public float minScale = 0.5f;  // Escala mínima
    public float maxScale = 2f;    // Escala máxima

    private RectTransform rectTransform;
    private Canvas canvas;
    


    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("Este script requiere que el elemento esté dentro de un Canvas.");
        }
    }

    private void LateUpdate()
    {
        if (target == null || canvas == null) return;

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.position + offset);

        if (screenPosition.z < 0)
        {
            rectTransform.gameObject.SetActive(false);
        }
        else
        {
            rectTransform.gameObject.SetActive(true);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                screenPosition,
                canvas.worldCamera,
                out Vector2 localPosition);

            rectTransform.localPosition = localPosition;

            float distance = Vector3.Distance(Camera.main.transform.position, target.position);
            float scale = Mathf.Clamp(scaleMultiplier / distance, minScale, maxScale);
            rectTransform.localScale = Vector3.one * scale;
        }
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

}

