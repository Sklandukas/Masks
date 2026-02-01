Shader "Custom/SimpleWind"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Wind Speed", Range(0, 10)) = 15.5
        _Amount ("Sway Amount", Range(0, 1)) = 0.18
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off 

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
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Speed;
            float _Amount;

            v2f vert (appdata v)
            {
                v2f o;
                
                // --- WIND LOGIC ---
                // Calculate sway based on Time and the Y position of the vertex
                // v.uv.y acts as a mask: 0 (bottom) moves 0%, 1 (top) moves 100%
                float sway = sin(_Time.y * _Speed + v.vertex.y) * _Amount * v.uv.y;
                
                v.vertex.x += sway; 
                // ------------------

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                return col;
            }
            ENDCG
        }
    }
}