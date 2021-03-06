Function Startup(ByVal args As String()) As Integer

	Dim theSession As Session = Session.GetSession()
	Dim theUI As UI = UI.GetUI()
	Dim theUfSession As UFSession = UFSession.GetUFSession()

	Try

		'Dim partTag As Tag = Nothing
		'Dim status As UFPart.LoadStatus = Nothing
		'theUfSession.Part.Open("C:\Users\Prajwal Rao\Documents\Visual Studio 2015\Projects\TEST_NXOPEN_VB\TEST.prt",
		'                   partTag, status)
		'Dim workPart As Part = theSession.Parts.Work()
		Dim workPart As Part = theSession.Parts.Work
		Dim displayPart As Part = theSession.Parts.Display

		Dim sketch As Sketch = Nothing
		Dim sketchBuilder As SketchInPlaceBuilder = workPart.Sketches.CreateNewSketchInPlaceBuilder(sketch)

		Dim sketchplane As DatumPlane = Nothing
		Dim sketchaxis As DatumAxis = Nothing

		For Each datumelem As Object In workPart.Datums
			Dim datumplane As DatumPlane = TryCast(datumelem, DatumPlane)

			If datumplane Is Nothing Then
			Else
				If datumplane.Normal.X = 0 And datumplane.Normal.Y = 0 And datumplane.Normal.Z = 1 Then
					sketchplane = datumplane
				End If
			End If

			Dim datumAxis As DatumAxis = TryCast(datumelem, DatumAxis)

			If datumAxis Is Nothing Then
			Else
				If datumAxis.Direction.X = 1 Then
					sketchaxis = datumAxis
				End If
			End If
		Next

		sketchBuilder.PlaneOrFace.Value = sketchplane
		sketchBuilder.Axis.Value = sketchaxis
		sketchBuilder.SketchOrigin = workPart.Points.CreatePoint(New Point3d(0, 0, 0))

		sketch = sketchBuilder.Commit()
		sketchBuilder.Destroy()

		sketch.Activate(Sketch.ViewReorient.True)

		Dim count As Integer
		Dim partList() As Tag = Nothing
		Dim erList() As Integer = Nothing
		theUfSession.Part.SaveAll(count, partList, erList)
		'theUfSession.Part.CloseAll()

	Catch E1 As Exception

		Dim logPath As String = "C:\Users\Prajwal Rao\Documents\Visual Studio 2015\Projects\TEST_NXOPEN_VB\NXErrorLog.log"
		Using SW = New StreamWriter(logPath)
			SW.WriteLine("Some error occurred:")
			SW.WriteLine(E1.Message)
		End Using

		Dim count As Integer
		Dim partList() As Tag = Nothing
		Dim erList() As Integer = Nothing
		theUfSession.Part.SaveAll(count, partList, erList)
		theUfSession.Part.CloseAll()

	End Try

	' TODO: Add your application code here 

	Return 0

End Function