using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private GameObject player;
    private GameObject boss;
    // Start is called before the first frame update
    void Start()
    {
        int stat1 = PlayerPrefs.GetInt("stat1");
        int stat2 = PlayerPrefs.GetInt("stat2");
        int stat3 = PlayerPrefs.GetInt("stat3");

        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement>().maxHealth = 10 * stat1;
        player.GetComponent<PlayerMovement>().damage = 10 * stat2;
        player.GetComponent<PlayerMovement>().maxEnergy = 10 * stat3;

        boss = GameObject.FindGameObjectWithTag("Boss");
    }
}
