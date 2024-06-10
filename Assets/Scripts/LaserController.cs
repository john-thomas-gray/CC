using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    // Laser color
    public string color;
    public string shotColor;
    public GameObject player;
    private bool magicLaser = false;
    // Laser movement
    private float destroyPlain = 5.3f;
    private float laserSpeed = 20f;
    private bool deflected = false;
    // ColorManager
    ColorManager colorManager;
    // Max time on screen
    private float destroyTimer = 0;

    // Raycast
    private float rayLength;
    private LayerMask layersToHit = (1 << 8 | 1 << 10); // Combine enemy and shield layers
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        shotColor = player.GetComponent<PlayerController>().shotColor;

        colorManager = ColorManager.Instance;

        colorManager.SetColor(this.gameObject, shotColor);

        if(colorManager.magicLaser)
        {
            magicLaser = true;
        }
        colorManager.magicLaser = false;

        rayLength = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        MoveLaser();
        if(magicLaser)
        {
            colorManager.Multicolor(this.gameObject);
        }
        // Destroy laser if onscreen too long
        destroyTimer += Time.deltaTime;
        if(destroyTimer > 1.5)
        {
            Destroy(this.gameObject);
        }

    }

    void FixedUpdate()
    {
        Vector3 rightEdgePosition = transform.position + new Vector3(rayLength/2, .15f, 0);
        RaycastHit2D[] hits = Physics2D.RaycastAll(rightEdgePosition, Vector2.left, rayLength, layersToHit);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                GameObject hitGameObject = hit.collider.gameObject;
                if(hit.collider.gameObject.layer == 8)
                {
                    Enemy enemyScript = hitGameObject.GetComponent<Enemy>();
                    string enemyColor = enemyScript.color;
                    if(enemyScript != null)
                    {
                        enemyScript.hitByLaser(color, magicLaser);
                        if(color == enemyColor && !magicLaser)
                        {
                            Destroy(this.gameObject);
                        }
                    }
                }
                else if(hit.collider.gameObject.layer == 10 && !magicLaser)
                {
                    Shield shieldScript = hitGameObject.GetComponent<Shield>();
                    float reflectRotation = 180f;

                    if(color != shieldScript.shieldEnemyColor)
                    {
                        // BASIC ABSORB
                        if (!shieldScript.special)
                        {
                            shieldScript.absorb();
                            Destroy(this.gameObject);
                        }
                        // SPECIAL DEFLECT
                        // deflected bool keeps raycast from firing multiple times
                        if (deflected == false && shieldScript.special)
                        {
                            deflected = true;
                            colorManager.turnWhite(this.gameObject);
                            transform.Rotate(new Vector3(0f, 0f, reflectRotation));
                            laserSpeed = laserSpeed * 0.5f;
                        }

                    }
                }
            }
        }

        Debug.DrawRay(rightEdgePosition, transform.TransformDirection(Vector2.left) * rayLength, Color.red);
    }

    void MoveLaser()
    {
        transform.Translate(Vector3.up * laserSpeed * Time.deltaTime);

        // Destroy offscreen laser
        if(Mathf.Abs(transform.position.y) > destroyPlain)
        {
            Destroy(this.gameObject);
        }
    }
}
