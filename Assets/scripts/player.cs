using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public Vector2 velocity;
    private bool walk, leftwalk, rightwalk, jump;
    public LayerMask wallMask;
    public float jumpVelocity;
    public float gravity;
    public LayerMask floorMask;
    public float bounceVelocity;
    public enum PlayerState
    {
        jumping,
        idle,
        walking,
        bouncing
    }
    private PlayerState playerstate = PlayerState.idle;
    private bool ground = false;
    private bool bounce = false;
    
    void Start()
    {
        //Fall();
    }

    // Update is called once per frame
    void Update()
    {
        checkPlayerInput();
        checkPlayerPosition();
        updateAnimationState();
    }
    void checkPlayerPosition()
    {
        Vector3 pos = transform.localPosition;
        Vector3 scale = transform.localScale;
        if(walk)
        {
            if(leftwalk)
            {
                pos.x -= velocity.x * Time.deltaTime;
                scale.x = -1f;
            }
            if(rightwalk)
            {
                pos.x += velocity.x * Time.deltaTime;
                scale.x = 1f;
            }
            //after raycasting updating player position
            pos=CheckWallRays(pos,scale.x);
        }

        //updating player position after void fall function
        if(jump&& playerstate != PlayerState.jumping)
        {
            playerstate = PlayerState.jumping;
            velocity = new Vector2(velocity.x, jumpVelocity);
        }
        if(playerstate== PlayerState.jumping)
        {
            pos.y += velocity.y * Time.deltaTime;
            velocity.y -= gravity * Time.deltaTime;
        }

        if(bounce && playerstate!=PlayerState.bouncing)
        {
            playerstate = PlayerState.bouncing;
            velocity = new Vector2(velocity.x, bounceVelocity);
        }

        if(playerstate == PlayerState.bouncing)
        {
            pos.y += velocity.y * Time.deltaTime;
            velocity.y -= gravity * Time.deltaTime;
        }

        if (velocity.y <= 0)
            pos = checkFloorRays(pos);
        if (velocity.y >= 0)
            pos = checkCeilingRays(pos);

        transform.localPosition = pos;
        transform.localScale = scale;

        
    }
    void checkPlayerInput()
    {
        bool left_walk = Input.GetKey(KeyCode.LeftArrow);
        bool right_walk = Input.GetKey(KeyCode.RightArrow);
        bool input_jump = Input.GetKeyDown(KeyCode.Space);
        walk = left_walk || right_walk;
        leftwalk = left_walk && !right_walk;
        rightwalk = !left_walk && right_walk;
        jump = input_jump ;
    }
    void updateAnimationState()
    {
        if(ground && !walk && !bounce)
        {
            GetComponent<Animator>().SetBool("isJumping", false);
            GetComponent<Animator>().SetBool("isRunning", false);
        }
        if(ground && walk)
        {
            GetComponent<Animator>().SetBool("isJumping", false);
            GetComponent<Animator>().SetBool("isRunning", true);
        }
        if(playerstate == PlayerState.jumping)
        {
            GetComponent<Animator>().SetBool("isJumping", true);
            GetComponent<Animator>().SetBool("isRunning", false);
        }
    }
   
    Vector3 CheckWallRays (Vector3 pos, float direction)
    {
        Vector2 originTop = new Vector2(pos.x + direction * .4f, pos.y + 1f - 0.2f);
        Vector2 originMiddle = new Vector2(pos.x + direction * .4f, pos.y);
        Vector2 originBottom = new Vector2(pos.x + direction * .4f, pos.y - 1f + 0.2f);


        RaycastHit2D topWall = Physics2D.Raycast(originTop, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);
        RaycastHit2D middleWall = Physics2D.Raycast(originMiddle, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);
        RaycastHit2D bottomWall = Physics2D.Raycast(originBottom, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);

        //for checking raycasting
        if(topWall.collider!=null || middleWall.collider!=null || bottomWall.collider !=null)
        {
            pos.x -= velocity.x * Time.deltaTime * direction;
        }
        return pos;
    }
    Vector3 checkCeilingRays(Vector3 pos)
    {
        Vector2 originLeft = new Vector2(pos.x - 0.5f + 0.2f, pos.y + 1f);
        Vector2 originMiddle = new Vector2(pos.x, pos.y+1f);
        Vector2 originRight = new Vector2(pos.x + 0.5f - 0.2f, pos.y + 1f);

        RaycastHit2D ceilLeft = Physics2D.Raycast(originLeft, Vector2.up, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D ceilMiddle = Physics2D.Raycast(originMiddle, Vector2.up, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D ceilRight = Physics2D.Raycast(originRight, Vector2.up, velocity.y * Time.deltaTime, floorMask);

        if(ceilLeft.collider!=null||ceilMiddle.collider!=null||ceilRight.collider!= null)
        {
            RaycastHit2D hitRays = ceilLeft;
            if(ceilLeft)
            {
                hitRays = ceilLeft;
            }
            else if(ceilMiddle)
            {
                hitRays = ceilMiddle;
            }
           else if(ceilRight)
            {
                hitRays = ceilRight;
            }

            if(hitRays.collider.tag =="QuestionBlock")
            {
                hitRays.collider.GetComponent<Qblock>().questionBlock();
            }
            pos.y = hitRays.collider.bounds.center.y - hitRays.collider.bounds.size.y / 2 - 1;
            Fall();
        }
        return pos;

    }
    void Fall()
    {
        velocity.y = 0;
        playerstate = PlayerState.jumping;
        ground = false;
        bounce = false;
    }
    Vector3 checkFloorRays(Vector3 pos)
    {
        Vector2 originLeft = new Vector2(pos.x - 0.5f + 0.2f, pos.y - 1f);
        Vector2 originMiddle = new Vector2(pos.x, pos.y - 1f);
        Vector2 originRight = new Vector2(pos.x + 0.5f - 0.2f, pos.y - 1f);

        RaycastHit2D floorLeft = Physics2D.Raycast(originLeft, Vector2.down, velocity.y * Time.deltaTime,floorMask);
        RaycastHit2D floormiddle = Physics2D.Raycast(originMiddle, Vector2.down, velocity.y * Time.deltaTime,floorMask);
        RaycastHit2D floorright= Physics2D.Raycast(originRight, Vector2.down, velocity.y * Time.deltaTime, floorMask);

        if(floorLeft.collider!=null||floormiddle.collider!=null||floorright.collider!=null)
        {
            RaycastHit2D hitRays = floorright;
            if(floorLeft)
            {
                hitRays = floorLeft;
            }
            else if(floormiddle)
            {
                hitRays = floormiddle;
            }
            else if(floorright)
                {
                hitRays = floorright;
            }

            if(hitRays.collider.tag=="Enemy")
            {
                bounce = true;
                hitRays.collider.GetComponent<enemyAI>().crush();
            }
            playerstate = PlayerState.idle;
            ground = true;
            velocity.y = 0;

            pos.y = hitRays.collider.bounds.center.y + hitRays.collider.bounds.size.y/2+1;
        }
        else
        {
            if(playerstate != PlayerState.jumping)
            {
                Fall();
            }
        }
        return pos;
    }
}