public static int Startup()
{
	try
	{
		theProgram = new Program();

		string partPath = @"F:\CASE\ASM.prt";
		PartLoadStatus PLD;
		Part workPart = theSession.Parts.OpenBaseDisplay(partPath, out PLD) as Part;

		if (workPart != null)
		{
			Component root = workPart.ComponentAssembly.RootComponent;
			Traverse(root);

			Component selectComp = null;

			foreach(Component comp in leafList)
			{
				if (comp.DisplayName == "FLANGE(10K_150A_G)_K3029930#1")
				{
					selectComp = comp;
				}
			}

			Part selectPart = selectComp.Prototype as Part;
			Body[] bodies = selectPart.Bodies.ToArray();
			Body thisBody = bodies[0];

			Body protoBody =selectComp.FindOccurrence(thisBody) as Body;

			NXOpen.Session.UndoMarkId markId1;
			markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

			NXOpen.Features.Feature nullNXOpen_Features_Feature = null;
			NXOpen.Features.WaveLinkBuilder waveLinkBuilder1;
			waveLinkBuilder1 = workPart.BaseFeatures.CreateWaveLinkBuilder(nullNXOpen_Features_Feature);

			NXOpen.Features.WaveDatumBuilder waveDatumBuilder1;
			waveDatumBuilder1 = waveLinkBuilder1.WaveDatumBuilder;

			NXOpen.Features.CompositeCurveBuilder compositeCurveBuilder1;
			compositeCurveBuilder1 = waveLinkBuilder1.CompositeCurveBuilder;

			NXOpen.Features.WaveSketchBuilder waveSketchBuilder1;
			waveSketchBuilder1 = waveLinkBuilder1.WaveSketchBuilder;

			NXOpen.Features.WaveRoutingBuilder waveRoutingBuilder1;
			waveRoutingBuilder1 = waveLinkBuilder1.WaveRoutingBuilder;

			NXOpen.Features.WavePointBuilder wavePointBuilder1;
			wavePointBuilder1 = waveLinkBuilder1.WavePointBuilder;

			NXOpen.Features.ExtractFaceBuilder extractFaceBuilder1;
			extractFaceBuilder1 = waveLinkBuilder1.ExtractFaceBuilder;

			NXOpen.Features.MirrorBodyBuilder mirrorBodyBuilder1;
			mirrorBodyBuilder1 = waveLinkBuilder1.MirrorBodyBuilder;

			NXOpen.GeometricUtilities.CurveFitData curveFitData1;
			curveFitData1 = compositeCurveBuilder1.CurveFitData;

			curveFitData1.Tolerance = 0.01;

			curveFitData1.AngleTolerance = 0.5;

			extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;

			waveLinkBuilder1.Type = NXOpen.Features.WaveLinkBuilder.Types.BodyLink;

			extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;

			extractFaceBuilder1.AngleTolerance = 45.0;

			waveDatumBuilder1.DisplayScale = 2.0;

			extractFaceBuilder1.ParentPart = NXOpen.Features.ExtractFaceBuilder.ParentPartType.OtherPart;

			mirrorBodyBuilder1.ParentPartType = NXOpen.Features.MirrorBodyBuilder.ParentPart.OtherPart;

			theSession.SetUndoMarkName(markId1, "WAVE Geometry Linker Dialog");

			compositeCurveBuilder1.Section.DistanceTolerance = 0.01;

			compositeCurveBuilder1.Section.ChainingTolerance = 0.0094999999999999998;

			compositeCurveBuilder1.Section.AngleTolerance = 0.5;

			compositeCurveBuilder1.Section.DistanceTolerance = 0.01;

			compositeCurveBuilder1.Section.ChainingTolerance = 0.0094999999999999998;

			extractFaceBuilder1.Associative = true;

			extractFaceBuilder1.MakePositionIndependent = false;

			extractFaceBuilder1.FixAtCurrentTimestamp = false;

			extractFaceBuilder1.HideOriginal = false;

			extractFaceBuilder1.InheritDisplayProperties = false;

			NXOpen.ScCollector scCollector1;
			scCollector1 = extractFaceBuilder1.ExtractBodyCollector;

			extractFaceBuilder1.CopyThreads = true;

			extractFaceBuilder1.FeatureOption = NXOpen.Features.ExtractFaceBuilder.FeatureOptionType.OneFeatureForAllBodies;

			NXOpen.Body[] bodies1 = new NXOpen.Body[1];
			bodies1[0] = protoBody;
			NXOpen.BodyDumbRule bodyDumbRule1;
			bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies1, true);

			NXOpen.SelectionIntentRule[] rules1 = new NXOpen.SelectionIntentRule[1];
			rules1[0] = bodyDumbRule1;
			scCollector1.ReplaceRules(rules1, false);

			NXOpen.Session.UndoMarkId markId2;
			markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "WAVE Geometry Linker");

			NXOpen.NXObject nXObject1;
			nXObject1 = waveLinkBuilder1.Commit();

			theSession.DeleteUndoMark(markId2, null);

			theSession.SetUndoMarkName(markId1, "WAVE Geometry Linker");

			waveLinkBuilder1.Destroy();
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