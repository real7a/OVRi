using UnityEngine;
using VRStandardAssets.Utils;

namespace VRStandardAssets.Examples
{
    // This script is a simple example of how an interactive item can
    // be used to change things on gameobjects by handling events.
    public class InteractiveItem : MonoBehaviour
    {
        [SerializeField] private Material m_NormalMaterial0;
        [SerializeField] private Material m_NormalMaterial1;
        [SerializeField] private Material m_NormalMaterial2;
        [SerializeField] private Material m_OverMaterial;                  
        [SerializeField] private VRInteractiveItem m_InteractiveItem;
        [SerializeField] private Renderer m_Renderer;
        private Material m_CurrentNormalMaterial;
        private Material[] m_Materials;

        private void Awake ()
        {
            m_Materials = new[] { m_NormalMaterial0, m_NormalMaterial1, m_NormalMaterial2 };
            m_CurrentNormalMaterial = m_NormalMaterial0;

            m_Renderer.material = m_CurrentNormalMaterial;
        }


        private void OnEnable()
        {
            m_InteractiveItem.OnOver += HandleOver;
            m_InteractiveItem.OnOut += HandleOut;
            m_InteractiveItem.OnClick += HandleClick;
        }


        private void OnDisable()
        {
            m_InteractiveItem.OnOver -= HandleOver;
            m_InteractiveItem.OnOut -= HandleOut;
            m_InteractiveItem.OnClick -= HandleClick;
        }


        //Handle the Over event
        private void HandleOver()
        {
            Debug.Log("Show over state");
            GameConfig.showMenu = true;
            m_Renderer.material = m_OverMaterial;
        }


        //Handle the Out event
        private void HandleOut()
        {
            Debug.Log("Show out state");
            GameConfig.showMenu = false;
            m_Renderer.material = m_CurrentNormalMaterial;
        }


        //Handle the Click event
        private void HandleClick()
        {
            Debug.Log("Show click state");
            GameConfig.currentTex++;
            if (GameConfig.currentTex >= GameConfig.numTex)
            {
                GameConfig.currentTex = 0;
            }
            m_CurrentNormalMaterial = m_Materials[GameConfig.currentTex];
            m_Renderer.material = m_CurrentNormalMaterial;
        }
    }
}