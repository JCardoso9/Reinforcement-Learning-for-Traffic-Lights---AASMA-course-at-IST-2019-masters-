using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Crossroads : MonoBehaviour
{
    public List<Semaphore> horizontalSemaphores;
    public List<Semaphore> verticalSemaphores;
    private enum ActionSemaphore { switchHorizontal, switchVertical};
    enum SemColor { Green, Red = 2 }
    public float[][] q_table;
    private int[] actions;
    public int maximumCarsHorizontal = 15;
    public int maximumCarsVertical = 15;
    public int maximumCarsMiddle = 30;
    public int matrixRowsColumns = 5;
    public int actionsNumber = 2;
    public float costWeightCars = 2f;
    public float costWeightTimeRed = 0.5f;
    public float maximumTimeWaiting = 15f;
    float gamma = 0.99f; // Discount factor for calculating Q-target.
    float e = 1; // Initial epsilon value for random action selection.
    float eMin = 0.1f; // Lower bound of epsilon.
    private float step = 0.3f;
    public int action;

    private bool wroteFlag = false;

    private List<float> costs = new List<float>();
    private List<int> colorsVertical = new List<int>();
    private List<int> horizontalCarsQueued = new List<int>();
    private List<int> verticalCarsQueued = new List<int>();
    private List<int> allQueues = new List<int>();
    

    private void Start()
    {
        InitializeQMatrix();
        InitializeActionsMatrix();
        StartCoroutine(RunMDP());
        colorsVertical.Add((int)SemColor.Red);
    }

    private void InitializeQMatrix()
    {
        q_table = new float[(matrixRowsColumns * (matrixRowsColumns+1)) + matrixRowsColumns + 1][];
        for (int i = 0; i < (matrixRowsColumns * (matrixRowsColumns + 1)) + matrixRowsColumns + 1; i++)
        {
            q_table[i] = new float[actionsNumber];
            for (int j = 0; j < actionsNumber; j++)
            {
                q_table[i][j] = 0f;
            }
        }
        //WriteQToFile("Q_table", q_table);
        //q_table = ReadQFile("Q_table");
        //WriteQToFile("Q_table1", q_table);
    }

    private void InitializeActionsMatrix()
    {
        // 0 = horizontal green vertical red, 1 = horizontal red vertical green
        actions = new int[actionsNumber];
        for (int i = 0; i < actionsNumber; i++)
        {
            actions[i] = i;
        }
    }

    private int SelectActionGreedy(float[][] Q, int stateIndex, float e)
    {
        float[] q_values = Q[stateIndex];
        float value = Random.Range(0f, 1f);
        Debug.Log(value);
        if (value <= e)
        {
            int index = Random.Range(0, actionsNumber);
            return index;
        }
        else
        {
            return GetMinIndexFromList(q_values);
        }
    }

    private int GetMinIndexFromList(float[] list)
    {
        int minIndex = 0;
        float minValue = float.MaxValue;
        for (int i = 0; i < actionsNumber; i++)
        {
            if (minValue > list[i])
            {
                minValue = list[i];
                minIndex = i;
            }
        }
        return minIndex;
    }

    private float GetMinValueFromList(float[] list)
    {
        float minValue = float.MaxValue;
        for (int i = 0; i < actionsNumber; i++)
        {
            if (minValue > list[i])
            {
                minValue = list[i];
            }
        }
        return minValue;
    }

    IEnumerator RunMDP()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            int[] indexes = SelectCurrentState();
            int currentState = (indexes[0] * (matrixRowsColumns + 1) + indexes[1]);
            action = SelectActionGreedy(q_table, currentState, e);
            float cost = CalculateCostBasedOnWaitTime(action);
            costs.Add(cost);
            Debug.Log(cost);
            int nextState = SelectNextState(indexes, action);
            Debug.Log("ACTION: " + action);
            Debug.Log("NEXT STATE: " + nextState);
            Debug.Log("CURRENT STATE : " + currentState);
            int nextAction = SelectActionGreedy(q_table, nextState, e);
            q_table[currentState][action] = q_table[currentState][action] + step * (cost + gamma * q_table[nextState][nextAction] - q_table[currentState][action]);
            //TO USE Q-LEARNING COMMENT LINE ABOVE AND REMOVE COMMENT FROM LINE BELOW
            //q_table[currentState][action] = q_table[currentState][action] + step * (cost + gamma * GetMinValueFromList(q_table[nextState]) - q_table[currentState][action]);
            SelectAction(action);
            computeLengthQueue();
            if (e > eMin) {
                e -= 0.001f;
                Debug.Log("E constant: " + e);
            }

            else
            {
                e = eMin;

                if (!wroteFlag) {
                    string fileName = this.name +"_costs"; 
                    WriteDataToFileFloat(fileName,costs);

                    fileName = this.name + "_colorVerticalSemaphorSequence";
                    WriteDataToFileInt(fileName, colorsVertical);

                    fileName = this.name + "_queueVertical";
                    WriteDataToFileInt(fileName, verticalCarsQueued);

                    fileName = this.name + "_queueHorizontal";
                    WriteDataToFileInt(fileName, horizontalCarsQueued);

                    fileName = this.name + "_totalQueues";
                    WriteDataToFileInt(fileName, allQueues);

                    fileName = this.name + "Q_table";
                    WriteQToFile(fileName, q_table);


                    wroteFlag = true;
                }

            }

        }
    }

    public void AddCost(float cost) {
        costs.Add(cost);
    }


    static void WriteDataToFileInt(string fileName, List<int> costs) {
        string path = "Assets/Resources/" + fileName + ".txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, false);
        foreach (float number in costs) {
            writer.WriteLine(number);
        }

        writer.Close();
    }


    static void WriteDataToFileFloat(string fileName, List<float> costs) {
        string path = "Assets/Resources/" + fileName + ".txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, false);
        foreach (float number in costs) {
            writer.WriteLine(number);
        }
        
        writer.Close();
    }



    void WriteQToFile(string fileName, float[][] q_table) {
        string path = "Assets/Resources/" + fileName + ".txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, false);
        for (int i = 0; i < (matrixRowsColumns * (matrixRowsColumns + 1)) + matrixRowsColumns + 1; i++) {
            for (int j = 0; j < actionsNumber - 1; j++) {
                writer.Write(q_table[i][j] + "|");
            }
            writer.Write(q_table[i][actionsNumber - 1] + "\n");
        }
        writer.WriteLine("------------------------------------------");
        writer.Close();
    }

    float[][] ReadQFile(string fileName) {
        string path = "Assets/Resources/" + fileName + ".txt";

        //read some text from the test.txt file
        StreamReader reader = new StreamReader(path);
        for (int i = 0; i < (matrixRowsColumns * (matrixRowsColumns + 1)) + matrixRowsColumns + 1; i++) {
            string[] line = reader.ReadLine().Split('|');
            for (int j = 0; j < actionsNumber - 1; j++) {
                q_table[i][j] = float.Parse(line[j]);
            }
        }
        reader.Close();
        return q_table;
    }

    // May need to store cost values in a matrix
    private float CalculateCost(int selectedState, int action)
    {
        float cost = 0f;
        switch (action)
        {
            case ((int)ActionSemaphore.switchHorizontal):
                foreach (Semaphore semaphore in horizontalSemaphores)
                {
                    float timeInRed = semaphore.getTimeSinceRed();
                    int carsWaiting = semaphore.getStoppedCars();
                    cost += ((costWeightCars * carsWaiting) * (costWeightTimeRed * timeInRed)) / (maximumCarsHorizontal + maximumCarsMiddle);
                }
                costs.Add(cost);
                return cost;
    
            case ((int)ActionSemaphore.switchVertical):
                foreach (Semaphore semaphore in verticalSemaphores)
                {
                    float timeInRed = semaphore.getTimeSinceRed();
                    int carsWaiting = semaphore.getStoppedCars();
                    cost += ((costWeightCars * carsWaiting) * (costWeightTimeRed * timeInRed)) / (maximumCarsHorizontal + maximumCarsMiddle);
                }
                costs.Add(cost);
                return cost;

            default:
                return cost;
        }
    }

    private void computeLengthQueue() {
        int carsStoppedHorizontal = 0;
        int carsPassingHorizontal = 0;
        int carsPassingVertical = 0;
        int carsStoppedVertical = 0;
        foreach (Semaphore semaphore in horizontalSemaphores) {
            carsStoppedHorizontal += semaphore.getStoppedCars();
            carsPassingHorizontal += semaphore.getTotalCars();
        }

        foreach (Semaphore semaphore in verticalSemaphores) {
            carsStoppedVertical += semaphore.getStoppedCars();
            carsPassingVertical += semaphore.getTotalCars();

        }

        horizontalCarsQueued.Add(carsStoppedHorizontal);
        verticalCarsQueued.Add(carsStoppedVertical);
        allQueues.Add(carsStoppedVertical + carsStoppedHorizontal);
    }


    // May need to store cost values in a matrix
    /*private float CalculateCostBothWays(int selectedState, int action) {
        float cost = 0f;
        float timeInRedhorizontal = -1 ;
        int carsStoppedHorizontal= 0;
        int carsPassingHorizontal= 0;
        float timeInRedVertical = -1;
        int carsPassingVertical = 0;
        int carsStoppedVertical = 0;
        foreach (Semaphore semaphore in horizontalSemaphores) {
            timeInRedhorizontal = semaphore.getTimeSinceRed();
            carsStoppedHorizontal += semaphore.getStoppedCars();
            carsPassingHorizontal += semaphore.getTotalCars();
        }

        foreach (Semaphore semaphore in verticalSemaphores) {
            timeInRedVertical = semaphore.getTimeSinceRed();
            carsStoppedVertical += semaphore.getStoppedCars();
            carsPassingVertical += semaphore.getTotalCars();
            
        }
        switch (action) {
            case ((int)ActionSemaphore.switchHorizontal):
                int trafficFlow = (carsStoppedHorizontal == 0) ? (carsStoppedHorizontal / 4) : (carsPassingHorizontal);
                cost += ((((costWeightTimeRed /2)* -trafficFlow) + (carsStoppedVertical * costWeightCars)) * (costWeightTimeRed * timeInRedhorizontal)) / (maximumCarsHorizontal + maximumCarsMiddle + maximumCarsVertical);
                Debug.Log(this.name + " Cost: " + cost);
                Debug.Log("cars passing horizontal" + carsPassingHorizontal);
                costs.Add(cost);
                return cost;

            case ((int)ActionSemaphore.switchVertical):
                int trafficFlowVert = (carsStoppedVertical == 0) ? (carsStoppedVertical / 4) : (carsPassingVertical);
                cost += ((((costWeightTimeRed / 2) * -trafficFlowVert) + carsStoppedHorizontal * costWeightCars) * (costWeightTimeRed * timeInRedVertical)) / (maximumCarsHorizontal + maximumCarsMiddle + maximumCarsVertical);
                Debug.Log(this.name + " Cost: " + cost);
                Debug.Log("cars passing vertical" + carsPassingVertical);

                costs.Add(cost);
                return cost;

            default:
                return cost;
        }
    }*/

    public float CalculateCostBasedOnWaitTime(int action)
    {
        float cost = 0f;
        float waitTime = 0f;
        switch (action)
        {
            case ((int)ActionSemaphore.switchHorizontal):
                foreach (Semaphore semaphore in verticalSemaphores)
                {
                    float timeInRed = semaphore.getTimeSinceRed();
                    waitTime += semaphore.getCarsWaitTime();
                    cost += waitTime;
                }
                //Debug.Log(this.name + " Cost: " + cost);
                //if (!wroteToFile) {
                //    costs.Add(cost);
                //}
                return cost;

            case ((int)ActionSemaphore.switchVertical):
                foreach (Semaphore semaphore in horizontalSemaphores)
                {
                    float timeInRed = semaphore.getTimeSinceRed();
                    waitTime += semaphore.getCarsWaitTime();
                    cost += waitTime;
                }
                //Debug.Log(this.name + " Cost: " + cost);
                //if (!wroteToFile) {
                //    costs.Add(cost);
                //}
                return cost;

            default:
                return cost;
        }
    }

    private void SelectAction(int actionIndex)
    {
        switch (actionIndex)
        {
            case ((int)ActionSemaphore.switchHorizontal):
                foreach(Semaphore semaphore in horizontalSemaphores)
                {
                    if (semaphore.GetColor() == 2)
                    {

                        semaphore.ChangeColor();
                        //Debug.Log("Semaphore new color: " + semaphore.GetColor());
                    }
                }
                foreach (Semaphore semaphore in verticalSemaphores)
                {
                    // Debug.Log("color before " + this.name + " " + semaphore.GetColor());
                    if (semaphore.GetColor() == 0) {
                        semaphore.ChangeColor();
                    }
                    //Debug.Log("color after " + this.name + " "+semaphore.GetColor());
                }
                colorsVertical.Add((int)SemColor.Red);
                break;
            case ((int)ActionSemaphore.switchVertical):
                foreach (Semaphore semaphore in verticalSemaphores)
                {
                    if (semaphore.GetColor() == 2) {
                        semaphore.ChangeColor();
                    }
                }
                foreach (Semaphore semaphore in horizontalSemaphores)
                {
                    if (semaphore.GetColor() == 0)
                    {
                        semaphore.ChangeColor();
                    }
                }
                colorsVertical.Add((int)SemColor.Green);

                break;
        }
    }

    private int[] SelectCurrentState()
    {
        int verticalIndex = 0;
        int horizontalIndex = 0;
        /*foreach (Semaphore semaphore in verticalSemaphores)
        {
            float timeInRed = semaphore.getTimeSinceRed();
            if (timeInRed > maximumTimeWaiting)
            {
                timeInRed = maximumTimeWaiting;
            }
            verticalIndex = (int)Mathf.Round((timeInRed / maximumTimeWaiting) * matrixRowsColumns);
            //Debug.Log("Vertical Index: " + verticalIndex);
        }
        foreach (Semaphore semaphore in horizontalSemaphores)
        {
            float timeInRed = semaphore.getTimeSinceRed();
            if (timeInRed > maximumTimeWaiting)
            {
                timeInRed = maximumTimeWaiting;
            }
            horizontalIndex = (int)Mathf.Round((timeInRed / maximumTimeWaiting) * matrixRowsColumns);
            //Debug.Log("Horizontal Index: " + horizontalIndex);
        }*/
        float stoppedCars = 0f;
        foreach (Semaphore semaphore in verticalSemaphores)
        {
            int stoppedCarsSemaphore = semaphore.getStoppedCars();
            if (stoppedCarsSemaphore > (maximumCarsVertical))
            {
                stoppedCarsSemaphore = maximumCarsVertical;
            }
            stoppedCars += stoppedCarsSemaphore;
            Debug.Log("VERTICAL STOPPED CARS: " + stoppedCars);
            //Debug.Log("Vertical Index: " + verticalIndex);
        }
        //Maybe here consider the denominator the maximum cars overall? maybe just cars middle?
        verticalIndex = (int)Mathf.Round((stoppedCars/ (maximumCarsVertical*2)) * matrixRowsColumns);
        Debug.Log("Vertical index : " + verticalIndex);
        stoppedCars = 0;
        foreach (Semaphore semaphore in horizontalSemaphores)
        {
            int stoppedCarsSemaphore = semaphore.getStoppedCars();
            if (stoppedCarsSemaphore > maximumCarsHorizontal)
            {
                stoppedCarsSemaphore = maximumCarsHorizontal;
            }
            stoppedCars += stoppedCarsSemaphore;
            //Debug.Log("Horizontal Index: " + horizontalIndex);
        }
        horizontalIndex = (int)Mathf.Round((stoppedCars / (maximumCarsHorizontal*2)) * matrixRowsColumns);
        return new int[] { horizontalIndex, verticalIndex };
    }

    private int SelectNextState(int[] currentState, int action)
    {
        int verticalIndex = currentState[1];
        int horizontalIndex = currentState[0];
        switch (action)
        {
            /*case ((int)ActionSemaphore.switchHorizontal):
                foreach (Semaphore semaphore in horizontalSemaphores)
                {
                    if (semaphore.GetColor() == 2)
                    {
                        horizontalIndex = 0;
                    }
                    else
                    {
                        if (currentState[0] == matrixRowsColumns)
                        {
                            horizontalIndex = currentState[0];
                        }
                        else {
                            horizontalIndex = currentState[0] + 1;
                        }
                    }
                }
                break;
            case ((int)ActionSemaphore.switchVertical):
                foreach (Semaphore semaphore in verticalSemaphores)
                {
                    if (semaphore.GetColor() == 2)
                    {
                        verticalIndex = 0;
                    }
                    else
                    {
                        if (currentState[1] == matrixRowsColumns)
                        {
                            verticalIndex = currentState[1];
                        }
                        else
                        {
                            verticalIndex = currentState[1] + 1;
                        }
                    }
                }
                break;*/

            case ((int)ActionSemaphore.switchHorizontal):
                Semaphore semaphore = horizontalSemaphores[0];
                
                if (semaphore.GetColor() == 2)
                {
                    if (horizontalIndex != 0) {
                        horizontalIndex -= 1;
                    }
                    if(verticalIndex != matrixRowsColumns)
                    {
                        verticalIndex = SelectIndexChangingGreenToRed(false);
                    }
                }
                else
                {
                    if (horizontalIndex != 0)
                    {
                        horizontalIndex -= 1;
                    }
                }
                break;
            case ((int)ActionSemaphore.switchVertical):
                Semaphore semaphoreVert = verticalSemaphores[0];
                if (semaphoreVert.GetColor() == 2)
                {
                    if (verticalIndex != 0)
                    {
                        verticalIndex -= 1;
                    }
                    if (horizontalIndex != matrixRowsColumns)
                    {
                        horizontalIndex = SelectIndexChangingGreenToRed(true);
                    }
                }
                else
                {
                    if (verticalIndex != 0)
                    {
                        verticalIndex -= 1;
                    }
                }
                
                break;
        }
        return (horizontalIndex * (matrixRowsColumns + 1) + verticalIndex);
    }


    private int SelectIndexChangingGreenToRed(bool horizontal)
    {
        float movingCars = 0;
        if (!horizontal)
        {
            foreach (Semaphore semaphore in verticalSemaphores)
            {
                int cars = semaphore.getTotalCars();
                if (cars > (maximumCarsVertical))
                {
                    cars = maximumCarsVertical;
                }
                movingCars += cars;
                //Debug.Log("Vertical Index: " + verticalIndex);
            }
            return (int)Mathf.Round((movingCars / maximumCarsVertical) * matrixRowsColumns);
        }
        else
        {
            foreach (Semaphore semaphore in horizontalSemaphores)
            {
                int cars = semaphore.getTotalCars();
                if (cars > maximumCarsHorizontal)
                {
                    cars = maximumCarsHorizontal;
                }
                movingCars += cars;
                //Debug.Log("Horizontal Index: " + horizontalIndex);
            }
            return (int)Mathf.Round((movingCars / (maximumCarsHorizontal *2)) * matrixRowsColumns);
        }
    }
}
