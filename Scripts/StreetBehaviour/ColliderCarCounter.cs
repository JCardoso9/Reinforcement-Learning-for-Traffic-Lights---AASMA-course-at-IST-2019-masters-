using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCarCounter : MonoBehaviour
{
    public int nrCars = 0;
    public int totalCars = 0;
    private bool countCars = false;
    public bool inMiddle;

    // Start is called before the first frame update
    public List<Collider2D> stoppedCars = new List<Collider2D>();
    List<Collider2D> allCars = new List<Collider2D>();

    void FixedUpdate()
    {
        stoppedCars.Clear(); //clear the list of all tracked objects.
    }


    // if there is collision with an object in either Enter or Stay, add them to the list 
    // (you can check if it already exists in the list to avoid double entries, 
    // just in case, as well as the tag).
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(8))
        {
            collision.gameObject.GetComponent<Car>().StartCountingWaitTime(this);
            if (!allCars.Contains(collision)) {
                allCars.Add(collision);
            }
            totalCars += 1;
        }
        if (!stoppedCars.Contains(collision) && collision.gameObject.layer.Equals(8))
        {
            if (collision.gameObject.GetComponent<Car>().isStopped())
            {
                stoppedCars.Add(collision);
            }
            
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!stoppedCars.Contains(collision) && collision.gameObject.layer.Equals(8))
        {
            if (collision.gameObject.GetComponent<Car>().isStopped())
            {
                stoppedCars.Add(collision);
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(8)) {
            collision.gameObject.GetComponent<Car>().StopCountingWaitTime();
            totalCars -= 1;
            allCars.Remove(collision);
        }
    }

    void Update()
    {
        nrCars = stoppedCars.Count;
    }


    public int GetNrCars()
    {
        return nrCars;
    }

    public int getTotalCars()
    {
        return totalCars;
    }

    public float getCarsTotalWaitTime()
    {
        float totalWaitTime = 0f;
        foreach (Collider2D collider in stoppedCars)
        {
            if (collider != null || collider.gameObject != null || collider.gameObject.GetComponent<Car>() != null) {
                totalWaitTime += collider.gameObject.GetComponent<Car>().GetWaitTime();
            }

        }
       
        return totalWaitTime;
    }

    public void RemoveCollider(Collider2D collider)
    {
        if (stoppedCars.Contains(collider))
        {
            stoppedCars.Remove(collider);
        }
    }

    public bool GetInMiddle()
    {
        return this.inMiddle;
    }
}
