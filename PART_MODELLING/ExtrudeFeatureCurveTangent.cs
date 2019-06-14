public static int Startup()
{
	try
	{
		theProgram = new Program();
		theSession.ListingWindow.Open();
		string partPath = @"F:\CASE\TEST_PRT.prt";
		PartLoadStatus PLD;
		Part workPart = theSession.Parts.OpenBaseDisplay(partPath, out PLD) as Part;

		if (workPart != null)
		{
			Curve[] curveCollection = workPart.Curves.ToArray();
			Curve selectionCurve = null;
			theSession.ListingWindow.Open();
			foreach(Curve curve in curveCollection)
			{
				if(curve.Name == "ARC")
				{
					selectionCurve = curve;
				}
			}

			SelectionIntentRule ruleFactory = workPart.ScRuleFactory.CreateRuleCurveTangent(selectionCurve, null, true, 0.005, 0.005);
			SelectionIntentRule[] rules = new SelectionIntentRule[1];
			rules[0] = ruleFactory;

			Extrude extruedFeature = null;
			ExtrudeBuilder builder = workPart.Features.CreateExtrudeBuilder(extruedFeature);

			NXObject nullObj = null;
			Section section = workPart.Sections.CreateSection();
			section.AddToSection(rules, nullObj, nullObj, nullObj, new Point3d(0, 0, 0), Section.Mode.Create);

			builder.Section = section;

			Point3d origin = new Point3d(0, 0, 0);
			Vector3d axis = new Vector3d(0, 0, 1);
			Direction direct = workPart.Directions.CreateDirection(origin,axis,SmartObject.UpdateOption.DontUpdate);
			builder.Direction = direct;

			builder.Limits.StartExtend.Value.RightHandSide = "-0.25";
			builder.Limits.EndExtend.Value.RightHandSide = "0.5";

			bool validation = builder.Validate();
			builder.Commit();

		}

		PartSaveStatus ps;
		bool partSaved;
		theSession.Parts.SaveAll(out partSaved, out ps);

		theUFSession.Part.CloseAll();
	}
	catch(NXException nxe)
	{
		theSession.ListingWindow.Open();
		theSession.ListingWindow.WriteLine(nxe.ToString());
	}
	catch(Exception e)
	{
		theSession.ListingWindow.Open();
		theSession.ListingWindow.WriteLine(e.ToString());
	}

	return 0;
}