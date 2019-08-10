using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class CarGenerator : MonoBehaviour
{
    private float timeBtwCar = -1f;
    private float timeBtwNormalSection = -1f;
    public float startTimeBtwCar = 0.2f;
    public bool horizontalRoad;
    public float gridLimitY = 100f;
    public float gridLimitX = 100f;
    private float std = 1f;
    private float mean = 0f;
    private int normalDistributionSection = -4;
    public float startTimeBtwSection = 5f;
    public float normalDistributionSpeedModifier = 5f;
    private float distrSection = -4f;
    public float timeBtwSectionContinuous = 0.1f;

    [HideInInspector]
    public bool carOnTopOfGenerator;

    public bool toRight = true;
    public bool goingUp = true;
    public GameObject car;
    public GameObject crossRoad;
    private bool allowCarSpawning = true;
    private GameObject collider;
    private int maxCars;
    public int nrCars = 0;
    public float speedMultiplier = 2f;
    public float timeMultiplier = 1f;
    public enum generationOptions
    {
        Same,
        TwiceVerticalOneHorizontal,
        TwiceHorizontalOneVertical,
        NormalDiscrete,
        NormalContinuous
          
    };
    public generationOptions dropDown = generationOptions.Same;
    // Start is called before the first frame update
    void Start()
    {
        getCorrespondingCollider();
        startTimeBtwSection = startTimeBtwSection * std;
        switch (dropDown)
        {
            case (generationOptions.NormalDiscrete):
                timeBtwNormalSection = startTimeBtwSection;
                startTimeBtwCar = Mathf.Abs(normalDistributionSection * std) / normalDistributionSpeedModifier;
                break;
            case (generationOptions.NormalContinuous):
                timeBtwNormalSection = timeBtwSectionContinuous;
                startTimeBtwCar = Mathf.Abs(normalDistributionSection * std) / normalDistributionSpeedModifier;
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        CanSpawnCars();
        switch(dropDown)
        {
            case (generationOptions.Same):
                SpawnCarsSameSpeed();
                break;
            case (generationOptions.TwiceVerticalOneHorizontal):
                SpawnDoubleSpeedCars(false, true);
                break;
            case (generationOptions.TwiceHorizontalOneVertical):
                SpawnDoubleSpeedCars(true, false);
                break;
            case (generationOptions.NormalDiscrete):
                SpawnCarsNormalDistribution();
                break;
            case (generationOptions.NormalContinuous):
                SpawnCarsNormalContinuous();
                break;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        carOnTopOfGenerator = true;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        carOnTopOfGenerator = false;
    }
    public float NextGaussian()
     {
            float v1, v2, s;
            do
            {
                v1 = 2.0f * UnityEngine.Random.Range(mean, std) - 1.0f;
                v2 = 2.0f * UnityEngine.Random.Range(mean, std) - 1.0f;
                s = v1 * v1 + v2 * v2;
            } while (s >= std || s == mean);

            s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);

            return v1 * s;
    }

    public void getSecondsToWait()
    {
        float number = NextGaussian();
        if (number > 4*std)
        {
            number = 4 * std;
        }
        else if (number < 4 * -std)
        {
            number = 4 * -std;
        }
        startTimeBtwCar = 3f - number;
    }

    private void CanSpawnCars()
    {
        nrCars = collider.GetComponent<ColliderCarCounter>().GetNrCars();
        if (carOnTopOfGenerator) {
            allowCarSpawning = false;
        }
        else
        {
            allowCarSpawning = true;
        }
    }

    private void getCorrespondingCollider()
    {
        if (horizontalRoad)
        {
            maxCars = crossRoad.GetComponent<Crossroads>().maximumCarsHorizontal;
            GameObject streetHor = crossRoad.transform.Find("Crossroads").transform.Find("StreetHor").gameObject;
            if (toRight)
            {
                collider = streetHor.transform.Find("ColliderLeft").gameObject;
            }
            else
            {
                collider = streetHor.transform.Find("ColliderRight").gameObject;
            }
        }
        else
        {
            maxCars = crossRoad.GetComponent<Crossroads>().maximumCarsVertical;
            GameObject streetHor = crossRoad.transform.Find("Crossroads").transform.Find("StreetVert").gameObject;
            if (goingUp)
            {
                collider = streetHor.transform.Find("ColliderBottom").gameObject;
            }
            else
            {
                collider = streetHor.transform.Find("ColliderTop").gameObject;
            }
        }
    }

    private void SpawnCarsSameSpeed()
    {
        if (timeBtwCar < 0 && allowCarSpawning)
        {
            if (horizontalRoad)
            {

                if (toRight)
                {
                    //var carInstantiated = Instantiate(car, new Vector3(2.5f, gridLimitY/2, -1), Quaternion.identity);
                    var carInstantiated = Instantiate(car, transform.position, transform.rotation, this.transform.parent);
                    //carInstantiated.transform.parent = this.transform.parent;
                }
                else
                {
                    var carInstantiated = Instantiate(car, transform.position, transform.rotation, this.transform.parent);
                    Car carInstance = carInstantiated.GetComponent<Car>();
                    carInstance.acceleration = -car.GetComponent<Car>().GetAcceleration();
                    carInstantiated.transform.parent = this.transform.parent;
                }
            }
            else
            {
                if (goingUp)
                {
                    var carInstantiated = Instantiate(car, transform.position, Quaternion.identity, this.transform.parent);
                    //carInstantiated.transform.parent = this.transform.parent;
                }

                else
                {
                    var carInstantiated = Instantiate(car, transform.position, transform.rotation, this.transform.parent);
                    Car carInstance = carInstantiated.GetComponent<Car>();
                    carInstance.acceleration = -car.GetComponent<Car>().GetAcceleration();
                    carInstantiated.transform.parent = this.transform.parent;
                }
            }
            timeBtwCar = startTimeBtwCar;
        }
        else
        {
            timeBtwCar -= Time.deltaTime;
        }
    }

    private void SpawnDoubleSpeedCars(bool horizontal, bool vertical)
    {
        if (timeBtwCar < 0 && allowCarSpawning)
        {
            
            if (horizontalRoad)
            {

                if (toRight)
                {
                    //var carInstantiated = Instantiate(car, new Vector3(2.5f, gridLimitY/2, -1), Quaternion.identity);
                    var carInstantiated = Instantiate(car, transform.position, transform.rotation, this.transform.parent);
                    //carInstantiated.transform.parent = this.transform.parent;
                }
                else
                {
                    var carInstantiated = Instantiate(car, transform.position, transform.rotation, this.transform.parent);
                    Car carInstance = carInstantiated.GetComponent<Car>();
                    carInstance.acceleration = -car.GetComponent<Car>().GetAcceleration();
                    carInstantiated.transform.parent = this.transform.parent;
                }
            }
            else
            {
                if (goingUp)
                {
                    var carInstantiated = Instantiate(car, transform.position, Quaternion.identity, this.transform.parent);
                    //carInstantiated.transform.parent = this.transform.parent;
                }

                else
                {
                    var carInstantiated = Instantiate(car, transform.position, transform.rotation, this.transform.parent);
                    Car carInstance = carInstantiated.GetComponent<Car>();
                    carInstance.acceleration = -car.GetComponent<Car>().GetAcceleration();
                    carInstantiated.transform.parent = this.transform.parent;
                }
            }
            if ((horizontal && horizontalRoad) || (vertical && !horizontalRoad))
            {
                timeBtwCar = startTimeBtwCar/ speedMultiplier;
            }
            else
            {
                timeBtwCar = startTimeBtwCar;
            }
        }
        else
        { 
            timeBtwCar -= Time.deltaTime;
        }
    }

    private void SpawnCarsNormalDistribution()
    {

        if (timeBtwNormalSection < 0)
        {
            if (normalDistributionSection == 4)
            {
                normalDistributionSection = -4;
                startTimeBtwCar = Mathf.Abs(normalDistributionSection * std)/ normalDistributionSpeedModifier;
            }
            else {
                normalDistributionSection += 1;
                if (normalDistributionSection == 0)
                {
                    startTimeBtwCar = Mathf.Abs(0.5f * std) / normalDistributionSpeedModifier;
                }
                else {
                    startTimeBtwCar = Mathf.Abs(normalDistributionSection * std)/ normalDistributionSpeedModifier;
                }

            }
            timeBtwNormalSection = startTimeBtwSection;
        }
        else
        {
            timeBtwNormalSection -= Time.deltaTime;
        }
        
        if (timeBtwCar < 0 && allowCarSpawning)
        {
            if (horizontalRoad)
            {

                if (toRight)
                {
                    //var carInstantiated = Instantiate(car, new Vector3(2.5f, gridLimitY/2, -1), Quaternion.identity);
                    var carInstantiated = Instantiate(car, transform.position, transform.rotation, this.transform.parent);
                    //carInstantiated.transform.parent = this.transform.parent;
                }
                else
                {
                    var carInstantiated = Instantiate(car, transform.position, transform.rotation, this.transform.parent);
                    Car carInstance = carInstantiated.GetComponent<Car>();
                    carInstance.acceleration = -car.GetComponent<Car>().GetAcceleration();
                    carInstantiated.transform.parent = this.transform.parent;
                }
            }
            else
            {
                if (goingUp)
                {
                    var carInstantiated = Instantiate(car, transform.position, Quaternion.identity, this.transform.parent);
                    //carInstantiated.transform.parent = this.transform.parent;
                }

                else
                {
                    var carInstantiated = Instantiate(car, transform.position, transform.rotation, this.transform.parent);
                    Car carInstance = carInstantiated.GetComponent<Car>();
                    carInstance.acceleration = -car.GetComponent<Car>().GetAcceleration();
                    carInstantiated.transform.parent = this.transform.parent;
                }
            }
            timeBtwCar = startTimeBtwCar;
        }
        else
        {
            timeBtwCar -= Time.deltaTime;
        }
    }

    private void SpawnCarsNormalContinuous()
    {
        Debug.Log(normalDistributionSection);
        if (timeBtwNormalSection < 0)
        {
            if (distrSection > 4f)
            {
                distrSection = -4f;
                startTimeBtwCar = Mathf.Abs(distrSection * std) / normalDistributionSpeedModifier;
            }
            else
            {
                distrSection += Time.deltaTime;
                if (distrSection == 0f)
                {
                    distrSection = 0.1f;
                    startTimeBtwCar = Mathf.Abs(distrSection * std) / normalDistributionSpeedModifier;
                }
                else
                {
                    startTimeBtwCar = Mathf.Abs(distrSection * std) / normalDistributionSpeedModifier;
                }

            }
            timeBtwNormalSection = 0.1f;
        }
        else
        {
            timeBtwNormalSection -= Time.deltaTime;
        }

        if (timeBtwCar < 0 && allowCarSpawning)
        {
            if (horizontalRoad)
            {

                if (toRight)
                {
                    //var carInstantiated = Instantiate(car, new Vector3(2.5f, gridLimitY/2, -1), Quaternion.identity);
                    var carInstantiated = Instantiate(car, transform.position, transform.rotation, this.transform.parent);
                    //carInstantiated.transform.parent = this.transform.parent;
                }
                else
                {
                    var carInstantiated = Instantiate(car, transform.position, transform.rotation, this.transform.parent);
                    Car carInstance = carInstantiated.GetComponent<Car>();
                    carInstance.acceleration = -car.GetComponent<Car>().GetAcceleration();
                    carInstantiated.transform.parent = this.transform.parent;
                }
            }
            else
            {
                if (goingUp)
                {
                    var carInstantiated = Instantiate(car, transform.position, Quaternion.identity, this.transform.parent);
                    //carInstantiated.transform.parent = this.transform.parent;
                }

                else
                {
                    var carInstantiated = Instantiate(car, transform.position, transform.rotation, this.transform.parent);
                    Car carInstance = carInstantiated.GetComponent<Car>();
                    carInstance.acceleration = -car.GetComponent<Car>().GetAcceleration();
                    carInstantiated.transform.parent = this.transform.parent;
                }
            }
            timeBtwCar = startTimeBtwCar;
        }
        else
        {
            timeBtwCar -= Time.deltaTime;
        }
    }
}
