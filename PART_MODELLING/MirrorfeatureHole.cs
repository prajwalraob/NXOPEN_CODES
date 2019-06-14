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
			theSession.ListingWindow.Open();
			Feature[] features = workPart.Features.ToArray();
			DisplayableObject[] datums =  workPart.Datums.ToArray();

			DatumPlane selectDatum = null;
			HolePackage hole = null;
			foreach(Feature feature in features)
			{
				if(feature.FeatureType == "HOLE PACKAGE")
				{
					hole = feature as HolePackage;
					break;
				}
			}
			foreach(DisplayableObject datum in datums)
			{
				if(datum.Name == "DATUM")
				{
					selectDatum = datum as DatumPlane;
					break;
				}
			}

			Mirror mirror = null;
			MirrorBuilder builder = workPart.Features.CreateMirrorBuilder(mirror);

			builder.PatternService.PatternType = NXOpen.GeometricUtilities.PatternDefinition.PatternEnum.Mirror;
			builder.FeatureList.Add(hole);
			builder.PatternService.MirrorDefinition.ExistingPlane.Value = selectDatum;
			builder.ReferencePointService.Point = workPart.Points.CreatePoint(hole.Location);
			builder.ParentFeatureInternal = false;

			bool val = builder.Validate();
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
}