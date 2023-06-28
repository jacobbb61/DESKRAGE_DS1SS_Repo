using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerControllerV2 PC;
    public Transform HitStartPos;

    private void Start()
    {
        PC = GetComponentInParent < PlayerControllerV2 > ();
    }




    public void LightAttack()
    {
        int layerMask = ~(LayerMask.GetMask("Player"));
        RaycastHit2D hit = Physics2D.Raycast(HitStartPos.position, new Vector2(PC.PlayerDirection, 0), PC.LightAttackRange, layerMask);


        if (hit.collider != null)
        {
            
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<EnemySaveManager>().TakeLightDamageEvent.Invoke();
            }
            else
            {
                Debug.Log("Player hit" + hit.transform.name);
            }

        }
    }
    public void LightAttackFollowUp()
    {
        int layerMask = ~(LayerMask.GetMask("Player"));
        RaycastHit2D hit = Physics2D.Raycast(HitStartPos.position, new Vector2(PC.PlayerDirection, 0), PC.LightAttackFollowUpRange, layerMask);


        if (hit.collider != null)
        {

            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<EnemySaveManager>().TakeLightDamageEvent.Invoke();
            }
            else
            {
                Debug.Log("Player hit" + hit.transform.name);
            }
        }
    }
    public void HeavyAttack()
    {
        int layerMask = ~(LayerMask.GetMask("Player"));
        RaycastHit2D hit = Physics2D.Raycast(HitStartPos.position, new Vector2(PC.PlayerDirection, 0), PC.HeavyAttackRange, layerMask);


        if (hit.collider != null)
        {

            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<EnemySaveManager>().TakeHeavyDamageEvent.Invoke();
            }
            else
            {
                Debug.Log("Player hit" + hit.transform.name);
            }
        }
    }
    public void HeavyAttackFollowUp()
    {
        int layerMask = ~(LayerMask.GetMask("Player"));
        RaycastHit2D hit = Physics2D.Raycast(HitStartPos.position, new Vector2(PC.PlayerDirection, 0), PC.HeavyAttackFollowUpRange, layerMask);


        if (hit.collider != null)
        {

            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<EnemySaveManager>().TakeHeavyDamageEvent.Invoke();
            }
            else
            {
                Debug.Log("Player hit" + hit.transform.name);
            }
        }
    }
}
