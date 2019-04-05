Shader "PostEffect/contranst"
{
	Properties
	{
		_MainTex("MainTex", 2D) = ""{}
		_Contrast("Contrast", Range(1, 20)) = 10

		_AffectRange("AffectRange", Range(0, 1)) = 0

	}

	SubShader
	{
		Pass{
		CGPROGRAM
		
		#include "UnityCg.cginc"
		#pragma vertex vert_img
		#pragma fragment frag

		sampler2D _MainTex;
		float _Contrast;
		float _AffectRange;


		float _OffsetX;
		float _OffsetY;
	
		float4 frag(v2f_img i) : 
		COLOR{

			float4 c = tex2D(_MainTex, i.uv);
			if (i.uv.y > _AffectRange) 
				c = 1 / (1 + exp(-_Contrast * (c - .5)));
	

			return c;
			}

		ENDCG
		}
	}
}
