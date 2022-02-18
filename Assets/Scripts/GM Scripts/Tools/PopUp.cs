using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    public Text displayedTxt;
    private float showDuration;

    void Start()
    {
        //displayedTxt = this.GetComponentInChildren<Text>();
        showDuration = Time.time + 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= showDuration)
        {
            Destroy(this.gameObject);
        }

        transform.LookAt(transform.position + GameManager.manager.playerAndCamera.transform.GetChild(1).forward);
    }

    public void SetUp(string message, Color color)
    {
        if (displayedTxt != null)
        {
            displayedTxt.text = message;
            displayedTxt.color = color;
        }
    }
}
