// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Dots" {
    Properties {
        _Scale ("Grid Scale", Float) = .1
        _DotSize ("Dot Size", Float) = .01
    }
    SubShader {
        Tags 
        {
			"Queue"="Transparent"  
		    "IgnoreProjector"="True" 
		    "RenderType"="Transparent"
		}
        LOD 200
        
        Pass {
        	ZWrite Off Cull Off Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
            
                struct v2f {
                    float4 pos : SV_POSITION;
                    //float2 uv_MainTex : TEXCOORD0;
                    float3  worldPos : TEXCOORD0;
                    float3  worldNormal : TEXCOORD1;
                };
            
                //float4 _MainTex_ST;
            
            
             v2f vert (appdata_base v)
			{
				v2f o;
				
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldNormal = mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz;
				
				return o;
			}
                    
                    
              //  v2f vert(appdata_base v) {
              //      v2f o;
              //      o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
             //       o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
            //        return o;
            //    }
            
                //sampler2D _MainTex;
            uniform float _Scale;
            uniform float _DotSize;
            
                float4 frag(v2f In) : COLOR 
                {
					//float scale = .1f;
					float3 grid = float3(0,0,0);
					float3 pos = In.worldPos.xyz ;
					
					grid.x = fmod(pos.x, _Scale);
					grid.y = fmod(pos.y, _Scale);
					grid.z = fmod(pos.z, _Scale);
					
					 
					float zgrid = length(abs(grid.xy) - (0.5 * _Scale));
					float xgrid = length(abs(grid.yz) - (0.5 * _Scale));
					float ygrid = length(abs(grid.xz) - (0.5 * _Scale));
					
					float3 norm = In.worldNormal;
					float nyblend = abs(norm.y);
					float nzblend = abs(norm.z);
					
					//float dotsize = 0.01;
					float3 result = step(lerp(lerp(xgrid,ygrid,nyblend),zgrid, nzblend), _DotSize);
					
					float4 ret = float4(1, result); 
					return ret;
                
                
                
                    //half4 c = tex2D (_MainTex, IN.uv_MainTex);
                    //return c;
                }
            ENDCG
        }
    }
}