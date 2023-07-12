// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Skybox_00"
{
    Properties
    {
        [ShaderFeatureToggle(F_MAIN_TEXTURES, Skybox Textures)] _F_texture_transition("Skybox Textures", Int) = 0 
        [ShaderFeature(F_MAIN_TEXTURES)] _MainTex("Cloud Texture", 2D) = "white" {}
        [ShaderFeature(F_MAIN_TEXTURES)]_SecondTex("Background Texture", 2D) = "none" {}
        
        [ShaderFeatureToggle(F_MAIN_COLORS, Skybox Colors)] _F_colors("Skybox Colors", Int) = 0 
        [ShaderFeature(F_MAIN_COLORS)]_Color ("1-st Color", Color) = (1,1,1,1)
        [ShaderFeature(F_MAIN_COLORS)]_Color1 ("2-nd Color", Color) = (0,0,1,1)
        [ShaderFeature(F_MAIN_COLORS)]_Pos ("1-st Color Level", Range(-1, 1)) = 0
        [ShaderFeature(F_MAIN_COLORS)]_Pow ("Colors Power", Float) = 1
        [ShaderFeature(F_MAIN_COLORS)]_ColorCloud ("Color Cloud", Color) = (1,1,1,1)
        [ShaderFeature(F_MAIN_COLORS)]_ColorBack ("Color Background", Color) = (1,1,1,1)
        [ShaderFeature(F_MAIN_COLORS)]_Lerp("Lerp Background", Range(-20, 20)) = 0
        
        
        [ShaderFeatureToggle(F_SPEED, Skybox Clouds Speed)] _F_speed("Clouds Speed", Int) = 0 
        [ShaderFeature(F_SPEED)]_Frequency ("Cloud Wave Frequency", float) = 0
        [ShaderFeature(F_SPEED)]_Amplitude ("Cloud Wave Amplitude", Range (0, 0.5)) = 0
        [ShaderFeature(F_SPEED)]_Speed("Cloud Wave Speed", float) = 0
        [ShaderFeature(F_SPEED)]_LevelCloud("Cloud Level", Range(0, 20))= 10
        //_Lerp2("Lerp_Range2", Range(-1, 1)) = 0
        
    }
 
    SubShader
    {

      Tags {"Queue" = "Background"}  
      Pass
      {
         CGPROGRAM
 
         #pragma vertex vert
         #pragma fragment frag
         #include "UnityCG.cginc"
         
         struct appdata
         {
            float4 vertex : POSITION;
            float2 texcoord: TEXCOORD1;
            float2 texcoord2 : TEXCOORD2;
            float3 normal : NORMAL;
         };
 
         struct v2f
         {
            float4 pos : SV_POSITION;
            fixed3 objSPos : TEXCOORD0;
            float2 uv : TEXCOORD1;
            float2 uv2 : TEXCOORD2;
            
         };

         sampler2D _MainTex;
         sampler2D _SecondTex;
         float4 _MainTex_ST;
         float4 _SecondTex_ST;
         fixed4 _Color;
         fixed4 _Color1;
         fixed4 _ColorCloud;
         fixed4 _ColorBack;
         fixed _Pos;
         half _Pow;
         half _Frequency;
         half _Amplitude;
         float _Speed;
         float _Lerp;
         //float _Lerp2;
         float _LevelCloud;
 
         v2f vert(appdata v)
         {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.objSPos = v.vertex;
            o.uv = TRANSFORM_TEX(v.texcoord , _MainTex) + (sin(v.texcoord.x * _Frequency + _Speed * _Time.x) * _Amplitude) + _Time.x * _Speed;
            o.uv2 = TRANSFORM_TEX(v.texcoord2, _SecondTex);
            return o;
         }
 
         fixed4 frag(v2f IN) : COLOR
         {
            IN.objSPos = normalize(IN.objSPos);
            fixed ang = dot(fixed3(0.0, 1.0, 0.0), IN.objSPos);
            fixed level = ang;
            ang -= _Pos;
            ang = pow(ang, _Pow);
           
            fixed4 Color_lerp = lerp(_Color, _Color1, ang);
            fixed4 final_Color_2 = tex2D(_SecondTex, IN.uv2) * lerp(_ColorBack,_Color, _Lerp*(1-ang)) ;
            fixed4 final_Color_1 = (level * _LevelCloud) > 0.5? tex2D(_MainTex, IN.uv) * _ColorCloud : final_Color_2;
            fixed4 final_Color = tex2D(_MainTex, IN.uv).a * (level * _LevelCloud) > 0.5? final_Color_1 : tex2D(_SecondTex, IN.uv2).a > 0.1? final_Color_2 : Color_lerp ;
            
            return final_Color;
         }
 
         ENDCG
      }
   }
}