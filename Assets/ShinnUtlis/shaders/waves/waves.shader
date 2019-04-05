Shader "Custom/waves" {


	Properties
	{
		_Center("Center", Vector) = (110, 139, 0, 0)
		_WaveColor("WaveColor", Color) = (1, 1, 1, 1)
		_CircleColor("CircleColor", Color) = (1, 1, 1, 1)
		_Thickness("Thickness", Range(0, 5))=3
		_Speed("Speed", Range(0, 5)) = 1

	}

	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard 
		#pragma target 3.0

		struct Input {
			float3 worldPos;
		};


		float4 _Center;
		float4 _WaveColor;
	
		float _Speed;
		float _Thickness;
	
		float4 _CircleColor;

		void surf(Input IN, inout SurfaceOutputStandard o) {

			float dist = distance(fixed3(_Center.x, _Center.y, _Center.z), IN.worldPos);
			float val = abs(sin(dist*_Thickness - _Time * 100 * _Speed));
	
			if (val > 0.98) 
				o.Albedo = fixed4(_CircleColor.r, _CircleColor.g, _CircleColor.b, _CircleColor.a);
			else 
				o.Albedo = fixed4(_WaveColor.r, _WaveColor.g, _WaveColor.b, _WaveColor.a);
			
		}
		ENDCG
	}
		FallBack "Diffuse"
}
