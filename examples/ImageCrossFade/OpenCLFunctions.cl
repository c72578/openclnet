kernel void CrossFade( float ratio,
                         size_t width,
                         intptr_t height,
                         global uchar* pInput0,
                         intptr_t inputStride0,
                         global uchar* pInput1,
                         intptr_t inputStride1,
                         global uchar* pOutput,
                         intptr_t outputStride )
{
	size_t x = get_global_id(0);
	size_t y = get_global_id(1);
	global uchar4* pI0 = (global uchar4*)(pInput0+y*inputStride0);
	global uchar4* pI1 = (global uchar4*)(pInput1+y*inputStride1);
	global uchar4* pO = (global uchar4*)(pOutput+y*outputStride);
	uchar4 mixed;
	
	mixed = convert_uchar4_sat_rte( mix( convert_float4(pI0[x]), convert_float4(pI1[x]), ratio ) );
	pO[x] = mixed;
}
