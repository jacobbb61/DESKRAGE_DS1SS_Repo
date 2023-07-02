using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Undead_C : MonoBehaviour
{

    [Header("Stats")]
    public float Health;
    public float MaxHealth;
    public string Behaviour;
    public int LookDirection;

    [Header("Move and Detect Data")]
    public float Speed;
    public float BaseSpeed;
    public float ForwardDetectionRange;
    public float BackwardsDetectionRange;
    public float AttackTriggerRange;
    public float VerticalSpeed;
    public float FallSpeed;


    [Header("Combat Data")]
    public float TimeBeforeAttack;
    public float AttackAnimationTime;
    public float AttackCoolDownTime;
    public float StaggerTime;
    public float AttackRange;
    public float AttackDamage;

    public float CombatTime;

    [Header("Bools")]
    public bool SeePlayer;
    public bool IsAttacking;
    public bool IsDead;
    public bool IsAtOrigin;
    public bool IsGrounded;
    public bool IsOnSlope;
    public bool FootAOnSlope;
    public bool FootBOnSlope;

    private Animator Anim;
    private GameObject Player;
    private Rigidbody2D RB;
    private Coroutine AttackingCoroutine;

    public GameObject Assets;
    public GameObject Eyes;

    public Transform GroundCheckPosA;
    public Transform GroundCheckPosB;

    public Transform HitStartPos;
    public Slider HealthSlider;
    public Vector3 OriginPosition;

    void Start()
    {
        Anim = GetComponentInChildren<Animator>();
        RB = GetComponent<Rigidbody2D>();
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Behaviour == null) { Behaviour = "Idle"; }
        if (LookDirection == 1) { Assets.transform.localScale = new Vector3(2, 2, 2); }
        else if (LookDirection == -1) { Assets.transform.localScale = new Vector3(-2, 2, 2); }

        IsAttacking = false;

        HealthSlider.maxValue = MaxHealth;

        if (Health > 0) { IsDead = false; } else { Dead(); Behaviour = "Dead"; }
    }

    public void Respawn()
    {
        IsDead = false;
        Assets.SetActive(true);
        gameObject.SetActive(true);
        Health = MaxHealth;
        HealthSlider.value = Health;
        transform.localPosition = OriginPosition;
        Behaviour = "Idle";
    }

    void Update()
    {   
        UpdateUI();
        IsByOriginPosition();
        if (Health <= 0) { StartCoroutine(Death()); }
        else
        {
            IsDead = false;
            switch (Behaviour)
            {
                case "Idle":
                    LookForPlayer();
                    Anim.Play("UndeadAnim_C_Idle");
                    if (SeePlayer) { Behaviour = "Hostile"; }
                    if (!IsAtOrigin) { Behaviour = "Returning"; } else { RB.velocity = Vector2.zero; }
                    break;
                case "Hostile":
                    CombatTime += Time.deltaTime;
                    LookForPlayer();
                    if (IsInAttackRange()) { Behaviour = "Attacking"; }
                    else
                    {
                        FacePlayer();
                        Walk();
                    }
                    if (!SeePlayer && !IsAtOrigin && CombatTime>4) {Behaviour = "Returning"; CombatTime = 0; }
                    break;
                case "Attacking":
                    GroundCheck();
                    if (!IsAttacking) { AttackingCoroutine =  StartCoroutine(Attack()); } 
                    break;
                case "Staggered":
                    StartCoroutine(Staggered());
                    RB.velocity = Vector2.zero;
                    break;
                case "Parried":
                    break;
                case "Returning":
                    LookForPlayer();
                    if (SeePlayer) { Behaviour = "Hostile"; }                  
                    if (!IsAtOrigin) { FaceOrigin(); WalkToOrigin(); GroundCheck(); }
                    else { Behaviour = "Idle"; }
                    break;
                case "Dead":
                    Dead();
                    RB.velocity = Vector2.zero;
                    break;
                default:
                    Behaviour = "Idle";
                    break;
            }
        }
    }



    IEnumerator Death()
    {
        Behaviour = "Dead";
        if (IsAttacking) { StopCoroutine(AttackingCoroutine); }
        HealthSlider.value = 0;
        Anim.Play("UndeadAnim_C_Death");
        yield return new WaitForSeconds(3);

        Dead();

    }

    public void Dead()
    {
        Health = 0;
        IsDead = true;
        Assets.SetActive(false);
        gameObject.SetActive(false);
    }

    public void TakeLightDamage()
    {
        Health -= 6;
    }
    public void TakeHeavyDamage()
    {
        Health -= 9;
        Behaviour = "Staggered";
    }
    IEnumerator Staggered()
    {  
        Anim.Play("UndeadAnim_C_GettingHit");
        yield return new WaitForSeconds(StaggerTime);
        Behaviour = "Hostile";
    }

    void UpdateUI()
    {
        HealthSlider.value = Health;
    }

    void LookForPlayer()
    {

        float Range = 0;

        if (transform.position.x > Player.transform.position.x && LookDirection == 1) //looking towards player
        {
            Range = ForwardDetectionRange;
        }
        if (transform.position.x < Player.transform.position.x && LookDirection == -1) //looking towards player
        {
            Range = ForwardDetectionRange;
        }
        if (transform.position.x > Player.transform.position.x && LookDirection == -1) //looking away from player
        {
            Range = BackwardsDetectionRange;
        }
        if (transform.position.x < Player.transform.position.x && LookDirection == 1) //looking away from player
        {
            Range = BackwardsDetectionRange;
        }


        
        
        if (Vector3.Distance(Eyes.transform.position, Player.transform.position) < Range)
        {
            int layerMask = ~(LayerMask.GetMask("Ignore Raycast"));
            RaycastHit2D hit = Physics2D.Raycast(Eyes.transform.position, (Player.transform.position - Eyes.transform.position), Range, layerMask);
            
            if (hit.transform != null)
            {
                
                
                if (hit.transform.CompareTag("Player"))
                {
                    SeePlayer = true;
                }
                else
                {
                    Debug.Log("Undead C sees" + hit.transform.name);
                    SeePlayer = false;
                }

            } 
            else
                {
                    SeePlayer = false;
                }
        }
        else
        {
            SeePlayer = false;
        }
    }
    void FacePlayer()
    {
        if(transform.position.x > Player.transform.position.x)
        {
            LookDirection = 1;
        }
        else
        {
            LookDirection = -1;
        }

        if (LookDirection == 1) { Assets.transform.localScale = new Vector3(2, 2, 2); }
        else if (LookDirection == -1) { Assets.transform.localScale = new Vector3(-2, 2, 2); }
    }


    void GroundCheck()
    {
        int layerMask = ~(LayerMask.GetMask("Player"));
        RaycastHit2D hitA = Physics2D.Raycast(GroundCheckPosA.position, Vector2.down, .45f, layerMask);
            RaycastHit2D hitB = Physics2D.Raycast(GroundCheckPosB.position, Vector2.down, .45f, layerMask);

            if (hitA.collider != null)
            {
                if (hitA.transform.CompareTag("Ground"))
                {
                    IsGrounded = true;
                    FootAOnSlope = false;
                }
                else if (hitA.transform.CompareTag("Slope"))
                {
                    IsGrounded = true;
                    FootAOnSlope = true;
                }
                else
                {
                    IsGrounded = false;
                    FootAOnSlope = false;
                }
            }
            if (hitB.collider != null)
            {
                if (hitB.transform.CompareTag("Ground"))
                {
                    IsGrounded = true;
                    FootBOnSlope = false;
                }
                else if (hitB.transform.CompareTag("Slope"))
                {
                    IsGrounded = true;
                    FootBOnSlope = true;
                }
                else
                {
                    IsGrounded = false;
                    FootBOnSlope = false;
                }
            }
            if (hitA.collider == null && hitB.collider == null)
            {
                IsGrounded = false;
            }


            
                if (FootAOnSlope && FootBOnSlope && hitA.transform != null) { OnSlope(hitA.transform); }
                else if (FootAOnSlope && !FootBOnSlope && hitA.transform != null) { OnSlope(hitA.transform); }
                else if (!FootAOnSlope && FootBOnSlope && hitB.transform != null) { OnSlope(hitB.transform); }
                else if (!FootAOnSlope && !FootBOnSlope)
                {
                    //not on slope reset speeds
                    Speed = BaseSpeed;
                   if (IsGrounded) { VerticalSpeed = 0.5f; } else { VerticalSpeed = FallSpeed; }
                }
                 
    }

    void OnSlope(Transform Slope)
    {

        float SlopeAngle = Slope.transform.transform.eulerAngles.z;
        if (SlopeAngle <= 35 && SlopeAngle < 60)
        {
            // right  a 35 or less slope
            if (LookDirection == 1)
            { //going against slope
                Speed = 1f;
            }
            else
            {//going with slope

                Speed = 4f;
                if (FootAOnSlope && !FootBOnSlope) { Speed = 2.5f; VerticalSpeed = 0f; }
                if (!FootAOnSlope && FootBOnSlope) { Speed = 2.5f; VerticalSpeed = 0f; }
            }

        }
        else if (SlopeAngle > 35 && SlopeAngle < 60)
        {
            // right  a 35 or MORE slope
            if (LookDirection == 1)
            { //going against slope
                Speed = 4.5f;
            }
            else
            {//going with slope
                Speed = 1f;
                if (FootAOnSlope && !FootBOnSlope) { Speed = 2.5f; VerticalSpeed = 0f; }
                if (!FootAOnSlope && FootBOnSlope) { Speed = 2.5f; VerticalSpeed = 0f; }
            }
        }
        if (SlopeAngle >= 325 && SlopeAngle > 300)
        {
            // left a 35 or less slope
            if (LookDirection == 1)
            { //going against slope
                Speed = 1f;
                if (FootAOnSlope && !FootBOnSlope) { Speed = 2.5f; VerticalSpeed = 0f; }
                if (!FootAOnSlope && FootBOnSlope) { Speed = 2.5f; VerticalSpeed = 0f; }
            }
            else
            {//going with slope
                Speed = 4f;
            }
        }
        else if (SlopeAngle < 325 && SlopeAngle > 300)
        {
            // left a 35 or MORE slope
            if (LookDirection == 1)
            { //going with slope
                Speed = 1f;

                if (FootAOnSlope && !FootBOnSlope) { Speed = 2.5f; VerticalSpeed = 0f; }
                if (!FootAOnSlope && FootBOnSlope) { Speed = 2.5f; VerticalSpeed = 0f; }
            }
            else
            {//going against slope
                Speed = 4.5f;
            }
        }
    }

    void Walk()
    {
        GroundCheck();
        if (IsGrounded)
        {
            RB.velocity = new Vector2(-Speed * LookDirection, -VerticalSpeed);
            Anim.Play("UndeadAnim_C_Walk");
        }
    }

    void IsByOriginPosition()
    {
        if (Vector3.Distance(transform.localPosition, OriginPosition) < 2)
        {
            IsAtOrigin = true;
        }
        else
        {
            IsAtOrigin = false;
        }
    }


    void FaceOrigin()
    {
        if (transform.position.x > OriginPosition.x)
        {
            LookDirection = -1;
        }
        else
        {
            LookDirection = 1;
        }

        if (LookDirection == 1) { Assets.transform.localScale = new Vector3(2, 2, 2); }
        else if (LookDirection == -1) { Assets.transform.localScale = new Vector3(-2, 2, 2); }
    }
    void WalkToOrigin()
    {
        RB.velocity = new Vector2(-Speed * LookDirection, -5);
        Anim.Play("UndeadAnim_C_Walk");
    }



    bool IsInAttackRange()
    {
        if (Vector3.Distance(Eyes.transform.position, Player.transform.position) < AttackTriggerRange)
        {
            Anim.Play("UndeadAnim_C_Idle");
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator Attack()
    {
        IsAttacking = true;
        RB.velocity = Vector2.zero;
        yield return new WaitForSeconds(TimeBeforeAttack);
        FacePlayer();
        Anim.Play("UndeadAnim_C_SwingAttack");
        yield return new WaitForSeconds(AttackAnimationTime);
        Anim.Play("UndeadAnim_C_Idle");
        yield return new WaitForSeconds(AttackCoolDownTime);
        IsAttacking = false;
        Behaviour = "Hostile";
    }

    public void AttackStep()
    {
        RB.velocity = new Vector2((-Speed -1) * LookDirection, -VerticalSpeed);
    }

    public void AttackRegister()
    {
        int layerMask = ~(LayerMask.GetMask("Enemy"));
        RaycastHit2D hit = Physics2D.Raycast(HitStartPos.position, new Vector2(-LookDirection, 0), AttackRange, layerMask);


        if (hit.collider != null)
        {
            if (hit.transform.CompareTag("Player"))
            {
                hit.transform.GetComponent<PlayerControllerV2>().PlayerTakeDamage(AttackDamage, false, 0);
            }
        }
    }
}
