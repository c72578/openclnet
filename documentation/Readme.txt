AUgust 22, 2009
---------------
Started work on a unit tester, which apparently was a good idea. Several embarrassing bugs were eliminated in the process, such as infinite recursion in a few of the wrapper functions where I had forgotten to add the "cl" prefix. What fun. =)

Also fleshed out the low level API a bit more. I think there's only one function missing for full OpenCL 1.0 support at that level.

Added a bunch of new SetXXXArg functions to the Kernel class. They should be a bit easier to use from visual basic than the overloaded SetArg.

What has been tested at this point could be said to boil down to "that which does not involve images". Which has a natural explanation. ATI's version of OpenCL doesn't support them yet. Even here there are some exceptions, but they're getting fewer as I flesh out the unit-tester. The high level API is certainly usable at this point and far more fun to play around with than the C API. I'm not promising that it's frozen or anything, but feel free to play with it.

There are two caveats though:

1) If you're going to make your own project to test stuff with, be aware that ATI's driver refuses to run clBuild() in the managed debugger(I think there may be some environment issue so it can't find its own compiler or something), so you have to enable native debugging in the project's debug tab.

2) I have no idea if it even runs with Nvidia's driver. I'd be a bit surprised if it didn't, but feel free to tell me if it works or not. =) I'll buy an Nvidia card once the DX11 cards hit the shelves, but that's probably a few months off. In the meantime I'll focus on testing the other stuff and using this library in the private project that motivated it in the first place.



August 20, 2009
---------------

Got a basic VB project up and running and cleaned up the OpenCL event handling a bit.
All the Enqueue* methods now have two new overloads. One that chops off the "out event" parameter,
and another that also chops out the event wait list and its size parameter.

This solves the problem of forcing the caller to process events for every single enqueued function
without dragging pointers all the way to the high level API.

Also changed the SetArg(*) methods so they take the parameter number as an integer instead of as an
uint to make things less painful for VB. I'm also considering adding aliases for SetArg that specify
the type in the method name due to cryptic runtimeerror messages in VB when dealing with the overloaded ones
that are so convenient in strongly typed languages.

I've decided to leave events as IntPtrs for now, even though an Event weapper class has been checked in.
There's not really much use for an event class other than to garbage collect events(Which has a dangerous aspect to it. It might be better to fail fast during development if there are that kind of memory leaks present.)
and make code look prettier due to better type checking. I haven't completely decided yet, we'll see what happens after the unit tests are done. Converting to use an event class should mostly involve a few regular expression search and replaces anyway.


August 19, 2009
---------------

OpenCLNet - Quick feature walkthough

The library includes a low level c# bridge to OpenCL 1.0 that's almost complete. On top of that, we have classes for each of the major concepts in OpenCL.

Platform
Device
Context
Mem
Sampler
Program
Kernel
CommandQueue

These classes expose all OpenCL information as properties, which is great for debugging purposes, as you have all information at your fingertips in the debugger and can drill down through objects to figure out what's going on rather than having to write code to dump debug information into text files. At the moment, not all of them are complete, and much remains untested.
Error handling is exception based and results in far more readable code than the C equivalent.

A quick example:

OpenCL openCL = new OpenCL(new ATI());
Platform openCLPlatform = openCL.GetPlatform( 0 );
Context openCLContext = openCLPlatform.CreateDefaultContext( null, IntPtr.Zero );
Program mandelBrotProgram = openCLContext.CreateProgramWithSource( File.ReadAllText( "Mandelbrot.cl" ) );
mandelBrotProgram.Build();
Kernel mandelbrotKernel = mandelBrotProgram.CreateKernel( "Mandelbrot" );
Device[] openCLDevices = openCLPlatform.QueryDevices( DeviceType.ALL );
CommandQueue openCLCQ = openCLContext.CreateCommandQueue( openCLDevices[0], (CommandQueueProperties)0 );

//... blah blah... lots of other stuff happens, and then we want to call the kernel

mandelbrotKernel.SetArg( 0, Left );
mandelbrotKernel.SetArg( 1, Top );
mandelbrotKernel.SetArg( 2, Right );
mandelbrotKernel.SetArg( 3, Bottom );
mandelbrotKernel.SetArg( 4, bd.Stride );
mandelbrotKernel.SetArg( 5, mandelbrotMemBuffer );

IntPtr _event;
IntPtr[] globalWorkSize = new IntPtr[2] { new IntPtr( BitmapWidth ), new IntPtr( BitmapHeight ) };
IntPtr[] localWorkSize = new IntPtr[2] { new IntPtr( 1 ), new IntPtr( 1 ) };
openCLCQ.EnqueueNDRangeKernel( mandelbrotKernel, 2u, null, globalWorkSize, null, 0, null, out _event );

openCL.CL.WaitForEvents( 1, new IntPtr[] { _event } );
openCL.CL.ReleaseEvent( _event );


Or, in english, instantiate an OpenCL API from ATI's DLL(this will likely work also on nvidia systems and will go away in the
future unless there are major interop bugs that demand vendor specific code in a low layer). Grab platform 0. The platform class also contains methods to enumerate them, if desired. Create a context to get at the more meatier functionality. Use the context to load and build a source. Extract the known kernel function, create a command queue, set arguments and fire off the kernel. Setting arguments is very simple in most cases, as there are overloads for most common types. In the event that you have to pass structs by value, or something like that, something similar to the raw memory copy function of the C api is also available. Once the arguments are set, we can just enqueue a kernel operation on the commandqueue with EnqueueNDRangeKernel.

Waiting for results is one thing that will change in the public API before any release is made. There are two questions regarding that: One is if it's worthwhile to create a class to wrap events and simply live with the fact that we'll have to allocate a dynamic object for each enqueued operation. The other thing is that the current high level API demands that every function that can return an event does so. It's an out parameter, so you can't specify a null pointer. I'm leading towards overloading all functions that can return events with ones that lack the trailing "out event" argument and just pass null pointers transparently below the highlevel API.

The basic initialization would probably be wrapped in a try{}catch(OpenCLException){} block. OpenCLExceptions contain both a text string explaining th cause of the error, as well as the OpenCL error code that caused the exception.







The DLL imports are close to complete, but inaccesible to some .Net languages due to use of low level features.
This will probably not change. What will change, however, is that the high level API will get a cleanup so it will interoperate across the board. It should be pretty close already, but I haven't had time to check. As it is, C# appears to work for simple tests, but a more thorough test application is needed.

Currently, the low level API is referenced through an instance of the ATI class that implements the OpenCLAPI interface. I'm a bit unsure about whether that abstraction is needed or not, but I decided to do it this way both because I don't have an nvidia card in my development system and because it could be nice to have a place to plug in workarounds for vendor specific bugs. If you stick to the high level API and bootstrap the system from the OpenCL class, as shown in the OpenCLTest example, you should pretty much be immune to such low-level issues however.

Immediate future plans include finishing the last few fuctions of the lowlevel API and removing non-interopable constructs from the high level classes so I can get a simple example in visual basic up and running. Once that's done, it's probably time to write unit tests to make sure I haven't screwed up any of the function bridges.
