Shader "Transparent/Double-Sided Vertex Lit" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }
 
    SubShader {
        Tags {"RenderType"="Transparent" "Queue"="Transparent"}
        // Render into depth buffer only
        Pass {
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB
            Cull off
            Material {
                Diffuse [_Color]
                Ambient [_Color]
            }
            Lighting On
            SetTexture [_MainTex] {
                Combine texture * primary DOUBLE, texture * primary
            }
        } 
    }
}