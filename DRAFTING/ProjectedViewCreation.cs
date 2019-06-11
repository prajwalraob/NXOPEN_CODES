   public static int Startup()
    {
        try
        {
            theProgram = new Program();
            theSession.ListingWindow.Open();
            string partPath = @"F:\CASE\FLANGE(10K_150A_G)_K3029930#1.prt";
            PartLoadStatus PLD;
            Part workPart = theSession.Parts.OpenBaseDisplay(partPath, out PLD) as Part;

            if (workPart != null)
            {
                DrawingSheet[] sheets = workPart.DrawingSheets.ToArray();
                DrawingSheet sheet = sheets[0];
                sheet.Open();
                DraftingView[] view = sheet.SheetDraftingViews.ToArray();
                BaseView baseView = view[0] as BaseView;

                ProjectedView projectdView = null;
                ProjectedViewBuilder builder = workPart.DraftingViews.CreateProjectedViewBuilder(projectdView);
                builder.Parent.View.Value = baseView;
                builder.Placement.Associative = true;
                builder.Placement.AlignmentMethod = ViewPlacementBuilder.Method.Horizontal;
                builder.Placement.AlignmentVector = null;
                builder.Placement.AlignmentView.Value = null;
                builder.Placement.AlignmentPoint.Value = null;
                builder.Placement.AlignmentOption = ViewPlacementBuilder.Option.ToView;
                builder.Placement.AlignmentView.Value = baseView;

                Point3d point3 = new Point3d(650.21254406580488, 407.83805816686248, 0.0);
                builder.Placement.Placement.SetValue(null, workPart.Views.WorkView, point3);

                bool valid = builder.Validate();

                projectdView = builder.Commit() as ProjectedView;

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