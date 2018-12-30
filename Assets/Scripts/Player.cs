using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    const float MOVEMENT_X_GROUND = 3;
    const float MOVEMENT_X_AIR = 2.5F;
    const float MOVEMENT_Y = 225;
    

    const float MARGIN_TO_COLLIDE = 0.05f;

    // Jump Variables
    const float JUMP_RELOAD = 0.15f;
    private float timeReloadJump = 0;

    const float JUMP_TIME_LIMIT_PRESS = 0.3f;
    public bool pressedJump = false;
    public float timeJumpPressed = 0;

    public LayerMask groundLayer;

    public bool canMove = true;
    public bool isGrounded = false;


    // Components
    Rigidbody2D rb;
    BoxCollider2D bc;

    // Player bound for Ladder platform
    public static float playerBoundsY;

    private enum wallCollidingDirection { None, Left, Right, Booth };



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        playerBoundsY = bc.bounds.extents.y;
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        Control();
    }

    public void Control()
    {
        Vector2 velocity = rb.velocity;

        float moveX = MOVEMENT_X_AIR;

        if (pressedJump && Input.GetAxis("Vertical") > 0 && timeJumpPressed < JUMP_TIME_LIMIT_PRESS)
        {
            timeJumpPressed += Time.deltaTime;
            velocity.y = MOVEMENT_Y * Time.deltaTime;
        }
        else
        {
            pressedJump = false;
        }

        if (isGround())
        {
            pressedJump = false;
            timeReloadJump -= Time.deltaTime;
            moveX = MOVEMENT_X_GROUND;
            velocity.x = 0;
            if (Input.GetAxis("Vertical") > 0 && timeReloadJump <= 0)
            {
                // Jump
                velocity.y = MOVEMENT_Y * Time.deltaTime;
                timeReloadJump = JUMP_RELOAD;
                timeJumpPressed = 0;
                isGrounded = false;
                pressedJump = true;

            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                // DownTheLadder

            }
        }

        wallCollidingDirection wallHitting = isHittingWall();
        if (canMove && wallHitting != wallCollidingDirection.Booth)
        {
            if (Input.GetAxis("Horizontal") > 0 && wallHitting != wallCollidingDirection.Right)
            {
                // Right
                velocity.x = moveX;
            }
            else if (Input.GetAxis("Horizontal") < 0 && wallHitting != wallCollidingDirection.Left)
            {
                // Left
                velocity.x = -moveX;
            }
        }

        rb.velocity = velocity;


    }

    #region Collisions

    private bool isGround()
    {
        Vector2 position = transform.position;
        position.x += bc.bounds.extents.x;
        position.y -= bc.bounds.extents.y;

        Vector2 position2 = transform.position;
        position2.x -= bc.bounds.extents.x;
        position2.y = position.y;

        if (isCollidingInDirection(position, Vector3.down) || isCollidingInDirection(position2, Vector3.down))
        {
            return true;
        }
        return false;
    }

    private wallCollidingDirection isHittingWall()
    {
        wallCollidingDirection collidingDirection = wallCollidingDirection.None;

        Vector2 positionR = transform.position;
        positionR.x += bc.bounds.extents.x;
        positionR.y += bc.bounds.extents.y;

        Vector2 positionR2 = transform.position;
        positionR2.x = positionR.x;
        positionR2.y -= bc.bounds.extents.y;

        if (isCollidingInDirection(positionR, Vector3.right) || isCollidingInDirection(positionR2, Vector3.right))
        {
            collidingDirection = wallCollidingDirection.Right;
        }

        Vector2 positionL = transform.position;
        positionL.x -= bc.bounds.extents.x;
        positionL.y += bc.bounds.extents.y;

        Vector2 positionL2 = transform.position;
        positionL2.x = positionL.x;
        positionL2.y -= bc.bounds.extents.y;


        if (isCollidingInDirection(positionL, Vector3.left) || isCollidingInDirection(positionL2, Vector3.left))
        {
            if (collidingDirection != wallCollidingDirection.None)
            {
                collidingDirection = wallCollidingDirection.Booth;
            }
            else
            {
                collidingDirection = wallCollidingDirection.Left;
            }
        }

        return collidingDirection;
    }

    private bool isCollidingInDirection(Vector2 position, Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, direction, MARGIN_TO_COLLIDE, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }

    #endregion
}
