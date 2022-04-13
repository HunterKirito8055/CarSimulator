using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    [SerializeField] AIpedestrian walker;

    #region OnTriggerMethods

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == StringConstants.player || other.tag == StringConstants.aiCarLayer)
        {
            walker.WalkStop = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == StringConstants.player || other.tag == StringConstants.aiCarLayer)
        {
            walker.WalkStop = false;
        }
    }
    #endregion
}
