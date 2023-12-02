using System.Collections;
using System.IO;
using System.Xml;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ParseController : MonoBehaviour
{
	private readonly string CHECK_START = "{\"deviceRespBeen\"";

	[SerializeField] private RectTransform inputTextContainer;
	[SerializeField] private RectTransform resultContainer;
	[SerializeField] private TextMeshProUGUI inputFileMesh;
	[SerializeField] private TextMeshProUGUI warningMesh;
	[SerializeField] private GameObject entryPrefab;
	[SerializeField] private GameObject inputView;
	[SerializeField] private GameObject resultView;
	[SerializeField] private Button extractButton;

	private ExtractedData data;

	#region Singleton

	private static ParseController instance;

	public static ParseController Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<ParseController>();
			}

			return instance;
		}
	}

	#endregion

	private void Awake ()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	private void Start ()
	{
		Application.targetFrameRate = 30;
		ResetView();
	}
	
	public void OnBackClicked ()
	{
		ResetView();
	}

	public void OnExtractClicked ()
	{
		DisplayExtractedData(data);
	}
	
	//standalone and editor
	public void OnChooseFileClicked ()
	{
		ExtensionFilter[] extensions = new[] {new ExtensionFilter("Txt Files", "xml")};
		string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

		if (paths != null && paths.Length > 0)
		{
			StartCoroutine(OutputRoutine(paths[0]));
		}
	}

	public IEnumerator OutputRoutine (string url)
	{
		UnityWebRequest loader = new(url);
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

		XmlDocument doc = new();
		string xmlData = loadedFile;
		doc.Load(new StringReader(xmlData));
		string loadedXml = doc.InnerText;

		inputFileMesh.text = loadedXml; //show loaded file in window

		LayoutRebuilder.ForceRebuildLayoutImmediate(inputTextContainer); //make sure scroll bar appear

		if (IsEmpty(loadedXml) == false)
		{
			string cleanText = GetTrimmedText(loadedXml);

			// if (IsXmlValid(cleanText) == false)
			if (IsXmlValid(cleanText) == false)
			{
				SetWarningMessage("Xml do not look valid");
			}
			else
			{
				string json = ExtractJsonObject(cleanText);
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
					extractButton.interactable = true;
				}
			}
		}
		else
		{
			SetWarningMessage("This file looks empty");
		}
	}

	private string ReadFileStandalone (string path)
	{
		string textFileContent = string.Empty;
		StreamReader reader = new(path);
		textFileContent = reader.ReadToEnd();
		reader.Close();

		if (IsEmpty(textFileContent) == true)
		{
			SetWarningMessage("This file looks empty");
		}

		return textFileContent;
	}

	private void DisplayExtractedData (ExtractedData data)
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
		LayoutRebuilder.ForceRebuildLayoutImmediate(resultContainer); //make sure scroll bar appear
	}
	
	private void ResetView ()
	{
		SetWarningMessage(string.Empty);
		inputFileMesh.text = string.Empty;
		resultContainer.transform.ClearChildren();
		inputView.SetActiveOptimized(true);
		resultView.SetActiveOptimized(false);
		extractButton.interactable = false;
	}

	private void SetWarningMessage (string message)
	{
		warningMesh.text = message;
	}

	private string GetTrimmedText (string inputText)
	{
		return inputText.Trim();
	}

	private string ExtractJsonObject (string mixedString)
	{
		int startIndex = mixedString.IndexOf(CHECK_START);
		string rawJson = mixedString.Remove(0, startIndex);

		return rawJson;
	}

	private bool IsEmpty (string text)
	{
		if (string.IsNullOrEmpty(text) == true)
		{
			return true;
		}

		return false;
	}

	private bool IsXmlValid (string text)
	{
		if (text.Contains(CHECK_START) == true)
		{
			return true;
		}

		return false;
	}
}
