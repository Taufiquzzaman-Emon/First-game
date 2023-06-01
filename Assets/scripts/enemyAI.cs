using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class enemyAI : MonoBehaviour
{
    public float gravity;
    public Vector2 velocity;
    public bool walkingLeft = true;
    private bool grounded = false;
    public LayerMask floorMask;
    public LayerMask wallMask;

    private bool Die = false;
    private float deathTimer = 0;
    private float timeBeforeDestroy = 1.0f;
    private enum EnemyState
    {
        walking,
        falling,
        dead
    }
    private EnemyState state = EnemyState.falling;
    void Start()
    {
        //to stop the moving of an enemy off camera
        enabled = false;
        fall();
    }
   
    // Update is called once per frame
    void Update()
    {
        updateEnemyPosition();
        checkCrushed();
    }
    void updateEnemyPosition()
    {
        if(state!=EnemyState.dead)
        {
            Vector3 pos = transform.localPosition;
            Vector3 scale = transform.localScale;
            if(state==EnemyState.falling)
            {
                pos.y += velocity.y * Time.deltaTime;
                velocity.y -= gravity * Time.deltaTime;
            }
            if(state==EnemyState.walking)
            {
                if(walkingLeft)
                {
                    pos.x -= velocity.x * Time.deltaTime;
                    scale.x = 1;
                }
                else
                {
                    pos.x += velocity.x * Time.deltaTime;
                    scale.x = 1;
                }
            }
            if (velocity.y <= 0)
                pos = checkGround(pos);

            checkWalls (pos,scale.x); 

            transform.localPosition = pos;
            transform.localScale = scale;
        }
    
    }
    void fall()
    {
        velocity.y = 0;
        state = EnemyState.falling;
        grounded = false;
        
    }

    void checkCrushed()
    {
        if(Die)
        {
            if(deathTimer<=timeBeforeDestroy)
            {
                deathTimer += Time.deltaTime;
            }
            else
            {
                Die = false;
                Destroy(this.gameObject);
            }
        }
    }
    Vector3 checkGround(Vector3 pos)
    {
        Vector2 originLeft = new Vector2(pos.x - 0.5f + 0.2f, pos.y - .5f);
        Vector2 originMiddle = new Vector2(pos.x, pos.y - .5f);
        Vector2 originRight = new Vector2(pos.x + 0.5f - 0.2f, pos.y - .5f);

        RaycastHit2D groundLeft = Physics2D.Raycast(originLeft, Vector2.down, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D groundMiddle = Physics2D.Raycast(originMiddle, Vector2.down, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D groundRight = Physics2D.Raycast(originRight, Vector2.down, velocity.y * Time.deltaTime, floorMask);

        if(groundLeft.collider!=null || groundMiddle.collider!=null || groundRight.collider!=null)
        {
            RaycastHit2D hitRays =groundLeft;
            if(groundLeft)
            {
                hitRays = groundLeft;
            }
            else if(groundMiddle)
            {
                hitRays = groundMiddle;
            }
            else if(groundRight)
            {
                hitRays = groundRight;
            }
            if (hitRays.collider.tag=="Player")
            {
                SceneManager.LoadScene("GameOver");
            }

            pos.y = hitRays.collider.bounds.center.y + hitRays.collider.bounds.size.y / 2 + .5f;
            grounded = true;
            velocity.y = 0;
            state = EnemyState.walking;
        }
        else
        {
            if(state!=EnemyState.falling)
            {
                fall();
            }
        }

        return pos;
    }

    public void crush()
    {
        state = EnemyState.dead;
        GetComponent<Animator>().SetBool("isCrushed", true);
        GetComponent<Collider2D>().enabled = false;
        Die = true;
    }
    void checkWalls(Vector3 pos, float direction)
    {
        Vector2 originTop = new Vector2(pos.x + direction * 0.4f, pos.y + 0.5f - 0.2f);
        Vector2 originMiddle = new Vector2(pos.x + direction * 0.4f, pos.y);
        Vector2 originBottom = new Vector2(pos.x + direction * 0.4f, pos.y - 0.5f + 0.2f);

        RaycastHit2D topWall = Physics2D.Raycast(originTop, new Vector2(direction, 0), velocity.x * Time.deltaTime,wallMask);
        RaycastHit2D middleWall = Physics2D.Raycast(originMiddle, new Vector2(direction, 0), velocity.x * Time.deltaTime,wallMask);
        RaycastHit2D bottomWall = Physics2D.Raycast(originBottom, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);

        if(topWall.collider !=null || middleWall.collider !=null || bottomWall.collider !=null)
        {
            RaycastHit2D hitRays = topWall;

            if(topWall)
            {
                hitRays = topWall;
            }
            else if(middleWall)
            {
                hitRays = middleWall;
            }
            else if(bottomWall)
            {
                hitRays = bottomWall;
            }
            if (hitRays.collider.tag == "Player")
            {
                SceneManager.LoadScene("GameOver");
                Debug.Log("GameOver!");
            }

            walkingLeft = !walkingLeft;
        }
    }
    void OnBecameVisible()
    {
        //when enemy come to on camera it began moving
        enabled = true;
    }

}
