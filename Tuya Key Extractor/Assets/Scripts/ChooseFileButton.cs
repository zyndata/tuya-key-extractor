using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

[RequireComponent(typeof(Button))]
public class ChooseFileButton : MonoBehaviour, IPointerDownHandler
{
#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnPointerDown(PointerEventData eventData) {
               UploadFile(gameObject.name, "OnFileUpload", ".xml", false);
    }

    // Called from browser
    public void OnFileUpload(string url) {
        StartCoroutine(ParseController.Instance.OutputRoutine(url));
    }
#else

    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }
    public void OnPointerDown(PointerEventData eventData) { }

    private void OnClick()
    {
        ParseController.Instance.OnChooseFileClicked();
    }
#endif

}
