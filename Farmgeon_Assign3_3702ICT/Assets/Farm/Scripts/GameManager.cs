using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float bulletLife = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        exit = GameObject.FindGameObjectWithTag("Exit");
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStart)
        {
            playTime += Time.deltaTime;
            waveTime += Time.deltaTime;
        }

        difficultyUpdate();
        waveUpdate();
    }

    void difficultyUpdate()
    {
        difficulty = 0.0f;
        difficulty += fledCrops * 2;
        difficulty += grownCrops;
        difficulty += playTime;
        difficulty /= 100;
    }

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

    public void PlayerExit()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerMovement>().playerStats;
        PlayerPrefs.SetInt("Stat1", playerStats[0]);
        PlayerPrefs.SetInt("Stat2", playerStats[1]);
        PlayerPrefs.SetInt("Stat3", playerStats[2]);

        PlayerPrefs.SetInt("fledCrops", fledCrops);
        PlayerPrefs.SetInt("grownCrops", grownCrops);
        PlayerPrefs.SetInt("enemiesKilled", enemiesKilled);
        PlayerPrefs.SetFloat("playTime", playTime);
    }

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
