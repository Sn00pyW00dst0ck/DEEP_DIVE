//https://www.kodeco.com/5671826-introduction-to-shaders-in-unity
Shader "Custom/3DBoidShader" {
    Properties{
      _Color("Color", Color) = (1, 1, 1, 1)
      _MainTex("Texture", 2D) = "white" {}
      _Scale("Scale", Float) = 1.0
      _Glossiness("Smoothness", Range(0, 1.0)) = 0.5
      _Metallic("Metallic", Range(0, 1.0)) = 0.0
    }
    SubShader{
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard vertex:vert addshadow fullforwardshadows
        #pragma instancing_options procedural:setup
        #pragma target 4.5
        void setup() {}

        struct Input {
            float2 uv_MainTex;
        };

        struct Boid {
            float3 pos;
            float3 vel;
            float pad0;
            float pad1;
        };

        #if defined(SHADER_API_D3D11) || defined(SHADER_API_METAL)
            StructuredBuffer<float3> conePositions;
            StructuredBuffer<float3> coneNormals;
            StructuredBuffer<int> coneTriangles;
            StructuredBuffer<Boid> boids;
            int vertCount;
        #endif

        // Below based on some old math I did for project 1, only done in a shader now
        void rotate3D(inout float3 v, float3 vel) {
            float3 up = float3(0, 1, 0);
            float3 axis = normalize(cross(up, vel));
            float angle = acos(dot(up, normalize(vel))); //acos might be slow?????  http://www.fractalforums.com/programming/shader-function-or-instruction-cost-(performance)/msg84618/#msg84618
            v = v * cos(angle) + cross(axis, v) * sin(angle) + axis * dot(axis, v) * (1.0 - cos(angle));
        }

        float _Scale;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert(inout appdata_full v) {
            UNITY_SETUP_INSTANCE_ID(v);

            // https://docs.unity3d.com/Manual/SL-BuiltinMacros.html
            #if defined(UNITY_INSTANCING_ENABLED) || defined(UNITY_PROCEDURAL_INSTANCING_ENABLED) || defined(UNITY_STEREO_INSTANCING_ENABLED)
                // FIND WAY TO GET THE VERTEX ID IN THIS FUNCTION!!
                Boid boid = boids[unity_InstanceID];
                // Get the vertex ID from the uv4 channel (NOT WORKING??)
                instanceVertexID = v.texcoord4.u;

                // Calc position
                float3 pos = conePositions[coneTriangles[instanceVertexID]];
                rotate3D(pos, boid.vel);
                v.vertex = float4((pos * _Scale) + boid.pos, 1);

                // Calc normal
                float3 normal = coneNormals[coneTriangles[instanceVertexID]];
                rotate3D(normal, boid.vel);
                v.normal = normal;
            #endif
        }

        sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        // UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        // UNITY_INSTANCING_BUFFER_END(Props)

        // Main shader function
        void surf(Input IN, inout SurfaceOutputStandard o) {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
