using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionName : MonoBehaviour
{
    public Text textRef;
    public string textPreVersion = "Version ";
    // Start is called before the first frame update
    void Start()
    {
        if(textRef != null)
        {
            textRef.text = textPreVersion + Application.version;
        }
    }

}
