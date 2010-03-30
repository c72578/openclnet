kernel void FilterImage( float inputLeft,
                         float inputTop,
                         float inputWidth,
                         float inputHeight,
                         float outputLeft,
                         float outputTop,
                         float outputWidth,
                         float outputHeight,
                         read_only image2d_t input,
                         write_only image2d_t output,
                         sampler_t sampler )
{
	size_t x = get_global_id(0);
	size_t y = get_global_id(1);
	int width = get_global_size(0);
	int height = get_global_size(1);
	
	float nX = x/(float)(width-1);
	float nY = y/(float)(height-1);
	float inputX = inputLeft+inputWidth*nX;
	float inputY = inputTop+inputHeight*nY;
	float outputX = outputLeft+width*outputWidth*nX;
	float outputY = outputTop+height*outputHeight*nY;
	float4 rgba = read_imagef( input, sampler, (float2)(inputX,inputY) );
	write_imagef(output,convert_int2((float2)(outputX,outputY)),rgba);
}
/*
kernel void FilterHorz( read_only image2d_t input,
                        write_only image2d_t output,
                        sampler_t sampler,
                        local float4* pFilter,
                        int filterWidth )
{
	int width = get_global_size(0);
	int height = get_global_size(1);
	float pixelWidth = 1.0f/width;
	float pixelHeight = 1.0f/height;
	float y = pixelHeight/2+get_global_id(1)*pixelHeight;
	int filterWidthHalf = filterWidth/2;

	for( float x=pixelWidth/2; x<1.0f; x+=pixelWidth )
	{
	    float4 rgba = 0;
        for( int df=0; df<filterWidth; df++ )
		    rgba += read_imagef( input, sampler, (float2)(x+(df-filterWidthHalf)*pixelWidth,y) )*pFilter[df];
        write_imagef(output,convert_int2((float2)(x*width,y*height)),rgba);
    }
	
	float nX = x/(float)(width-1);
	float nY = y/(float)(height-1);
	float inputX = inputLeft+inputWidth*nX;
	float inputY = inputTop+inputHeight*nY;
	float outputX = outputLeft+width*outputWidth*nX;
	float outputY = outputTop+height*outputHeight*nY;
	float4 rgba = read_imagef( input, sampler, (float2)(inputX,inputY) );
	write_imagef(output,convert_int2((float2)(outputX,outputY)),rgba);
}
*/