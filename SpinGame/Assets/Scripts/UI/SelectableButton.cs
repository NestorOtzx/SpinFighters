using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectableButton : MonoBehaviour
{
    public Image buttonImage;
    public Color baseColor = Color.white; 
    public Color selectedColor = Color.green; 

    public bool isDefault;

    private static SelectableButton selectedButton;

    public UnityEvent onSelected;

    private void Start()
    {
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
        if (selectedButton == this) return;

        if (selectedButton != null)
        {
            selectedButton.Deselect();
        }

        Select();
    }

    private void Select()
    {
        if (buttonImage != null)
        {
            buttonImage.color = selectedColor;
        }
        onSelected?.Invoke();
        selectedButton = this;
    }

    private void Deselect()
    {
        if (buttonImage != null)
        {
            buttonImage.color = baseColor;
        }
    }
}