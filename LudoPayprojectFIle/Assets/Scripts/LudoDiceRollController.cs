using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LudoDiceRollController : MonoBehaviour
{
        
    public void Finish_RollDice() 
    {
        gameObject.SetActive(false);
        transform.parent.GetComponent<LudoDiceController>().Dice_RandomValue();
    }
    
}
