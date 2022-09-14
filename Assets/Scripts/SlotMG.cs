using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMG : MonoBehaviour
{
    public SlotData Slot;

    public int MyPrice { get; set; }

    private void Start()
    {
        MyPrice = Slot.Price;
    }

    public GameObject GetFoodType()
    {
        return Slot.FoodType;
    }
}
