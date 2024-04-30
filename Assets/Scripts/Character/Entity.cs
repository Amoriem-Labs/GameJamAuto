using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    private EntityUI ui;

    public virtual float maxHealth { get; set; } = 100f;
    public virtual float shield { get; set; } = 0f;
    public virtual float attackDamage { get; set; } = 50f;
    public virtual float abilityPower { get; set; } = 100f;
    public virtual float startingMana { get; set; } = 50f;
    public virtual float maxMana { get; set; } = 100f;
    public virtual float attackCooldown { get; set; } = 1f;

    public virtual float moveSpeed { get; set; } = 1f;
    public virtual int range { get; set; } = 1;

    public float health;
    public float currMana;

    public float attackCooldownTimer = 0;

    public Entity target;

    public Team targetTeam;

    public Tile currentTile;
    public Tile destinationTile;
    public Tile targetTile;

    public List<Tile> currentPath;

    protected float stunTimer = 0f;
    protected State lastStunState;

    public delegate void EntityEvent(Entity self);
    public delegate void EntityDamageEvent(Entity self, float dmg, Entity sender = null);

    public static event EntityEvent onSpawn;
    public static event EntityEvent onDeath;
    public static event EntityEvent onDestroy;
    public static event EntityDamageEvent onTakeHealthDamage;

    public enum Team {
        PLAYER,
        ENEMY,
        NEUTRAL,
        NULL,

    };
    public enum State {
        IDLE,
        MOVE,
        PATHFINDING,
        ATTACK,
        CASTING,
        STUNNED
    }
    public Team team = Team.NULL;
    public State state = State.IDLE;
    public Animator anim;

    void Awake() {
        ui = GetComponent<EntityUI>();
    }
    protected void Start() {
        health = maxHealth;
        onSpawn?.Invoke(this);
    }

    public void commence(Tile tile) {
        currentTile = tile;
        tile.currentOccupant = this;
        tile.tileType = Tile.TileType.OCCUPIED;
    }

    void Update() {
        ui.updateUI(this);
    }

    public virtual void tick() {
        if (!(GameManager.Instance.game?.gameplayState == Game.GameplayState.BATTLING)) { return; }
        switch (state) {
            case State.IDLE:
                if (anim != null) anim.Play("Idle");
                target = null;
                destinationTile = null;
                targetTile = null;
                if (GameManager.Instance.game.enemyUnits.Count > 0) {
                    state = State.PATHFINDING;
                }
                break;
            case State.PATHFINDING:
                // Pathfind to target
                if (anim != null) anim.Play("IDLE");

                target = null;
                destinationTile = null;
                targetTile = null;

                currentPath = FindNearestPathToTeam(targetTeam);

                int numTilesToTarget = currentPath.Count - 1;
                if (currentPath != null && currentPath.Count > 0) {
                    target = currentPath[currentPath.Count - 1].currentOccupant;
                    targetTile = currentPath[currentPath.Count - 1];
                }
                else {
                    state = State.IDLE;
                    break;
                }

                if (numTilesToTarget <= range) {
                    currentPath = new List<Tile>() { currentTile };
                    state = State.ATTACK;
                    break;
                }

                if (target.destinationTile != null && currentTile != null && Board.getDistBetweenTiles(target.destinationTile, currentTile) <= 1) {
                    //print("waiting");
                    state = State.IDLE;
                    break;
                }
                if (currentPath.Count > 1 && destinationTile == null && currentPath[1].tileType == Tile.TileType.FREE) {
                    destinationTile = currentPath[1];

                    destinationTile.tileType = Tile.TileType.RESERVED;
                    state = State.MOVE;
                    break;
                }
                break;
            case State.MOVE:
                // Move to target
                if (anim != null) {
                    anim.Play("Skeleton|Walk");
                }
                turnTowardTile(destinationTile);
                bool arrivedAtTile = MoveToTile(currentPath);
                if (arrivedAtTile) {
                    currentTile.tileType = Tile.TileType.FREE;
                    currentTile.currentOccupant = null;
                    currentTile = currentPath[1];
                    currentPath[1].tileType = Tile.TileType.OCCUPIED;
                    currentPath[1].currentOccupant = this;
                    destinationTile = null;
                    if (Board.getDistBetweenTiles(target.currentTile, currentTile) > range) {
                        state = State.PATHFINDING;
                    }
                    else {
                        state = State.ATTACK;
                    }
                }
                break;
            case State.ATTACK:
                // Attack target
                if (target == null) {
                    state = State.PATHFINDING;
                }
                if (Board.getDistBetweenTiles(target.currentTile, currentTile) > range) {
                    state = State.PATHFINDING;
                }
                else {
                    turnTowardTile(target.currentTile);
                    if (attackCooldownTimer > 0) {
                        if (anim != null) anim.Play("IDLE");
                        attackCooldownTimer -= Time.deltaTime;
                    }
                    else {
                        if (anim != null) {
                            anim.Play("Skeleton|Attack");
                        }
                        attackCooldownTimer = attackCooldown;
                        attack(target, attackDamage);
                        if (target == null) {
                            state = State.PATHFINDING;
                        }
                    }
                }
                break;
            case State.STUNNED:
                stunTimer -= Time.deltaTime;
                if (stunTimer <= 0) {
                    stunTimer = 0;
                    state = lastStunState;
                    break;
                }
                break;
        }
    }

    public void turnTowardTile(Tile tile) {
        Vector3 targetPosition = new Vector3(tile.transform.position.x, transform.position.y, tile.transform.position.z);
        Quaternion toRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
        transform.rotation = toRotation;
    }

    public abstract void doAbility();

    public virtual void attack(Entity otherEntity, float dmg) {
        //print($"attack {this.team}");
        if (otherEntity != null) {
            otherEntity.TakeDamage(dmg);
        }
        currMana += 10f;
        if (currMana >= maxMana) {
            currMana = 0;
            doAbility();
        }
    }

    public Entity FindNearestOfTeam(Team searchTeam, bool pureDistance = false) {
        List<Tile> path = GameManager.Instance.board.GetPathToTile(currentTile, tile => (tile.currentOccupant?.team ?? Entity.Team.NULL) == searchTeam, !pureDistance);

        return path[path.Count - 1].currentOccupant;
    }

    public List<Tile> FindNearestPathToTeam(Team searchTeam, bool pureDistance = false) {
        List<Tile> path = GameManager.Instance.board.GetPathToTile(currentTile, tile => (tile.currentOccupant?.team ?? Entity.Team.NULL) == searchTeam, !pureDistance);

        return path;
    }

    public List<Tile> GetPathToEntity(Entity otherEntity) {
        return GameManager.Instance.board.GetPathToTile(currentTile, tile => tile.xCoord == otherEntity.currentTile.xCoord && tile.yCoord == otherEntity.currentTile.yCoord);
    }

    public bool MoveToTile(List<Tile> path) {
        float step = moveSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, path[1].transform.position + new Vector3(0, transform.position.y, 0), step);

        if (Vector3.Distance(transform.position, path[1].transform.position + new Vector3(0, transform.position.y, 0)) < 0.001f) {
            return true;
        }
        return false;
    }

    public void validateUI() {

    }

    public void TakeDamage(float dmg, Entity sender = null) {
        if (this == null) {
            return;
        }
        if (shield > dmg) {
            shield -= dmg;
            dmg = 0;
        }
        else {
            dmg -= shield;
            shield = 0;
        }
        health -= dmg;
        onTakeHealthDamage?.Invoke(this, dmg, sender);
        if (health <= 0) {
            die();
        }
    }

    public virtual void die() {
        currentTile.currentOccupant = null;
        currentTile.tileType = Tile.TileType.FREE;
        onDeath?.Invoke(this);
        Destroy(gameObject);
        onDestroy?.Invoke(this);
    }

    public virtual void stun(float time) {
        stunTimer += time;
        if (state != State.STUNNED) {
            lastStunState = state;
            state = State.STUNNED;
        }
    }
}
