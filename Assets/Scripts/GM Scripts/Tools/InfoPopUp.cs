using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopUp : MonoBehaviour
{
    public Text displayedTxt;

    private Color textColour;
    private float activeDuration = 0.5f; // after duration popup will start fading
    private float fadeSpeed = 3;
    private float yMoveSpeed = 2;

    // Update is called once per frame
    void Update()
    {
        
    }
    private void LateUpdate()
    {
        transform.LookAt(transform.position + GameManager.manager.playerAndCamera.transform.GetChild(1).forward);

        if (displayedTxt != null) // text exists
        {
            transform.position += new Vector3(0f, yMoveSpeed * Time.deltaTime, 0);

            activeDuration -= Time.deltaTime;

            if (activeDuration <= 0) // time left to be alive expires
            {
                textColour.a -= fadeSpeed * Time.deltaTime;
                displayedTxt.color = textColour;

                if (textColour.a <= 0) // visibility is gone
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void SetUp(string message, Color color)
    {
        if (displayedTxt != null)
        {
            textColour = color;
            displayedTxt.text = message;
            displayedTxt.color = textColour;
        }
    }
}
