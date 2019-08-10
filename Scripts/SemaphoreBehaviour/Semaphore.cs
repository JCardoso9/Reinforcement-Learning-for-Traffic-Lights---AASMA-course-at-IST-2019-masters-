using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Semaphore : MonoBehaviour
{
    enum SemColor { Green, Red = 2 }
    private BoxCollider2D collider;
    private int currentColor = (int)SemColor.Red;
    private float timeBtwColor;
    public float startTimeBtwColor;

    public float timeSinceLastChange = 0;

    public int nrStoppedCars = 0;
    public int maximumAmountOfCars = 3;
    private float timeInRed = 0f;
    private int totalCars = 0;
    private Renderer rend;
    public GameObject sameStreetCollider;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        rend = GetComponent<Renderer>();
        timeBtwColor = startTimeBtwColor;
        //ChangeColor(currentColor);
    }


   

    // Update is called once per frame
    void Update()
    {
        //nrStoppedCars = this.transform.parent.GetComponent<Street>().nrCars;
       // nrStoppedCars = getStoppedCars();
        //Debug.Log("Number of stopped cars: " + nrStoppedCars);

        timeSinceLastChange += 5*Time.deltaTime;
        if (this.currentColor == (int)SemColor.Red)
        {
            timeInRed += 5*Time.deltaTime;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(8) && currentColor == (int)SemColor.Red)
        {
            collision.gameObject.GetComponent<Car>().StopMovement(true, true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.gameObject.layer.Equals(8))
        {
            if (currentColor == (int)SemColor.Green)
            {
                collision.gameObject.GetComponent<Car>().StopMovement(false, true);
            }
            else
            {
                collision.gameObject.GetComponent<Car>().StopMovement(true, true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(8))
        {
          collision.gameObject.GetComponent<Car>().StopMovement(false, true);
        }
    }

    public int GetColor() { return currentColor; }


    public int getStoppedCars() {
        if (currentColor == (int)SemColor.Red) {
            nrStoppedCars = sameStreetCollider.GetComponent<ColliderCarCounter>().GetNrCars();
            return nrStoppedCars;
        }
        else return 0;
    }

    public void resetTimer() {
        timeSinceLastChange = 0;
    }

    public float GetTimeSinceLastChange() {
        return timeSinceLastChange;
    }

    public void ChangeColor()
    {
        resetTimer();
        switch (currentColor)
        {
            case (int) SemColor.Green:
                GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
                currentColor = (int)SemColor.Red;
                timeInRed = 0;
                break;
            case (int) SemColor.Red:
                GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.green);
                currentColor = (int)SemColor.Green;
                break;
        }
    }

    public float getTimeSinceRed()
    {
        return timeInRed;
    }

    public int getTotalCars()
    {
        totalCars = sameStreetCollider.GetComponent<ColliderCarCounter>().getTotalCars();
        return totalCars;
    }

    public float getCarsWaitTime()
    {
        return sameStreetCollider.GetComponent<ColliderCarCounter>().getCarsTotalWaitTime();
    }

    public bool GetInMiddle()
    {
        return sameStreetCollider.GetComponent<ColliderCarCounter>().GetInMiddle();
    }
}
