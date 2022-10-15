using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    private Image sprite;
    public bool isWaiting;
    public float _timeWaiting;
    public float maxTimeWaiting = 15;

    public int stallNumber;

    public List<Kue> orders = new List<Kue>();

    public void Initialize(StageDataObject dataObject)
    {
        // start the waiting time, sprite, animation, etc
        maxTimeWaiting = dataObject.custSpwnTimeBase + 15;

        // randomize number of orders (1-2)
        int randOrder;
        randOrder = Random.Range(0, 2);

        // random number before start the loop
        int randNum;

        for (int i = 0; i <= randOrder; i++)
        {
            Kue newCake = gameObject.AddComponent<Kue>();


            // randomize cake flavor
            randNum = Random.Range(0, 3);
            newCake.adonan = (Adonan)randNum;

            // randomize topping
            randNum = Random.Range(0, 4);
            newCake.topping = (Topping)randNum;

            orders.Add(newCake);
        }

        ShowOrderBubble();
        SetRandomSprite();
    }

    void SetRandomSprite()
    {
        Image spriteScrpt = transform.GetChild(0).GetComponent<Image>();
        int randNum = Random.Range(0, 6);
        spriteScrpt.sprite = Resources.LoadAll<Sprite>("Sprites/Jelly_Colored")[randNum];
    }

    private void Update()
    {
        // leave if wait too long
        _timeWaiting += Time.deltaTime;
        if (_timeWaiting >= maxTimeWaiting) CancelOrder();
    }

    public void OrderDone(Kue cake)
    {
        orders.Remove(cake);
        Destroy(cake);

        // update the bubble
        ShowOrderBubble();

        // play sfx
        Gameplay.Instance.PlaySfxOrderDone();
    }

    void CancelOrder()
    {
        // tell game to remove self from queue
        Gameplay.Instance.CustomerCancelled(this);

        // play sfx
        Gameplay.Instance.PlaySfxCustomerLeft();
    }

    public void LeaveQueue()
    {
        // do animation then destroy
        Destroy(gameObject);
    }

    public List<Image> orderBubbleList = new List<Image>();

    void ShowOrderBubble()
    {
        orderBubbleList[0].gameObject.SetActive(false);
        orderBubbleList[1].gameObject.SetActive(false);

        // show the orders on the customer head
        for (int orderNum = 0; orderNum < orders.Count; orderNum++)
        {
            Kue cake = orders[orderNum];
            Image bubble = orderBubbleList[orderNum];

            // set the cake color
            Color newCol;
            Image cakeImg = bubble.GetComponent<Image>();
            switch (cake.adonan)
            {
                case Adonan.Biasa:
                    ColorUtility.TryParseHtmlString("#FFF274", out newCol);
                    cakeImg.color = newCol;
                    break;
                case Adonan.Coklat:
                    ColorUtility.TryParseHtmlString("#9E5340", out newCol);
                    cakeImg.color = newCol;
                    break;
                case Adonan.Matcha:
                    ColorUtility.TryParseHtmlString("#97E0A1", out newCol);
                    cakeImg.color = newCol;
                    break;
            }

            // set the topping color
            Image toppingImg = bubble.transform.GetChild(0).GetComponent<Image>();
            switch (cake.topping)
            {
                case Topping.None:
                    ColorUtility.TryParseHtmlString("#00000000", out newCol);
                    toppingImg.color = newCol;
                    break;
                case Topping.Keju:
                    ColorUtility.TryParseHtmlString("#D19F1C", out newCol);
                    toppingImg.color = newCol;
                    break;
                case Topping.Coklat:
                    ColorUtility.TryParseHtmlString("#610707", out newCol);
                    toppingImg.color = newCol;
                    break;
                case Topping.Kacang:
                    ColorUtility.TryParseHtmlString("#F1B187FF", out newCol);
                    toppingImg.color = newCol;
                    break;
            }

            // initially inactive, activate it
            bubble.gameObject.SetActive(true);
        }
    }
}

public enum Adonan
{
    Biasa,
    Matcha,
    Coklat
}

public enum Topping
{
    None,
    Coklat,
    Keju,
    Kacang
}
