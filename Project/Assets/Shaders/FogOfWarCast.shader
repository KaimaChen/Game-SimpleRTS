Shader "Kaima/FogOfWarCast" {
	Properties {
		_FogOfWarColor("FogOfWar Color", Color) = (0, 0, 0, 1)
		_FogOfWarTex("FogOfWar Tex", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert 
			#pragma fragment frag 
			#pragma target 2.0

			#include "UnityCG.cginc"

			uniform fixed4 _FogOfWarColor;
			uniform sampler2D _FogOfWarTex;
			uniform float4x4 _MatCastViewProj;

			struct v2f {
				float4 position : SV_POSITION;
				half4 UVFogOfWar : TEXCOORD0;
			};

			v2f vert(appdata_base input) {
				v2f o;
				o.position = mul(UNITY_MATRIX_MVP, input.vertex);
				float4x4 matMVP = mul(_MatCastViewProj, unity_ObjectToWorld);
				o.UVFogOfWar = mul(matMVP, input.vertex);
				return o;
			}

			float4 frag(v2f input) : COLOR {
				half2 uv = 0.5 * input.UVFogOfWar.xy / input.UVFogOfWar.w + 0.5;

				#if !UNITY_UV_STARTS_AT_TOP
				uv.y = 1 - uv.y;
				#endif

				half alpha = tex2D(_FogOfWarTex, uv).a;
				return half4(0,0,0,alpha);
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
