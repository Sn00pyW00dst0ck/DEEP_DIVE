//https://www.kodeco.com/5671826-introduction-to-shaders-in-unity
// Shadows are slowing the GPU down A LOT
Shader "Custom/3DBoidShader" {
    Properties{
      _Color("Color", Color) = (1, 1, 1, 1)
      _Scale("Scale", Float) = 1.0
      _Glossiness("Smoothness", Range(0, 1)) = 0.5
      _Metallic("Metallic", Range(0, 1)) = 0.0

      _Offset("Offset", Vector) = (0, 0, 0, 0)
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            #pragma surface surf Standard vertex:vert  addshadow fullforwardshadows
            #pragma target 3.0

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent: TANGENT;
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
                uint vertexID : SV_VertexID;
            };

            struct Input {
                float4 color : COLOR;
            };

            struct Boid {
                float3 pos;
                float3 vel;
                float pad0;
                float pad1;
            };

            float _Scale;
            float4 _Offset;
            #if defined(SHADER_API_D3D11) || defined(SHADER_API_METAL)
                StructuredBuffer<float3> conePositions;
                StructuredBuffer<float3> coneNormals;
                StructuredBuffer<int> coneTriangles;
                StructuredBuffer<Boid> boids;
                int triangleCount;
            #endif

            void vert(inout appdata v) {
                //https://docs.unity3d.com/Manual/SL-BuiltinMacros.html
                #if defined(SHADER_API_D3D11) || defined(SHADER_API_METAL)
                    uint instanceID = v.vertexID / triangleCount;
                    uint instanceVertexID = v.vertexID - instanceID * triangleCount;
                    Boid boid = boids[instanceID];
                    float3 pos = conePositions[coneTriangles[instanceVertexID]];
                    float3 normal = coneNormals[coneTriangles[instanceVertexID]];

                    // Below for rotations to keep up direcion locked for the rotation
                    // Based on post from bgolus at: https://forum.unity.com/threads/rotate-mesh-inside-shader.1109660/
                    float3 forward = normalize(boid.vel);
                    float3 right = normalize(cross(forward, float3(0, 1, 0)));
                    float3 up = cross(right, forward); // does not need to be normalized
                    float3x3 rotationMatrix = float3x3(right, up, forward);

                    float3 worldPosition = mul(pos, rotationMatrix) * _Scale + boid.pos + _Offset;
                    v.vertex = float4(worldPosition, 1);

                    // We need the normal
                    v.normal = float4(mul(normal, rotationMatrix), 1);

                    

                #endif
            }

            half _Glossiness;
            half _Metallic;
            fixed4 _Color;

            // Main shader function
            void surf(Input IN, inout SurfaceOutputStandard o) {
                o.Albedo = _Color.rgb;
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = _Color.a;
            }
            ENDCG
    }
        FallBack "Diffuse"
}