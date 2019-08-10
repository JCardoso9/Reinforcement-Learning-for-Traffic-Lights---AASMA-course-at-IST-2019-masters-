using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{

    
    public bool horizontalMovement;
    public float acceleration;
    private Rigidbody2D rb;
    public float gridLimitUpperX = 200f;
    public float gridLimitLowerX = -300f;
    public float gridLimitUpperY = 200f;
    public float gridLimitLowerY = -142f;
    public float gridLimitY = 100f;
    public LayerMask layerMask;
    private Vector2 boxPosition;
    private Vector2 boxScale;
    private bool stopMovement = false;
    private bool fromSemaphore = false;
    public float waitTime = 0f;
    private bool countWaitTime = false;
    private ColliderCarCounter counter;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Or detect red light
        bool carInFront = DetectCarInFront();
        if (!carInFront && !stopMovement)
        {
            StopMovement(false, false);
            Move();
            CheckIfCollideLimits();
        }
        else
        {
            if (fromSemaphore)
            {
                StopMovement(true, true);
            }
            else
            {
                StopMovement(true, false);
            }
            
        }
        if(countWaitTime){
            
            if (((rb.constraints & RigidbodyConstraints2D.FreezePositionX) != RigidbodyConstraints2D.None && horizontalMovement) || ((rb.constraints & RigidbodyConstraints2D.FreezePositionY) != RigidbodyConstraints2D.None && !horizontalMovement))
            {
                waitTime += 5*Time.deltaTime;
            }
        }
    }

    private void Move()
    {
        Vector2 movement;
        if (horizontalMovement)
        {
            movement = new Vector2(30f * acceleration, 0);
        }
        else
        {
            movement = new Vector2(0, 30f * acceleration);
        }
        rb.velocity = movement;
    }

    private void CheckIfCollideLimits()
    {
        if (horizontalMovement && transform.position.x >= gridLimitUpperX || horizontalMovement && transform.position.x <= gridLimitLowerX)
        {
            Destroy(gameObject);
        }
        else if (!horizontalMovement && transform.position.y >= gridLimitUpperY || !horizontalMovement && transform.position.y <= gridLimitLowerY)
        {
            Destroy(gameObject);
        }
    }

    private bool DetectCarInFront()
    {
        if (horizontalMovement)
        {

            if (acceleration > 0)
            {
                boxPosition = new Vector3(transform.position.x + (transform.localScale.x ), transform.position.y, transform.position.z);
            }
            else
                boxPosition = new Vector3(transform.position.x - (transform.localScale.x ), transform.position.y, transform.position.z);
            boxScale = new Vector3(transform.localScale.x, transform.localScale.y , transform.localScale.z);
        }
        else
        {
            if (acceleration > 0)
            {
                boxPosition = new Vector3(transform.position.x, transform.position.y + (transform.localScale.y ), transform.position.z);
            }
            else
                boxPosition = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y ), transform.position.z);
            boxScale = new Vector3(transform.localScale.x, transform.localScale.y , transform.localScale.z);
        }
        if (!fromSemaphore)
        {
            StopMovement(false, false);
        }
        Collider2D[] collisions = Physics2D.OverlapBoxAll(boxPosition, boxScale, 0, layerMask);
        if (collisions.Length > 1)
        {
            bool inFront = false;
            for (int i = 0; i < collisions.Length; i++)
            {
                bool oppositeDirectionMovement = (collisions[i].gameObject.GetComponent<Car>().getIsMovingHorizontally() == horizontalMovement) ? false : true;
                if (oppositeDirectionMovement)
                {
                    counter.RemoveCollider(gameObject.GetComponent<BoxCollider2D>());
                    Destroy(gameObject);
                    Destroy(collisions[i].gameObject);
                }
                else if (!oppositeDirectionMovement && collisions[i].gameObject.transform.position != gameObject.transform.position)
                {
                    inFront = true;
                }
            }
            return inFront;
        }
        return false;
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        //Gizmos.DrawWireCube(boxPosition, boxScale);
    }

    public bool getIsMovingHorizontally()
    {
        return horizontalMovement;
    }

    public void StopMovement(bool remainStopped, bool semaphore)
    {
        if (!remainStopped)
        {
            if (semaphore) {
                this.fromSemaphore = true;
            }
            else
            {
                this.fromSemaphore = false;
            }

            if (horizontalMovement)
            {
                rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
            }
            else
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            }
        }
        else
        {
            if (semaphore)
            {
                this.fromSemaphore = true;
            }
            else
            {
                this.fromSemaphore = false;
            }
            
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
            
            
           
        }
        this.stopMovement = remainStopped;
    }

    public bool isStopped() { return stopMovement; }

    public float GetAcceleration()
    {
        return this.acceleration;
    }

    public void StartCountingWaitTime(ColliderCarCounter counter)
    {
        this.counter = counter;
        this.waitTime = 0f;
        this.countWaitTime = true;
    }

    public void StopCountingWaitTime()
    {
        this.countWaitTime = false;
    }

    public float GetWaitTime()
    {
        return this.waitTime;
    }
}
