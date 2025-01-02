using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ControlsUI : MonoBehaviour
{
    [SerializeField] GameObject container;


    public void SetPanel(bool t)
    {
        container.SetActive(t);
    }

    public void TogglePanel()
    {
        container.SetActive(!container.activeSelf);
    }
}
