Shader "Kaima/FogOfWar" {
  SubShader {
    Tags{ "RenderType"="Opaque" "Queue"="Geometry" }
    Pass {
      Name "FogOfWar"
      Lighting Off
      ZWrite Off
      AlphaTest Off
      BlendOp Min
      Blend SrcAlpha DstAlpha

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma target 2.0

      #include "UnityCG.cginc"

      uniform half2 _ViewCenter;
      uniform half _AspectRatio;
      uniform half _FogDensity;
      uniform sampler2D _NoiseTex;
      uniform half _BrushSize;
      uniform half _EdgeSmoothValue;

      struct v2f {
        float4 position : SV_POSITION;
        half2 texCoord : TEXCOORD0;
      };

      v2f vert(appdata_base input) {
        v2f o;
        o.position = mul(UNITY_MATRIX_MVP, input.vertex);
        o.texCoord = input.texcoord;
        return o;
      }

      fixed4 frag(v2f input) : COLOR {
        fixed4 noiseVal = tex2D(_NoiseTex, input.texCoord).r;

        half2 brushArea = half2(_BrushSize, _BrushSize * _AspectRatio) * noiseVal; //让显示范围更加碎片
        // half2 brushArea = half2(_BrushSize, _BrushSize * _AspectRatio) * 0.3; //显示区域接近圆形

        half xDist = (input.texCoord.x - _ViewCenter.x) * (input.texCoord.x - _ViewCenter.x);
        half M = xDist / (brushArea.x * brushArea.x); //这里进行除法是为了在圆内返回的是[0,1]的值，用于下面的smoothstep
        half yDist = (input.texCoord.y - _ViewCenter.y) * (input.texCoord.y - _ViewCenter.y);
        half S = yDist  / (brushArea.y * brushArea.y);
        half dist = M + S;

        half alpha = smoothstep(brushArea.x - _EdgeSmoothValue, brushArea.x + _EdgeSmoothValue, dist) * _FogDensity; //根据当前片元与viewer的距离来决定迷雾的深浅

        return float4(0, 0, 0, alpha);
      }

      ENDCG
    }
  }

  FallBack "Diffuse"
}
