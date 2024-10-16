using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Crop : MonoBehaviour
{
    public bool isPlanted = false;
    public GameObject[] cropList;

    public Image moodIndicator;
    
    private float water = 50.0f;
    private float growth = 0.0f;
    private float mood = 50.0f;
    private string stat;
    private bool isFertilized = false;
    private float fertilizeTime;
    private float elapsedTime;
    private Transform player;
    private GameObject currentCrop;
    private GameObject GameManager;

    private GameObject canvas;
    private Image waterMask;
    private Image moodMask;
    private Image growthMask;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        GameManager = GameObject.FindGameObjectWithTag("GameController");
        elapsedTime = 0.0f;

        // UI Startup
        canvas = transform.Find("Meters").gameObject;
        waterMask = transform.Find("Meters").Find("Water").Find("Image").gameObject.GetComponent<Image>();
        moodMask = transform.Find("Meters").Find("Mood").Find("Image").gameObject.GetComponent<Image>();
        growthMask = transform.Find("Meters").Find("Growth").Find("Image").gameObject.GetComponent<Image>();
        moodIndicator.enabled = false;
        canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Growing Logic
        // Water + Mood down based on player actions
        if (water > 0)
        {
            water -= Time.deltaTime/2;
        }
        if (mood > 0)
        {
            if (!isFertilized)
            {
                float playerDist = Vector3.Distance(player.position, transform.position)/10;
                if (playerDist < 10.0f)
                {
                    mood -= (water/100 + 1) * Time.deltaTime;
                }
                else{
                    mood -= (water/100 + 1) * Time.deltaTime/2;
                }
            }
        }
        if (growth < 100)
        {
            if (isFertilized)
            {
                growth += (water + mood)/100 * Time.deltaTime * 1.5f;
            }
            else
            {
                growth += (water + mood)/100 * Time.deltaTime;
            }
        }

        // Displays the UI Popup if the player is nearby
        float dist = Vector3.Distance(player.position, transform.position);
        if (dist < 5.0f & isPlanted)
        {
            canvas.SetActive(true);
        }
        else
        {
            canvas.SetActive(false);
        }

        // Updates Object States
        UpdateUI();
        UpdateOvergrown();
        UpdateOverMood();
        fertilizeTime -= Time.deltaTime;

        if (fertilizeTime < 0.0f & isFertilized)
        {
            isFertilized = false;
        }
    }

    // Updates the UI
    // Rotates the display towards the player
    void UpdateUI()
    {
            Quaternion rotation = Quaternion.LookRotation(new Vector3(player.transform.position.x, 0, player.transform.position.z) - transform.position);
            canvas.transform.rotation = Quaternion.Slerp(canvas.transform.rotation, rotation, Time.deltaTime * 10); 
            waterMask.fillAmount = water / 100;
            moodMask.fillAmount = mood / 100;
            growthMask.fillAmount = growth / 100;
            rotation = Quaternion.LookRotation(new Vector3(player.transform.position.x, 0, player.transform.position.z) - transform.position);
            moodIndicator.transform.rotation = Quaternion.Slerp(moodIndicator.transform.rotation, rotation, Time.deltaTime * 10); 
    }

    // Updates the state if the plant is ready to be harvested
    void UpdateOvergrown()
    {
        if (growth >= 100 & isPlanted)
        {
            elapsedTime += Time.deltaTime;
            moodIndicator.enabled = true;
            if (elapsedTime > 30.0f)
            {
                isPlanted = false;
                GameObject plantNPC = Instantiate(currentCrop, new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), transform.rotation);
            }
        }
    }

    // Updates the state if the plant is too low mood
    void UpdateOverMood()
    {
        if (mood <= 0 & isPlanted)
        {
            elapsedTime += Time.deltaTime;
            moodIndicator.enabled = true;
            if (elapsedTime > 30.0f)
            {
                isPlanted = false;
                GameObject plantNPC = Instantiate(currentCrop, new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), transform.rotation);
            }
        }
    }

    // Increments water stat + mood
    public void AddWater(float amount)
    {
        if (water < 90.0f)
        {
            water += amount;
            mood += amount/2;
            if (water > 100.0f)
            {
                water = 100.0f;
            }
        }
        else
        {
            mood -= amount/2;
        }
    }

    // Handles if the player shoots fire at the plants
    public void AddFire(float amount)
    {
        mood -= amount;
        water -= amount * 1.5f;
    }

    // Applies fertiliser buff
    public void AddEarth()
    {
        isFertilized = true;
        fertilizeTime = 30.0f;
    }

    // Handles the player interacting with the crop
    // Picking/Planting
    public void Interact(int seed)
    {
        // Collect grown plant
        if (growth >= 100.0f)
        {
            isPlanted = false;
            moodIndicator.enabled = false;
            player.SendMessage("GainStat", stat);
            GameManager.SendMessage("collectCrop");
        }
        // Plants in empty plot
        else if (!isPlanted)
        {
            GameObject cropObject = cropList[seed-1];
            switch (seed)
            {
                case 1:
                    stat = "strength";
                    break;
                case 2:
                    stat = "magic";
                    break;
                case 3:
                    stat = "vitality";
                    break;
            }
            Plant(cropObject);
        }
    }

    // Resets state for new plant
    public void Plant(GameObject seed)
    {
        currentCrop = seed;
        isPlanted = true;
        growth = 0.0f;
        mood = 50.0f;
        water = 25.0f;
        elapsedTime = 0.0f;
    }

    // Handles an enemy taking out a crop
    public void Uproot()
    {
        isPlanted = false;
        GameObject plantNPC = Instantiate(currentCrop, new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), transform.rotation);
    }
}

