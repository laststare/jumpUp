Shader "GradientSky"
{
 Properties
 {
        _MainTex("Main Texture", 2D) = "white" {}
        _DarkColor("Dark Color", Color) = (1, 1, 1, 1)
        _BrightColor("Bright Color", Color) = (1, 1, 1, 1)
        _ReverseY("ReverseY", float) = 1.0
 }
 
 Subshader
 {
		Tags { "Queue" = "Geometry+1" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
     Pass
     {
     	Cull Off ZWrite Off Fog { Mode Off }
         
         CGPROGRAM
         #pragma fragmentoption ARB_precision_hint_fastest
         #pragma vertex vert
         #pragma fragment frag

         #include "UnityCG.cginc"
         
        sampler _MainTex;
		float4	_BrightColor;
		float4	_DarkColor;
        float _ReverseY;

         struct outdata
         {
             half4 pos          : POSITION;
             half screenPos     : TEXCOORD0;
         };

         outdata vert (appdata_full v)
         {
            outdata o;
            
            o.pos = UnityObjectToClipPos(v.vertex);
            o.screenPos = _ReverseY * saturate((o.pos.y / o.pos.w) * 0.5f + 0.5f);

            return o;  
         }

         half4 frag (outdata i) : COLOR
         {
            float brightness = tex2D(_MainTex, float2(0.0f, i.screenPos)).r;
            half4 clr = lerp(_DarkColor, _BrightColor, brightness);
            return clr;
         } 

         ENDCG
     }

 }

 Fallback off
}