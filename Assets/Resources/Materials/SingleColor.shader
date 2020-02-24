// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SingleColor"
{
    Properties
    {
        // Color property for material inspector, default to white
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            fixed4 _Colors[2048];
            fixed4 _Colors2[2047];
            #pragma vertex vert
            #pragma fragment frag
            
            // vertex shader
            // this time instead of using "appdata" struct, just spell inputs manually,
            // and instead of returning v2f struct, also just return a single output
            // float4 clip position
            float4 vert (float4 vertex : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(vertex);
            }
            
            // color from the material
            fixed4 _Color;

            // pixel shader, no inputs needed
            fixed4 frag (uint triangleID: SV_PrimitiveID) : SV_Target
            {
                //_Color.r *= triangleID%3;
                _Color = _Colors[triangleID];
                //_Color.r = _Colors[triangleID][0];
                //_Color.g = _Colors[triangleID].g;
                return _Color; // just return it
            }
            ENDCG
        }
    }
}