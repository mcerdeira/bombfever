sampler s0;  
texture lightMask;  
sampler lightSampler = sampler_state{Texture = lightMask;};  
  
float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0  
{  
	float4 color = tex2D(s0, coords);  
	float4 lightColor = tex2D(lightSampler, coords);  
	return color * lightColor;  
}  

// http://blog.josack.com/2011/08/my-first-2d-pixel-shaders-part-3.html
	
	
technique hit
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}