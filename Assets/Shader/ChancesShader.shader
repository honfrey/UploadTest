// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Unlit/ChancesShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TintColor ("Color", Color) = (1,0,0,1) 
		_Offset ("Offset", Range(0,5)) = 0 
	}
	SubShader
	{
		Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma multi_compile __ LIGHTMAP_ON
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				#if defined(LIGHTMAP_ON)
				float2 uv2: TEXCOORD1;
				#endif
				float4 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				#if defined(LIGHTMAP_ON)
				float2 uv2 : TEXCOORD1;
				#endif
				UNITY_FOG_COORDS(2)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Offset;
			float4 _TintColor;
			
			v2f vert (appdata v)
			{
				v2f o;

				v.vertex.xyz += v.normal * _Offset;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				#if defined(LIGHTMAP_ON)
				o.uv2 = v.uv2.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _TintColor;

				#if defined(LIGHTMAP_ON)
				col.rgb *= (DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2)));
				#endif

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
