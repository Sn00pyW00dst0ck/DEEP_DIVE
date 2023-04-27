//https://www.kodeco.com/5671826-introduction-to-shaders-in-unity
// Shadows are slowing the GPU down A LOT
Shader "Custom/3DBoidShader" {
    Properties{
        _Color("Color", Color) = (1, 1, 1, 1)
        _Scale("Scale", Float) = 1.0
        _Glossiness("Smoothness", Range(0, 1)) = 0.5
        _Metallic("Metallic", Range(0, 1)) = 0.0

        _Offset("Offset", Vector) = (0, 0, 0, 0)

        _MainTex("Texture", 2D) = "white" {}
        _BumpMap("Bumpmap", 2D) = "bump" {}
        _Detail("Detail", 2D) = "gray" {}


        _SpeedX("SpeedX", Range(0, 10)) = 1
        _FrequencyX("FrequencyX", Range(0, 10)) = 1
        _AmplitudeX("AmplitudeX", Range(0, 0.2)) = 1
        _SpeedY("SpeedY", Range(0, 10)) = 1
        _FrequencyY("FrequencyY", Range(0, 10)) = 1
        _AmplitudeY("AmplitudeY", Range(0, 0.2)) = 1
        _SpeedZ("SpeedZ", Range(0, 10)) = 1
        _FrequencyZ("FrequencyZ", Range(0, 10)) = 1
        _AmplitudeZ("AmplitudeZ", Range(0,  2)) = 1
        _HeadLimit("HeadLimit", Range(-2,  2)) = 0.05

    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            #pragma surface surf Standard vertex:vert addshadow fullforwardshadows 
            #pragma target 3.0

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent: TANGENT;
                float4 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
                uint vertexID : SV_VertexID;
            };

            struct Input {
                float2 uv_MainTex;
                float2 uv_BumpMap;
                float2 uv_Detail;
            };

            struct Boid {
                float3 pos;
                float3 vel;
                float pad0;
                float pad1;
            };

            // Inspiration for fish shader animation effect
            // https://www.reddit.com/r/Unity3D/comments/7bvx9d/made_a_vertex_animation_shader_for_a_fish/
            //
            // X AXIS
            float _SpeedX;
            float _FrequencyX;
            float _AmplitudeX;

            // Y AXIS
            float _SpeedY;
            float _FrequencyY;
            float _AmplitudeY;

            // Z AXIS
            float _SpeedZ;
            float _FrequencyZ;
            float _AmplitudeZ;

            // Head Limit (Head wont shake so much)
            float _HeadLimit;

            float3 applyFishAnimation(float3 position) {
                //Z AXIS
                position.z += sin((position.z + _Time.y * _SpeedX) * _FrequencyX) * _AmplitudeX;

                //Y AXIS
                position.y += sin((position.z + _Time.y * _SpeedY) * _FrequencyY) * _AmplitudeY;

                //X AXIS
                if (position.z > _HeadLimit)
                {
                    position.x += sin((0.05 + _Time.y * _SpeedZ) * _FrequencyZ) * _AmplitudeZ * _HeadLimit;
                }
                else
                {
                    position.x += sin((position.z + _Time.y * _SpeedZ) * _FrequencyZ) * _AmplitudeZ * position.z;
                }
                return position;
            }



            // Buffers of input
            #if defined(SHADER_API_D3D11) || defined(SHADER_API_METAL)
                StructuredBuffer<float3> meshPositions;
                StructuredBuffer<float3> meshNormals;
                StructuredBuffer<int> meshTriangles;
                StructuredBuffer<float2> meshUVs;
                StructuredBuffer<Boid> boids;
                int triangleCount;
            #endif

                // Vertex displacement function moves vertices based on boid position and velocity
                float _Scale;
                float4 _Offset;
                void vert(inout appdata v) {
                    //https://docs.unity3d.com/Manual/SL-BuiltinMacros.html
                    #if defined(SHADER_API_D3D11) || defined(SHADER_API_METAL)
                        uint instanceID = v.vertexID / triangleCount;
                        uint instanceVertexID = v.vertexID - instanceID * triangleCount;
                        Boid boid = boids[instanceID];
                        float3 pos = meshPositions[meshTriangles[instanceVertexID]];
                        float3 normal = meshNormals[meshTriangles[instanceVertexID]];

                        // Below for rotations to keep up direcion locked for the rotation
                        // Based on post from bgolus at: https://forum.unity.com/threads/rotate-mesh-inside-shader.1109660/
                        float3 forward = normalize(boid.vel);
                        float3 right = normalize(cross(forward, float3(0, 1, 0)));
                        float3 up = cross(right, forward); // does not need to be normalized
                        float3x3 rotationMatrix = float3x3(right, up, forward);

                        // Calculate new vertex position
                        float3 worldPosition = mul(applyFishAnimation(pos), rotationMatrix) * _Scale + boid.pos + _Offset;
                        v.vertex = float4(worldPosition, 1);

                        // We need the normal
                        v.normal = float4(mul(normal, rotationMatrix), 1);
                        v.texcoord.xy = meshUVs[meshTriangles[instanceVertexID]].xy;
                        //o.uv_Detail = meshUVs[meshTriangles[instanceVertexID]];

                    #endif
                }

                // Main shader function
                fixed4 _Color;
                half _Glossiness;
                half _Metallic;
                sampler2D _MainTex;
                sampler2D _Detail;
                void surf(Input IN, inout SurfaceOutputStandard o) {
                    // Set color based on textures (TODO: add bumpmap).
                    o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
                    // o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
                    o.Albedo *= tex2D(_Detail, IN.uv_Detail).rgb * 4;
                    o.Albedo *= _Color.rgb;

                    // Set metallic and smoothness
                    o.Metallic = _Metallic;
                    o.Smoothness = _Glossiness;
                }

                ENDCG
        }
            FallBack "Diffuse"
}