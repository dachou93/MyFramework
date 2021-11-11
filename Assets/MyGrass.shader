Shader "Unlit/MyGrass"
{
	Properties
	{
		_TopColor("Top Color",Color)=(1,1,1,1)
		_BottomColor("Bottom Color",Color) = (1,1,1,1)
		_TranslucentGain("Translucent Gain",Range(0,1))=0.5
		_BladeWidth("Blade Width",Float)=0.05
		_BladeWidthRandom("Blade Width Random",Float) = 0.02
		_BladeHeight("Blade Height",Float)=0.5
		_BladeHeightRandom("Blade Height Random",Float) = 0.3
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" "LightMode"="ForwardBase"}
		Cull Off
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geo
			#pragma fragment frag

			#include "Lighting.cginc"
			#include "UnityCG.cginc"
			#include "Autolight.cginc"
			#pragma target 4.6

			
			float4 _TopColor;
			float4 _BottomColor;
			float _TranslucentGain;

			float rand(float3 co)
			{
				return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539)))*43758.5453);
			}

			float3x3 AngleAxis3x3(float angle, float3 axis)
			{
				float c, s;
				sincos(angle, s, c);

				float t = 1 - c;
				float x = axis.x;
				float y = axis.y;
				float z = axis.z;

				return float3x3(
					t*x*x + c, t*x*y - s * z, t*x*z + s * y,
					t*x*y + s * z, t*y*y + c, t*y*z - s * x,
					t*x*z - s * y, t*y*z + s * x, t*z*z + c
					);
			}

			float _BladeHeight;
			float _BladeHeightRandom;
			float _BladeWidth;
			float _BladeWidthRandom;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct vertexOutput {
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};



			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;
				o.vertex = v.vertex;
				o.normal = v.normal;
				o.tangent = v.tangent;

				return o;
			}

			struct geometryOutput
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			geometryOutput CreateGeoOutput(float3 pos ,float2 uv)
			{
				geometryOutput o;
				o.pos = UnityObjectToClipPos(pos);
				o.uv = uv;
				return o;
			}

			//单个调用的最大顶点个数
			[maxvertexcount(3)]
			void geo(triangle vertexOutput IN[3]: SV_POSITION,inout TriangleStream<geometryOutput> triStream) {
				
				float3 pos = IN[0].vertex;
				float3 vNormal = IN[0].normal;
				float4 vTangent = IN[0].tangent;
				float3 vBinormal = cross(vNormal, vTangent)*vTangent.w;

				float height = (rand(pos.zyx) * 2 - 1)*_BladeHeightRandom + _BladeHeight;
				float width = (rand(pos.xzy) * 2 - 1)*_BladeWidthRandom + _BladeWidth;

				float3x3 facingRotationMatrix = AngleAxis3x3(rand(pos)*UNITY_TWO_PI, float3(0, 0, 1));
				float3x3 tangentToLocal = float3x3(
					vTangent.x, vBinormal.x, vNormal.x,
					vTangent.y, vBinormal.y, vNormal.y,
					vTangent.z, vBinormal.z, vNormal.z
					);
				float3x3 transformationMat = mul(tangentToLocal, facingRotationMatrix);

				geometryOutput o;

				triStream.Append(CreateGeoOutput(pos + mul(transformationMat, float3(width, 0, 0)), float2(0, 0)));
				triStream.Append(CreateGeoOutput(pos + mul(transformationMat, float3(-width, 0, 0)), float2(1, 0)));
				triStream.Append(CreateGeoOutput(pos + mul(transformationMat, float3(0, 0, height)), float2(0.5, 1)));
			}

			float4 frag(geometryOutput i,fixed facing:VFACE) : SV_Target
			{
				fixed4 col = lerp(_BottomColor,_TopColor,i.uv.y);
				return col;
			}
			ENDCG
		}
	}
}