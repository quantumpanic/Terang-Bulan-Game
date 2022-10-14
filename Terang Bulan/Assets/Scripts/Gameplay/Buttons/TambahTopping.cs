using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TambahTopping : MonoBehaviour
{
    public Topping topping;

    public void AddTopping(){
        // add topping to left most cake
        Gameplay.Instance.AddTopping(topping);
    }
}
