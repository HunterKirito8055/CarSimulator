using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//AShish Code
public class GearButton : MonoBehaviour
{
    [SerializeField] Scrollbar scrollbar;
    float value;

    private void Start()
    {
        value = scrollbar.value;
    }
    public void ChangeGear()
    {
        if (value == 0)
            value = 1;
        else
            value = 0;
        scrollbar.value = value; 
    }
}
