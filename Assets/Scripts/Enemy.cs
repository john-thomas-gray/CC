using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    static System.Random random = new System.Random();
    public string color;
    public bool alive;
    public bool dead;
    public bool special;
    public bool super;
    public bool isChecked;
    public bool colorCounted;

    private Transform cellTransform;
    private Transform fleetTransform;
    private GameObject fleet;
    private GridManager gridManagerInstance;

    ColorManager colorManager;
    private Dictionary<string, int> colorCounts;

    public Sprite specialSkin;

    // RED ABILITIES
    private float timeSinceLastShot = 0;
    private double shotCoolDown;
    private float[] shotCoolDownRange = {6, 14};
    private bool onCoolDown = false;
    public GameObject missilePrefab;

    void Awake()
    {
        alive = true;
        dead = false;

        // Color
        color = "";
        colorManager = ColorManager.Instance;
        colorCounts = colorManager.colorCounts;
        colorManager.SetColor(this.gameObject);

        special = false;
        super = false;
        isChecked = false;

        cellTransform = transform.parent;
        fleetTransform = cellTransform.parent;
        fleet = fleetTransform.gameObject;
        gridManagerInstance = fleet.GetComponent<GridManager>();

        CheckNeighbors();

    }

    void Update()
    {
        Abilities();
    }

    void Abilities()
    {
        // Check color
        if(color == "Red")
        {
            // Base ability
            if (!special)
            {
                fireMissile();
            }
            // Special ability
            else if (special)
            {

            }
            else if (super)
            {

            }
        }
        // for each
        // Base abilities

        // Special abilities

        // Super abilities
    }


    public void CheckNeighbors()
    {
        if(isChecked == false)
        {
            // Get instance of current grid
            List<GameObject> grid = gridManagerInstance.grid;
            // Get instance of cell holding this enemy
            Transform cellTransform = transform.parent;
            // Get the cell number
            int cellNumber = cellTransform.gameObject.GetComponent<Cell>().number;
            // Debug.Log("Checking Cell " + cellNumber + "'s neighbors");

            // Check the neighboring cells
            List<GameObject> neighbors = new List<GameObject>();

            // Check up if not bot row
            if (cellNumber > 5)
            {
                GameObject up = grid[cellNumber - 6];
                neighbors.Add(up);
            }
            // Check left if not left column
            if (cellNumber % 6 != 0)
            {
                GameObject left = grid[cellNumber - 1];
                neighbors.Add(left);
            }
            // Check right if not right column
            if ((cellNumber + 1) % 6 != 0)
            {
                GameObject right = grid[cellNumber + 1];
                neighbors.Add(right);
            }
            // Check down if not bot row
            if (cellNumber < 66)
            {
                GameObject down = grid[cellNumber + 6];
                neighbors.Add(down);
            }

            // List enemies with matching colors
            List<GameObject> matches = new List<GameObject>();
            for (int j = 0; j < neighbors.Count; j++)
            {
                GameObject neighbor = neighbors[j];
                if (neighbor.GetComponent<Cell>().enemy)
                {
                    GameObject neighborEnemy = neighbor.GetComponent<Cell>().enemy;
                    string neighborColor = neighborEnemy.GetComponent<Enemy>().color;
                    // If neighbor is a like color add it to list of matches
                    if (neighborColor == color)
                    {
                        matches.Add(neighborEnemy);
                    }
                }
            }
            // Set Special
            if (matches.Count >= 2)
            {
                // Turn on currentEnemy's special
                if (!special)
                {
                    gameObject.GetComponent<Enemy>().special = true;
                    gameObject.GetComponent<SpriteRenderer>().sprite = specialSkin;
                }
                // Turn on the neighbor's specials
                for (int k = 0; k < matches.Count; k++)
                {
                    GameObject match = matches[k];
                    match.GetComponent<Enemy>().special = true;
                    match.GetComponent<SpriteRenderer>().sprite = specialSkin;
                }
            }

            // Kill protocol -- If current enemy !alive set matches to !alive
            if(!alive)
            {
                for (int k = 0; k < matches.Count; k++)
                {
                    GameObject match = matches[k];
                    match.GetComponent<Enemy>().alive = false;
                    isChecked = true;
                    match.GetComponent<Enemy>().CheckNeighbors();
                }
            }
        }
        // Call death to kill current enemy
        death();
    }

    // ||| ABILITIES |||

    // RED

    // BASIC
    void fireMissile()
    {
        if(!onCoolDown)
        {
            shotCoolDown = random.NextDouble() * (shotCoolDownRange[1] - shotCoolDownRange[0]) + shotCoolDownRange[0];
            onCoolDown = true;
        }

        timeSinceLastShot += Time.deltaTime;

        if(timeSinceLastShot >= shotCoolDown)
        {
                // Debug.Log("shotColor: " + shotColor);
                // Debug.Log("color: " + color);
            colorCounts = colorManager.colorCounts;

            if (colorManager.colorSet)
            {
                // Spawn missile in front of enemy
                Instantiate(missilePrefab, new Vector3(transform.position.x, transform.position.y - .75f, transform.position.z), transform.rotation);
                // Reset cooldown
                timeSinceLastShot = 0.0f;
                onCoolDown = false;
            }
        }
    }


    public void death()
    {
        if(alive == false && dead == false)
        {

            // Set dead to true to prevent multiple runs in same frame
            dead = true;
            // Subtract color from colorCounts dictionary instance
            colorCounts = colorManager.colorCounts;
            colorCounts[color] -= 1;

            // Check if current color count is 0
            if (colorCounts[color] == 0)
            {
                gridManagerInstance.FleetWipeCheck();
                if (!gridManagerInstance.wipedOut)
                {
                    colorManager.magicLaser = true;
                    Debug.Log("MAGIC LASER");
                }
            }

            Destroy(this.gameObject);

        }
    }

}
