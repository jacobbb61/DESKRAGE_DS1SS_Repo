using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.SceneManagement;

public class Bonfire : MonoBehaviour
{
    private PlayerManager PM;
    public PlayerControllerV2 PC;
    public OscarManager OM;

    public int BonfireTagNum;
    public int BonfireLayer;
    public bool BonfireEverUsed;

    private Animator Anim;

    public GameObject[] EnemySaveManagerList;
    public PotManager[] PotManagers;
    public AsylumDemon AsylumDemon;
    public Pursuer Pursuer;
    private GameObject BonfirePromptUI;
    

    public LayerManagerV2 LayerManager;

    public Animator FireAnim;

    public EventReference LitBonfire;

    bool nightmare;

    void Start()
    {

        PM = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerV2>();
       // EnemySaveManagerList = GameObject.FindGameObjectsWithTag("Enemy");

        Anim = GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasManager>().BonfireAnim;
        BonfirePromptUI = GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasManager>().InteractPrompt;
        StartCoroutine(WaitForFire());

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Build Nightmare"))
        {
            nightmare = true;
            Anim.SetBool("Nightmare", true);
        }

    }


    IEnumerator WaitForFire()
    {
        yield return new WaitForSeconds(0.5f);


        if (BonfireEverUsed) { FireAnim.Play("BonfireLightIdle"); }
        else { FireAnim.Play("BonfireAshIdle"); }
    


    }


    public void InteractBonfire()
    {
        StartCoroutine(UseThisBonfire());
    }

    private void OnEnable()
    {
        if (BonfireEverUsed) {  FireAnim.Play("BonfireLightIdle"); }
        else { FireAnim.Play("BonfireAshIdle"); }
    }

    IEnumerator UseThisBonfire()
    {
        PC.MyRb.velocity = Vector2.zero;
        PM.LastBonfireVisited = BonfireTagNum;

        PC.CanMove = false;
        PC.CanAttack = false;
        PC.CanFollowUp = false;
        PC.Anim.Play("PlayerAnim_BonfireInteract");

        yield return new WaitForSeconds(1);

        if (nightmare)
        {
            if (BonfireEverUsed) { Anim.Play("BonfireLitActivate 1"); }
            else { Anim.Play("BonfireLitFirstTime 1"); BonfireEverUsed = true; FireAnim.Play("BonfireLightUp"); }
        }
        else
        {
            if (BonfireEverUsed) { Anim.Play("BonfireLitActivate"); }
            else { Anim.Play("BonfireLitFirstTime"); BonfireEverUsed = true; FireAnim.Play("BonfireLightUp"); }
        }


        RuntimeManager.PlayOneShot(LitBonfire, transform.position);

        yield return new WaitForSeconds(1);
        done();
    }


    void done()
    {
        Debug.LogWarning("Done");
        PC.PlayerFinishInteraction();
        BonfireRest();
        PC.Anim.Play("PlayerAnim_Idle");
        PC.CanMove = true;
        PC.CanAttack = true;
    }

    public void BonfireRest()
    {

            PC.CanMove = false;
            PC.IsMovingInput = false;
            PC.CM.PlayerHealthSlider.value = 100;
            PC.CM.PlayerHealthCatchupSlider.value = 100;
            PC.CM.PlayerStaminaSlider.value = 100;
            PC.CM.PlayerStaminaCatchupSlider.value = 100;


            LayerManager.ChangeLayer(BonfireLayer);

            PC.Health = PC.MaxHealth;
            PC.Stamina = PC.MaxStamina;
            // replenish estus
            PC.CurrentEstus = PC.MaxEstus;
            // Reset Enemies
            if (EnemySaveManagerList != null)
            {
                foreach (GameObject Enemy in EnemySaveManagerList)
                {
                    Enemy.GetComponent<EnemySaveManager>().RespawnEvent.Invoke();
                }
            }

            if (PM.DemonArena.currentState == "Active" || PM.DemonArena.currentState == "Idle") //player died to demon
            {
                PM.DemonArena.SwitchState("Idle");
                PM.DemonArena.ManualStart();

            }
            if (PM.PursuerArena.currentState == "Active") //player died to demon
            {
                PM.PursuerArena.SwitchState("Idle");
                PM.PursuerArena.ManualStart();
            }


            switch (PM.LastBonfireVisited)
            {
                case 1:
                    PM.gameObject.transform.position = PM.Bonfire_1.transform.position;
                    Debug.Log("reset pos");
                    //PM.Bonfire_1.BonfireRest();
                    break;
                case 2:
                    PM.gameObject.transform.position = PM.Bonfire_2.transform.position;
                    //PM.Bonfire_2.BonfireRest();
                    break;
                case 3:
                    PM.gameObject.transform.position = PM.Bonfire_3.transform.position;
                    //PM.Bonfire_3.BonfireRest();
                    break;
                default:
                    PM.gameObject.transform.position = new Vector2(-90, -18);
                    //PM.Bonfire_1.BonfireRest();
                    break;
            }

            //update boss pos
            AsylumDemon.ResetPos();
            Pursuer.ResetPos();

        //upadate oscar
        if (OM != null) { OM.Reload(); }

            //reset pots

            if (PotManagers != null)
            {
                foreach (PotManager PotManager in PotManagers)
                {
                    PotManager.ReloadPots();
                }
            }



        
    }







    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            BonfirePromptUI.SetActive(true);
            GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasManager>().InteractProptDescription.text = ":  Light flame";
            collision.GetComponent<PlayerControllerV2>().Interactable = GetComponent<InteractableV2>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            BonfirePromptUI.SetActive(false);
            GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasManager>().InteractProptDescription.text = "You should not see me";
            collision.GetComponent<PlayerControllerV2>().Interactable = null;
        }
    }

    public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData CurrentCharacterData)
    {

        switch (BonfireTagNum)
        {
            case 1:
                CurrentCharacterData.BonfireEverUsed_1 = BonfireEverUsed;
                break;
            case 2:
                CurrentCharacterData.BonfireEverUsed_2 = BonfireEverUsed;
                break;
            case 3:
                CurrentCharacterData.BonfireEverUsed_3 = BonfireEverUsed;
                break;
        }
    }
    public void LoadGameFromDataToCurrentCharacterData(ref CharacterSaveData CurrentCharacterData)
    {
        switch (BonfireTagNum)
        {
            case 1:
                BonfireEverUsed = CurrentCharacterData.BonfireEverUsed_1;
                break;
            case 2:
                BonfireEverUsed = CurrentCharacterData.BonfireEverUsed_2;
                break;
            case 3:
                BonfireEverUsed = CurrentCharacterData.BonfireEverUsed_3;
                break;
        }
    }

}
