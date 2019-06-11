public static int Startup()
{
	try
	{
		theProgram = new Program();
		theSession.ListingWindow.Open();
		string partPath = @"F:\CASE1\PAD(SUPPORT)_K3027821#1.prt";
		PartLoadStatus PLD;
		Part workPart = theSession.Parts.OpenBaseDisplay(partPath, out PLD) as Part;

		if (workPart != null)
		{
			Sketch workSketch = workPart.Sketches.ToArray()[0];
			NXObject[] one = workSketch.GetAllGeometry();
			List<Point> pointsList = new List<Point>();

			foreach(NXObject entity in one)
			{
				if(entity.GetType() == typeof(Point))
				{
					pointsList.Add(entity as Point);
				}
			}

			HolePackage holeFeature = null;
			HolePackageBuilder builder = workPart.Features.CreateHolePackageBuilder(holeFeature);

			builder.ScrewStandard = "ISO";
			builder.ThreadStandard = "Metric Coarse";
			builder.HolePosition.SetAllowedEntityTypes(Section.AllowTypes.OnlyPoints);
			builder.Tolerance = 0.01;
			builder.HolePosition.DistanceTolerance = 0.01;
			builder.HolePosition.ChainingTolerance = 0.01;
			builder.GeneralSimpleHoleDiameter.RightHandSide = "5";

			Section holeSection = workPart.Sections.CreateSection();
			Point[] pointSel = pointsList.ToArray();
			SelectionIntentRule rule = workPart.ScRuleFactory.CreateRuleCurveDumbFromPoints(pointSel);
			SelectionIntentRule[] rules = new SelectionIntentRule[1];
			rules[0] = rule;
			builder.HolePosition.AddToSection(rules, null, null, null, new Point3d(0,0,0), Section.Mode.Create, false);

			Body[] bodies = workPart.Bodies.ToArray();
			builder.BooleanOperation.SetTargetBodies(bodies);

			bool validationState = builder.Validate();

			holeFeature = builder.Commit() as HolePackage;

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