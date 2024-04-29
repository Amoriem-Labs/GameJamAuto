using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Character : MonoBehaviour
{
    [Header("Character Stats")]
    public float hp;
    public float attackDamage;
    [Tooltip("Number of seconds to wait after an attack before next attack.")]
    public float attackCooldown;
    public float attackCooldownTimer = 0;
    public int range;
    [Space]
    [Header("Level")]
    public int level;
    public float currentXP; 
    public float xpUntilNextLevel;
    public List<float> levelXPCaps;
    public List<float> levelAttackDamages;
    public List<int> coinsToGiveUponDeath;
    public List<int> xpToGiveUponDeath;
    public List<float> levelHPs;
    [Space]
    [Header("Frontend")]
    public float moveSpeed;
    public STATE state;
    public Tile currentTile;
    public Tile destinationTile;
    public List<Tile> path;
    public enum Team{
        Player,
        Enemy,
        Neutral,
    
    };
    public Team team;
    public float rotationSpeed;
    [Space]
    [Header("Target Variables")]
    public Character target;
    public int numTilesToTarget;
    private List<Character> enemyCharacters = new List<Character>();
    private List<int> pathLengths = new List<int>();
    private Animator anim;

    public enum STATE
    {
        Idle,
        Move,
        Pathfinding,
        Attack,
    }

    // Start is called before the first frame update
    void Start()
    {
        level = 0;
        state = STATE.Idle;
        hp = levelHPs[level];
        xpUntilNextLevel = levelXPCaps[level];
        attackDamage = levelAttackDamages[level];
        transform.position = currentTile.transform.position + new Vector3(0, 0.45f, 0);
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.battleOngoing){ // filler variable
            UpdateEnemiesList();
            if (enemyCharacters.Count == 0){ // no more enemies
                state = STATE.Idle;
                if (team == Team.Player){
                    GameManager.Instance.WinRound();
                } else {
                    GameManager.Instance.LoseRound();
                }
            } else {
                switch (state)
                {
                    case STATE.Idle:
                        if (anim != null) anim.Play("Idle");
                        state = STATE.Pathfinding;
                        break;
                    case STATE.Pathfinding:
                        // Pathfind to target
                        if (anim != null) anim.Play("Idle");
                        if (target == null) target = FindNearestEnemy();
                        path = GetPath(target);
                        numTilesToTarget = path.Count - 1;
                        if (path.Count > 1 && destinationTile == null){
                            destinationTile = path[1];
                        }
                        if ((target.state == STATE.Move && numTilesToTarget <= range) || (destinationTile != null && destinationTile.tileType == Tile.TileType.RESERVED)){
                            state = STATE.Idle;
                            break;
                        }
                        if (numTilesToTarget <= range){
                            path = new List<Tile>();
                            state = STATE.Attack;
                        } else {
                            destinationTile.tileType = Tile.TileType.RESERVED;
                            state = STATE.Move;
                        }
                        break;
                    case STATE.Move:
                        // Move to target
                        if (anim != null){
                            anim.Play("Skeleton|Walk");
                        }
                        Quaternion toRotation = Quaternion.LookRotation(destinationTile.transform.position + new Vector3(0, 0.45f, 0) - transform.position, Vector3.up);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
                        bool arrivedAtTile = MoveToTile(path);
                        if (arrivedAtTile)
                        {
                            if (numTilesToTarget > range){
                                state = STATE.Pathfinding;
                            }
                        }
                        break;
                    case STATE.Attack:
                        // Attack target
                        if (numTilesToTarget > range){
                            state = STATE.Pathfinding;
                        }
                        else{
                            Quaternion rotation = Quaternion.LookRotation(destinationTile.transform.position + new Vector3(0, 0.45f, 0) - transform.position, Vector3.up);
                            transform.rotation = rotation;
                            if (attackCooldownTimer > 0)
                            {
                                if (anim != null) anim.Play("Idle");
                                attackCooldownTimer -= Time.deltaTime;
                            }
                            else
                            {
                                if (anim != null){
                                    anim.Play("Skeleton|Melee_1");
                                    Debug.Log("playing melee animation");
                                }
                                attackCooldownTimer = attackCooldown;
                                if (Attack(target)) // if target is dead
                                {
                                    // target is dead
                                    state = STATE.Pathfinding;
                                }
                            }
                        }
                        break;
                }
            }
        }
    }

    public Character FindNearestEnemy()
    {
        UpdateEnemiesList();
        // find element with smallest length in paths
        int minIndex = pathLengths.IndexOf(pathLengths.AsQueryable().Min());
        return enemyCharacters[minIndex];
    }

    public void UpdateEnemiesList(){
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Character");
        enemyCharacters = new List<Character>();
        pathLengths = new List<int>();
        foreach (GameObject character in characters)
        {
            Character charScript = character.GetComponent<Character>();
            
            if ((team == Team.Player && charScript.team == Team.Enemy) || (team == Team.Enemy && charScript.team == Team.Player)){
                Tile enemyTile = charScript.currentTile;
                pathLengths.Add(GameManager.Instance.board.GetPathToTile(currentTile, tile => tile.xCoord == enemyTile.xCoord && tile.yCoord == enemyTile.yCoord).Count);
                enemyCharacters.Add(charScript);
            }
        }
    }

    public List<Tile> GetPath(Character otherChar)
    {
        return GameManager.Instance.board.GetPathToTile(currentTile, tile => tile.xCoord == otherChar.currentTile.xCoord && tile.yCoord == otherChar.currentTile.yCoord);
    }

    public bool MoveToTile(List<Tile> path)
    {
        float step = moveSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, path[1].transform.position + new Vector3(0, 0.45f, 0), step);
        currentTile.tileType = Tile.TileType.FREE;
        destinationTile.tileType = Tile.TileType.RESERVED;

        if (Vector3.Distance(transform.position, path[1].transform.position + new Vector3(0, 0.45f, 0)) < 0.001f)
        {
            currentTile = path[1];
            destinationTile.tileType = Tile.TileType.OCCUPIED;
            destinationTile = null;
            return true;
        }
        return false;
    }

    // Attack target. Return true if target is dead, return false otherwise.
    public bool Attack(Character target)
    {
        if (target == null) return true;
        // Attack target
        target.hp -= attackDamage;
        if (target.hp <= 0)
        {
            GameManager.Instance.coins += coinsToGiveUponDeath[level];
            currentXP += xpToGiveUponDeath[level];
            Destroy(target.gameObject);
            target = null;
            UpdateLevel();
            return true;
        }
        return false;
    }

    public void UpdateLevel()
    {
        if (currentXP >= xpUntilNextLevel)
        {
            level++;
            hp = levelHPs[level];
            xpUntilNextLevel = levelXPCaps[level];
            attackDamage = levelAttackDamages[level];
        }
    }
}
