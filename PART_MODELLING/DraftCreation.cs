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
                Face topFace = null;
                Face sideFace = null;

                Face[] bodyFaces = partBody.GetFaces();

                foreach(Face face in bodyFaces)
                {
                    if(face.Name == "TOP")
                    {
                        topFace = face;
                    }
                    if (face.Name == "SIDE")
                    {
                        sideFace = face;
                    }
                }

                Draft draft = null;
                DraftBuilder builder = workPart.Features.CreateDraftBuilder(draft);

                builder.AngleTolerance = 0.01;
                builder.DistanceTolerance = 0.01;

                Point3d origin = new Point3d(0, 0, 0);
                Vector3d axis = new Vector3d(0, 0, 1);
                Direction direct = workPart.Directions.CreateDirection(origin, axis, SmartObject.UpdateOption.DontUpdate);
                builder.Direction = direct;


                FaceDumbRule stationary = workPart.ScRuleFactory.CreateRuleFaceDumb(new Face[1] { topFace });
                builder.StationaryReference.ReplaceRules(new SelectionIntentRule[1] { stationary },false);

                FaceTangentRule sides = workPart.ScRuleFactory.CreateRuleFaceTangent(sideFace, new Face[0]);
                ScCollector collector = workPart.ScCollectors.CreateCollector();
                collector.ReplaceRules(new SelectionIntentRule[1] {sides},false);
                NXOpen.GeometricUtilities.TwoExpressionsCollectorSet collectorSet = workPart.CreateTwoExpressionsCollectorSet(collector, "5", "5", "Angle", 0);
                builder.TwoDimensionFaceSetsData.Append(collectorSet);

                bool status = builder.Validate();
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