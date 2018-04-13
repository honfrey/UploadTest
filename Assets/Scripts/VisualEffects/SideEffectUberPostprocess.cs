using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideEffectUberPostprocess : MonoBehaviour
{
    [SerializeField]
    private Shader sideEffectShader;         // Unity won't include this shader in build using Shader.Find() so I added a reference
    private Material m_mat;
    private int m_saturateShaderID;
    private int m_blinkShaderID;
    private float m_saturateVal = 1.0f;
    private float m_blinkVal = 0.0f;

    // Use this for initialization
    void Start()
    {
        //Shader sideEffectShader = Shader.Find("PPShader/SiderEffectUberPPShader");

        if(sideEffectShader != null)
        {
            m_mat = new Material(sideEffectShader);
            m_saturateShaderID = Shader.PropertyToID("_Saturation");
            m_blinkShaderID = Shader.PropertyToID("_EyeBlinkVal");
        }
        else
        {
            Debug.LogError("No SiderEffectUberPPShader found.");
        }

	}

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(m_mat != null)
        {
            m_mat.SetFloat(m_saturateShaderID, m_saturateVal);
            m_mat.SetFloat(m_blinkShaderID, m_blinkVal);

            Graphics.Blit(source, destination, m_mat);

        }
    }

    public void UpdateSaturateValue(float saturateVal)
    {
        m_saturateVal = saturateVal;
    }

    public void ResetSaturateValue()
    {
        m_saturateVal = 1.0f;
    }

    public void UpdateBlinkValue(float blinkVal)
    {
        m_blinkVal = blinkVal;
    }

    public void ResetBlinkValue()
    {
        m_blinkVal = 0.0f;
    }

   
}
