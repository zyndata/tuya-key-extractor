using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntryRow : MonoBehaviour
{
    private readonly string NAME_FORMAT = "{0}. {1}";

    [SerializeField] private TextMeshProUGUI deviceNameMesh;
    [SerializeField] private TMP_InputField deviceIdMesh;
    [SerializeField] private TMP_InputField localKeyMesh;

    public void SetData(int index, ExtractedData.KeyData data)
    {
        deviceNameMesh.text = string.Format(NAME_FORMAT, index, data.name);
        deviceIdMesh.text = data.devId;
        localKeyMesh.text = data.localKey;
    }


}
