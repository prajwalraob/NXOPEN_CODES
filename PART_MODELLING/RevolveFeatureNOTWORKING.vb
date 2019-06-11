Function Startup(ByVal args As String()) As Integer

	Dim theSession As Session = Session.GetSession()
	Dim theUI As UI = UI.GetUI()
	Dim theUfSession As UFSession = UFSession.GetUFSession()

	Try

		Dim partTag As Tag = Nothing
		Dim status As UFPart.LoadStatus = Nothing
		theUfSession.Part.Open("C:\Users\Prajwal Rao\Documents\Visual Studio 2015\Projects\TEST_NXOPEN_VB\TEST.prt",
						   partTag, status)
		Dim workPart As Part = theSession.Parts.Work

		Dim collect As SketchCollection = workPart.Sketches
		Dim sks As Sketch() = collect.ToArray()
		Dim sketch1 As Sketch = sks(0)
		Dim sketchCurves(3) As Curve
		Dim cnt = 0
		For Each cuvs As Object In sketch1.GetAllGeometry()
			sketchCurves(cnt) = TryCast(cuvs, Curve)
			cnt += 1
		Next

		Dim dumbRule As CurveDumbRule = workPart.ScRuleFactory.CreateRuleCurveDumb(sketchCurves)
		Dim rule As SelectionIntentRule() = {dumbRule}

		Dim nullObj As NXObject = Nothing
		Dim section As Section = workPart.Sections.CreateSection
		section.AddToSection(rule, nullObj, nullObj, nullObj, New Point3d(0, 0, 0), Section.Mode.Create)

		Dim revolveBuilder As Features.RevolveBuilder = workPart.Features.CreateRevolveBuilder(Nothing)
		revolveBuilder.Section = section

		Dim origin As New NXOpen.Point3d(0, 0, 0)
		Dim axisX As New NXOpen.Vector3d(1, 0, 0)
		Dim updateOption = SmartObject.UpdateOption.DontUpdate

		Dim direction As Direction = workPart.Directions.CreateDirection(origin, axisX, updateOption)


		Dim axis As Axis = workPart.Axes.CreateAxis(origin, axisX, updateOption)

		revolveBuilder.Axis = axis

		'workPart.Directions.CreateDirection(origin, axisZ, updateOption)

		revolveBuilder.Limits.StartExtend.Value.RightHandSide = "0"
		revolveBuilder.Limits.EndExtend.Value.RightHandSide = "60"

		revolveBuilder.Commit()

		revolveBuilder.Destroy()

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