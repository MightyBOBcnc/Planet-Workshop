using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceColourRefresh : MonoBehaviour
{
    void Awake()
    {
        foreach(ColourProperty cp in FindObjectsOfType<ColourProperty>()) {
            cp.Init ();
        }
    }
}
