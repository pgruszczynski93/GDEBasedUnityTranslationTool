using UnityEngine;
using GameDataEditor;
using ForExperienceTools;

public class LanguageController : MonoBehaviour{

    static LanguageController _instance = null;

    public Languages language = Languages.POLISH;

    public static LanguageController Instance
    {
        get {
            return _instance;
        }
    }
        
    public void MakeSingleInstance()
    {
        if(_instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        GDEDataManager.Init("gde_data_enc", true);
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Awake()
    {
        MakeSingleInstance();
    }

    public void TranslateOnDemand(int lang)
    {
        Instance.language = (Languages)lang;
        ChangeLanguageVersion[] translatables = FindObjectsOfType<ChangeLanguageVersion>();
        for(int i=0; i<translatables.Length; i++)
        {
            translatables[i].ChangeLanguageOnDemand();
        }
    }

}
