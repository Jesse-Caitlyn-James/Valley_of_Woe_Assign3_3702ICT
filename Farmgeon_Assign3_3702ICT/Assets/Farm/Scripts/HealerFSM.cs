using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class HealerFSM : MonoBehaviour
{
    public enum FSMModes{
        None,
        Stalk,
        Convert,
        Hide,
        Dead,
        Boss,
    }
    public FSMModes currentState;
    public float health = 50.0f;
    public float damage = 50.0f;
    public float range = 10.0f;
    public GameObject projectile;
    
    private Transform playerTransform;
    private NavMeshAgent nav;
    private GameObject[] hidePoints;
    private GameObject currentHidePoint;
    private GameObject currentConvertTarget;
    private GameObject GameManager;
    private float pathTime;
    private float cooldown;
    private float convertTime;
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        GameManager = GameObject.FindGameObjectWithTag("GameController");
        nav = GetComponent<NavMeshAgent>();
        currentState = FSMModes.Stalk;
        hidePoints = GameObject.FindGameObjectsWithTag("HidePoint");
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case FSMModes.None:
                break;
            case FSMModes.Stalk:
                Stalk();
                break;
            case FSMModes.Convert:
                Convert();
                break;
            case FSMModes.Hide:
                Hide();
                break;
            case FSMModes.Dead:
                Dead();
                break;
            case FSMModes.Boss:
                Boss();
                break;
        }

        pathTime += Time.deltaTime;
        cooldown += Time.deltaTime;

        Attack();

        // Handles low health states
        if (health < 30.0f)
        {
            currentState = FSMModes.Hide;
        }
        if (health <= 0.0f)
        {
            currentState = FSMModes.Dead;
        }
    }

    // Follows the player from a distance
    // Checks if it can convert a crop
    void Stalk()
    {
        convertTime += Time.deltaTime;
        if (pathTime > 1.0f)
        {
            pathTime = 0.0f;
            nav.SetDestination(playerTransform.position);
            nav.stoppingDistance = 20.0f;
        }

        if (convertTime > 60.0f)
        {
            currentState = FSMModes.Convert;
        }
    }

    // Forcefully converts a plant in a crop plot to an enemy
    void Convert()
    {
        if (currentConvertTarget == null)
        {
            foreach (GameObject farmland in GameObject.FindGameObjectsWithTag("Farmland"))
            {
                // Finds a planted field
                if(farmland.GetComponent<Crop>().isPlanted)
                {
                    currentConvertTarget = farmland;
                    nav.SetDestination(currentConvertTarget.transform.position);
                    nav.stoppingDistance = 0.0f;
                    break;
                }
            }
            if ( currentConvertTarget == null)
            {
                convertTime = 0.0f;
                currentState = FSMModes.Stalk;
            }
        }
        else
        {
            // Once converted return to normal state
            float dist = Vector3.Distance(transform.position, currentConvertTarget.transform.position);
            if (dist < 2.0f)
            {
                currentConvertTarget.SendMessage("Uproot");
                convertTime = 0.0f;
                currentConvertTarget = null;
                currentState = FSMModes.Stalk;
            }
        }
    }

    // Sends the npc to a spot generally out of player line of sight
    void Hide()
    {
        if (currentHidePoint == null)
        {
            hidePoints = GameObject.FindGameObjectsWithTag("HidePoint");
            currentHidePoint = hidePoints[Random.Range(0, hidePoints.Length - 1)];

            nav.SetDestination(currentHidePoint.transform.position);
            nav.stoppingDistance = 0.0f;
        }
    }

    // Dead Logic
    void Dead()
    {
        if (!isDead)
        {
            isDead = true;
            nav.isStopped = true;
            explode();
            Destroy(gameObject, 2.0f);
            GameManager.SendMessage("enemyKilled");
        }
    }

    // Simpler state for boss battle
    void Boss()
    {
        if (pathTime > 1.0f)
        {
            pathTime = 0.0f;
            nav.SetDestination(playerTransform.position);
            nav.stoppingDistance = 20.0f;
        }
        Attack();
    }

    // Shoots homing projectiles when player nearby
    void Attack()
    {
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (cooldown > 5.0f & dist <= 15.0f)
        {
            cooldown = 0.0f;
            GameObject spore = Instantiate(projectile, transform.position, transform.rotation);
        }
    }

    // Handler for a large explosion on death
    void explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 15.0f);
        foreach (Collider hit in hitColliders)
        {
            if (hit.tag == "Player")
            {
                hit.gameObject.SendMessage("ApplyDamage", 50.0f);
            }
            if (hit.tag == "Enemy")
            {
                hit.gameObject.SendMessage("ApplyHeal", 100.0f);
            }
        }
    }

    // Takes Damage
    public void ApplyDamge(float amount)
    {
        health -= amount;
        nav.velocity = transform.forward * -1 * 3.0f;
    }

    // Gains Health
    public void ApplyHeal(float amount)
    {
        health += amount;
        if (health > 100.0f)
        {
            health = 100.0f;
        }
    }
}
