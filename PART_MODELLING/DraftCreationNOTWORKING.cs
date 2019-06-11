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
                Body[] bodies = workPart.Bodies.ToArray();
                Body workBody = bodies[0];

                Face[] faceCollection = workBody.GetFaces();
                Face sideFace = null;
                Face topFace = null;

                foreach(Face face in faceCollection)
                {
                    if (face.Name == "SIDE")
                    {
                        sideFace = face;
                    }

                    if (face.Name == "TOP")
                    {
                        topFace = face;
                    }
                }

                Draft draft1 = null;
                DraftBuilder builder = workPart.Features.CreateDraftBuilder(draft1);
                builder.AngleTolerance = 0.5;
                builder.DistanceTolerance = 0.01;
                builder.DraftIsoclineOrTruedraft = DraftBuilder.Method.Isocline;
                builder.TypeOfDraft = DraftBuilder.Type.Face;
                builder.TwoDimensionFaceSetsData.Clear(ObjectList.DeleteOption.Delete);
                builder.FaceSetAngleExpressionList.Clear(ObjectList.DeleteOption.Delete);
                builder.EdgeSetAngleExpressionList.Clear(ObjectList.DeleteOption.Delete);

                ScCollector topFaceCollector = workPart.ScCollectors.CreateCollector();
                Face[] faceSelection = new Face[1];
                faceCollection[0] = topFace;
                FaceDumbRule selectionRule = workPart.ScRuleFactory.CreateRuleFaceDumb(faceCollection);
                SelectionIntentRule[] selectionIntentRule = new SelectionIntentRule[1];
                selectionIntentRule[0] = selectionRule;

                builder.StationaryReference.ReplaceRules(selectionIntentRule, false);

                ScCollector sidefaceCollector = workPart.ScCollectors.CreateCollector();
                Face[] sideFaceSelection = new Face[1];
                sideFaceSelection[0] = sideFace;
                FaceTangentRule sideSelectionRule = workPart.ScRuleFactory.CreateRuleFaceTangent(sideFace, new Face[0],0.5);
                SelectionIntentRule[] sideSelectionIntentRule = new SelectionIntentRule[1];
                sideSelectionIntentRule[0] = sideSelectionRule;
                sidefaceCollector.ReplaceRules(sideSelectionIntentRule, false);

                NXOpen.GeometricUtilities.TwoExpressionsCollectorSet twoExSet = workPart.CreateTwoExpressionsCollectorSet(sidefaceCollector,".2", "0", "Angle",0);
                builder.TwoDimensionFaceSetsData.Append(twoExSet);

                UnitCollection units = workPart.UnitCollection;
                Unit[] angle = units.GetMeasureTypes("Angle");
                //twoExSet.Collector = sidefaceCollector;
                //twoExSet.ItemValue.RightHandSide = "5";

                if(builder.Validate())
                {
                    builder.CommitFeature();
                }

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