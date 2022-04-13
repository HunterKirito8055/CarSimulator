using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLimit : MonoBehaviour
{
    public int speedLimit;

    public TextMesh speedtext;
    private void OnDrawGizmos()
    {
        if(speedtext!=null)
        speedtext.text = speedLimit.ToString();
    }
}
