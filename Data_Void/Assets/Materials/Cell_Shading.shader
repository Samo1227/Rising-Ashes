Shader "Custom/Cell_Shading" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	    _SpecColor ("Specular Color", Color) = (1,1,1,1)
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(.002, 0.03)) = .005
		//_Glossiness("Smoothness", Range(0,1)) = 0.5
		//_Metallic("Metallic", Range(0,1)) = 0.0
	}
	
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Ramp

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _Ramp;
		

		half4 LightingRamp(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) 
		{
			half NdotL = dot(s.Normal, lightDir);
			if (NdotL <= -0.5) NdotL = -0.1;
			else if (NdotL <= -0.05) NdotL = 0.1;
			else if (NdotL <= 0.3) NdotL = 0.3;
			else if (NdotL >= 0.98) NdotL = 1;
			else NdotL = 0.5;
			half4 c;
		
			half NdotC = dot(s.Normal, viewDir);

			if (NdotL >= 0.98) {
				NdotL = 1;
				c.rgb = s.Albedo * _LightColor0.rgb * _SpecColor*(NdotL * atten * 2);
			}
			else  {//(NdotL < 0.99)
				c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
			}

			if (NdotC <= 0.3) {
				//c.rgb = s.Albedo*_LightColor0.rgb*_OutlineColor(NdotC*atten*2);
				//c.rgb = 0, 0, 0; //s.Albedo * 0;
				c.rgb = s.Albedo * _LightColor0.rgb * (-0.5 * atten * 2);

			}
			c.a = s.Alpha;
			return c;
			}

			struct Input {
				float2 uv_MainTex;
				//float3 WorldSpaceViewDir (float4 v);
			};

			fixed4 _Color;
			//fixed4 _OutlineColor;
			//fixed4 _SpecColor;
			
			sampler2D _MainTex;

			void surf(Input IN, inout SurfaceOutput o) {
				o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb*_Color;
			}
			ENDCG

		}
			FallBack "Toon/Basic Outline"
}
