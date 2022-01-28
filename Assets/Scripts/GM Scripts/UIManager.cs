using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Canvas titleMenu;
    public Canvas gameplay;
    public Canvas winDisplay;
    public Canvas loseDisplay;
    public Canvas pause;
    public Canvas options;
    public Canvas credits;

    private static float creditsBottomPos = 2800;
    private static float creditsTopPos = -1425;
    private float creditsYPos = -500;
    private float creditsScrollRate = 1;

    private void Awake()
    {
        LoadTitleMenu();
    }

    private void Update()
    {
        if (credits.enabled == true)
        {
            ScrollCredits();
        }
        Debug.Log(credits.enabled);
    }

    public void DisableAll()
    {
        titleMenu.enabled = false;
        options.enabled = false;
        credits.enabled = false;
        gameplay.enabled = false;
        winDisplay.enabled = false;
        loseDisplay.enabled = false;
        pause.enabled = false;
    }

    public void LoadTitleMenu()
    {
        DisableAll();
        titleMenu.enabled = true;
    }

    public void LoadGameplay()
    {
        DisableAll();
        gameplay.enabled = true;
    }

    public void LoadWinScreen()
    {
        winDisplay.enabled = true;
    }

    public void LoadLoseScreen()
    {
        loseDisplay.enabled = true;
    }

    public void LoadPauseScreen()
    {
        credits.enabled = false;
        options.enabled = false;
        pause.enabled = true;
    }

    public void LoadOptions()
    {
        options.enabled = true;
    }

    public void LoadCredits()
    {
        DisableAll();
        credits.enabled = true;
    }

    private void ScrollCredits() 
    {
        int childOrderNum = 1;
        credits.transform.GetChild(childOrderNum).transform.position = new Vector3(credits.transform.GetChild(childOrderNum).transform.position.x, creditsYPos, credits.transform.GetChild(childOrderNum).transform.position.z);

        if (credits.transform.GetChild(childOrderNum).transform.position.y >= creditsBottomPos) { creditsYPos = creditsTopPos; }
        else if (credits.transform.GetChild(childOrderNum).transform.position.y <= creditsBottomPos && credits.transform.GetChild(childOrderNum).transform.position.y >= creditsTopPos)
        {
            creditsYPos = creditsYPos += Time.deltaTime * creditsScrollRate;
        }
    }
}
