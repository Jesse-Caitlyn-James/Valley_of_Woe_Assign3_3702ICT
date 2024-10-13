using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossFSM : MonoBehaviour
{
    public enum FSMModes{
        None,
        Phase1,
        Phase2,
        Phase3,
        Phase4,
        Dead,
    }
    public FSMModes currentState;
    public float health = 100.0f;
    public GameObject projectile;
    public GameObject healer;
    public GameObject ranger;
    public GameObject melee;
    
    private Transform playerTransform;
    private GameObject GameManager;
    private GameObject[] weakSpots;
    private float weakSpotCooldown;
    private float cooldown;
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        GameManager = GameObject.FindGameObjectWithTag("GameController");
        currentState = FSMModes.None;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case FSMModes.None:
                break;
            case FSMModes.Phase1:
                Phase1();
                break;
            case FSMModes.Phase2:
                Phase2();
                break;
            case FSMModes.Phase3:
                Phase3();
                break;
            case FSMModes.Phase4:
                Phase4();
                break;
            case FSMModes.Dead:
                Dead();
                break;
        }
        cooldown += Time.deltaTime;
    }

// First Phase - Low Missiles
// Shoots several missiles every 10 seconds
    void Phase1()
    {
        weakSpots[0].SetActive(true);
        if (cooldown < 10.0f)
        {
            cooldown = 0.0f;
            Invoke("Instantiate(projectile, transform)", 0.0f);
            Invoke("Instantiate(projectile, transform)", 1.0f);
            Invoke("Instantiate(projectile, transform)", 2.0f);
        }

        if(health <= 0.0f)
        {
            currentState = FSMModes.Phase2;
            health = 100;
        }
    }

// Second Phase - High Missiles
// Shoots several missiles every 10 seconds
    void Phase2()
    {
        weakSpots[0].SetActive(true);

        if (cooldown < 7.5f)
        {
            cooldown = 0.0f;
            Invoke("Instantiate(projectile, transform)", 0.0f);
            Invoke("Instantiate(projectile, transform)", 1.0f);
            Invoke("Instantiate(projectile, transform)", 2.0f);
            Invoke("Instantiate(projectile, transform)", 3.0f);
            Invoke("Instantiate(projectile, transform)", 4.0f);
        }

        if(health <= 0.0f)
        {
            currentState = FSMModes.Phase3;
            health = 100;
            weakSpots[0].SetActive(false);
        }
    }

// Third Phase - High Missiles + Enemy Spawns
// Shoots several missiles every 10 seconds and spawns enemies
    void Phase3()
    {
        weakSpots[1].SetActive(true);

        if (cooldown < 15.0f)
        {
            cooldown = 0.0f;
            Invoke("Instantiate(projectile, transform)", 0.0f);
            Invoke("Instantiate(projectile, transform)", 1.0f);
            Invoke("Instantiate(projectile, transform)", 2.0f);
            Invoke("Instantiate(projectile, transform)", 3.0f);
            Invoke("Instantiate(projectile, transform)", 4.0f);

            GameObject[] spawnList = GameObject.FindGameObjectsWithTag("SpawnPoint");

            // Healer
            GameObject randomSpawn = spawnList[Random.Range(0, spawnList.Length - 1)];
            Vector3 spawnPoint = new Vector3(randomSpawn.transform.position.x, randomSpawn.transform.position.y, randomSpawn.transform.position.z + Random.Range(-10, 10));
            Instantiate(healer, spawnPoint, randomSpawn.transform.rotation);

            // Ranger
            for (int i = 0; i < 2; i++)
            {
                randomSpawn = spawnList[Random.Range(0, spawnList.Length - 1)];
                spawnPoint = new Vector3(randomSpawn.transform.position.x, randomSpawn.transform.position.y, randomSpawn.transform.position.z + Random.Range(-10, 10));
                Instantiate(ranger, spawnPoint, randomSpawn.transform.rotation);
            }

            // Melee
            for (int i = 0; i < 3; i++)
            {
                randomSpawn = spawnList[Random.Range(0, spawnList.Length - 1)];
                spawnPoint = new Vector3(randomSpawn.transform.position.x, randomSpawn.transform.position.y, randomSpawn.transform.position.z + Random.Range(-10, 10));
                Instantiate(melee, spawnPoint, randomSpawn.transform.rotation);
            }
        }

        if(health <= 0.0f)
        {
            currentState = FSMModes.Phase4;
            health = 100;
            weakSpots[1].SetActive(false);
        }
    }

// Fourth Phase - High Missiles + Enemy Spawn
// Shoots several missiles every 10 seconds and spawns enemies
    void Phase4()
    {
        weakSpots[2].SetActive(true);

        if (cooldown < 10.0f)
        {
            cooldown = 0.0f;
            Invoke("Instantiate(projectile, transform)", 0.0f);
            Invoke("Instantiate(projectile, transform)", 1.0f);
            Invoke("Instantiate(projectile, transform)", 2.0f);
            Invoke("Instantiate(projectile, transform)", 3.0f);
            Invoke("Instantiate(projectile, transform)", 4.0f);

            GameObject[] spawnList = GameObject.FindGameObjectsWithTag("SpawnPoint");

            // Healer
            GameObject randomSpawn = spawnList[Random.Range(0, spawnList.Length - 1)];
            Vector3 spawnPoint = new Vector3(randomSpawn.transform.position.x, randomSpawn.transform.position.y, randomSpawn.transform.position.z + Random.Range(-10, 10));
            Instantiate(healer, spawnPoint, randomSpawn.transform.rotation);

            // Ranger
            for (int i = 0; i < 2; i++)
            {
                randomSpawn = spawnList[Random.Range(0, spawnList.Length - 1)];
                spawnPoint = new Vector3(randomSpawn.transform.position.x, randomSpawn.transform.position.y, randomSpawn.transform.position.z + Random.Range(-10, 10));
                Instantiate(ranger, spawnPoint, randomSpawn.transform.rotation);
            }

            // Melee
            for (int i = 0; i < 3; i++)
            {
                randomSpawn = spawnList[Random.Range(0, spawnList.Length - 1)];
                spawnPoint = new Vector3(randomSpawn.transform.position.x, randomSpawn.transform.position.y, randomSpawn.transform.position.z + Random.Range(-10, 10));
                Instantiate(melee, spawnPoint, randomSpawn.transform.rotation);
            }
        }

        if(health <= 0.0f)
        {
            currentState = FSMModes.Dead;
            weakSpots[2].SetActive(false);
        }
    }

    void Dead()
    {
        SceneManager.LoadScene("Leader");
    }

    public void ApplyDamage(float amount)
    {
        health -= amount;
    }

// Begins the boss fight
    public void StartFight()
    {
        if (currentState == FSMModes.None)
        {
            currentState = FSMModes.Phase1;

            GameObject.FindGameObjectWithTag("StartPoint").SetActive(false);
        }
    }
}
