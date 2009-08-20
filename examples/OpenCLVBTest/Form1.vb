Imports System.IO
Imports System.Drawing.Imaging
Imports OpenCLNet


Public Class Form1
    Dim clATI
    Dim openCL
    Dim platform
    Dim context
    Dim program
    Dim kernel
    Dim bitmap
    Dim cq
    Dim devices

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            clATI = New OpenCLNet.ATI()
            openCL = New OpenCLNet.OpenCL(clATI)
            platform = openCL.GetPlatform(0)
            devices = platform.QueryDevices(DeviceType.ALL)
            context = platform.CreateDefaultContext(Nothing, Nothing)
            cq = context.CreateCommandQueue(devices(0), 0)
            program = context.CreateProgramWithSource(System.IO.File.ReadAllText("Mandelbrot.cl"))
            program.Build()
        Catch ex As Exception
            Print(ex.ToString())
        End Try
        kernel = program.CreateKernel("Mandelbrot")
        bitmap = New Bitmap(1024, 1024, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
    End Sub

    Private Sub DrawMandelbrot(ByVal g As Graphics)
        CalcMandelbrot()
        g.DrawImageUnscaled(bitmap, 0, 0)
    End Sub

    Private Sub CalcMandelbrot()
        Dim bd = bitmap.LockBits(New Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb)
        Dim bitmapSize = bd.Stride * bd.Height * 4
        Dim mandelbrotMemBuffer = context.CreateBuffer(MemFlags.WRITE_ONLY + MemFlags.USE_HOST_PTR, bitmapSize, bd.Scan0)
        Dim _event = IntPtr.Zero
        Dim globalWorkSize(0 To 1) As IntPtr
        Dim eventWaitList(0 To 0) As IntPtr
        Dim _Left As Single
        Dim _Top As Single
        Dim _Right As Single
        Dim _Bottom As Single

        _Left = -2.0
        _Top = 2.0
        _Right = 2.0
        _Bottom = -2.0
        kernel.SetArg(0, _Left)
        kernel.SetArg(1, _Top)
        kernel.SetArg(2, _Right)
        kernel.SetArg(3, _Bottom)
        kernel.SetArg(4, bd.Stride)
        kernel.SetArg(5, mandelbrotMemBuffer)

        globalWorkSize(0) = New IntPtr(CType(bd.Width, Long))
        globalWorkSize(1) = New IntPtr(CType(bd.Height, Long))
        cq.EnqueueNDRangeKernel(kernel, 2, Nothing, globalWorkSize, Nothing, 0, Nothing, _event)
        eventWaitList(0) = _event
        openCL.CL.WaitForEvents(1, eventWaitList)

        openCL.CL.ReleaseEvent(_event)
        bitmap.UnlockBits(bd)
        mandelbrotMemBuffer.Dispose()
    End Sub

    Private Sub Form1_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint
        Try
            If IsNothing(openCL) Then
                Return
            End If
            DrawMandelbrot(e.Graphics)

        Catch ex As Exception
            Print(ex.ToString())
        End Try
    End Sub
End Class
