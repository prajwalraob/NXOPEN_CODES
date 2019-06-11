public static int Startup()
{
	try
	{
		theProgram = new Program();

		string partPath = @"F:\CASE\FLANGE(10K_150A_G)_K3029930#1.prt";
		PartLoadStatus PLD;
		Part workPart = theSession.Parts.OpenBaseDisplay(partPath, out PLD) as Part;

		if (workPart != null)
		{
			DrawingSheetCollection sheets = workPart.DrawingSheets;
			DrawingSheet sheet1 = null;
			DrawingSheetBuilder builder = sheets.DrawingSheetBuilder(sheet1);

			builder.Height = 841;
			builder.Length = 1189;

			builder.Revision = "A";
			builder.Number = "1";

			builder.MetricSheetTemplateLocation = "\\\\Inblqtsh01\\qtsh\\UGS\\NX11\\UGII\\templates\\Drawing-A0-Size2D-template.prt";
			builder.MetricSheetTemplateLocation = "\\\\Inblqtsh01\\qtsh\\UGS\\NX11\\UGII\\templates\\Drawing-A0-Size2D-template.prt";

			sheet1 = builder.Commit() as DrawingSheet;
			builder.Destroy();
			sheet1.Open();
			//sheet1 = sheets.ToArray()[0];

			BaseView baseView = null;
			BaseViewBuilder viewBuilder = workPart.DraftingViews.CreateBaseViewBuilder(baseView);

			ModelingViewCollection views = workPart.ModelingViews;
			viewBuilder.SelectModelView.SelectedView = views.FindObject("Top");

			viewBuilder.Style.ViewStyleBase.Part = workPart;

			viewBuilder.Style.ViewStyleBase.PartName = workPart.FullPath;

			viewBuilder.Scale.Denominator = 3;

			viewBuilder.Placement.Placement.SetValue(null, workPart.Views.WorkView, new Point3d(100, 100, 0));

			baseView = viewBuilder.Commit() as BaseView;
			viewBuilder.Destroy();
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