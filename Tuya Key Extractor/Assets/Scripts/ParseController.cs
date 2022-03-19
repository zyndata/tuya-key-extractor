using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System;

public class ParseController : MonoBehaviour
{
    private readonly string CHECK_START = "{\"deviceRespBeen\"";
    private readonly string CHECK_END = "</string></map>";

    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI warningMesh;
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private GameObject inputView;
    [SerializeField] private GameObject resultView;
    [SerializeField] private Transform entryContainer;
    [Space(10)]
    [SerializeField] private ExtractedData data; //serialized only for testing inside editor


    private void Start()
    {
        ResetView();
        inputField.text = string.Empty;
    }

    private void ResetView()
    {
        SetWarningMessage(string.Empty);
        entryContainer.ClearChildren();
        inputView.SetActiveOptimized(true);
        resultView.SetActiveOptimized(false);
    }

    public void OnBackClicked()
    {
        ResetView();
    }

    public void OnExtractClicked()
    {
        SetWarningMessage(string.Empty);

        string input = inputField.text;
        //Debug.Log("input field " + input);

        if (IsEmpty(input) == true)
        {
            SetWarningMessage("Paste xml data first");
            return;
        }

        string cleanText = CleanupInputText(inputField.text);

        if (IsXmlValid(cleanText) == false)
        {
            SetWarningMessage("Xml do not look valid");
            return;
        }

        string json = ExtractJsonObject(cleanText);
        // Debug.Log(json);

        data = JsonUtility.FromJson<ExtractedData>(json);

        if (data == null)
        {
            SetWarningMessage("Error parsing json");
            return;
        }
        else if (data.deviceRespBeen == null || data.deviceRespBeen.Length == 0)
        {
            SetWarningMessage("No linked device found");
            return;
        }

        DisplayExtractedData(data);
    }

    private void DisplayExtractedData(ExtractedData data)
    {
        for (int i = 0; i < data.deviceRespBeen.Length; i++)
        {
            GameObject entryObject = Instantiate(entryPrefab, entryContainer);
            entryObject.name = "Device " + i;
            EntryRow entryScript = entryObject.GetComponent<EntryRow>();
            entryScript.SetData(i + 1, data.deviceRespBeen[i]);
        }

        inputView.SetActiveOptimized(false);
        resultView.SetActiveOptimized(true);
    }

    private void SetWarningMessage(string message)
    {
        warningMesh.text = message;
    }

    private string CleanupInputText(string cleanText)
    {
        cleanText = cleanText.Trim();
        cleanText = cleanText.Replace("&quot;", "\"");
        cleanText = cleanText.Replace("& quot;", "\"");
        cleanText = Regex.Replace(cleanText, @"\r\n?|\n", string.Empty);
        return cleanText;
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
