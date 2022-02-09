using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private GameObject maleDisplay;
    [SerializeField] private GameObject femaleDisplay;
    [SerializeField] private Text characterNameText;
    private bool isMale = true;

    private void Awake()
    {
        maleDisplay.SetActive(isMale);
        femaleDisplay.SetActive(!isMale);
        SetPlayerModel();
        if (isMale) characterNameText.text = "Captain Pi";
        else characterNameText.text = "Ratspirilla";
    }

    public void ChangeCharacter()
    {
        isMale = !isMale;
        maleDisplay.SetActive(isMale);
        femaleDisplay.SetActive(!isMale);
        SetPlayerModel();
        if (isMale) characterNameText.text = "Captain Pi";
        else characterNameText.text = "Ratspirilla";
    }

    public void SetPlayerModel()
    {
        GameManager.manager.SwitchPlayer(isMale);
    }

    public void ShowModels()
    {
        maleDisplay.SetActive(isMale);
        femaleDisplay.SetActive(!isMale);
    }

    public void HideModels()
    {
        maleDisplay.SetActive(false);
        femaleDisplay.SetActive(false);
    }
}
