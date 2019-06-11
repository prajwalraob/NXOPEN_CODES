Imports NXOpen
Imports NXOpen.Assemblies
Imports NXOpen.UF
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.Diagnostics

Imports System
Imports System.IO
Imports System.Collections.Generic

Public Class NX_A

    Private Shared theSession As Session
    Private Shared theUFSession As UFSession
    Private Shared theProgram As NX_A

    Sub New()

        Try
            theSession = TryCast(System.Activator.GetObject(GetType(Session), "http://localhost:4567/NXOpenSession"), Session)
            theUFSession = TryCast(System.Activator.GetObject(GetType(UFSession), "http://localhost:4567/UfSession"), UFSession)
        Catch

        End Try

    End Sub

    Public Shared Function Main(args() As String) As Integer

        Try

            theProgram = New NX_A()
            'Dim partPath As String = "C:\Users\Prajwal Rao\Documents\Visual Studio 2015\Projects\CarPartFiles\Car_Assembly.prt"
            'Dim tag As Tag = Nothing
            'Dim loadStatus As UFPart.LoadStatus = Nothing
            'theUFSession.Part.Open(partPath, tag, loadStatus)

            Dim workPart As Part = theSession.Parts.Work
            Dim asm As ComponentAssembly = workPart.ComponentAssembly

            Dim bl = asm.RootComponent.GetChildren()
            Dim na As Part = TryCast(bl(0).Prototype, Part)

            Dim asn As Body = na.Bodies(0)
            Dim asss() As Face = asn.GetFaces()

            Console.WriteLine(na)
            Console.WriteLine("Good")
            Console.ReadKey()

        Catch e As Exception

            Using Sw = New StreamWriter("ErrorLog.log")
                Sw.WriteLine("Error occurred")
                Sw.WriteLine(e.ToString())
            End Using

        End Try

        Return 0
    End Function
    Public Shared Function ObjectFromTag(tag As NXOpen.Tag) As NXOpen.NXObject
        Dim obj As NXOpen.TaggedObject = NXOpen.Utilities.NXObjectManager.Get(tag)
        Dim nxObject As NXOpen.NXObject = CType(obj, NXOpen.NXObject)
        Return nxObject
    End Function
    Public Shared Function Startup() As Integer


        Return 0
    End Function

End Class
