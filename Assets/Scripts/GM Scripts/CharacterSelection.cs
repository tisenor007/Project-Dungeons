using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    [HideInInspector] public bool isMale = true;
    [SerializeField] private GameObject maleDisplay;
    [SerializeField] private GameObject femaleDisplay;
    [SerializeField] private GameObject backgroundDisplay;
    [SerializeField] private Text characterNameText;
    private static string maleName = "Captain Pi";
    private static string femaleName = "Ratspirilla";

    void Start()
    {
        isMale = true;
        maleDisplay.SetActive(isMale);
        femaleDisplay.SetActive(!isMale);
        SetPlayerModel();
    }

    void Update()
    {
        
        if (isMale) characterNameText.text = maleName;
        else characterNameText.text = femaleName;
    }


    public void ChangeCharacter()
    {
        isMale = !isMale;
        maleDisplay.SetActive(isMale);
        femaleDisplay.SetActive(!isMale);
        SetPlayerModel();
        SoundManager.PlaySound(SoundManager.Sound.Clang);
        //Debug.LogError("CLANG");
    }


    public void SetPlayerModel()
    {
        GameManager.manager.SwitchPlayer(isMale);
        if (isMale) characterNameText.text = maleName;
        else characterNameText.text = femaleName;
    }


    public void ShowModels()
    {
        maleDisplay.SetActive(isMale);
        femaleDisplay.SetActive(!isMale);
        backgroundDisplay.SetActive(true);
    }


    public void HideModels()
    {
        maleDisplay.SetActive(false);
        femaleDisplay.SetActive(false);
        backgroundDisplay.SetActive(false);
    }
}
