Shader "Custom/Gradation" {
	Properties{


		_StartColor("StartColor", Color) = (1, 1, 1, 1)
		_EndColor("EndColor", Color) = (1, 1, 1, 1)
		_Displacement("Displacement", Range(0, 10)) = 0

	}

	SubShader{

		Tags{ "RenderType" = "Transparent" }
		Cull Off ZWrite On Blend SrcAlpha OneMinusSrcAlpha

		Pass{
		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		struct v2f {
		float4 pos : SV_POSITION;
		float3 worldPos : TEXCOORD0;
		};

		float4 _StartColor;
		float4 _EndColor;
		float _Displacement;
			
		v2f vert(appdata_base v)
		{
			v2f o;
			float3 n = UnityObjectToWorldNormal(v.normal);
			o.pos = UnityObjectToClipPos(v.vertex) + float4(n * _Displacement, 0);
			//o.pos = UnityObjectToClipPos(v.vertex);
			o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	
			return o;
		}


		half4 frag(v2f i) : COLOR
		{
			//float4 red = float4(_StartColor / 255,  _StartColor / 255,  _StartColor / 255,1);
			//float4 blue = float4(_EndColor / 255,   _EndColor / 255,    _EndColor / 255,1);
		return lerp(_StartColor, _EndColor, i.worldPos.y*0.2);
		}
	
		ENDCG
		}
	
	}
	FallBack Off
}