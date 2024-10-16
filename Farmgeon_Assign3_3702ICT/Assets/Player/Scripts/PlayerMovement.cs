using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public enum Inventory
    {
        Water = 1,
        Earth = 2,
        Fire = 3,
    }
    public enum Seeds
    {
        Strength = 1,
        Magic = 2,
        Vitality = 3,
    }
    public Image hand;
    public int[] playerStats;
    public Slider healthBar;
    public Slider energyBar;
    public float walkSpeed = 5f; // Walking backwards and crouching will be half walkSpeed
    public float sprintSpeed = 10f;
    public float sprintFOV = 75f;
    public float normalFOV = 60f;
    public float jumpHeight = 4f;
    public float gravity = -10f;
    public float maxEnergy = 100f;
    public float damage;
    public float energySprintDrain = 20f;
    public float energyRecoveryRate = 30f;
    public float energyRecoveryDelay = 1f;
    public float maxHealth = 100f;
    public Image inventoryDisplay1;
    public Image seedDisplay;
    public Inventory currentItem = Inventory.Water;
    public Seeds currentSeed = Seeds.Strength;
    public GameObject waterBlast;
    public GameObject earthBlast;
    public GameObject fireBlast;
    public Sprite fireImg;
    public Sprite waterImg;
    public Sprite fertImg;
    public Sprite closed;
    public Sprite open;
    public Sprite strengthSeed;
    public Sprite vitalitySeed;
    public Sprite magicSeed;

    private CharacterController controller;
    private Transform cameraTarget;
    private Vector3 movement;
    private GameObject currentProjectile;
    private GameObject GameManager;
    private float moveSpeed;
    private float elapsedTime;
    private float cooldown;
    private float invChangeTime;
    private bool doubleJump = true;
    public float health;
    private float energy;
    private bool crouching = false;
    private float FOV;

    // 
    // This File is partially from Assignment 2
    // It has been repurposed to hold stats, inventories and magic/interaction
    // 

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        controller = gameObject.GetComponent<CharacterController>();
        cameraTarget = GameObject.FindGameObjectWithTag("CameraTarget").transform;
        GameManager = GameObject.FindGameObjectWithTag("GameController");

        moveSpeed = walkSpeed;
        energy = maxEnergy;
        energyBar.maxValue = energy;
        health = maxHealth;
        healthBar.value = health;
	    healthBar.maxValue = health;
        FOV = normalFOV;
        cooldown = 0.0f;
        playerStats = new int[3] {1,1,1};
    }

    void Update()
    {
        // State Updates
        UpdateMovement();
        UpdateMouseLook();
        UpdateSprint();
        UpdateCrouch();
        UpdateJump();
        UpdateEnergy();
        UpdateFOV();
        UpdateHotbar();
        UpdateSeeds();

        // Switch case for projectiles (left mouse button)
        if (Input.GetMouseButtonDown(0))
        {
            switch (currentItem)
            {
                case Inventory.Water:
                    currentProjectile = waterBlast;
                    break;
                case Inventory.Earth:
                    currentProjectile = earthBlast;
                    break;
                case Inventory.Fire:
                    currentProjectile = fireBlast;
                    break;
            }
            UseAttack();
        }

        // Interact UI Changes
        if (Input.GetMouseButtonDown(1))
        {
            UseAction();
            hand.sprite = closed;
        }else{
            hand.sprite = open;
        }

        elapsedTime += Time.deltaTime;
        cooldown += Time.deltaTime;

        // When dead sends player back to the menu
        if(health <= 0)
        {
            SceneManager.LoadScene("Menu");
        }
    }

    void UpdateMovement()
    {
        // Gets directional movement for the player inputs
        Vector3 hMovement = Quaternion.Euler(0, cameraTarget.transform.eulerAngles.y, 0) * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Checks whether the player is moving in a valid manner for sprinting and drains energy
        if ((Input.GetAxis("Horizontal") != 0.0f | Input.GetAxis("Vertical") > 0.0f) & moveSpeed == sprintSpeed & energy > 0)
        {
            energy -= energySprintDrain * Time.deltaTime;
            elapsedTime = 0.0f;
        }

        // Checks if the player is moving backwards and applies speed decrease
        if (Input.GetAxis("Vertical") < 0)
        {
            moveSpeed = walkSpeed / 2;
        }

        // Moves the player
        controller.Move(Time.deltaTime * moveSpeed * hMovement);
    }

    void UpdateMouseLook()
    {
        // Gets the movement of the mouse
        float rotateHorizontal = Input.GetAxis ("Mouse X");
        float rotateVertical = 0.0f;
        // Checks if the player is looking too high or low
        if (transform.rotation.x > -85 & transform.rotation.x < 85)
        {
		    rotateVertical = Input.GetAxis ("Mouse Y");
        }

        // Rotates the cameraTarget using eularangles cause Rotate and rotation both did weird things
        Vector3 fullRotation = new Vector3(-rotateVertical, rotateHorizontal, 0);
        transform.localEulerAngles += fullRotation;
    }

    void UpdateSprint()
    {
        // Checks whether the player can and wants to sprint
        if(Input.GetKey("left shift") & !crouching & energy > 0)
        {
            moveSpeed = sprintSpeed;
            FOV = sprintFOV;
        }
        else
        {
            moveSpeed = walkSpeed;
            FOV = normalFOV;
            // if crouching applies speed decrease
            if (crouching)
            {
                moveSpeed /= 2;
            }
        }
    }

    void UpdateCrouch()
    {
        // Toggles the player crouching and applies the height difference to the player
        if (!crouching & Input.GetKeyDown("left ctrl"))
        {
            crouching = true;
            controller.height /= 2;
        }
        else if (crouching & Input.GetKeyDown("left ctrl"))
        {
            crouching = false;
            controller.height *= 2;
        }
    }

    void UpdateJump()
    {
        // Applies gravity effects first as doing it later ends in sadness
        movement.y += gravity * Time.deltaTime;
        controller.Move(movement * Time.deltaTime);

        // Checks if the is on the ground to update whether player can double jump
        if (controller.isGrounded & !doubleJump)
        {
            doubleJump = true;
        }

        // Stops the player from getting effected by gravity if they are on the ground
        if (controller.isGrounded & movement.y < 0)
        {
            movement.y = 0f;
        }


        if (Input.GetButtonDown("Jump"))
        {
            // Checks if the player can perform the initial jump and drains energy
            if (controller.isGrounded & energy > energySprintDrain)
            {
                movement.y += jumpHeight;
                energy -= energySprintDrain;
                elapsedTime = 0.0f;
            } 
            // Checks if the player can perform the second jump
            else if (!controller.isGrounded & doubleJump)
            {
                doubleJump = false;
                movement.y += jumpHeight;
            }
        }
    }

    void UpdateEnergy()
    {
        // Checks if the player hasn't performed any energy actions recently
        // Then regenerates energy
        if (energy < maxEnergy & elapsedTime >= energyRecoveryDelay)
        {
            energy += energyRecoveryRate * Time.deltaTime;
        }
        energyBar.value = energy;
    }

    void UpdateFOV()
    {
        // Changes FOV based on player speed/actions
        Camera.main.fieldOfView += (FOV - Camera.main.fieldOfView) * Time.deltaTime;
    }

    void UpdateHotbar()
    {
        if (Input.mouseScrollDelta.y > 0.0f & invChangeTime > 0.5f)
        {
            invChangeTime = 0;
            currentItem++;
            if ((int) currentItem > 3){
                currentItem = Inventory.Water;
            }
        }
        if (Input.mouseScrollDelta.y < 0.0f)
        {
            invChangeTime = 0;
            currentItem--;
            if ((int) currentItem < 1){
                currentItem = Inventory.Fire;
            }
        }
        //magic image change
        if(currentItem == Inventory.Water){
            inventoryDisplay1.sprite = waterImg;
        }
        if(currentItem == Inventory.Fire){
            inventoryDisplay1.sprite = fireImg;
        }
        if(currentItem == Inventory.Earth){
            inventoryDisplay1.sprite = fertImg;
        }
        
        invChangeTime += Time.deltaTime;
    }

    // Creates the current projectile and fires it
    void UseAttack()
    {
        if (cooldown > 1.0f){
            Instantiate(currentProjectile, transform.position, transform.rotation);
            cooldown = 0.0f;
        }
    }

    // Checks if the player is interacting with farmland
    void UseAction()
    {
        RaycastHit playerSee;
        if (Physics.Linecast(transform.position, transform.position + transform.forward * 5, out playerSee))
        {
            if (playerSee.collider.tag == "Farmland")
            {
                playerSee.collider.SendMessage("Interact", (int)currentSeed);
            }
        }
    }

    // Changes Inventory state and UI for plantable seeds
    void UpdateSeeds()
    {
        if (Input.GetKeyDown("q"))
        {
            currentSeed++;
            if ((int) currentSeed > 3){
                currentSeed = Seeds.Strength;
            }
        }
        if (Input.GetKeyDown("e"))
        {
            currentSeed--;
            if ((int) currentSeed < 1){
                currentSeed = Seeds.Vitality;
            }
        }
        //seed image change
        if(currentSeed == Seeds.Vitality){
            seedDisplay.sprite = vitalitySeed;
        }
        if(currentSeed == Seeds.Strength){
            seedDisplay.sprite = strengthSeed;
        }
        if(currentSeed == Seeds.Magic){
            seedDisplay.sprite = magicSeed;
        }

    }

    // Apply Damage
    public void ApplyDamage()
    {
        health -= 25;
        healthBar.value = health;
    }

    // Gain Player Stats
    public void GainStat(string stat)
    {
        switch (stat)
        {
            case "strength":
                playerStats[0]++;
                break;
            case "magic":
                playerStats[1]++;
                break;
            case "vitality":
                playerStats[2]++;
                break;
        }
    }
}