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
			Body partBody = workPart.Bodies.ToArray()[0];
			Face[] faces = partBody.GetFaces();
			Face selectFace = null;
			foreach(Face face in faces)
			{
				if(face.Name == "TOP")
				{
					selectFace = face;
				}
			}

			Thicken thk = null;
			ThickenBuilder builder = workPart.Features.CreateThickenBuilder(thk);
			builder.Tolerance = 0.01;
			builder.RegionToPierce.DistanceTolerance = 0.01;
			builder.RegionToPierce.AngleTolerance = 0.001;
			builder.RegionToPierce.ChainingTolerance = 0.001;

			SelectionIntentRule rule = workPart.ScRuleFactory.CreateRuleFaceDumb(new Face[1] { selectFace });
			builder.FaceCollector.ReplaceRules(new SelectionIntentRule[1] { rule }, false);

			builder.FirstOffset.RightHandSide = "5";
			builder.BooleanOperation.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;
			builder.BooleanOperation.SetTargetBodies(new Body[] { partBody });
			builder.Commit();
		}

		PartSaveStatus ps;
		bool partSaved;
		theSession.Parts.SaveAll(out partSaved, out ps);

		theUFSession.Part.CloseAll();
	}
	catch (NXException nxe)
	{
		theSession.ListingWindow.Open();
		theSession.ListingWindow.WriteLine(nxe.ToString());
	}
	catch (Exception e)
	{
		theSession.ListingWindow.Open();
		theSession.ListingWindow.WriteLine(e.ToString());
	}

	return 0;
// }