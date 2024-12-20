using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinRotation : MonoBehaviour
{
// Velocidad de rotación en grados por segundo
    public float rotationSpeed = 100f;

    private float timeElapsed = 0;
    private bool canRotate;

    private float timeStartRotation = 0;
    public void Start()
    {
        timeStartRotation = Random.Range(0f, 1f);
        
    }

    void Update()
    {
        if (timeElapsed > timeStartRotation)
        {
            float rotationAngle = rotationSpeed * Time.deltaTime;
            transform.Rotate(0, rotationAngle, 0);
        }else{
            timeElapsed+=Time.deltaTime;
        }
        
    }
}
