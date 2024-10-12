using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CropFSM : MonoBehaviour
{
    public enum FSMModes{
        None,
        RunPlayer,
        RunExit,
        Attack,
        Hide,
        Dead,
    }
    public FSMModes currentState;
    public float range = 20.0f;
    public float health = 100.0f;
    public float damage = 50.0f;
    public float mood;
    public GameObject drop;
    
    private Transform playerTransform;
    private GameObject GameManager;
    private NavMeshAgent nav;
    private GameObject[] escapePoints;
    private GameObject[] hidePoints;
    private Transform exit;
    private float pathTime;
    private float cooldown;
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        exit = GameObject.FindGameObjectWithTag("Exit").transform;
        GameManager = GameObject.FindGameObjectWithTag("GameManager");
        nav = GetComponent<NavMeshAgent>();
        escapePoints = GameObject.FindGameObjectsWithTag("EscapePoint");
        hidePoints = GameObject.FindGameObjectsWithTag("HidePoint");

        if (mood > 50.0f)
        {
            currentState = FSMModes.RunPlayer;
        }
        else
        {
            currentState = FSMModes.Attack;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case FSMModes.None:
                break;
            case FSMModes.RunPlayer:
                RunPlayer();
                break;
            case FSMModes.RunExit:
                RunExit();
                break;
            case FSMModes.Attack:
                Attack();
                break;
            case FSMModes.Hide:
                Hide();
                break;
            case FSMModes.Dead:
                Dead();
                break;
        }

        pathTime += Time.deltaTime;
        cooldown += Time.deltaTime;

        if (health < 30.0f)
        {
            currentState = FSMModes.Hide;
        }
        if (health <= 0.0f)
        {
            currentState = FSMModes.Dead;
        }

        float dist = Vector3.Distance(transform.position, exit.transform.position);
        if (dist < 3.0f)
        {
            GameManager.SendMessage("cropEscape");
            Destroy(gameObject);
        }
    }

    void RunPlayer()
    {
        if (pathTime > 1.0f)
        {
            pathTime = 0.0f;
            bool locationSet = false;
            foreach (GameObject location in escapePoints)
            {
                RaycastHit playerSee;
                if (Physics.Linecast(location.transform.position, playerTransform.position, out playerSee))
                {
                    if (playerSee.collider.tag != "Player")
                    {
                        nav.SetDestination(location.transform.position);
                        locationSet = true;
                        break;
                    }
                }
            }
            if (!locationSet)
            {
                nav.SetDestination(exit.position);
            }
        }

        float playerDist = Vector3.Distance(playerTransform.position, transform.position);
        RaycastHit hit;
        if (Physics.Linecast(transform.position, playerTransform.position, out hit))
        {
            if (hit.collider.tag != "Player")
            {
                currentState = FSMModes.RunExit;
            }
        }
        if (playerDist > range)
        {
            currentState = FSMModes.RunExit;
        }
    }

    void RunExit()
    {
        if (pathTime > 1.0f)
        {
            nav.SetDestination(exit.position);
            pathTime = 0.0f;
        }

        float playerDist = Vector3.Distance(playerTransform.position, transform.position);
        if (playerDist < range)
        {
            currentState = FSMModes.RunPlayer;
        }
    }

    void Attack()
    {
        if (pathTime > 1.0f)
        {
            nav.SetDestination(playerTransform.position);
            pathTime = 0.0f;
        }
    }

    void Hide()
    {
        if (pathTime > 1.0f)
        {
            pathTime = 0.0f;
            if (nav.remainingDistance > 2.0f)
            {
                bool locationSet = false;
                foreach (GameObject location in hidePoints)
                {
                    RaycastHit playerSee;
                    if (Physics.Linecast(location.transform.position, playerTransform.position, out playerSee))
                    {
                        if (playerSee.collider.tag != "Player")
                        {
                            nav.SetDestination(location.transform.position);
                            locationSet = true;
                            break;
                        }
                    }
                }
                if (!locationSet)
                {
                    nav.SetDestination(exit.position);
                }
            }
            else
            {
                RaycastHit playerSee;
                if (Physics.Linecast(transform.position, playerTransform.position, out playerSee))
                {
                    if (playerSee.collider.tag != "Player")
                    {
                        nav.SetDestination(exit.position);
                    }
                }
            }
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
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other.collider.tag == "Player" && currentState == FSMModes.Attack && cooldown > 1.0f)
        {
            cooldown = 0.0f;
            other.collider.SendMessage("ApplyDamage", damage);
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
