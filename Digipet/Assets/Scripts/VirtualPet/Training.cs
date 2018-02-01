using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// This script handles one of the systems of interacting with our character - 
// teaching him to sit/bark/look around.

public class Training : MonoBehaviour {

    public Care FunCare; 
    public Image SitButton;
    public Image BarkButton;
    public Image LookButton;
    public Color32 Normal;
    public Color32 Highlighted;

    private bool _sitting;
    private bool _barking;
    private bool _looking;

    public bool sitting 
    { 
        get
        { 
            return _sitting;
        } 
        set
        {
            _sitting = value;
            if (SitButton != null)
            {
                if (_sitting)
                    SitButton.color = Highlighted;
                else
                    SitButton.color = Normal;
            }
        }
    }

    public bool barking 
    { 
        get
        { 
            return _barking;
        } 
        set
        {
            _barking = value;
            if (BarkButton != null)
            {
                if (_barking)
                    BarkButton.color = Highlighted;
                else
                    BarkButton.color = Normal;
            }
        }
    }

    public bool looking 
    { 
        get
        { 
            return _looking;
        } 
        set
        {
            _looking = value;
            if (LookButton != null)
            {
                if (_looking)
                    LookButton.color = Highlighted;
                else
                    LookButton.color = Normal;
            }
        }
    }

    // chance to make right command in the start of the game.
    // 0- Sit          1- Bark           3-Look
    private float ChanceToSit;
    private float ChanceToBark;
    private float ChanceToLookAround;

    // Use this for initialization
	void Start () 
    {
        // PlayerPrefs.DeleteAll();

        // set default values for all properties
        sitting = false;
        barking = false;
        looking = false;

	    // load from player prefs
        if (PlayerPrefs.HasKey("ChanceSit"))
            ChanceToSit = PlayerPrefs.GetFloat("ChanceSit");
        else
            ChanceToSit = Random.Range(0.35f, 0.50f);

        if (PlayerPrefs.HasKey("ChanceBark"))
            ChanceToBark = PlayerPrefs.GetFloat("ChanceBark");
        else
            ChanceToBark = Random.Range(0.35f, 0.50f);

        if (PlayerPrefs.HasKey("ChanceLook"))
            ChanceToLookAround = PlayerPrefs.GetFloat("ChanceLook");
        else
            ChanceToLookAround = Random.Range(0.35f, 0.50f);
	}
	
    public void SavePlayerPrefs()
    {
        PlayerPrefs.SetFloat("ChanceSit", ChanceToSit);
        PlayerPrefs.SetFloat("ChanceBark", ChanceToBark);
        PlayerPrefs.SetFloat("ChanceLook", ChanceToLookAround);
        PlayerPrefs.Save();
    }

    void OnApplicationQuit()
    {
        SavePlayerPrefs();
    }

    public void SitHandler()
    {
        float chance = Random.Range(0f, 1f);

        if (chance < ChanceToSit)
        {
            // execute right command, change XP
            sitting =!sitting;
            SaveManager.Instance.XP++;
            ChanceToSit += (0.99f - ChanceToSit) / 100f;
            // cahnge fun
            FunCare.AddCare();
        }
        else
        {
            // execute random wrong command
            int wrong = Random.Range(0, 4);
            if (wrong == 0)
            {
                barking = !barking;
                looking = false;
            }
            else if (wrong == 1)
            {
                looking = !looking;
                barking = false;
            }
            // else just do nothing
            DogTalkManager.Instance.Say("I am a smart boy, Am I?", 5);
        }

    }

    public void BarkHandler()
    {
        float chance = Random.Range(0f, 1f);
        if (chance < ChanceToBark)
        {
            // execute right command, change XP
            barking =!barking;
            looking = false;
            SaveManager.Instance.XP++;
            ChanceToBark += (0.99f - ChanceToBark) / 100f;

            // cahnge fun
            FunCare.AddCare();
        }
        else
        {
            // execute random wrong command
            int wrong = Random.Range(0, 4);
            if (wrong == 0)
            {
                sitting = !sitting;
            
            }
            else if (wrong == 1)
            {
                looking = !looking;
                barking = false;
            }
            // else just do nothing
			DogTalkManager.Instance.Say("Just wanted to say Hi!! Wooff!! Wooff!!", 5);
        }

    }

    public void LookHandler()
    {
        float chance = Random.Range(0f, 1f);
        if (chance < ChanceToLookAround)
        {
            // execute right command, change XP
            looking =!looking;
            barking = false;
            SaveManager.Instance.XP++;
            ChanceToLookAround += (0.99f - ChanceToLookAround) / 100f;

            // cahnge fun
            FunCare.AddCare();
        }
        else
        {
            // execute random wrong command
            int wrong = Random.Range(0, 4);
            if (wrong == 0)
            {
                sitting = !sitting;

            }
            else if (wrong == 1)
            {
                barking = !barking;
                looking = false;
            }
            // else just do nothing
            DogTalkManager.Instance.Say("Dont tickle me human!!!", 5);
        }

    }
}
