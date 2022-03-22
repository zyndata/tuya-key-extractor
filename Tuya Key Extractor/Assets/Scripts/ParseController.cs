using System.Collections;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using SFB;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ParseController : MonoBehaviour
{
    private readonly string CHECK_START = "{\"deviceRespBeen\"";
    private readonly string CHECK_END = "</string></map>";

    [SerializeField] private RectTransform inputTextContainer;
    [SerializeField] private RectTransform resultContainer;
    [SerializeField] private TextMeshProUGUI inputFileMesh;
    [SerializeField] private TextMeshProUGUI warningMesh;
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private GameObject inputView;
    [SerializeField] private GameObject resultView;
    [SerializeField] private Button extractButton;

    private ExtractedData data;


    #region  Singleton

    static ParseController instance;

    public static ParseController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<ParseController>();
            }
            return instance;
        }
    }

    #endregion


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 30;
        ResetView();
    }

    private void ResetView()
    {
        SetWarningMessage(string.Empty);
        inputFileMesh.text = string.Empty;
        resultContainer.transform.ClearChildren();
        inputView.SetActiveOptimized(true);
        resultView.SetActiveOptimized(false);
        extractButton.interactable = false;
    }

    //standalone and editor
    public void OnChooseFileClicked()
    {
        ExtensionFilter[] extensions = new[] { new ExtensionFilter("Txt Files", "xml") };
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

        if (paths != null && paths.Length > 0)
        {
            StartCoroutine(OutputRoutine(paths[0]));
        }
    }


    public IEnumerator OutputRoutine(string url)
    {
        UnityWebRequest loader = new UnityWebRequest(url);
        loader.downloadHandler = new DownloadHandlerBuffer();
        yield return loader.SendWebRequest();

        string loadedFile = string.Empty;

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            loadedFile = loader.downloadHandler.text;
        }
        else
        {
            loadedFile = ReadFileStandalone(url);
            SetWarningMessage(url);
        }

        inputFileMesh.text = loadedFile; //show loaded file in window
        LayoutRebuilder.ForceRebuildLayoutImmediate(inputTextContainer);//make sure scroll bar appear

        bool isEmpty = IsEmpty(loadedFile);
        if (isEmpty == false)
        {
            string cleanText = GetCleanText(loadedFile);

            if (IsXmlValid(cleanText) == false)
            {
                SetWarningMessage("Xml do not look valid");
            }
            else
            {
                string json = ExtractJsonObject(cleanText);
                // Debug.Log(json);

                data = JsonUtility.FromJson<ExtractedData>(json);

                if (data == null)
                {
                    SetWarningMessage("Error parsing json");
                }
                else if (data.deviceRespBeen == null || data.deviceRespBeen.Length == 0)
                {
                    SetWarningMessage("No linked device found");
                }
                else
                {
                    extractButton.interactable = !isEmpty;
                }
            }
        }
        else
        {
            SetWarningMessage("This file looks empty");
        }
    }

    private string ReadFileStandalone(string path)
    {
        string textFileContent = string.Empty;
        StreamReader reader = new StreamReader(path);
        textFileContent = reader.ReadToEnd();
        reader.Close();

        if (IsEmpty(textFileContent) == true)
        {
            SetWarningMessage("This file looks empty");
        }
        return textFileContent;
    }

    public void OnBackClicked()
    {
        ResetView();
    }

    public void OnExtractClicked()
    {
        DisplayExtractedData(data);
    }

    private void DisplayExtractedData(ExtractedData data)
    {
        for (int i = 0; i < data.deviceRespBeen.Length; i++)
        {
            GameObject entryObject = Instantiate(entryPrefab, resultContainer.transform);
            entryObject.name = "Device " + i;
            EntryRow entryScript = entryObject.GetComponent<EntryRow>();
            entryScript.SetData(i + 1, data.deviceRespBeen[i]);
        }

        inputView.SetActiveOptimized(false);
        resultView.SetActiveOptimized(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(resultContainer);//make sure scroll bar appear
    }

    private void SetWarningMessage(string message)
    {
        warningMesh.text = message;
    }

    private string GetCleanText(string inputText)
    {
        inputText = inputText.Trim();
        inputText = inputText.Replace("&quot;", "\"");
        inputText = inputText.Replace("& quot;", "\"");
        inputText = Regex.Replace(inputText, @"\r\n?|\n", string.Empty);
        return inputText;
    }

    private string ExtractJsonObject(string mixedString)
    {
        int startIndex = mixedString.IndexOf(CHECK_START);
        int endIndex = mixedString.IndexOf(CHECK_END);

        int totalLength = mixedString.Length;
        int endLenght = totalLength - endIndex;

        string rawJson = mixedString.Remove(endIndex, endLenght);
        rawJson = rawJson.Remove(0, startIndex);

        return rawJson;
    }

    private bool IsEmpty(string text)
    {
        if (string.IsNullOrEmpty(text) == true)
        {
            return true;
        }
        return false;
    }

    private bool IsXmlValid(string text)
    {
        if (text.Contains(CHECK_END) == true && text.Contains(CHECK_START) == true)
        {
            return true;
        }
        return false;
    }


}
