Shader "Custom/DiffuseColor"
{
	Properties
	{
		_Tint("Tint", Color) = (1,1,1,1)
		_Brightness("Brightness", Range(0,1)) = 0
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Opaque" }

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			fixed4 _Tint;
			float _Brightness;

			struct v2f
			{
				float4 vertex : SV_Position;
				float3 worldPos : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
			};
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 worldNormal = normalize(i.worldNormal);
				float3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

				float3 diff = saturate(dot(worldNormal, worldLightDir));
				diff = diff * (1 - _Brightness) + _Brightness;
				fixed3 diffuse = _LightColor0.rgb * _Tint.rgb * diff;
				return fixed4(diffuse, _Tint.a);
			}
			ENDCG
		}
	}
}
