using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Street : MonoBehaviour
{
    enum SemColor { Green, Red =2}

    public int nrCars = 0;
    public int nrStoppedCars = 0;
    public int maxStoppedCars = 2;
    public float maxWaitTime = 5;
    public float timeSinceLastChange;
    public GameObject collider1;
    public GameObject collider2;

    public int peerColor;
    public float stoppedCarsInPeerStreet;

    public Semaphore[] sem;

    // Start is called before the first frame update
    void Start()
    {
        //sem = this.transform.GetChild(0).GetComponent<Semaphore>();
    }
    public int GetNumberCars() {
        nrCars = collider1.GetComponent<ColliderCarCounter>().GetNrCars();
        nrCars += collider2.GetComponent<ColliderCarCounter>().GetNrCars();
        return nrCars;
    }

    public int GetColorSemaphor(Semaphore sem) { return sem.GetColor(); }



    //public int GetNumberStoppedCars() {
    //    nrStoppedCars = 0;
    //    foreach (Transform child in transform) {
    //        Car car = child.GetComponent<Car>();
    //        if (car != null) {
    //            if (car.isStopped()) nrStoppedCars += 1;
    //        }
    //    }
    //    return nrStoppedCars;
    //}

    // Get current color and number of stopped cars in this street
    //public List<int> GetInfoStreet() {
    //    List<int> colorAndCars = new List<int>();
    //    colorAndCars.Add(GetColorSemaphor());
    //    colorAndCars.Add(GetNumberStoppedCars());
    //    return colorAndCars;
    //}


    // Get color and number of cars stopped in the other street
    //public void requestColorAndStoppedCarsFromPeer() {
    //    List<int> colorAndCars = new List<int>();
    //    Street street;
    //    foreach (Transform child in transform.parent) {
    //        if (child != this) {
    //            street = child.GetComponent<Street>();
    //            colorAndCars = street.GetInfoStreet();
    //        }
    //    }
    //    peerColor = colorAndCars[0];
    //    stoppedCarsInPeerStreet = colorAndCars[1];
    //}


}
