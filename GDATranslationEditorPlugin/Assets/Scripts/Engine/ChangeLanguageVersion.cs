using UnityEngine;
using UnityEngine.UI;
using GameDataEditor;
using ForExperienceTools;

public class ChangeLanguageVersion : MonoBehaviour {

    protected GDELanguageVersionsData languageVersion;

    Text textField;

    protected virtual void Start ()
    {
        textField = GetComponent<Text>();
        ChangeLanguage();
	}

	protected virtual void ChangeLanguage()
    {
        switch (LanguageController.Instance.language)
        {
            case Languages.POLISH:
                {
                    languageVersion = new GDELanguageVersionsData(gameObject.name);
                    textField.text = languageVersion.POLISH;
                }
                break;
            case Languages.ENGLISH:
                {
                    languageVersion = new GDELanguageVersionsData(gameObject.name);
                    textField.text = languageVersion.ENGLISH;
                }
                break;
            case Languages.GERMAN:
                {
                    languageVersion = new GDELanguageVersionsData(gameObject.name);
                    textField.text = languageVersion.GERMAN;
                }
                break;
            case Languages.SPANISH:
                {
                    languageVersion = new GDELanguageVersionsData(gameObject.name);
                    textField.text = languageVersion.SPANISH;
                }
                break;
            default:
                break;
        }
    }

    public void ChangeLanguageOnDemand()
    {
        ChangeLanguage();
    }
}
