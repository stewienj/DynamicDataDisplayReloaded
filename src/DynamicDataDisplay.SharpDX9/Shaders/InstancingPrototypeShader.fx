float4x4 worldViewProj;

void InstancingWith0Textures(float4 position : POSITION, float4 tex0 : TEXCOORD0, float4 tex1 : TEXCOORD1, float4 tex2 : TEXCOORD2, float4 tex3 : TEXCOORD3, out float4 transformedPosition : POSITION)
{
	// Use the values from the 4 texture coordinates to compose a transformation matrix.
	float4x4 transformation = { tex0, tex1, tex2, tex3 };

	// Transform the vertex into world coordinates.
	transformedPosition = mul(position, transformation);

	// Transform the vertex from world coordinates into screen coordinates.
	transformedPosition = mul(transformedPosition, worldViewProj);
}

technique Instance0Textures
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 InstancingWith0Textures();
		PixelShader = NULL;
	}
}