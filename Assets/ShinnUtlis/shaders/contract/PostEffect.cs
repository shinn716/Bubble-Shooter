using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffect : MonoBehaviour
{

    public Shader shader;
    public Material mat;
    
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if(mat!=null)
            Graphics.Blit(src, dest, this.mat);
    }
}