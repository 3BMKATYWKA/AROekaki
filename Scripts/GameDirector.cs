using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    GameObject tapConditionText;
    string tapCondition = "tapCondition:::No Tap";

    public void Tapping()
    {
        tapCondition = "tapCondition:::Tapping!!!";
    }
    public void NoTapping()
    {
        tapCondition = "tapCondition:::No Tap";
    }


    // Start is called before the first frame update
    void Start()
    {
        this.tapConditionText = GameObject.Find("TapCondition");
    }

    // Update is called once per frame
    void Update()
    {
        this.tapConditionText.GetComponent<Text>().text = tapCondition;
    }
}
