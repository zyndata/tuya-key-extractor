using System;

[Serializable]
public class ExtractedData
{
    public KeyData[] deviceRespBeen;

    [Serializable]
    public class KeyData
    {
        public string localKey; //local key
        public string devId; //device id
        public string name; //device name
    }

}
