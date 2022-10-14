using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay : MonoBehaviour
{
    #region Variables
    // static variables
    public static Gameplay Instance;

    public StageDataObject stageData;
    private string stageName;
    private int customersGoal = 0;
    private int coinsGoal;
    public float timeLimit;
    public float custSpwnTimeBase;
    public float custSpwnTimeRand;
    public GameObject customerPrefab;
    public GameObject cakePrefab;

    // dynamic vars
    private int _customersDone = 0;
    private int _coinsGet;
    public List<Customer> customerQueue = new List<Customer>();
    public List<RectTransform> stallPositions = new List<RectTransform>();
    public List<PindahkanAdonan> bakingQueue = new List<PindahkanAdonan>();
    public List<KueJadi> readyCakesQueue = new List<KueJadi>();
    public float timerSpawnCustomer;
    public float _timerSpawnCustomer;
    public float _timerStage;

    // references
    public Text scoreBoard;
    public Text gameTimer;
    public GameObject victoryScreen;
    public GameObject retryScreen;
    public GameObject smokePrefab;
    #endregion

    #region Loadings

    // instance
    private void Awake()
    {
        if (!Instance) Instance = this;
    }

    // data loading
    private void OnEnable()
    {
        // get the data object from game manager
        stageData = GameManager.Instance.selectedStageData;

        // unload
        stageName = stageData.stageName;
        customersGoal = stageData.customersGoal;
        coinsGoal = stageData.coinsGoal;
        timeLimit = stageData.timeLimitBase;
        custSpwnTimeBase = stageData.custSpwnTimeBase;
        custSpwnTimeRand = stageData.custSpwnTimeRand;

        // add penalty
        customersGoal += GameManager.Instance._penalty;

        UpdateScoreBoard();
    }
    #endregion

    #region State Machine
    public State GAMESTATE = State.Start;
    public bool gameIsRunning;

    private void Update()
    {
        // increment the timers
        if (gameIsRunning)
        {
            _timerStage += Time.deltaTime;
            _timerSpawnCustomer += Time.deltaTime;
            UpdateGameTimer();
        }

        switch (GAMESTATE)
        {
            case State.Start:
                // wait 5 seconds before spawn first customer
                StartGame();
                GAMESTATE = State.Idle;
                break;

            case State.WaitToSpawnCustomer:
                // wait random seconds
                WaitToSpawnCustomer();
                GAMESTATE = State.Idle;
                break;

            case State.Idle:
                // check the time limit
                if (_timerStage >= timeLimit) GAMESTATE = State.TimesUp;
                // check the customer spawn timer
                if (_timerSpawnCustomer >= timerSpawnCustomer) GAMESTATE = State.SpawnNewCustomer;
                break;

            case State.SpawnNewCustomer:
                // try to spawn customer
                CustomerArrive();
                break;

            case State.TimesUp:
                // check the win condition
                if (_customersDone >= customersGoal) GAMESTATE = State.Victory;
                else GAMESTATE = State.TryAgain;
                break;

            case State.Victory:
                // show the victory screen
                ShowVictoryScreen();
                break;

            case State.TryAgain:
                // give option to try again
                ShowDefeatScreen();
                break;
        }
    }

    #endregion

    #region Methods

    // processes

    void StartGame()
    {
        print("gemu starto");
        gameIsRunning = true;
        _timerStage = 0;
        _timerSpawnCustomer = 0;
        timerSpawnCustomer = 5;
        GAMESTATE = State.Idle;
    }

    void WaitToSpawnCustomer()
    {
        _timerSpawnCustomer = 0;
        timerSpawnCustomer = custSpwnTimeBase + Random.Range(0, custSpwnTimeRand);
        GAMESTATE = State.Idle;
    }

    void CustomerArrive()
    {
        // // debug [REMOVE THIS]
        // print("customer arrived!");
        // GAMESTATE = State.WaitToSpawnCustomer;
        // return;

        // if there is empty stall, spawn new cust. if full, wait again
        if (customerQueue.Count >= 4)
        {
            print("queue is full! waiting to spawn");
            GAMESTATE = State.WaitToSpawnCustomer;
            return;
        }

        // else, spawn new customer and resume wait
        CustomerChooseStall();
        GAMESTATE = State.WaitToSpawnCustomer;
    }

    void CustomerChooseStall()
    {
        // set where the customer will wait
        // first get the random number 1-4
        int randNum = Random.Range(0, 4);

        // now check if any of the existing customer has that number
        bool stallIsTaken = false;
        Customer cust = customerQueue.Find(x => x.stallNumber == randNum);
        if (!cust)
        {
            // stall not taken, spawn the customer
            SpawnCustomerObject(randNum);
            GAMESTATE = State.WaitToSpawnCustomer;
            return;
        }
        else
        {
            stallIsTaken = true;
        }

        if (stallIsTaken)
        {
            // stall is occupied, reroll
            print("stall occupied! rerolling");
            CustomerChooseStall();
            return;
        }
    }

    void SpawnCustomerObject(int randNum)
    {
        // instantiate the prefab
        Customer newCust = Instantiate(customerPrefab).GetComponent<Customer>();
        newCust.gameObject.name = "Stall #" + (randNum + 1);
        newCust.transform.SetParent(stallPositions[randNum].transform);
        newCust.transform.localPosition = Vector3.zero;
        newCust.transform.localScale = Vector3.one;
        customerQueue.Add(newCust);

        // assign the stall and position to start the customer
        newCust.stallNumber = randNum;
        newCust.Initialize(stageData);

    }

    // feedbacks

    void OrderSuccessful(Customer cust, Kue order)
    {
        // add the points
        // _customersDone++;
        cust.OrderDone(order);
        cust._timeWaiting -= 5f; // time bonus
        print("order done!");
    }

    void DismissCustomer(Customer cust)
    {
        // add the points
        _customersDone++;
        customerQueue.Remove(cust);
        cust.LeaveQueue();

        UpdateScoreBoard();

        print("customer satisfied!");
    }

    public void CustomerCancelled(Customer cust)
    {
        customerQueue.Remove(cust);
        cust.LeaveQueue();
    }

    public void RestartGame()
    {
        // add penalty when retrying
        GameManager.Instance._penalty++;
        GameManager.Instance.LoadGame(stageData);
    }

    public void Exit()
    {
        GameManager.Instance.UnlockStagesFromDataObj(stageData);
        GameManager.Instance.OpenStageSelect();
    }

    // UI

    void ShowVictoryScreen()
    {
        gameIsRunning = false;
        victoryScreen.SetActive(true);
    }

    void ShowDefeatScreen()
    {
        gameIsRunning = false;
        retryScreen.SetActive(true);
    }

    void UpdateScoreBoard()
    {
        scoreBoard.text = "Goal: " + _customersDone + "/" + customersGoal;
    }

    void UpdateGameTimer()
    {
        float timeLeft = timeLimit - _timerStage;
        gameTimer.text = timeLeft.ToString("F0");
    }


    // inputs

    public void StartBaking(BuatAdonan maker)
    {
        // got the cake data, put it in baking if there is empty spot
        int emptySpotPos = 0;
        bool foundVacancy = false;
        for (int index = 0; index < bakingQueue.Count; index++)
        {
            if (!bakingQueue[index].kue)
            {
                foundVacancy = true;
                emptySpotPos = index;
                break;
            }
        }

        if (foundVacancy)
        {
            // move the cake to ready spot and remove from baking spot
            PindahkanAdonan bakingSpot = bakingQueue[emptySpotPos];
            Kue cake = maker.MakeNewCake();

            cake.transform.position = bakingSpot.transform.position;
            bakingSpot.BeginBaking(cake);
        }
    }

    public void MoveCakeToTable(Kue cake, PindahkanAdonan bakingSpot)
    {
        // cake done baking, try move it to the table if there is empty spot
        int emptySpotPos = 0;
        bool foundVacancy = false;
        for (int index = 0; index < readyCakesQueue.Count; index++)
        {
            if (!readyCakesQueue[index].kue)
            {
                foundVacancy = true;
                emptySpotPos = index;
                break;
            }
        }

        // move the cake to ready spot and remove from baking spot
        if (foundVacancy)
        {
            KueJadi readySpot = readyCakesQueue[emptySpotPos];
            cake.transform.position = readySpot.transform.position;
            bakingSpot.CakeMoved();
            readySpot.ReceiveBakedCake(cake);
        }
    }

    public void AddTopping(Topping topping)
    {
        // add topping if there is a cake on the table WTIHOUT TOPPING
        int emptySpotPos = 0;
        bool foundVacancy = false;
        for (int index = 0; index < readyCakesQueue.Count; index++)
        {
            if (readyCakesQueue[index].kue)
            {
                if (readyCakesQueue[index].kue.topping == Topping.None)
                {
                    foundVacancy = true;
                    emptySpotPos = index;
                    break;
                }
            }
        }

        if (foundVacancy)
        {
            readyCakesQueue[emptySpotPos].AddTopping(topping);
        }
    }

    public void GiveOrder(Kue cake, KueJadi tableSpot)
    {
        // find any matching orders
        Customer foundCustomer = null;
        Kue foundCake = null;
        // foreach (Customer cust in customerQueue)
        // {
        //     foreach (Kue order in cust.orders)
        //     {
        //         if (order.adonan == cake.adonan && order.topping == cake.topping){
        //             foundCake = order;
        //             foundCustomer = cust;
        //             break;
        //         }
        //     }
        // }

        for (int customerIndex = 0; customerIndex < customerQueue.Count; customerIndex++)
        {
            Customer currCustomer = customerQueue[customerIndex];
            foreach (Kue order in customerQueue[customerIndex].orders)
            {
                if (order.adonan == cake.adonan && order.topping == cake.topping)
                {
                    foundCake = order;
                    foundCustomer = currCustomer;
                    break;
                }
            }
        }

        if (foundCake)
        {
            OrderSuccessful(foundCustomer, foundCake);
            // remove the cake from table spot
            tableSpot.RemoveCake();
            if (foundCustomer.orders.Count <= 0) DismissCustomer(foundCustomer);
        }
        else
        {
            // make some buzzer sound
            print("wrong order!");
        }
    }

    #endregion

}

public enum State
{
    Start,
    WaitToSpawnCustomer,
    Idle,
    SpawnNewCustomer,
    ShopClosed,
    TimesUp,
    Victory,
    TryAgain,
}
