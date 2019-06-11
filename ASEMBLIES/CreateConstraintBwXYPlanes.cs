using System;
using System.Collections.Generic;
using NXOpen;
using NXOpen.UF;
using NXOpen.Assemblies;
using NXOpen.Features;
using NXOpen.Positioning;

public class Program
{
    // class members
    private static Session theSession;
    private static UI theUI;
    private static UFSession theUfSession;
    public static Program theProgram;
    public static bool isDisposeCalled;
    private List<Component> leafList;

    //------------------------------------------------------------------------------
    // Constructor
    //------------------------------------------------------------------------------
    public Program()
    {
        try
        {
            theSession = Session.GetSession();
            theUI = UI.GetUI();
            theUfSession = UFSession.GetUFSession();
            isDisposeCalled = false;
            leafList = new List<Component>();
        }
        catch (NXOpen.NXException ex)
        {
            // ---- Enter your exception handling code here -----
            // UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, ex.Message);
        }
    }

    //------------------------------------------------------------------------------
    //  Explicit Activation
    //      This entry point is used to activate the application explicitly
    //------------------------------------------------------------------------------
    public static int Main(string[] args)
    {
        int retValue = 0;
        try
        {
            theProgram = new Program();

            theProgram.Dispose();
        }
        catch (NXOpen.NXException ex)
        {
            // ---- Enter your exception handling code here -----

        }
        return retValue;
    }



    //------------------------------------------------------------------------------
    //  NX Startup
    //      This entry point activates the application at NX startup

    //Will work when complete path of the dll is provided to Environment Variable 
    //USER_STARTUP or USER_DEFAULT
    //------------------------------------------------------------------------------
    public static int Startup()
    {
        int retValue = 0;
        try
        {
            theProgram = new Program();

            //string partPath = @"F:\TEST.prt";
            //string partPath = @"F:\CASE\ASM.prt";
            string partPath = @"F:\CASE1\Case_1-1_stp.prt";
            PartLoadStatus PLD;
            Part workPart = theSession.Parts.OpenBaseDisplay(partPath, out PLD) as Part;

            if (workPart != null)
            {
                ComponentAssembly assembly = workPart.ComponentAssembly;
                Component rootComponent = assembly.RootComponent;      
                //try {object flange1 = assembly.FindObject("FLANGE(10K_150A_G)_K3029930#1");}
                //catch(Exception ex) { theUI.NXMessageBox.Show("Null", NXMessageBox.DialogType.Error, ex.Message); }

                theProgram.Traverse(rootComponent);

                Component comp1_1 = rootComponent.FindObject("COMPONENT OIL_PIPE_TYPE1_ASSY-01 2") as Component;
                Component comp1 = comp1_1.FindObject("COMPONENT FLANGE(10K_150A_G)_K3029930#1 1") as Component;
                Component comp2_1 = rootComponent.FindObject("COMPONENT OIL_PIPE_TYPE1_ASSY-01 3") as Component;
                Component comp2 = comp2_1.FindObject("COMPONENT FLANGE(10K_150A_N)_K3034074#1 1") as Component;

                Part one = comp1.Prototype as Part;
                Part two = comp2.Prototype as Part;

                DatumPlane oneXY = null;
                DatumPlane twoXY = null;

                //foreach(Component leaf in theProgram.leafList)
                //{
                //    if(leaf.DisplayName == "FLANGE(10K_150A_G)_K3029930#1")
                //    {
                //        //comp1 = leaf;
                //        one = leaf.Prototype as Part;
                //    }

                //    if (leaf.DisplayName == "FLANGE(10K_150A_N)_K3034074#1")
                //    {
                //        //comp2 = leaf;
                //        two = leaf.Prototype as Part;
                //    }
                //}

                if (one != null && two!= null)
                {
                    theProgram.GetXYPlane(one, out oneXY);
                    theProgram.GetXYPlane(two, out twoXY);

                    oneXY = comp1.FindOccurrence(oneXY) as DatumPlane;
                    twoXY = comp2.FindOccurrence(twoXY) as DatumPlane;

                }

                //Tag tag1 = theUfSession.Assem.AskPartOccurrence(oneXY.Tag);

                try
                {
                    ComponentPositioner positioner1 = assembly.Positioner;
                    positioner1.BeginAssemblyConstraints();

                    Constraint constraint1 = positioner1.CreateConstraint(true);
                    ComponentConstraint componentConstraint1 = constraint1 as ComponentConstraint;

                    componentConstraint1.ConstraintAlignment = Constraint.Alignment.ContraAlign;
                    componentConstraint1.ConstraintType = Constraint.Type.Touch;
                    ConstraintReference ref1 = componentConstraint1.CreateConstraintReference(comp1, oneXY, false, false, false);
                    ConstraintReference ref2 = componentConstraint1.CreateConstraintReference(comp2, twoXY, false, false, false);
                    //componentConstraint1.FlipAlignment();
                    //componentConstraint1.SetAlignmentHint(NXOpen.Positioning.Constraint.Alignment.ContraAlign);
                }
                catch(Exception E)
                {
                    theUI.NXMessageBox.Show("Error", NXMessageBox.DialogType.Error, E.Message);
                }

            }


            PartSaveStatus ps;
            bool partSaved;
            theSession.Parts.SaveAll(out partSaved, out ps);

            theUfSession.Part.CloseAll();


        }
        catch (NXOpen.NXException ex)
        {
            // ---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("UI Styler", NXMessageBox.DialogType.Error, ex.Message);
        }
        return retValue;
    }

    private void GetXYPlane(Part component, out DatumPlane XYPlane)
    {
        DatumCollection datums = component.Datums;
        object[] datums1 = component.Datums.ToArray();

        List<DatumPlane> datumPlanes = new List<DatumPlane>();

        XYPlane = null;

        foreach (object obj in datums1)
        {
            DatumPlane plane = obj as DatumPlane;
            if (plane != null) datumPlanes.Add(plane);
        }

        foreach (DatumPlane plane in datumPlanes)
        {
            if (plane.Normal.Z == 1)
            {
                XYPlane = plane;
            }
        }

    }

    private void Traverse(Component root)
    {
        try
        {
            Component[] childComponents = root.GetChildren();

            if (childComponents.Length != 0)
            {
                foreach (Component children in childComponents)
                {
                    Traverse(children);
                }
            }
            else
            {
                leafList.Add(root);
            }
        }
        catch (NXOpen.NXException ex)
        {
            theSession.ListingWindow.Open();
            theSession.ListingWindow.WriteLine(ex.ToString());

            // ---- Enter your exception handling code here -----

        }
    }

    //------------------------------------------------------------------------------
    // Following method disposes all the class members
    //------------------------------------------------------------------------------
    public void Dispose()
    {
        try
        {
            if (isDisposeCalled == false)
            {
                //TODO: Add your application code here 
            }
            isDisposeCalled = true;
        }
        catch (NXOpen.NXException ex)
        {
            // ---- Enter your exception handling code here -----

        }
    }

    public static int GetUnloadOption(string arg)
    {
        //Unloads the image explicitly, via an unload dialog
        //return System.Convert.ToInt32(Session.LibraryUnloadOption.Explicitly);

        //Unloads the image immediately after execution within NX
        return System.Convert.ToInt32(Session.LibraryUnloadOption.Immediately);

        //Unloads the image when the NX session terminates
        // return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
    }

}

