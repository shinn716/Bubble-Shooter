Shader "Custom/wireframe" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_Emission("Emission", Range(0,3)) = 1.0
	}
		SubShader{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 200
		Cull off
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
#pragma surface surf Standard fullforwardshadows alpha:fade
#pragma target 3.0

		struct Input {
		float2 uv_MainTex;
	};

	half _Emission;
	fixed4 _Color;

	void surf(Input IN, inout SurfaceOutputStandard o) {
		fixed4 c = _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
		o.Metallic = o.Smoothness = 0;
		o.Emission = _Emission;
	}
	ENDCG

	}
		FallBack "Diffuse"
}