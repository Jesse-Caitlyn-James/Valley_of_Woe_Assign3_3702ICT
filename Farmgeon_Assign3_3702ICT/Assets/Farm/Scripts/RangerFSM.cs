using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangerFSM : MonoBehaviour
{
      public enum FSMModes{
        None,
        Guard,
        Attack,
        Run,
        Heal,
        Dead,
        Boss,
    }
    public FSMModes currentState;
    public float health = 100.0f;
    public float damage = 50.0f;
    public float range = 10.0f;
    public GameObject projectile;
    public GameObject drop;
    
    private Transform playerTransform;
    private NavMeshAgent nav;
    private GameObject[] healers;
    private GameObject currentHealer;
    private GameObject[] guardPoints;
    private GameObject currentGuardPoint;
    private GameObject GameManager;
    private float pathTime;
    private float cooldown;
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        GameManager = GameObject.FindGameObjectWithTag("GameController");
        nav = GetComponent<NavMeshAgent>();
        currentState = FSMModes.Guard;
        FindHealer();
        guardPoints = GameObject.FindGameObjectsWithTag("EscapePoint");
        currentGuardPoint = guardPoints[Random.Range(0, guardPoints.Length - 1)];
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealer == null)
        {
            FindHealer();
        }

        switch (currentState)
        {
            case FSMModes.None:
                break;
            case FSMModes.Guard:
                Guard();
                break;
            case FSMModes.Attack:
                Attack();
                break;
            case FSMModes.Run:
                Run();
                break;
            case FSMModes.Heal:
                Heal();
                break;
            case FSMModes.Dead:
                Dead();
                break;
            case FSMModes.Boss:
                Boss();
                break;
        }

        newGuardPos();

        float dist = Vector3.Distance(transform.position, currentHealer.transform.position);
        if (dist < 10.0f && currentHealer.tag != "Player")
        {
            ApplyHeal(10.0f * Time.deltaTime);
        }

        pathTime += Time.deltaTime;
        cooldown += Time.deltaTime;

        if (health < 30.0f)
        {
            currentState = FSMModes.Run;
        }
        if (health <= 0.0f)
        {
            currentState = FSMModes.Dead;
        }
    }

    void FindHealer()
    {
        healers = GameObject.FindGameObjectsWithTag("Healer");
        if (healers.Length > 0)
        {
            currentHealer = healers[0];
        }
        else
        {
            currentHealer = playerTransform.gameObject;
        }

        foreach (GameObject healer in healers)
        {
            float dist = Vector3.Distance(transform.position, healer.transform.position);
            float currentDist = Vector3.Distance(transform.position, currentHealer.transform.position);
            if (dist < currentDist)
            {
                currentHealer = healer;
            }
        }
    }

    void Guard()
    {
        if (!nav.hasPath | nav.remainingDistance < 2.0f)
        {
            Vector3 guardPos = currentGuardPoint.transform.position;
            Vector3 newPos = new Vector3(guardPos.x + Random.Range(-10.0f, 10.0f), guardPos.y, guardPos.z + Random.Range(-10.0f, 10.0f));
            nav.SetDestination(newPos);
            nav.stoppingDistance = 1.0f;
        }

        RaycastHit NPCSee;
        if (Physics.Linecast(transform.position, playerTransform.position, out NPCSee))
        {
            float dist = Vector3.Distance(transform.position, playerTransform.position);
            if (NPCSee.collider.tag == "Player" & dist < 20.0f)
            {
                currentState = FSMModes.Attack;
            }
        }
    }

    void Attack()
    {
        if (cooldown > 3.0f)
        {
            cooldown = 0.0f;
            GameObject arrow = Instantiate(projectile, transform.position, transform.rotation);
        }

        if (pathTime > 1.0f)
        {
            nav.SetDestination(playerTransform.position);
            nav.stoppingDistance = 15.0f;
            pathTime = 0.0f;
        }

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist > range)
        {
            nav.Move(transform.forward * -1 * 10.0f * Time.deltaTime);
        }
        if (dist > 20.0f)
        {
            currentState = FSMModes.Guard;
        }
        RaycastHit NPCSee;
        if (Physics.Linecast(transform.position, playerTransform.position, out NPCSee))
        {
            if (NPCSee.collider.tag != "Player")
            {
                currentState = FSMModes.Guard;
            }
        }
    }

    void Run()
    {
        FindHealer();
        nav.SetDestination(currentHealer.transform.position);

        float dist = Vector3.Distance(transform.position, currentHealer.transform.position);
        if (dist < 10.0f & currentHealer.tag != "Player")
        {
            currentState = FSMModes.Heal;
        }
        if (currentHealer.tag == "Player")
        {
            currentState = FSMModes.Attack;
        }
    }

    void Heal()
    {
        if (currentHealer == null)
        {
            FindHealer();
        }
        Vector3 healerPos = currentHealer.transform.position;
        float dist = Vector3.Distance(transform.position, healerPos);
        if (!nav.hasPath | nav.remainingDistance < 2.0f | dist > 20.0f)
        {
            Vector3 newPos = new Vector3(healerPos.x + Random.Range(-10.0f, 10.0f), healerPos.y, healerPos.z + Random.Range(-10.0f, 10.0f));
            nav.SetDestination(newPos);
        }

        if (pathTime > 1.0f)
        {
            pathTime = 0.0f;
            FindHealer();
            nav.SetDestination(currentHealer.transform.position);

            if (currentHealer.tag == "Player")
            {
                currentState = FSMModes.Attack;
            }
        }

        if (health >= 100)
        {
            currentState = FSMModes.Guard;
        }
    }

    void Dead()
    {
        if (!isDead)
        {
            isDead = true;
            nav.isStopped = true;
            Destroy(gameObject, 2.0f);
            GameObject loot = Instantiate(drop, transform);
            GameManager.SendMessage("enemyKilled");
        }
    }

    void Boss()
    {
        if (cooldown > 3.0f)
        {
            cooldown = 0.0f;
            GameObject arrow = Instantiate(projectile, transform.position, transform.rotation);
        }

        if (pathTime > 1.0f)
        {
            nav.SetDestination(playerTransform.position);
            nav.stoppingDistance = 15.0f;
            pathTime = 0.0f;
        }

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist > range)
        {
            nav.Move(transform.forward * -1 * 10.0f * Time.deltaTime);
        }
    }

    void newGuardPos()
    {
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist < 3.0f)
        {
            currentGuardPoint = guardPoints[Random.Range(0, guardPoints.Length - 1)];
        }
    }

    public void ApplyDamge(float amount)
    {
        health -= amount;
        nav.velocity = transform.forward * -1 * 3.0f;
    }

    public void ApplyHeal(float amount)
    {
        health += amount;
        if (health > 100.0f)
        {
            health = 100.0f;
        }
    }
}
