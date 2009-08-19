__constant uint Palette[17][4] = 
{
    { 0x00, 0x00, 0x00, 0xFF },
    { 0x00, 0xA8, 0x76, 0xFF },
    { 0x20, 0x7E, 0x62, 0xFF },
    { 0x00, 0x6D, 0x4C, 0xFF },
    { 0x35, 0xD4, 0xA4, 0xFF },
    { 0x5F, 0xD4, 0xB1, 0xFF },
    { 0x0D, 0x56, 0xA6, 0xFF },
    { 0x27, 0x4F, 0x7D, 0xFF },
    { 0x04, 0x35, 0x6C, 0xFF },
    { 0x41, 0x86, 0xD3, 0xFF },
    { 0x68, 0x9A, 0xD3, 0xFF },
    { 0x4D, 0xDE, 0x00, 0xFF },
    { 0x55, 0xA6, 0x2A, 0xFF },
    { 0x32, 0x90, 0x00, 0xFF },
    { 0x7A, 0xEE, 0x3C, 0xFF },
    { 0x99, 0xEE, 0x6B, 0xFF },
    { 0x99, 0xEE, 0x6B, 0xFF },
/*
    {0x00, 0x00, 0x5a, 0xff,},
    {0x39, 0x8c, 0xdb, 0xff,},
    {0x9b, 0xb9, 0xff, 0xff,},
    {0x4a, 0xae, 0x92, 0xff,},
    {0x4a, 0xae, 0x92, 0xff,},*/
};

#define MAXITER 100
#define PALETTELENGTH 16

__constant   int8 i8_01234567 = (int8)( 0, 1, 2, 3, 4, 5, 6, 7 );
__constant   int8 i8_00000000 = (int8)( 0, 1, 2, 3, 4, 5, 6, 7 );
__constant   int8 i8_11111111 = (int8)( 0, 1, 2, 3, 4, 5, 6, 7 );

__kernel void Mandelbrot( float left, float top, float right, float bottom, int stride, __global uchar4* pOutput )
{
  size_t width = get_global_size(0);
  size_t height = get_global_size(1);
  size_t cx = get_global_id(0);
  size_t cy = get_global_id(1);
  float dx = (right-left)/(float)width;
  float dy = (bottom-top)/(float)height;
  
  float x0 = left+dx*(float)cx;
  float y0 = top+dy*(float)cy;
  float x = 0.0f;
  float y = 0.0f;
  int iteration = 0;
  int max_iteration = MAXITER;
  
  while( x*x-y*y<=(2*2) && iteration<max_iteration )
  {
    float xtemp = x*x-y*y+x0;
    y = 2*x*y+y0;
    x = xtemp;
    iteration++;
  }
  int index;
  index = iteration*PALETTELENGTH/MAXITER;
  //index = iteration%PALETTELENGTH;
  float4 color0 = (float4)(Palette[index][0],Palette[index][1],Palette[index][2],Palette[index][3]);
  float4 color1 = (float4)(Palette[index+1][0],Palette[index+1][1],Palette[index+1][2],Palette[index+1][3]);
  float mixFactor = clamp( (iteration%(MAXITER/PALETTELENGTH))/(float)(MAXITER/PALETTELENGTH), 0.0f, 1.0f);
  float4 mixFactors = (float4)(1.0f-mixFactor, 1.0f-mixFactor, 1.0f-mixFactor, 1.0f);
  float4 mixfc = mix(  color0, color1, mixFactors );
  mixfc = color0*mixFactors+color1*((float4)(1.0f)-mixFactors);
  pOutput[width*cy+cx] = convert_uchar4( mixfc );
}

#if 0
__kernel void MandelbrotVectorized( float left, float top, float right, float bottom, int stride, __global uchar4* pOutput )
{
  size_t width = get_global_size(0);
  size_t height = get_global_size(1);
  size_t cx = get_global_id(0);
  size_t cy = get_global_id(1);
  float dx = (right-left)/(float)width;
  float dy = (bottom-top)/(float)height;

  float8 dxs = (float8)(dxs)*i8_01234567;
  int8 cxs = (int8)(cx)*(int8)(8)+i8_01234567;
  float8 x0s = (float8)(left)+(float8)(cx)+dxs;
  float8 xs = i8_00000000;
  float8 ys = i8_00000000;
  
  float x0 = left+dx*(float)cx;
  float y0 = top+dy*(float)cy;
  float x = 0.0f;
  float y = 0.0f;
  int8 iterations = (int8)(0);
  int8 max_iterations = (int8)(255);

loop:
  float8 doneflags = (float8)(0);
  float8 looptest = (xs*xs-ys*ys)<=(float8)(2*2);
  doneflags |= ~loopTest;
  
  if( !any( (xs*xs-ys*ys)<=(float8)(2*2) ) && all( iterations<max_iterations ) )
    goto done;
    
  float8 xtemp = xs*xs-ys*ys+x0s;
  float8 ytemp = 2*xs*ys+y0s;
  xs = select( xs, xtemp, doneflags );
  ys = select( ys, ytemp, doneflags );

  iterations = select( iterations, iterations+i8_11111111, doneflags );
  goto loop;

done:

  int index;
  color = iteration==max_iteration?0.0f: (float)iteration/255.0f;
  index = clamp( iteration, 0.0f, 255.0f )/64;
  pOutput[width*cy+cx] = (uchar4)(Palette[index][0],Palette[index][1],Palette[index][2],Palette[index][3]);
}
#endif
