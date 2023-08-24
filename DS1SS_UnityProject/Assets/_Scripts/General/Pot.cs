using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
    public Animator Anim;
    public bool Broken;
    public Collider2D Collider;



    private void OnEnable()
    {
        if (Broken) { gameObject.SetActive(false); } 
        else
        {
            ResetPot();
        }
    }

    public void ResetPot()
    {
        Collider.enabled = true;
        gameObject.SetActive(true);
        Anim.Play("Pot_Idle");
        Broken = false;
 
    }
    public void BreakPot()
    {
        Collider.enabled = false;
        Anim.SetTrigger("Break");
        Broken = true;
        StartCoroutine(WaitToTurnOff());
    }

    IEnumerator WaitToTurnOff()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
}
