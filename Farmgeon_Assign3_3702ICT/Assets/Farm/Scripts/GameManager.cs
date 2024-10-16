using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject healer;
    public GameObject melee;
    public GameObject ranger;

    private GameObject exit;
    private bool gameStart = false;
    private int money = 20;
    private float difficulty;
    private float waveTime;
    private int grownCrops;
    private int fledCrops;
    private int enemiesKilled;
    private float playTime;
    private int[] playerStats;

    // Player Upgrades
    public float bulletLife = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        exit = GameObject.FindGameObjectWithTag("Exit");
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStart)
        {
            playTime += Time.deltaTime;
            waveTime += Time.deltaTime;
        }
        // Updates player stats based on crop yield
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerMovement>().playerStats;

        player.GetComponent<PlayerMovement>().maxHealth = 50 + 25 * playerStats[0];
        player.GetComponent<PlayerMovement>().damage = 25 * playerStats[1];
        player.GetComponent<PlayerMovement>().maxEnergy = 50 + 25 * playerStats[2];

        // State functions
        difficultyUpdate();
        waveUpdate();

        // Checks whether the player is leaving the level
        float dist = Vector3.Distance(player.transform.position, exit.transform.position);
        if (dist < 10.0f)
        {
            PlayerExit();
        }
    }

    // increases difficulty
    void difficultyUpdate()
    {
        difficulty = 0.0f;
        difficulty += fledCrops * 2;
        difficulty += grownCrops;
        difficulty += playTime;
        difficulty /= 100;
    }

    // Spawns waves every now and then
    // The waves scale with difficulty
    void waveUpdate()
    {
        if (waveTime > 60.0f)
        {
            waveTime = 0.0f;

            // Healer
            for (int i = 0; i < difficulty; i++)
            {
                Vector3 spawnPoint = new Vector3(exit.transform.position.x, exit.transform.position.y, exit.transform.position.z + Random.Range(-10, 10));
                Instantiate(healer, spawnPoint, exit.transform.rotation);
            }

            // Ranger
            for (int i = 0; i < 2 * difficulty; i++)
            {
                Vector3 spawnPoint = new Vector3(exit.transform.position.x, exit.transform.position.y, exit.transform.position.z + Random.Range(-10, 10));
                Instantiate(ranger, spawnPoint, exit.transform.rotation);
            }

            // Melee
            for (int i = 0; i < 3 * difficulty; i++)
            {
                Vector3 spawnPoint = new Vector3(exit.transform.position.x, exit.transform.position.y, exit.transform.position.z + Random.Range(-10, 10));
                Instantiate(melee, spawnPoint, exit.transform.rotation);
            }
        }
    }

    // Handles saving playerdata and loading the next level
    public void PlayerExit()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerMovement>().playerStats;
        PlayerPrefs.SetInt("Stat1", playerStats[0]);
        PlayerPrefs.SetInt("Stat2", playerStats[1]);
        PlayerPrefs.SetInt("Stat3", playerStats[2]);

        float score = playTime/10 + enemiesKilled + grownCrops - fledCrops;
        PlayerPrefs.SetFloat("score", score);

        SceneManager.LoadScene("Dungeon");
    }

    // Next functions are basic score tracking
    public void collectCrop()
    {
        if (!gameStart)
        {
            gameStart = true;
        }

        grownCrops++;
    }

    public void cropEscape()
    {
        if (!gameStart)
        {
            gameStart = true;
        }

        fledCrops++;
    }

    public void enemyKilled()
    {
        enemiesKilled++;
        money += 5;
    }
}
