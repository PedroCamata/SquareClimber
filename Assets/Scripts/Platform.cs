using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Platform : MonoBehaviour
{

    const float LADDER_MARGIN = 0.06f;
    const float LADDER_TIME_CHANGE = 0.15f;
    private float ladderTimer = 0;

    public bool ladderBehaviour = false;

    public bool checkPointBehaviour = false;
    public bool lastCheckPoint = false;

    public bool moveBehavior = false;
    private Vector3 startPosition;
    public Vector3 endPosition;
    public float movementTime = 1f;
    private bool isGoingToEndPosition = false;
    private float movementTimer;
    private Vector3 movement;

    public int hitsToBreak = -1; // -1 Don't break

    private Transform playerTransform;

    private float ladderTopCorner = 0;
    private BoxCollider2D boxCollider;

    //static Color PLATFORM_SOLID_COLOR = Color.white;
    static Color PLATFORM_LADDER_COLOR = Color.cyan;
    static Color PLATFORM_CHECKPOINT_NORMAL_COLOR = Color.blue;
    static Color PLATFORM_CHECKPOINT_SAVED_COLOR = Color.green;


    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        ladderTopCorner = gameObject.transform.position.y + boxCollider.bounds.extents.y;

        playerTransform = GameObject.Find(GameController.PLAYER_OBJ_NAME).transform;

        if (checkPointBehaviour)
        {
            GetComponent<SpriteRenderer>().color = PLATFORM_CHECKPOINT_NORMAL_COLOR;
            ladderBehaviour = true;
        }
        else if (ladderBehaviour)
        {
            GetComponent<SpriteRenderer>().color = PLATFORM_LADDER_COLOR;
        }

        if (moveBehavior)
        {
            startPosition = transform.localPosition;
        }
        movementTimer = movementTime;

    }

    void FixedUpdate()
    {
        ladderTransformation();
        platformMovement();
        checkPoint();
    }

    private void checkPoint()
    {
        if (checkPointBehaviour)
        {
            if (playerTransform.position.y - Player.playerBoundsY + LADDER_MARGIN > ladderTopCorner - LADDER_MARGIN)
            {
                GetComponent<SpriteRenderer>().color = PLATFORM_CHECKPOINT_SAVED_COLOR;
                //transform.localScale += Vector3.right * GameController.SCREEN_WORLD_SPACE_SIZE_X * Time.deltaTime;
                if (transform.localScale.x >= GameController.SCREEN_WORLD_SPACE_SIZE_X)
                {
                    if (lastCheckPoint)
                    {
                        // Restart scene
                        Scene loadedLevel = SceneManager.GetActiveScene();
                        SceneManager.LoadScene(loadedLevel.buildIndex);
                        return;
                    } else
                    {

                    checkPointBehaviour = false;
                    ladderBehaviour = false;
                    moveBehavior = false;
                    }
                }

            }
        }
    }

    private void platformMovement()
    {
        if (moveBehavior)
        {
            movementTimer += Time.deltaTime;
            if (movementTimer >= movementTime)
            {
                isGoingToEndPosition = !isGoingToEndPosition;

                Vector3 goingTo = startPosition;
                if (isGoingToEndPosition)
                {
                    goingTo = endPosition;
                }
                movement = ((goingTo - transform.localPosition) / movementTime); // Distance / Time
                movementTimer = 0;
            }

            transform.Translate(movement * Time.deltaTime);

        }
    }

    private void ladderTransformation()
    {
        if (ladderBehaviour)
        {
            if (moveBehavior)
            {
                ladderTopCorner = gameObject.transform.position.y + boxCollider.bounds.extents.y;
            }

            if (playerTransform.position.y - Player.playerBoundsY + LADDER_MARGIN > ladderTopCorner - LADDER_MARGIN)
            {
                // Make ladder collidable
                boxCollider.enabled = true;
            }
            // When player move from over to under the ladder
            // Start a timer to make the transition from collidable to not collidable right
            else if (ladderTimer > LADDER_TIME_CHANGE)
            {
                // Make ladder not collidable
                boxCollider.enabled = false;
                ladderTimer = 0;
            }
            else
            {
                ladderTimer += Time.deltaTime;
            }
        }
    }

    //void OnCollisionEnter2D (Collider2D col) {
    //    if(hitsToBreak > 0)
    //    {
    //        hitsToBreak--;
    //        if (hitsToBreak == 0)
    //        {
    //            Destroy(this);
    //        }
    //    }
    //}
}
