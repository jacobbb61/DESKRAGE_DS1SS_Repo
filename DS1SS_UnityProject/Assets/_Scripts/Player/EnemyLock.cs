using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLock : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Transform nearestEnemyPos;
    [SerializeField] private GameObject player;
    private bool enemyToRight;
    private PlayerController playerController;
    public bool running;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemies = GameObject.FindGameObjectsWithTag("Enemy"); // Assigns all enemy tagged objects into array
        nearestEnemyPos = player.transform;
    }
    void Update()
    {
        if (running) // Placeholder input, this needs to be set to left arrow on dpad
        {
            nearestEnemyPos = GetNearestEnemy(); // Finds nearest enemy every frame
            //nearestEnemyPos.gameObject.GetComponent<SpriteRenderer>().material.color = Color.red; // Turns enemy gameobject red for debugging
        }

        if (nearestEnemyPos.transform.position.x > transform.position.x) // Detects if enemy is to right of player
        {
            enemyToRight = true;
        }
        else
        {
            enemyToRight = false; // Enemy is to left
        }
    }

    public Transform GetNearestEnemy()
    {
        Transform trans = null; // Used for enemy transform
        float nearestDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            float enemyDistance = Vector3.Distance(transform.position, enemy.transform.position); // Distance between attached gameobject and enemy

            if (enemyDistance < nearestDistance)
            {
                nearestDistance = enemyDistance;
                trans = enemy.transform; // Sets transform to closest enemy's transform
            }
        }

        return trans;
    }
}