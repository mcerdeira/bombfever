float Percentage;

sampler TextureSampler: register(s0);
float4 PixelShaderFunction(float2 Tex: TEXCOORD0) : COLOR0
{
	float4 Color = tex2D(TextureSampler, Tex);
	float r = Color.r;
	float g = Color.g;
	float b = Color.b;	
	r = r * Percentage;
	g = g * Percentage;
	b = b * Percentage;
	Color.r = r;
	Color.g = g;
	Color.b = b;
	return Color;
}

technique hit
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}