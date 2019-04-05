Shader "QQ/Elastic"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Position("xyz:position,w:range",vector) = (0,0,0,1)
		_Normal("xyz:normal,w:intensity",vector) = (0,1,0,0)
		_PointTime("point time",float) = 0
		_Duration("duration",float) = 2
		_Frequency("frequency",float) = 5
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _Position;
				float4 _Normal;
				float _PointTime;
				float _Duration;
				float _Frequency;
				v2f vert(appdata v)
				{
					v2f o;
					float t = _Time.y - _PointTime;
					if (t > 0 && t < _Duration)
					{
						float r = 1 - saturate(length(v.vertex.xyz - _Position.xyz) / _Position.w);
						float l = 1 - t / _Duration;
						v.vertex.xyz += r * l * _Normal.xyz * _Normal.w * cos(t * _Frequency);
					}
						o.vertex = UnityObjectToClipPos(v.vertex);
						o.uv = TRANSFORM_TEX(v.uv, _MainTex);
						UNITY_TRANSFER_FOG(o, o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
			ENDCG
		}
		}
}