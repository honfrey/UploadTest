Shader "PPShader/SiderEffectUberPPShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Saturation("Saturation", range(0.0, 20.0)) = 1.0
		_EyeBlinkVal("Eye Blink Value", range(0.0, 1.0)) = 1.0

	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			half _Saturation;
			half _EyeBlinkVal;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				//Saturate
				fixed grey = dot(col.rgb, fixed3(0.222, 0.707, 0.071));
				col.rgb = lerp(fixed3(grey, grey, grey), col.rgb, _Saturation);

				//Eye Blink
				half curveFactor = (0.5 - i.uv.x) * (0.5 - i.uv.x) * 0.16;
				half v1 = i.uv.y - curveFactor;
				half v2 = i.uv.y + curveFactor;
		
				half fadeVal = saturate(_EyeBlinkVal - v1) * 6.5;
				fadeVal += saturate(_EyeBlinkVal - (1.0 - v2)) * 6.5;
				col.rgb *= (1.0 - fadeVal);


				return col;
			}
			ENDCG
		}
	}
}
