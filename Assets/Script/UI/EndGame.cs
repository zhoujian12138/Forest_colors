using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EndGame : MonoBehaviour
{
    bool canEnd=false;
    public GameObject flame;
    public GameObject UI;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canEnd = true;
        }
    }

    

    void Update()
    {
        if (canEnd && Input.GetMouseButtonDown(1))
        {
            flame.SetActive(false);
           
            StartCoroutine(ShowUI());
        }
    }

    IEnumerator ShowUI()
    {
        yield return new WaitForSeconds(1.5f);
        UI.SetActive(true);
        yield break;
    }
}
