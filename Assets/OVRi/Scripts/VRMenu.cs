/************************************************************************************

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.2 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculusvr.com/licenses/LICENSE-3.2

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using VR = UnityEngine.VR;

//-------------------------------------------------------------------------------------
/// <summary>
/// Shows debug information on a heads-up display.
/// </summary>
public class VRMenu : MonoBehaviour
{
    [SerializeField] private Material m_NormalMaterial0;
    [SerializeField] private Material m_NormalMaterial1;
    [SerializeField] private Material m_NormalMaterial2;
    [SerializeField] private Material m_NormalMaterial0a;
    [SerializeField] private Material m_NormalMaterial1a;
    [SerializeField] private Material m_NormalMaterial2a;

    #region GameObjects for Debug Information UIs   
    GameObject VRMenuManager;
    GameObject debugUIObject;
    GameObject riftPresent;    
    GameObject caption;
    GameObject tex0;
    GameObject tex1;
    GameObject tex2;
    GameObject tex0preview;
    GameObject tex1preview;
    GameObject tex2preview;
    GameObject ok;
    GameObject texts;
    #endregion

    #region Menu strings
    string strRiftPresent   = null; // "VR DISABLED"
    string strCaption       = null; // "Title";
    string strOK            = null; // "OK :P";
    string strTex0          = null; // "Tex 0: Red";
    string strTex1          = null; // "Tex 1: Green";
    string strTex2          = null; // "Tex 2: Blue";
    #endregion

    /// <summary>
    /// Managing for UI initialization
    /// </summary>
    bool  initUIComponent = false;
    bool  isInited        = false;

    /// <summary>
    /// UIs Y offset
    /// </summary>
    float offsetY = 55.0f;

    /// <summary>
    /// Managing for rift detection UI
    /// </summary>
    float riftPresentTimeout = 0.0f;

    /// <summary>
    /// Turn on / off VR variables
    /// </summary>
    bool showVRVars = false;

    #region MonoBehaviour handler

    /// <summary>
    /// Initialization
    /// </summary>
    void Awake()
    {
        // Create canvas for using new GUI
        VRMenuManager = new GameObject();
        VRMenuManager.name = "VRMenuManager";
        VRMenuManager.transform.parent = GameObject.Find("LeftEyeAnchor").transform;

        RectTransform rectTransform = VRMenuManager.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100f, 100f);
        rectTransform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        rectTransform.localPosition = new Vector3(-0.05f, 0.19f, 0.53f);
        rectTransform.localEulerAngles = Vector3.zero;

        Canvas canvas = VRMenuManager.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.pixelPerfect = false;
    }

    /// <summary>
    /// Updating VR variables and managing UI present
    /// </summary>
    void Update()
    {
        if (initUIComponent && !isInited)
        {
            InitUIComponents();
        }

        if (GameConfig.showMenu && riftPresentTimeout < 0.0f)
        {
            initUIComponent = true;
            showVRVars = true;
        }
        if (!GameConfig.showMenu)
        {
            showVRVars = false;
        }

        UpdateDeviceDetection();

        // Presenting VR variables
        if (showVRVars)
        {
            VRMenuManager.SetActive(true);
            UpdateVariable();
            UpdateStrings();           
        }
        else
        {
            VRMenuManager.SetActive(false);
        }
    }

    /// <summary>
    /// Initialize isInited value on OnDestroy
    /// </summary>
    void OnDestroy()
    {
        isInited = false;
    }
    #endregion

    #region Private Functions
    /// <summary>
    /// Initialize UI GameObjects
    /// </summary>
    void InitUIComponents()
    {
        float posY = 0.0f;
        int fontSize = 20;

        debugUIObject = new GameObject();
        debugUIObject.name = "VRMenu";
        debugUIObject.transform.parent = GameObject.Find("VRMenuManager").transform;
        debugUIObject.transform.localPosition = new Vector3(0.0f, 100.0f, 0.0f);
        debugUIObject.transform.localEulerAngles = Vector3.zero;
        debugUIObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // Print out for Caption
        if (!string.IsNullOrEmpty(strCaption))
        {
            caption = VariableObjectManager(caption, "Caption", posY -= offsetY, strCaption, fontSize);
        }

        // Print out for Tex0
        if (!string.IsNullOrEmpty(strTex0))
        {
            tex0 = VariableObjectManager(tex0, "Tex 0: Red", posY -= offsetY, strTex0, fontSize);
        }

        // Print out for Tex1
        if (!string.IsNullOrEmpty(strTex1))
        {
            tex1 = VariableObjectManager(tex1, "Tex 1: Green", posY -= offsetY, strTex1, fontSize);
        }

        // Print out for Tex2
        if (!string.IsNullOrEmpty(strTex2))
        {
            tex2 = VariableObjectManager(tex2, "Tex 2: Blue", posY -= offsetY, strTex2, fontSize);
        }

        // Print out for OK
        if (!string.IsNullOrEmpty(strOK))
        {
           ok = VariableObjectManager(ok, "OK", posY -= offsetY, strOK, fontSize);
        }

        initUIComponent = false;
        isInited = true;

    }

    /// <summary>
    /// Update VR Variables
    /// </summary>
    void UpdateVariable()
    {        
        UpdateCaption();
        UpdateTex0();
        UpdateTex1();
        UpdateTex2();
        UpdateOK();
    }

    /// <summary>
    /// Update Strings
    /// </summary>
    void UpdateStrings()
    {
        if (debugUIObject == null)
            return;       
                
        if (!string.IsNullOrEmpty(strCaption))
            caption.GetComponentInChildren<Text>().text = strCaption;

        if (!string.IsNullOrEmpty(strTex0))
            tex0.GetComponentInChildren<Text>().text = strTex0;

        tex0.GetComponentInChildren<Image>().material = m_NormalMaterial0;
        tex1.GetComponentInChildren<Image>().material = m_NormalMaterial1;
        tex2.GetComponentInChildren<Image>().material = m_NormalMaterial2;

        if (GameConfig.currentTex == 0)
        {
            tex0.GetComponentInChildren<Image>().material = m_NormalMaterial0a;
        }

        if (GameConfig.currentTex == 1)
        {
            tex1.GetComponentInChildren<Image>().material = m_NormalMaterial1a;
        }

        if (GameConfig.currentTex == 2)
        {
            tex2.GetComponentInChildren<Image>().material = m_NormalMaterial2a;
        }

        if (!string.IsNullOrEmpty(strTex1))
            tex1.GetComponentInChildren<Text>().text = strTex1;


        if (!string.IsNullOrEmpty(strTex2))
            tex2.GetComponentInChildren<Text>().text = strTex2;

        if (!string.IsNullOrEmpty(strOK))
            ok.GetComponentInChildren<Text>().text = strOK;
    }
	
	/// <summary>
    /// It's for rift present GUI
    /// </summary>
    void RiftPresentGUI(GameObject guiMainOBj)
    {
        riftPresent = ComponentComposition(riftPresent, "rift");
        riftPresent.transform.SetParent(guiMainOBj.transform);
        riftPresent.name = "RiftPresent";
        RectTransform rectTransform = riftPresent.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        rectTransform.localEulerAngles = Vector3.zero;

        Text text = riftPresent.GetComponentInChildren<Text>();
        text.text = strRiftPresent;
        text.fontSize = 20;
    }

    /// <summary>
    /// Updates the device detection.
    /// </summary>
    void UpdateDeviceDetection()
    {
        if (riftPresentTimeout >= 0.0f)
        {
            riftPresentTimeout -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Object Manager for Variables
    /// </summary>
    /// <returns> gameobject for each Variable </returns>
    GameObject VariableObjectManager(GameObject gameObject, string name, float posY, string str, int fontSize)
    {
        gameObject = ComponentComposition(gameObject, name);
        gameObject.name = name;
        gameObject.transform.SetParent(debugUIObject.transform);

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0.0f, posY -= offsetY, 0.0f);

        Text text = gameObject.GetComponentInChildren<Text>();
        text.text = str;
        text.fontSize = fontSize;
        gameObject.transform.localEulerAngles = Vector3.zero;

        rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        return gameObject;
    }

    /// <summary>
    /// Component composition
    /// </summary>
    /// <returns> Composed gameobject. </returns>
    GameObject ComponentComposition(GameObject GO, string name)
    {
        Debug.Log("name: " + name);
        GO = new GameObject();
        GO.AddComponent<RectTransform>();
        GO.AddComponent<CanvasRenderer>();
        GO.AddComponent<Image>();
        GO.GetComponent<RectTransform>().sizeDelta = new Vector2(350f, 50f);

        if (name == "Caption")
        {
            GO.GetComponent<Image>().color = Color.gray;
        } else if (name == "OK")
        {
            GO.GetComponent<Image>().color = new Color(77f / 255f, 77f / 255f, 77f / 255f, 255f / 255f);
        }
        else {
            GO.GetComponent<Image>().color = new Color(7f / 255f, 45f / 255f, 71f / 255f, 200f / 255f);
            GO.transform.position = new Vector3(-175, 0, 0);
            GO.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 50f);
        }

        texts = new GameObject();
        texts.AddComponent<RectTransform>();
        texts.AddComponent<CanvasRenderer>();
        texts.AddComponent<Text>();
        texts.GetComponent<RectTransform>().sizeDelta = new Vector2(350f, 50f);
		texts.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        texts.GetComponent<Text>().color = Color.black;
        texts.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        if (name != "Caption" && name != "OK")
        { 
            texts.GetComponent<Text>().alignment = TextAnchor.MiddleRight;
        }

        if (name == "Tex 0: Red")
        {
            GO.GetComponent<Image>().material = m_NormalMaterial0;
        }

        if (name == "Tex 1: Green")
        {
            GO.GetComponent<Image>().material = m_NormalMaterial1;
        }

        if (name == "Tex 2: Blue")
        {
            GO.GetComponent<Image>().material = m_NormalMaterial2;
        }

        texts.transform.SetParent(GO.transform);
        texts.name = "TextBox";

        return GO;
    }
    #endregion

    #region Debugging variables handler
    /// <summary>
    /// Updates the Caption.
    /// </summary>
    void UpdateCaption()
    {
        strCaption = "Click to Change Item Color";
    }

    void UpdateTex0()
    {
        strTex0 = "Tex 0: Red";
        if (GameConfig.currentTex == 0)
        {
            strTex0 += " (is Active)";
        }
    }

    void UpdateTex1()
    {
        strTex1 = "Tex 1: Green";
        if (GameConfig.currentTex == 1)
        {
            strTex1 += " (is Active)";
        }
    }

    void UpdateTex2()
    {
        strTex2 = "Tex 2: Blue";
        if (GameConfig.currentTex == 2)
        {
            strTex2 += " (is Active)";
        }
    }

    void UpdateOK()
    {
        strOK = "Roll Out = OK ;)";
    }
    #endregion
}
