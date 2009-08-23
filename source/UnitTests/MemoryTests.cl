/*
 * Copyright (c) 2009 Olav Kalgraf(olav.kalgraf@gmail.com)
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

__constant char TestMemory[] = "TestMemory";

__kernel void MemoryCopy( __global float* pSrc, __global float* pDst, size_t length )
{
	__global float* pEnd;
	
	pEnd = pSrc+length;
	while( pSrc<pEnd )
		*pDst++ = *pSrc++;
}

__kernel void LoopAndDoNothing( int iterations )
{
	for( int i=0; i<iterations; i++ )
		;
}

struct IOKernelArgs
{
    double outDouble;
    long outLong;
    int outInt;
    float outSingle;
    intptr_t outIntPtr;
};

__kernel void ArgIO( int i,
  long l,
  float s,
  double d,
  intptr_t p,
  __global struct IOKernelArgs* pA)
{
	pA->outInt = i;
	pA->outLong = l;
	pA->outSingle = s;
	pA->outDouble = d;
	pA->outIntPtr = p;
}

__kernel void TestReadMemory( __global char* pData, size_t length )
{
	int sum;
	
	for( size_t i=0; i<length; i++ )
		sum += pData[i];
}

__kernel void TestWriteMemory( __global char* pData, size_t length )
{
	for( size_t i=0; i<length; i++ )
		pData[i] = 1;
}

__kernel void TestReadWriteMemory( __global char* pData, size_t length )
{
	for( size_t i=0; i<length; i++ )
		pData[length-1-i] = pData[i];
}
