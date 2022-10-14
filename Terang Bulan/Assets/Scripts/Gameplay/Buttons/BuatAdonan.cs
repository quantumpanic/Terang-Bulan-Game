using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuatAdonan : MonoBehaviour
{
    public Adonan thisAdonan;
    public Kue cakeBeingMade;

    public void StartBaking()
    {
        Gameplay.Instance.StartBaking(this);
    }

    public Kue MakeNewCake()
    {
        // instantiate the cake being made
        Kue cakeBeingMade = Instantiate(Gameplay.Instance.cakePrefab).GetComponent<Kue>();
        cakeBeingMade.transform.SetParent(GameObject.Find("Canvas").transform, false);

        // bake the cake based on flavor
        cakeBeingMade.adonan = thisAdonan;

        // set the color
        Image scrpt = cakeBeingMade.GetComponent<Image>();
        Color newCol;
        switch (thisAdonan)
        {
            case Adonan.Biasa:
                ColorUtility.TryParseHtmlString("#FFF274", out newCol);
                scrpt.color = newCol;
                break;
            case Adonan.Coklat:
                ColorUtility.TryParseHtmlString("#9E5340", out newCol);
                scrpt.color = newCol;
                break;
            case Adonan.Matcha:
                ColorUtility.TryParseHtmlString("#97E0A1", out newCol);
                scrpt.color = newCol;
                break;
        }

        // send the cake data to gameplay manager
        return cakeBeingMade;
    }
}
