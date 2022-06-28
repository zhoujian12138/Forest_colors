using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum sceneName{testScene,mission3_01,mission3_02,mission3_03,mission3_04}
    public enum TransitionType
    {
        SameScene,DifferentScene
    }


    [Header("Transition Info")]
    public sceneName SceneName;
    public TransitionType transitionType;
    public TransitionDestination.DestinationTag destinationTag;
    

    private bool canTrans;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = false;
        }
    }
}
