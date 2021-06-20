using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePlanet : MonoBehaviour
{
    void Update()
    {
        transform.Rotate (0f, 360f * Time.deltaTime / 90f, 0f);   
    }
}
