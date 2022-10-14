using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KueJadi : MonoBehaviour, IPointerClickHandler
{
    public Kue kue;
    public bool cakeHasTopping;

    public void GiveCakeToCustomer()
    {
        if (!kue) return;
        // call the gameplay manager to find the matching customer order
        // remove the cake from this script if correct order
        Gameplay.Instance.GiveOrder(kue, this);
    }

    public void ReceiveBakedCake(Kue cake)
    {
        kue = cake;
    }

    public void AddTopping(Topping topping)
    {
        kue.topping = topping;
        print(kue.topping);

        // set the color
        Image scrpt = kue.transform.GetChild(0).GetComponent<Image>();
        Color newCol;
        switch (topping)
        {
            case Topping.Keju:
                ColorUtility.TryParseHtmlString("#D19F1C", out newCol);
                scrpt.color = newCol;
                break;
            case Topping.Coklat:
                ColorUtility.TryParseHtmlString("#610707", out newCol);
                scrpt.color = newCol;
                break;
            case Topping.Kacang:
                ColorUtility.TryParseHtmlString("#F1B187FF", out newCol);
                scrpt.color = newCol;
                break;
        }
    }

    public void RemoveCake()
    {
        Destroy(kue.gameObject);
        kue = null;
    }

    int tap;

    public void OnPointerClick(PointerEventData eventData)
    {
        tap = eventData.clickCount;

        if (tap == 2)
        {
            // get double click
            if (kue) RemoveCake();
        }

    }
}
