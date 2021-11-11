Shader "GeoShader/Line"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geo
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			#pragma target 4.0

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2g {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct g2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2g vert(appdata v)
			{
				v2g o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			//单个调用的最大顶点个数
			[maxvertexcount(3)]
			//以一个三角形为单位进行输入（每次同时输入三个顶点）
			//图元输入：point line lineadj triangle triangleadj
			//以线的形式进行输出
			//图元输出：LineStream PointStream TriangleStream
			void geo(triangle v2g input[3],inout LineStream<g2f> outStream) {
				g2f o = (g2f)0;
				//使用两个点组装成一条线段
				for (int i = 0; i < 2; i++) {
					o.vertex = input[i].vertex;
					o.uv = input[i].uv;
					outStream.Append(o);
				}
			}

			fixed4 frag(g2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}