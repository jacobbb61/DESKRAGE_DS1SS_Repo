using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using FMODUnity;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;

    public bool CanInput = true;

    public Animator SceneTransitionAnim;
    public string ActiveMenu = "Main";

    public EventReference MainMenuMusic;
    public EventReference PressSelect;
    public EventReference PressMove;
    public EventReference PressOk;
    public EventReference PressCancel;
    public FMOD.Studio.EventInstance FMODinstance;


    [Header("Main")]
    public RectTransform MainHightlightPos;
    public Animator MainHightlightAnim;
    private int MainOrder = 1;

    [Header("Character Menu")]
    public GameObject CharacterMenu;
    public RectTransform CharacterMenuHighlight;
    public UICharacterSaveSlot Slot01;
    public UICharacterSaveSlot Slot02;
    public UICharacterSaveSlot Slot03;
    public GameObject NoSlots;
    public Animator CharacterMenuExitAnim;
    private bool CharacterMenuOpen = false;
    private int CharacterMenuOrder = 1;

    public CharacterSlot CurrentSelectedSlot = CharacterSlot.NO_Slot;
    public GameObject DeleteSlotPopUp;

    [Header("Settings")]
    public GameObject Settings; 
    public RectTransform SettingsHightlightPos;
    public Animator SettingsExitAnim;
    private bool SettingsOpen = false;
    private int SettingsOrder;
    public bool HUDActive;
    public bool SubtitlesActive;

    public FMOD.Studio.VCA VcaMasterController;
    public FMOD.Studio.VCA VcaEffectsController;
    public FMOD.Studio.VCA VcaAmbienceController;
    public FMOD.Studio.VCA VcaMusicController;
    public FMOD.Studio.VCA VcaDialogueController;
    
    public float AudioMasterNum;
    public float AudioEffectsNum;
    public float AudioAmbienceNum;
    public float AudioMusicNum;
    public float AudioDialogNum;

    public TextMeshProUGUI HUDText;
    public TextMeshProUGUI SubtitlesText;
    public TextMeshProUGUI AudioMasterText;
    public TextMeshProUGUI AudioEffectsText;
    public TextMeshProUGUI AudioAmbienceText;
    public TextMeshProUGUI AudioMusicText;
    public TextMeshProUGUI AudioDialogText;

    [Header("Achievements")]
    public GameObject Achievements;
    public Animator AchievementsExitAnim;
    private bool AchievementsOpen=false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        WorldSaveGameManager.Instance.LoadAllCharacterSlots();

        ActiveMenu = "Main";
        MainOrder = 1;
        MoveMainHighlight();

        SettingsOpen = false;
        Settings.SetActive(false);

        AchievementsOpen = false;
        Achievements.SetActive(false);


        FMODinstance = FMODUnity.RuntimeManager.CreateInstance(MainMenuMusic);
        FMODinstance.start();
        FMODinstance.release();

        VcaMasterController = FMODUnity.RuntimeManager.GetVCA("vca:/MasterVCA");
        VcaEffectsController = FMODUnity.RuntimeManager.GetVCA("vca:/EffectsVCA");
        VcaAmbienceController = FMODUnity.RuntimeManager.GetVCA("vca:/AmbienceVCA");
        VcaMusicController = FMODUnity.RuntimeManager.GetVCA("vca:/MusicVCA");
        VcaDialogueController = FMODUnity.RuntimeManager.GetVCA("vca:/DialogueVCA");

        UpdateSettings();
        UpdateFMODSettings();
    }
    public void Up(InputAction.CallbackContext context)
    {
        if (context.action.triggered && CanInput == true)
        {
            PlayAudioPressMove();
            CanInput = false;
            Debug.Log("Up Button Pressed");
            switch (ActiveMenu)
            {
                case "Main":
                    MainOrder--;
                    MoveMainHighlight();
                    break;
                case "Settings":
                    SettingsOrder--;
                    MoveSettingsHighlight();
                    break;
                case "CharacterMenu":
                    CharacterMenuOrder--;
                    MoveCharacterMenuHighlight();
                    break;
                default:
                    CanInput = true;
                    break;
            }
        }
    }
    public void Down(InputAction.CallbackContext context)
    {
        if (context.action.triggered && CanInput == true)
        {
            PlayAudioPressMove();
            CanInput = false;
            Debug.Log("Down Button Pressed");
            switch (ActiveMenu)
            {
                case "Main":
                    MainOrder++;
                    MoveMainHighlight();
                    break;
                case "Settings":
                    SettingsOrder++;
                    MoveSettingsHighlight();
                    break;
                case "CharacterMenu":
                    CharacterMenuOrder++;
                    MoveCharacterMenuHighlight();
                    break;
                default:
                    CanInput = true;
                    break;
            }
        }
    }
    public void Left(InputAction.CallbackContext context)
    {
        if (context.action.triggered && CanInput == true)
        {
            PlayAudioPressSelect();
            CanInput = false;
            Debug.Log("Left Button Pressed");
            switch (ActiveMenu)
            {

                case "Settings":
                    ChangeSettings(true);
                    break;
                default:
                    CanInput = true;
                    break;
            }
        }
    }
    public void Right(InputAction.CallbackContext context)
    {
        if (context.action.triggered && CanInput == true)
        {
            PlayAudioPressSelect();
            CanInput = false;
            Debug.Log("Right Button Pressed");
            switch (ActiveMenu)
            {
  
                case "Settings":
                    ChangeSettings(false);
                    break;
                default:
                    CanInput = true;
                    break;
            }
        }
    }

    public void A(InputAction.CallbackContext context)
    {
        if (context.action.triggered && CanInput == true)
        {
            PlayAudioPressOk();
            Debug.Log("A Button Pressed");
            CanInput = false;
            switch (ActiveMenu)
            {             

                case "Main":
                    SelectMain();                   
                    break;
                case "NoSlots":
                    CloseNoSlotsMessage();
                    break;
                case "CharacterMenu":
                    SelectCharacterMenu();
                    break;
                case "DeleteSlot":
                    DeletCharacterSlot();
                    break;
                default:
                    CanInput = true;
                    break;
            }
        }
    }
    public void B(InputAction.CallbackContext context)
    {
        if (context.action.triggered && CanInput == true)
        {
            Debug.Log("B Button Pressed");
            CanInput = false;
            PlayAudioPressCancel();
            switch (ActiveMenu)
            {

                case "Achievements":
                    AchievementsExitAnim.SetTrigger("Active");
                    StartCoroutine(LoadAchievements());
                    break;
                case "Settings":                    
                    GameSaveGameManager.Instance.SaveSettings(HUDActive, SubtitlesActive, AudioMasterNum, AudioEffectsNum, AudioAmbienceNum, AudioMusicNum, AudioDialogNum);
                    SettingsExitAnim.SetTrigger("Active");
                    StartCoroutine(LoadSettings());
                    break;
                case "CharacterMenu":
                    CharacterMenuExitAnim.SetTrigger("Active");
                    CurrentSelectedSlot = CharacterSlot.NO_Slot;
                    StartCoroutine(LoadCharacterMenu());
                    break;
                case "DeleteSlot":
                    CloseDeleteSlot();
                    break;
                default:
                    CanInput = true;
                    break;
            }
        }
    }
    public void Y(InputAction.CallbackContext context)
    {
        if (context.action.triggered && CanInput == true)
        {
            Debug.Log("Y Button Pressed");
            CanInput = false;
            
            switch (ActiveMenu)
            {
                case "CharacterMenu":
                    AttemotToDeletSlot();
                    break;
                default:
                    CanInput = true;
                    break;
            }
        }
    }



    public void SelectMain()
    {
        MainHightlightAnim.SetTrigger("Active");
        switch (MainOrder)
        {
            case 1:
                StartCoroutine(LoadNewGame()); 
                break;

            case 2:
                StartCoroutine(LoadCharacterMenu());
                break;

            case 3:
                StartCoroutine(LoadSettings());
                break;

            case 4:
                StartCoroutine(LoadAchievements());
                break;

            case 5:
               StartCoroutine(LoadCredits());
                break;

            case 6:
                StartCoroutine(QuitGame());
                break;
        }
       
    }


    public void SelectCharacterMenu()
    {

        switch (CharacterMenuOrder)
        {
            case 1:
                if (Slot01.gameObject.activeInHierarchy)
                {
                    Slot01.LoadGameFromCharacterSlot();
                    CurrentSelectedSlot = CharacterSlot.CharacterSlot_01;
                }
                else { CanInput = true; }
                break;

            case 2:
                if (Slot02.gameObject.activeInHierarchy)
                {
                    Slot02.LoadGameFromCharacterSlot();
                    CurrentSelectedSlot = CharacterSlot.CharacterSlot_02;
                }
                else { CanInput = true;  }
                break;

            case 3:
                if (Slot03.gameObject.activeInHierarchy)
                {
                    Slot03.LoadGameFromCharacterSlot();
                    CurrentSelectedSlot = CharacterSlot.CharacterSlot_03;
                }
                else { CanInput = true; }
                break;
        }

    }



    public void MoveMainHighlight()
    {
        switch (MainOrder)
        {
            case 0:
                MainOrder = 6; MoveMainHighlight();
                break;

            case 1: 
                MainHightlightPos.anchoredPosition = new Vector2(0, -20);
                break;

            case 2:
                MainHightlightPos.anchoredPosition = new Vector2(0, -50);
                break;

            case 3:
                MainHightlightPos.anchoredPosition = new Vector2(0, -80);
                break;

            case 4:
                MainHightlightPos.anchoredPosition = new Vector2(0, -110);
                break;

            case 5:
                MainHightlightPos.anchoredPosition = new Vector2(0, -140);
                break;

            case 6:
                MainHightlightPos.anchoredPosition = new Vector2(0, -170);
                break;

            case 7:
                MainOrder = 1; MoveMainHighlight();
                break;
        }
        CanInput = true;
    }

    public void MoveSettingsHighlight()
    {
        switch (SettingsOrder)
        {
            case 0:
                SettingsOrder = 7; MoveSettingsHighlight();
                break;

            case 1:
                SettingsHightlightPos.anchoredPosition = new Vector2(-50, 80);
                break;

            case 2:
                SettingsHightlightPos.anchoredPosition = new Vector2(-50, 55);
                break;

            case 3:
                SettingsHightlightPos.anchoredPosition = new Vector2(-50, -20);
                break;

            case 4:
                SettingsHightlightPos.anchoredPosition = new Vector2(-50, -50);
                break;

            case 5:
                SettingsHightlightPos.anchoredPosition = new Vector2(-50, -80);
                break;

            case 6:
                SettingsHightlightPos.anchoredPosition = new Vector2(-50, -110);
                break;

            case 7:
                SettingsHightlightPos.anchoredPosition = new Vector2(-50, -140);
                break;

            case 8:
                SettingsOrder = 1; MoveSettingsHighlight();
                break;
        }
        CanInput = true;
    }
    public void MoveCharacterMenuHighlight()
    {
        switch (CharacterMenuOrder)
        {
            case 0:
                CharacterMenuOrder = 3; MoveCharacterMenuHighlight();
                break;

            case 1:
                CharacterMenuHighlight.anchoredPosition = new Vector2(0, 150); 
                if (Slot01.gameObject.activeInHierarchy)
                {
                    CurrentSelectedSlot = CharacterSlot.CharacterSlot_01;
                }
                else { CurrentSelectedSlot = CharacterSlot.NO_Slot; }
                break;

            case 2:
                CharacterMenuHighlight.anchoredPosition = new Vector2(0,0);
                if (Slot02.gameObject.activeInHierarchy)
                {
                    CurrentSelectedSlot = CharacterSlot.CharacterSlot_02;
                }
                else { CurrentSelectedSlot = CharacterSlot.NO_Slot; }
                break;

            case 3:
                CharacterMenuHighlight.anchoredPosition = new Vector2(0, -150);
                if (Slot03.gameObject.activeInHierarchy)
                {
                    CurrentSelectedSlot = CharacterSlot.CharacterSlot_03;
                }
                else { CurrentSelectedSlot = CharacterSlot.NO_Slot; }
                break;

            case 4:
                CharacterMenuOrder = 1; MoveCharacterMenuHighlight();
                break;
        }
        CanInput = true;
    }

    public void UpdateSettings()
    {  
        
        HUDActive = GameSaveGameManager.Instance.GameSaveData.HUD;
        SubtitlesActive = GameSaveGameManager.Instance.GameSaveData.Subtitles;

        AudioMasterNum = GameSaveGameManager.Instance.GameSaveData.Master;
        AudioEffectsNum = GameSaveGameManager.Instance.GameSaveData.Effects;
        AudioAmbienceNum = GameSaveGameManager.Instance.GameSaveData.Ambience;
        AudioMusicNum = GameSaveGameManager.Instance.GameSaveData.Music;
        AudioDialogNum = GameSaveGameManager.Instance.GameSaveData.Dialog;

        Debug.Log("Updated settigns");
    }
    public void ChangeSettings(bool Left)
    {
        switch (SettingsOrder)
        {
            case 1:
                HUDActive = !HUDActive;
                break;
            case 2:
                SubtitlesActive = !SubtitlesActive;
                break;
            case 3:
                if (Left) { AudioMasterNum--; } else { AudioMasterNum++; } ;
                break;
            case 4:
                if (Left) { AudioEffectsNum--; } else { AudioEffectsNum++; };
                break;
            case 5:
                if (Left) { AudioAmbienceNum--; } else { AudioAmbienceNum++; };
                break;
            case 6:
                if (Left) { AudioMusicNum--; } else { AudioMusicNum++; };
                break;
            case 7:
                if (Left) { AudioDialogNum--; } else { AudioDialogNum++; };
                break;
            default:
                CanInput = true;
                break;
        }
        AudioMasterNum = Mathf.Clamp(AudioMasterNum, 0, 10);
        AudioEffectsNum = Mathf.Clamp(AudioEffectsNum, 0, 10);
        AudioAmbienceNum = Mathf.Clamp(AudioAmbienceNum, 0, 10);
        AudioMusicNum = Mathf.Clamp(AudioMusicNum, 0, 10);
        AudioDialogNum = Mathf.Clamp(AudioDialogNum, 0, 10);


        CanInput = true;
        UpdateSettingsText();
        UpdateFMODSettings();
    }

    public void UpdateFMODSettings()
    {
        VcaMasterController.setVolume(AudioMasterNum / 10);
        VcaEffectsController.setVolume(AudioEffectsNum / 10);
        VcaAmbienceController.setVolume(AudioAmbienceNum / 10);
        VcaMusicController.setVolume(AudioMusicNum / 10);
        VcaDialogueController.setVolume(AudioDialogNum / 10);
    }
 
    public void UpdateSettingsText()
    {
        if (HUDActive) { HUDText.text = "ON"; } else { HUDText.text = "OFF"; }
        if (SubtitlesActive) { SubtitlesText.text = "ON"; } else { SubtitlesText.text = "OFF"; }
        AudioMasterText.text = AudioMasterNum.ToString();
        AudioEffectsText.text = AudioEffectsNum.ToString();
        AudioAmbienceText.text = AudioAmbienceNum.ToString();
        AudioMusicText.text = AudioMusicNum.ToString();
        AudioDialogText.text = AudioDialogNum.ToString();
    }



    public void OpenNoSlotsMessage()
    {

        NoSlots.SetActive(true);
        ActiveMenu = "NoSlots";
        CanInput = true;
    }
    public void CloseNoSlotsMessage()
    {
        NoSlots.SetActive(false);
        ActiveMenu = "Main";
        CanInput = true;
    }

    public void AttemotToDeletSlot()
    {
        if (CurrentSelectedSlot != CharacterSlot.NO_Slot)
        {
            PlayAudioPressOk();
            DeleteSlotPopUp.SetActive(true);
            ActiveMenu = "DeleteSlot";

            switch (CharacterMenuOrder)
            {
                case 1:
                    DeleteSlotPopUp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150);
                    break;

                case 2:
                    DeleteSlotPopUp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                    break;

                case 3:
                    DeleteSlotPopUp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -150);
                    break;
            }
        }
        else
        {
            PlayAudioPressCancel();
        }
        CanInput = true;
    }
    public void CloseDeleteSlot()
    {
        DeleteSlotPopUp.SetActive(false);
        ActiveMenu = "CharacterMenu";
        CanInput = true;
    }

    public void DeletCharacterSlot()
    {
        switch (CharacterMenuOrder)
        {
            case 1:
                Slot01.gameObject.SetActive(false);
                break;

            case 2:
                Slot02.gameObject.SetActive(false);
                break;

            case 3:
                Slot03.gameObject.SetActive(false);
                break;
        }
        DeleteSlotPopUp.SetActive(false);
        ActiveMenu = "CharacterMenu";
        WorldSaveGameManager.Instance.DeleteGame(CurrentSelectedSlot);
        CanInput = true;
    }

    IEnumerator LoadNewGame()
    {

        Debug.Log("Loading New Game");
        SceneTransitionAnim.SetTrigger("Active");
        yield return new WaitForSeconds(1.2f);
        //SceneManager.LoadScene("End Credits", LoadSceneMode.Single); Load new game scene

        WorldSaveGameManager.Instance.AttemptCreateNewGame();

    }
    IEnumerator LoadCredits()
    {
        FMODinstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        FMODinstance.release();
        Debug.Log("Loading End Credits");
        SceneTransitionAnim.SetTrigger("Active");
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene("End Credits", LoadSceneMode.Single);
    }
    IEnumerator LoadAchievements()
    {
        if (!AchievementsOpen) { ActiveMenu = "Achievements"; } else { ActiveMenu = "Main"; }
        Debug.Log("Loading Achievements");
        SceneTransitionAnim.SetTrigger("Active");
        yield return new WaitForSeconds(1.2f);
        if (!AchievementsOpen) { Achievements.SetActive(true); } else { Achievements.SetActive(false); }
        CanInput = true;
        AchievementsOpen = !AchievementsOpen;
    }
    IEnumerator LoadCharacterMenu()
    {
        if (!CharacterMenuOpen) { ActiveMenu = "CharacterMenu"; } else { ActiveMenu = "Main"; }
        Debug.Log("Loading CharacterMenu");
        SceneTransitionAnim.SetTrigger("Active");
        yield return new WaitForSeconds(1.2f);
        if (!CharacterMenuOpen) { CharacterMenu.SetActive(true); } else { CharacterMenu.SetActive(false); }
        CharacterMenuOrder = 1;
        MoveCharacterMenuHighlight();
        CanInput = true;
        CharacterMenuOpen = !CharacterMenuOpen;
    }
    IEnumerator LoadSettings()
    {

        if (!SettingsOpen) { ActiveMenu = "Settings"; } else { ActiveMenu = "Main"; }
        Debug.Log("Loading Settings");
        SceneTransitionAnim.SetTrigger("Active");
        yield return new WaitForSeconds(1.2f);
        UpdateSettingsText();
        if (!SettingsOpen) { Settings.SetActive(true); } else { Settings.SetActive(false); }
        SettingsOrder = 1;
        CanInput = true;
        SettingsOpen = !SettingsOpen;
    }
    IEnumerator QuitGame()
    {
        FMODinstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        FMODinstance.release();
        GameSaveGameManager.Instance.SaveGameData();
        Debug.Log("Quiting Game");
        SceneTransitionAnim.SetTrigger("Active");
        yield return new WaitForSeconds(1.2f);
        Application.Quit();
    }







    public void PlayAudioPressSelect()
    {
        FMODUnity.RuntimeManager.PlayOneShot(PressSelect);
    }
    public void PlayAudioPressMove()
    {
        FMODUnity.RuntimeManager.PlayOneShot(PressMove);
    }
    public void PlayAudioPressOk()
    {
        FMODUnity.RuntimeManager.PlayOneShot(PressOk);
    }
    public void PlayAudioPressCancel()
    {
        FMODUnity.RuntimeManager.PlayOneShot(PressCancel);
    }
}
