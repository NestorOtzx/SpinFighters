using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private GameObject winPanel, loosePanel, drawPanel;

    private void Awake(){
        instance = this;
    }

    public void SetWin(bool win){
        winPanel.SetActive(win);
    }

    public void SetLoose(bool loose){
        loosePanel.SetActive(loose);
    }

    public void SetDraw()
    {
        SetWin(false);
        SetLoose(false);
        drawPanel.SetActive(true);
    }
}
