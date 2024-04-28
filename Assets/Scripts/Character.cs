using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Character Stats")]
    public float HP;
    public float attackDamage;
    [Tooltip("Number of seconds to wait after an attack before next attack.")]
    public float attackCooldown;
    public float attackCooldownTimer = 0;
    public int range;
    [Space]
    [Header("Level")]
    public int level;
    public float currentXP; 
    public float XPuntilNextLevel;
    public List<float> levelXPCaps;
    public List<float> levelAttackDamageMultipliers;
    public List<int> coinsToGiveUponDeath;
    public List<int> XPtoGiveUponDeath;
    [Space]
    [Header("Frontend")]
    public float moveSpeed;
    public STATE state;
    [Space]
    [Header("Target Variables")]
    public Character target;
    public int numTilesToTarget;

    public enum STATE
    {
        Idle,
        Move,
        Pathfinding,
        Attack,
    }

    public enum CLASS
    {
        Melee,
        Ranged,
    }

    // Start is called before the first frame update
    void Start()
    {
        state = STATE.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.battleOngoing){ // filler variable
            switch (state)
            {
                case STATE.Idle:
                    state = STATE.Pathfinding;
                    break;
                case STATE.Pathfinding:
                    // Pathfind to target
                    if (target == null) target = FindNearestEnemy();
                    //Tile[] path = CreatePathToChar(target);
                    state = STATE.Move;
                    break;
                case STATE.Move:
                    /*bool arrivedAtTile = MoveToTile(path);
                    if (arrivedAtTile)
                    {
                        if (numTilesToTarget > range){
                            state = STATE.Pathfinding;
                        }
                        else{
                            state = STATE.Attack;
                        }
                    }*/
                    break;
                case STATE.Attack:
                    // Attack target
                    if (attackCooldownTimer > 0)
                    {
                        attackCooldownTimer -= Time.deltaTime;
                    }
                    else
                    {
                        attackCooldownTimer = attackCooldown;
                        if (Attack(target)) // if target is dead
                        {
                            // target is dead
                            state = STATE.Pathfinding;
                        }
                    }
                    break;
            }
        }
    }

    public Character FindNearestEnemy()
    {
        // Find nearest enemy
        return null;
    }

    public void CreatePathToChar(Character otherChar) // change return type to Tile[] when that is implemented
    {
        // Create path to other character
        
    }

    /*
    public bool MoveToTile(int[] path) // change parameter type to Tile[] when that is implemented
    {
        float step = moveSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, path[0].position, step);

        if (Vector3.Distance(transform.position, target.transform.position) < 0.001f)
        {
            return true;
        }
        return false;
    }
    */

    // Attack target. Return true if target is dead, return false otherwise.
    public bool Attack(Character target)
    {
        // Attack target
        target.HP -= attackDamage;
        if (target.HP <= 0)
        {
            target = null;
            GameManager.Instance.coins += coinsToGiveUponDeath[level];
            currentXP += XPtoGiveUponDeath[level];
            Destroy(target.gameObject);
            return true;
        }
        return false;
    }
}
