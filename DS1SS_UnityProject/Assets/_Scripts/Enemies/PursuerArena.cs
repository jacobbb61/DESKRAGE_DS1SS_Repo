using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PursuerArena : MonoBehaviour
{
    [Tooltip("Doors in this array should be , , ,  and , in that order")]
    [SerializeField] DoorManager doorT;
    [SerializeField] DoorManager doorU;
    [SerializeField] CollapseBridge Bridge; 
    public bool inBossFight;
    public bool arenaIsActive; 
    public bool IsSecondPhase;
    public string currentState;

    public GameObject BossUI;
    public PlayerManager playerManager;
    public SpeedRunManager speedRunManager;

    public Pursuer Boss;


    public GameObject BossCam;

    public GameObject ParticleSystem;
    public NightmareAttackSpawner nightmareAttackSpawner;


    public Animator VictoryAnim;

    public EventReference BossKilledAudio;
    public EventReference Theme_FirstPhase;
    public EventReference Theme_SecondPhase;
    public FMOD.Studio.EventInstance FMODinstance;

    public void ManualStart()
    {
        SwitchState(currentState);
        doorT.PursuerArena = this;
        doorU.PursuerArena = this;

        Boss.ManualStart();
    }
    private void OnDisable()
    {
        StopMusic();
    }
    public void StopMusic()
    {
        FMODinstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
    public void SecondPhase()
    {
        IsSecondPhase = true;
        StopMusic();
        FMODinstance = FMODUnity.RuntimeManager.CreateInstance(Theme_SecondPhase);
        FMODinstance.start(); 
        FMODinstance.release();
        ParticleSystem.SetActive(true);
    }
    public void EnterArena() //called from door manager
    {
        doorT.SwitchDoorState("Fog");
        doorU.SwitchDoorState("Fog");
        doorT.inBossFight = true;
        doorU.inBossFight = true;
        doorT.ManualStart();
        doorU.ManualStart();

        BossCam.SetActive(true);

        if (currentState == "FirstTime")
        {
            StartCinematic();


           // SwitchState("Active");

            inBossFight = true;
            arenaIsActive = true;
           // Boss.Behaviour = "Hostile";
        }

        if (currentState == "Idle")
        {
            Boss.ResetPos();
            SwitchState("Active");
            inBossFight = true;
            arenaIsActive = true;
            Boss.Behaviour = "Hostile";
        }

        FMODinstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        FMODinstance = FMODUnity.RuntimeManager.CreateInstance(Theme_FirstPhase);
        FMODinstance.start();
        FMODinstance.release();

    }
    IEnumerator Wait(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
    }
    private void OnEnable()
    {
        SwitchState(currentState);
    }

    public void SwitchState(string state)
    {
        currentState = state;

        switch (currentState)
        {
            case "FirstTime":
                {   //Door states are "Open", "Closed", "Locked", "OneSided", "Fog", "FogEnter"
                    speedRunManager.PersuerReset();
                    doorT.SwitchDoorState("Closed"); 
                    doorU.SwitchDoorState("Locked");
                    doorT.ManualStart();
                    doorU.ManualStart();
                    Bridge.currentState="Closed";
                    Bridge.ManualStart();
                    inBossFight = false;
                    arenaIsActive = false;
                    BossUI.SetActive(false);
                    Boss.IsActive = false;
                    Boss.IsDead = false;
                    Boss.IsTurning = false;
                    Boss.IsCoolingDown = false;
                    Boss.Health = Boss.MaxHealth;
                    Boss.Behaviour = "FirstTime";
                    Boss.CanPhaseChanged = false;


                    if (nightmareAttackSpawner != null) { nightmareAttackSpawner.stop = true; ParticleSystem.SetActive(false); }

                    IsSecondPhase = false;
                    break;
                }
            case "Idle":
                {
                    speedRunManager.PersuerReset();


                    ParticleSystem.SetActive(false);
                    if (nightmareAttackSpawner != null) { nightmareAttackSpawner.stop = true; }

                    doorT.SwitchDoorState("FogEnter");
                    doorU.SwitchDoorState("Fog");
                    doorT.ManualStart();
                    doorU.ManualStart();
                    Bridge.currentState = "Open";
                    Bridge.ManualStart();
                    inBossFight = false;
                    arenaIsActive = false;
                    BossUI.SetActive(false);
                    Boss.IsActive = false;
                    Boss.IsDead = false;
                    Boss.IsTurning = false;
                    Boss.IsCoolingDown = false;
                    Boss.Health = Boss.MaxHealth;
                    Boss.UpdateUI();
                    Boss.StopAllCoroutines();
                    Boss.Behaviour = "Idle";
                    Boss.ManualStart();
                    StopMusic();
                    IsSecondPhase = false;
                    Boss.CanPhaseChanged = false;
                    BossCam.SetActive(false);
                    break;
                }
            case "Active":
                {
                    speedRunManager.pursuerTime = 0;
                    speedRunManager.inPursuer = true;
                    doorT.SwitchDoorState("Fog");
                    doorU.SwitchDoorState("Fog");
                    doorT.ManualStart();
                    doorU.ManualStart();
                    Bridge.currentState = "Open";
                    Bridge.ManualStart();
                    inBossFight = true;
                    arenaIsActive = true;
                    BossUI.SetActive(true);
                    Boss.IsActive = true;
                    Boss.IsDead = false;
                    Boss.IsTurning = false;
                    Boss.IsCoolingDown = false;
                    Boss.Health = Boss.MaxHealth;
                    Boss.Behaviour = "Hostile";
                   /*
                    FMODinstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    FMODinstance = FMODUnity.RuntimeManager.CreateInstance(Theme_FirstPhase);
                    FMODinstance.start();
                    FMODinstance.release();
                   */
                    break;
                }
            case "Open":
                {
                    inBossFight = false;
                    arenaIsActive = false;
                    BossUI.SetActive(false);
                    Boss.IsActive = false;
                    Boss.IsDead = true;
                    Boss.IsTurning = false;
                    Boss.IsCoolingDown = false;
                    Boss.gameObject.SetActive(false);
                    Boss.Behaviour = "Dead";
                    if (nightmareAttackSpawner != null) { nightmareAttackSpawner.stop = true; ParticleSystem.SetActive(false);}

                    break;
                }
        }
    }

    public void Victory()
    {
        RuntimeManager.PlayOneShot(BossKilledAudio, transform.position);
        VictoryAnim.Play("Active");


        ParticleSystem.SetActive(false);
    }
    public void BossKilled() //Called by boss script
    {
        speedRunManager.PursuerKilled();
        SwitchState("Open");
        arenaIsActive = false;

        if (nightmareAttackSpawner != null) { nightmareAttackSpawner.stop = true; }

        BossCam.SetActive(false);

        doorT.SwitchDoorState("Locked");
        doorT.ManualStart();

        doorU.SwitchDoorState("Closed");
        doorU.ManualStart();

        Bridge.currentState = "Open";
        Bridge.ManualStart();




        WorldSaveGameManager.Instance.Player = playerManager;
        WorldSaveGameManager.Instance.SaveGame();
        // Audio stuffs
        // Wait(time);
        // Disable boss health
        // Achievement and saving stuff
    }



    public void StartCinematic()
    {
        inBossFight = true;
        arenaIsActive = true;
        Boss.Behaviour = "Cinematic";
        Boss.Assets.SetActive(true);
        //trigger bridge wait to break
        Bridge.StartCoroutine(Bridge.WaitToBreak());
        //trigger bridge animation
        Bridge.Anim.Play("Breaking");
        //trigger boss animation
        Boss.Anim.Play("Intro");

        StartCoroutine(WaitToStartBoss());
    }

    IEnumerator WaitToStartBoss()
    {
        yield return new WaitForSeconds(2.5f);
        SwitchState("Active");        
    }

}
