kernel void CrossFade( float ratio,
                         size_t width,
                         size_t height,
                         global uchar* pInput0,
                         size_t inputStride0,
                         global uchar* pInput1,
                         size_t inputStride1,
                         global uchar* pOutput,
                         size_t outputStride )
{
	size_t x = get_global_id(0);
	size_t y = get_global_id(1);
	uchar4 argb0;
	uchar4 argb1;
	uchar4 mixed;
	
	argb0 = vload4( x, pInput0+y*inputStride0 );
	argb1 = vload4( x, pInput1+y*inputStride1 );
	mixed = convert_uchar4_rte( mix( convert_float4(argb0), convert_float4(argb1), ratio ) );
	vstore4(mixed, x, pOutput+y*outputStride );
}
