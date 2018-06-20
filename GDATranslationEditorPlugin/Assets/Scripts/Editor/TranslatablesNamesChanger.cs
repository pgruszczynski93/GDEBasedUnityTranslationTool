#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using ForExperienceTools;

public class TranslatablesNamesChanger : EditorWindow
{

    const string GDE_FIELD_TYPES = "GDE_FIELD_TYPES";
    const string GDE_FIELD_NAMES = "GDE_FIELD_NAMES";
    const string GDE_IGNORE = "GDE_IGNORE";
    const string GDE_IGNORE_VALUE = "";

    string resultPath;
    string projectPath;
    string scenePrefix;
    string projectOutFileName;

    FileStream fileStream;
    StreamWriter writer;
    GameObject[] translatables;
    ChangeLanguageVersion[] translatablesWithComponet;

    TranslationOptions translationOption;
    Extensions extension;
    RenameType renameType;
    GDEFiledTypes fieldTypes;

#region EDITOR_GUI
    [MenuItem("4Experience/Tools/Setup Translations")]
    static void OpenTranslationWindow()
    {
        GetWindow(typeof(TranslatablesNamesChanger), false, "Translation setup");
    }

    void OnGUI()
    {
        DrawToolWindow();
    }

    void DrawToolWindow()
    {
        GUILayout.Label("Select trasnlation type", EditorStyles.boldLabel);
        projectOutFileName = EditorGUILayout.TextField("Output file name: ", projectOutFileName);
        scenePrefix = EditorGUILayout.TextField("Name key prefix: ", scenePrefix);
        translationOption = (TranslationOptions)EditorGUILayout.EnumPopup("Translate object type of: ", translationOption);
        extension = (Extensions)EditorGUILayout.EnumPopup("Select file's extension: ", extension);
        fieldTypes = (GDEFiledTypes)EditorGUILayout.EnumPopup("Select GDE fields type: ", fieldTypes);


        if (GUILayout.Button("Refresh output file"))
        {
            UpdateObjectWithTranslationComponent();
        }

        else if (GUILayout.Button("Confirm"))
        {
            renameType = RenameType.Selection;
            if (string.IsNullOrEmpty(projectOutFileName) || projectOutFileName.Length > 40)
            {
                projectOutFileName = "results";
                EditorUtility.DisplayDialog("Translation plugin message", "Output file name can't be empty or longher than 40 characters. Output file saved as \"results.txt\".\nYou can retype the name of output file via 4Experience/Tools/Setup Translations window.", "OK");
            }
            InvokeSelectWithUpdate();
            AssetDatabase.Refresh();
        }
    }

    void InvokeSelectWithUpdate()
    {
        SetTranslationComponent(translationOption);

        translatablesWithComponet = FindObjectsOfType<ChangeLanguageVersion>();
        if (translatablesWithComponet.Length > 0)
        {
            UpdateObjectWithTranslationComponent();
        }
        ShowFinalMessage();
    }

    void UpdateObjectWithTranslationComponent()
    {
        renameType = RenameType.Update;
        SetTranslationComponent(translationOption);
        AssetDatabase.Refresh();
    }

    void SetTranslationComponent(TranslationOptions options)
    {
        switch (options)
        {
            case TranslationOptions.Text:
                ChangeTranslatableNames<Text, ChangeLanguageVersion>();
                break;
            //case TranslationOptions.VRTKTooltip:
            //    ChangeTranslatableNames<VRTK_ObjectTooltip, VRTKTooltipTranslator>();
            //    break;
            //case TranslationOptions.TMProText:
            //    ChangeTranslatableNames<Text, ChangeLanguageVersion>();
            //    break;
            default:
                Debug.LogError("Unrecognized Option");
                break;
        }
    }
    #endregion

    #region IOOperations
    void SetupReferences()
    {
        Scene scene = SceneManager.GetActiveScene();
        translatables = Selection.gameObjects;

        scenePrefix = (string.IsNullOrEmpty(scenePrefix) ? scene.name.Substring(0, 3) : scenePrefix.Substring(0,3)) +"_"+ scene.buildIndex;
        projectPath = Application.dataPath + "/Translations/EditorToolsData/";
        Directory.CreateDirectory(projectPath);
    }

    void CreateOutputRootDirectory()
    {
        resultPath = projectPath + projectOutFileName + "."+ EnumUtils.GetSelectedEnum<Extensions>((int)extension);
        fileStream = new FileStream(resultPath, FileMode.OpenOrCreate);
        fileStream.SetLength(0);
    }

    void InitTextFile()
    {
        string resultLine = "";
        string[] languages = EnumUtils.GetEnumNames<Languages>();
        int languagesCount = languages.Length;

        resultLine += GDEOutputInitResultLine(GDE_FIELD_NAMES, GDE_IGNORE, languagesCount, "", languages);
        resultLine += GDEOutputInitResultLine(GDE_FIELD_TYPES, "", languagesCount, EnumUtils.GetSelectedEnum<GDEFiledTypes>((int)fieldTypes));
        resultLine += GDEOutputInitResultLine(GDE_IGNORE, "", languagesCount, GDE_IGNORE_VALUE);

        writer.WriteLine(resultLine);
    }

    string GDEOutputInitResultLine(string startString, string endString, int count, string cellValue, string[] array = null)
    {
        string result = "";
        bool getFromArray = (array == null) ? false : true;

        result += startString + "\t";

        if (getFromArray)
        {
            for (int i = 0; i < count; i++)
            {
                result += (array[i] + "\t");
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                result += (cellValue + "\t");
            }
        }

        result += endString + "\t\n";

        return result;
    }

    void SaveSelectedOutput<T, K>() where K: Component
    {
        System.Type translationObjectType = typeof(Text);//(typeof(T) == typeof(Text) ? typeof(Text) : typeof(VRTK_ObjectTooltip));
        GameObject translatable;
        for (int i = 0; i < translatables.Length; i++)
        {
            translatable = translatables[i];
            if (translatable.GetComponent<T>() != null)
            {
                string result = string.Format("{0}_{1}_{2}_{3}", scenePrefix, translatable.transform.parent.name, (typeof(T)).Name, i);
                /*(typeof(T)).Name, (typeof(T) == typeof(Text) ? 
                string.Empty : "_"+go.GetComponent<VRTK_ObjectTooltip>().displayText));*/
                translatable.name = result;
                writer.WriteLine(result + "\t" + translatable.GetComponent<Text>().text/*(typeof(T) == typeof(Text) ? go.GetComponent<Text>().text : go.GetComponent<VRTK_ObjectTooltip>().displayText)*/);
                if (translatable.GetComponent<K>() == null)
                {
                    translatable.AddComponent<K>();
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Translation plugin message", "GameObject " + translatable.name + " doesn't contain " + typeof(T).Name+" component.", "OK");
            }
        }
    }

    void SaveUpdatedOutput<T, K>() where K : Component
    {
        //translatablesWithComponet = FindObjectsOfType<ChangeLanguageVersion>();

        if (translatablesWithComponet.Length > 0)
        {
            ChangeLanguageVersion translatable;
            for (int i = 0; i < translatablesWithComponet.Length; i++)
            {
                translatable = translatablesWithComponet[i];
                string result = string.Format("{0}_{1}_{2}_{3}", scenePrefix, translatable.transform.parent.name, (typeof(T)).Name, i);
                translatable.name = result;
                writer.WriteLine(result + "\t" + translatable.GetComponent<Text>().text/*(typeof(T) == typeof(Text) ? go.GetComponent<Text>().text : go.GetComponent<VRTK_ObjectTooltip>().displayText)*/);
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Translation plugin message", "No tranlatables on this scene. Can't refresh.", "OK");
        }
    }

    void ChangeTranslatableNames<T, K>() where K: Component
    {
        SetupReferences();

        try
        {
            CreateOutputRootDirectory();
            writer = new StreamWriter(fileStream);
            InitTextFile();
            RenameHierarchyObjs<T,K>();

            writer.Flush();
            writer.Close();
        }
        catch (IOException ioe)
        {
            EditorUtility.DisplayDialog("Translation plugin message", "File save ERROR " + ioe.Message, "OK");
        }

    }

    void RenameHierarchyObjs<T, K>() where K : Component
    {
        switch (renameType)
        {
            case RenameType.Selection:
                RenameSelectedObjects<T, K>();
                break;
            case RenameType.Update:
                RenameUpdatedObjects<T, K>();
                break;
            default:
                break;
        }
    }

    void ShowFinalMessage()
    {
        int objectChanged = (renameType == RenameType.Selection) ? translatables.Length : translatablesWithComponet.Length;
        EditorUtility.DisplayDialog("Translation plugin message", "Operation: "+renameType +", "+ objectChanged+" objects changed. File generated correctly.\nSaved in " + resultPath, "OK");
    }

    void RenameSelectedObjects<T, K>() where K : Component
    {
        if (translatables.Length > 0)
        {
            SaveSelectedOutput<T, K>();
        }
    }

    void RenameUpdatedObjects<T,K>() where K: Component
    {
        SaveUpdatedOutput<T,K>();
    }
    #endregion
}
#endif
