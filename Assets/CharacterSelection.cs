using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private GameObject maleDisplay;
    [SerializeField] private GameObject femaleDisplay;
    private bool isMale = true;

    private void Awake()
    {
        maleDisplay.SetActive(isMale);
        femaleDisplay.SetActive(!isMale);
    }

    public void ChangeDisplayCharacter()
    {
        if (isMale)
        {
            isMale = false;
            
        } else
        {
            isMale = true;
        }
        maleDisplay.SetActive(isMale);
        femaleDisplay.SetActive(!isMale);
    }

    public void ChangePlayerModel()
    {
        GameManager.manager.SwitchPlayer(isMale);
    }
}
