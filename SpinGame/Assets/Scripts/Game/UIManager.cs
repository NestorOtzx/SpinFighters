using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    // Start is called before the first frame update
    [SerializeField] private GameObject winPanel, loosePanel;

    private void Awake(){
        instance = this;
    }

    public void SetWin(bool win){
        winPanel.SetActive(win);
    }

    public void SetLoose(bool loose){
        loosePanel.SetActive(loose);
    }
}
