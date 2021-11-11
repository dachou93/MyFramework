Shader "Unlit/byjc"
{
	//-----------------------------【属性】-----------------------------
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	// 描边程度
		_EdgeOnly("Edge Only", Float) = 1.0
		// 边缘颜色
		_EdgeColor("Edge Color", Color) = (0, 0.75, 0.68, 1)

		_EdgeColor1("Edge Color1", Color) = (1, 0.48, 0, 1)

		//_BJ("BJ",Float)=0
	}
		//-----------------------------【子着色器】-----------------------------
		SubShader{
			Pass {
				ZTest Always Cull Off ZWrite Off

				CGPROGRAM

				#include "UnityCG.cginc"

				#pragma vertex vert  
				#pragma fragment frag

				sampler2D _MainTex;
				uniform half4 _MainTex_TexelSize;
				fixed _EdgeOnly;
				fixed4 _EdgeColor;
				fixed4 _EdgeColor1;
				//float _BJ;

				struct v2f {
					float4 pos : SV_POSITION;
					float4 screenPos : TEXCOORD0;
					half2 uv[9] : TEXCOORD1;
					
				};

				//-----------------------------【顶点着色器】-----------------------------
				v2f vert(appdata_img v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.screenPos = ComputeScreenPos(o.pos);
					half2 uv = v.texcoord;
					//计算周围像素的纹理坐标位置，其中4为原始点，
					o.uv[0] = uv + _MainTex_TexelSize.xy * half2(-1, -1);
					o.uv[1] = uv + _MainTex_TexelSize.xy * half2(0, -1);
					o.uv[2] = uv + _MainTex_TexelSize.xy * half2(1, -1);
					o.uv[3] = uv + _MainTex_TexelSize.xy * half2(-1, 0);
					o.uv[4] = uv + _MainTex_TexelSize.xy * half2(0, 0);		//原点
					o.uv[5] = uv + _MainTex_TexelSize.xy * half2(1, 0);
					o.uv[6] = uv + _MainTex_TexelSize.xy * half2(-1, 1);
					o.uv[7] = uv + _MainTex_TexelSize.xy * half2(0, 1);
					o.uv[8] = uv + _MainTex_TexelSize.xy * half2(1, 1);

					return o;
				}

				// 转换为灰度
				fixed luminance(fixed4 color) {
					return  0.299 * color.r + 0.587 * color.g + 0.114 * color.b;
				}

				// sobel算子
				half Sobel(v2f i) {
					const half Gx[9] = {-1,  0,  1,
										-2,  0,  2,
										-1,  0,  1};
					const half Gy[9] = {-1, -2, -1,
										0,  0,  0,
										1,  2,  1};

					half texColor;
					half edgeX = 0;
					half edgeY = 0;
					for (int it = 0; it < 9; it++) {
						// 转换为灰度值
						texColor = luminance(tex2D(_MainTex, i.uv[it]));

						edgeX += texColor * Gx[it];
						edgeY += texColor * Gy[it];
					}
					// 合并横向和纵向
					half edge = 1 - (abs(edgeX) + abs(edgeY));
					return edge;
				}
				//-----------------------------【片元着色器】-----------------------------
				fixed4 frag(v2f i) : SV_Target {
					float _BJ = abs(sin(_Time.y/2))/**(saturate(sign(sin(2 * _Time.y))))*/ * 1920;
					//float _BJ = _SinTime.z * 1000;
					float2 center = float2(0.5*_ScreenParams.x, 0.5*_ScreenParams.y);
					
					float2 screenPos = i.screenPos.xy / i.screenPos.w;
					screenPos.xy *= _ScreenParams.xy;
					float dis = distance(center, screenPos);
					float v = saturate((_BJ - dis)/ 1920);

					float4 col= lerp(_EdgeColor, _EdgeColor1, v);
					//return col;
					half edge = Sobel(i);
					edge = step(0.5, edge);
					fixed4 edgeColor = lerp(col, tex2D(_MainTex, i.uv[4]), edge);
					edgeColor = lerp(tex2D(_MainTex, i.uv[4]), edgeColor, _EdgeOnly);
					return edgeColor;
				}

				ENDCG
			}
	}
		FallBack Off
}
