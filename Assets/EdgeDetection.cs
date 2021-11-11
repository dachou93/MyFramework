using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-----------------------------【边缘检测】-----------------------------
public class EdgeDetection : MonoBehaviour
{

    public Shader edgeDetectShader;
    private Material edgeDetectMaterial = null;
    public Material material
    {
        get
        {
            if(edgeDetectMaterial == null)
                edgeDetectMaterial = new Material(edgeDetectShader);
            return edgeDetectMaterial;
        }
    }
    [Range(0.0f, 1.0f)]
    public float edgesOnly = 0.0f;
    //[ColorUsageAttribute(true, true)]
    public Color edgeColor = Color.black;

    //[ColorUsageAttribute(true, true)]
    public Color edgeColor1 = Color.black;


    public Color backgroundColor = Color.white;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null)
        {
            material.SetFloat("_EdgeOnly", edgesOnly);
            //material.SetColor("_EdgeColor", edgeColor);
            //material.SetColor("_EdgeColor1", edgeColor1);
            material.SetColor("_BackgroundColor", backgroundColor);
            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
