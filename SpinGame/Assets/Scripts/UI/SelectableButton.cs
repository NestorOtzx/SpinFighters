using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectableButton : MonoBehaviour
{
    public Image buttonImage; // Referencia a la imagen del botón
    public Color baseColor = Color.white; // Color base
    public Color selectedColor = Color.green; // Color cuando está seleccionado

    public bool isDefault;

    private static SelectableButton selectedButton; // Referencia al botón actualmente seleccionado

    public UnityEvent onSelected;

    private void Start()
    {
        // Inicializa el color del botón al color base
        if (buttonImage != null)
        {
            buttonImage.color = baseColor;
        }
        if (isDefault)
        {
            Select();
        }
    }

    public void OnButtonClicked()
    {
        // Si este botón ya está seleccionado, no hacer nada
        if (selectedButton == this) return;

        // Desactivar el color seleccionado del botón previamente seleccionado
        if (selectedButton != null)
        {
            selectedButton.Deselect();
        }

        // Seleccionar este botón
        Select();
    }

    private void Select()
    {
        // Cambiar color del botón a seleccionado y actualizar el botón activo
        if (buttonImage != null)
        {
            buttonImage.color = selectedColor;
        }
        onSelected?.Invoke();
        selectedButton = this;
    }

    private void Deselect()
    {
        // Cambiar color del botón a base
        if (buttonImage != null)
        {
            buttonImage.color = baseColor;
        }
    }
}