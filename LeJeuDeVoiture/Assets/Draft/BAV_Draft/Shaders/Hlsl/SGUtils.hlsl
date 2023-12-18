// Make a float3 from the x, y, z components of a float4
void MakeVec3XYZFromVec4_float(float4 vec4, out float3 vec2)
{
    vec2 = float3(vec4.x, vec4.y, vec4.z);
}

// Make a float3 from the x, y, w components of a float4
void MakeVec3XYWFromVec4_float(float4 vec4, out float3 vec3)
{
    vec3 = float3(vec4.x, vec4.y, vec4.w);
}

// Make a float2 from the x and y components of a float4
void MakeVec2XYFromVec4_float(float4 vec4, out float2 vec2)
{
    vec2 = float2(vec4.x, vec4.y);
}
// Make a float2 from the x and z components of a float4
void MakeVec2XZFromVec4_float(float4 vec4, out float2 vec2)
{
    vec2 = float2(vec4.x, vec4.z);
}

// Make a float2 from the x and w components of a float4
void MakeVec2XWFromVec4_float(float4 vec4, out float2 vec2)
{
    vec2 = float2(vec4.x, vec4.w);
}

// Make a float2 from the y and z components of a float4
void MakeVec2YZFromVec4_float(float4 vec4, out float2 vec2)
{
    vec2 = float2(vec4.y, vec4.z);
}

// Make a float2 from the y and w components of a float4
void MakeVec2YWFromVec4_float(float4 vec4, out float2 vec2)
{
    vec2 = float2(vec4.y, vec4.w);
}

// Make a float2 from the z and w components of a float4
void MakeVec2ZWFromVec4_float(float4 vec4, out float2 vec2)
{
    vec2 = float2(vec4.z, vec4.w);
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// float make vector 4
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Make a float4 from the x, y, z, w components of a float4
void MakeVec4XYZWFromVec4_float(float4 vec4, out float4 vec4Out)
{
    vec4Out = float4(vec4.x, vec4.y, vec4.z, vec4.w);
}

// Make a float4 from the x, y, z, w components of a float3
void MakeVec4XYZWFromVec3_float(float3 vec3, out float4 vec4Out)
{
    vec4Out = float4(vec3.x, vec3.y, vec3.z, 1.0f);
}

// Make a float4 from the x, y, z, w components of a float2
void MakeVec4XYZWFromVec2_float(float2 vec2, out float4 vec4Out)
{
    vec4Out = float4(vec2.x, vec2.y, 0.0f, 1.0f);
}

// Make a float4 from the x, y, z, w components of a float1
void MakeVec4XYZWFromFloat_float(float value, out float4 vec4Out)
{
    vec4Out = float4(value, value, value, value);
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// int make vector 4
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Make a float4 from the x, y, z, w components of a int4
void MakeVec4XYZWFromVec4_int(int4 vec4, out float4 vec4Out)
{
    vec4Out = float4(vec4.x, vec4.y, vec4.z, vec4.w);
}

// Make a float4 from the x, y, z, w components of a int3
void MakeVec4XYZWFromVec3_int(int3 vec3, out float4 vec4Out)
{
    vec4Out = float4(vec3.x, vec3.y, vec3.z, 1.0f);
}

// Make a float4 from the x, y, z, w components of a int2
void MakeVec4XYZWFromVec2_int(int2 vec2, out float4 vec4Out)
{
    vec4Out = float4(vec2.x, vec2.y, 0.0f, 1.0f);
}

// Make a float4 from the x, y, z, w components of a int1
void MakeVec4XYZWFromInt_int(int value, out float4 vec4Out)
{
    vec4Out = float4(value, value, value, value);
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// uint make vector 4
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Make a float4 from the x, y, z, w components of a uint4
void MakeVec4XYZWFromVec4_uint(uint4 vec4, out float4 vec4Out)
{
    vec4Out = float4(vec4.x, vec4.y, vec4.z, vec4.w);
}

// Make a float4 from the x, y, z, w components of a uint3
void MakeVec4XYZWFromVec3_uint(uint3 vec3, out float4 vec4Out)
{
    vec4Out = float4(vec3.x, vec3.y, vec3.z, 1.0f);
}

// Make a float4 from the x, y, z, w components of a uint2
void MakeVec4XYZWFromVec2_uint(uint2 vec2, out float4 vec4Out)
{
    vec4Out = float4(vec2.x, vec2.y, 0.0f, 1.0f);
}

// Make a float4 from the x, y, z, w components of a uint1
void MakeVec4XYZWFromUint_uint(uint value, out float4 vec4Out)
{
    vec4Out = float4(value, value, value, value);
}

