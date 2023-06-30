Shader "VR/Shader_Glass_Reflection" 
{
    Properties 
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Texture Color", Color) = (1,1,1,1)
         
        _LightY ("Light Y", Range(0, 360)) = 0.0

        _DiffuseColor ("Diffuse Color", Color) = (1,1,1,1)

       	_SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
		_Shininess ("Specular Shininess", Range(0, 1)) = 0.5
		_SpecIntensity ("Specular Intensity", Range(0, 1)) = 1

		_RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0, 10)) = 3.0

        _ReflrectionMap ("Reflrection Map", CUBE) = "white" {}
        _ReflectionPower("Reflection Power", Range(0, 1)) = 1.0
        _ReflectionAddAlpha("Reflection Add Alpha", Range(0, 1)) = 1.0
        _ReflectionRimPower("Reflection Rim Power", Range(0, 10)) = 3.0
    }


	SubShader 
	{
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
            
        Pass
		{
            Lighting On
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
            ZWrite Off
            
            ZWrite On
			ColorMask 0
		}

		Pass
		{
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
            ZWrite Off
            
            CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

            #define PI 3.14159

			uniform sampler2D _MainTex;
			uniform float4 _Color; // Texture Color

			uniform float _LightY;

			uniform float4 _DiffuseColor; // Diffuse Color

			uniform float4 _SpecColor; // Specular Color
			uniform float _Shininess; // Specular Shininess
			uniform float _SpecIntensity; // Specular Intensity
			uniform float4 _RimColor;
			uniform float _RimPower;

            float4 _OverlapedColor;
            float _OverlapedPower;

            samplerCUBE _ReflrectionMap;
            float _ReflectionAddAlpha;
            float _ReflectionPower;
            float _ReflectionRimPower;

			struct vertexInput {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float3 normal : NORMAL;

				float2 uvMain : TEXCOORD0;
			};

			struct vertexOutput
            {
				float4 pos : SV_POSITION;
				float4 color : COLOR;
				float2 uvMain : TEXCOORD0;
				float4 diffuse : TEXCOORD1;
				float4 spec : TEXCOORD2;
				float4 rimLighting : TEXCOORD3;
                float4 reflection : TEXCOORD4;
			};

            inline float3 getLightRotation(float xAngle, float yAngle) {

                float3x3 yRot = float3x3(
                    cos(yAngle), 0, sin(yAngle),
                    0, 1, 0,
                    -sin(yAngle), 0, cos(yAngle)
                    );

                float3x3 xRot = float3x3(
                    1, 0, 0,
                    0, cos(xAngle), -sin(xAngle),
                    0, sin(xAngle), cos(xAngle)
                    );

                return mul(yRot, mul(xRot, float3(0, 0, -1)));
            }

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				output.pos = UnityObjectToClipPos(input.vertex);
				output.uvMain = input.uvMain;
				output.color = input.color;

				float3 normalDirection = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);

                float3 worldPosition = mul(modelMatrix, input.vertex).xyz;
				float3 viewDirection = normalize(_WorldSpaceCameraPos - worldPosition);
				float3 lightDirection = getLightRotation(50.0 / 180.0 * PI, _LightY / 180.0 * PI);
				float attenuation = 1.0;

                float dotNormalLight = dot(normalDirection, lightDirection);
				float3 diffuseReflection = lerp(_DiffuseColor.rgb, float3(1.0, 1.0, 1.0),
				clamp(attenuation * max(0.0, dotNormalLight), 0.0, 1.0));

				float4 specularReflection;

				if (dotNormalLight < 0.0) {
					specularReflection = float4(0.0, 0.0, 0.0, 0.0);
				} else {	
					specularReflection = attenuation 
					* _SpecColor * _SpecIntensity * pow(max(0.0, dot(
					reflect(-lightDirection, normalDirection),
					viewDirection)), (_Shininess * 199.0 + 1.0));
				}

				output.spec = specularReflection;
				output.diffuse = float4(diffuseReflection, 1.0);

				float rim = 1 - saturate(dot(normalize(viewDirection), normalDirection));
				output.rimLighting = float4(attenuation * _RimColor.xyz, 1) * pow(rim, _RimPower);

                float3 viewDirReal = worldPosition - _WorldSpaceCameraPos;
                output.reflection.xyz = reflect(viewDirReal, normalDirection);
                output.reflection.w = pow(rim, _ReflectionRimPower) * _ReflectionPower;

				return output;
			}


			fixed4 frag(vertexOutput input) : COLOR
			{
				float4 clearColor = tex2D(_MainTex, input.uvMain) * _Color * input.color;
				float4 resultColor = (clearColor + input.spec) * input.diffuse + input.rimLighting;
                
                resultColor.rgb = lerp(resultColor.rgb, _OverlapedColor.rgb, _OverlapedPower);

                float4 skyPixel = texCUBE(_ReflrectionMap, input.reflection.xyz);
                skyPixel.a += _ReflectionAddAlpha;

                resultColor = lerp(resultColor, skyPixel, input.reflection.w);

                return resultColor;
			}

			ENDCG
		}

        Pass
        {
            Tags{ "LightMode" = "ShadowCaster" }
            
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            struct vertexInput {
                float4 vertex : POSITION;
            };

            struct vertexOutput {
                float4 pos : SV_POSITION;
                float4 color : COLOR;

                float2 uvMain : TEXCOORD0;
                float4 diffuse : TEXCOORD1;
                float4 spec : TEXCOORD2;
                float4 rimLighting : TEXCOORD3;
            };


            vertexOutput vert(vertexInput input)
            {
                vertexOutput output;

                output.pos = UnityObjectToClipPos(input.vertex);

                return output;
            }


            fixed4 frag(vertexOutput input) : COLOR
            {
                return 1;
            }

            ENDCG
        }
	}
}

