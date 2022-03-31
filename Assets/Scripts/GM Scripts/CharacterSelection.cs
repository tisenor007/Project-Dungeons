using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    [HideInInspector] public bool isMale = true;
    [SerializeField] private GameObject maleDisplay;
    [SerializeField] private GameObject femaleDisplay;
    [SerializeField] private Text characterNameText;

    private enum Character
    {
        Captain_Pi,
        Ratspirilla
    }

    void Start()
    {
        isMale = true;
        maleDisplay.SetActive(isMale);
        femaleDisplay.SetActive(!isMale);
        SetPlayerModel();
    }

    void Update()
    {
        
        if (isMale) characterNameText.text = Character.Captain_Pi.ToString();
        else characterNameText.text = Character.Ratspirilla.ToString();
    }


    public void ChangeCharacter()
    {
        isMale = !isMale;
        maleDisplay.SetActive(isMale);
        femaleDisplay.SetActive(!isMale);
        SetPlayerModel();
        SoundManager.PlaySound(SoundManager.Sound.Clang);
        Debug.LogError("CLANG");
    }


    public void SetPlayerModel()
    {
        GameManager.manager.SwitchPlayer(isMale);
        if (isMale) characterNameText.text = Character.Captain_Pi.ToString();
        else characterNameText.text = Character.Ratspirilla.ToString();
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
