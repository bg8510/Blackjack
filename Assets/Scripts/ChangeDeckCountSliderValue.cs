using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeDeckCountSliderValue : MonoBehaviour
{
    Text deckCount;

    void Start()
    {
        deckCount = GetComponent<Text>();
    }

    public void UpdateDeckCount(float value)
    {
        deckCount.text = value.ToString();
    }
}