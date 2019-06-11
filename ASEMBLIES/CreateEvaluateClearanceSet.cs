using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NXOpen;
using NXOpen.UF;
using NXOpen.Assemblies;
using NXOpen.Features;

public class Program
{
    private static Session theSession;
    private static UFSession theUFSession;
    private static Program theProgram;
    private static bool isDisposeCalled;
    private List<Component> leafList;

    public Program()
    {
        theSession = Session.GetSession();
        theUFSession = UFSession.GetUFSession();
        isDisposeCalled = false;
        leafList = new List<Component>();
    }

    public static int Main(string[] args)
    {
        int retValue = 0;
        
        return retValue;
    }

    public static int Startup(string[] args)
    {
        int retValue = 0;
        try
        {
            theProgram = new Program();

            PartLoadStatus pld;
            string partPath = @"D:\ACCEPTED_NX_MODELS\FINGER\assembly.prt";
            theSession.Parts.OpenBaseDisplay(partPath, out pld);

            Part workPart = theSession.Parts.Work;
            ClearanceSet clearanceSet = null;
            ClearanceAnalysisBuilder clearanceBuilder = workPart.AssemblyManager.CreateClearanceAnalysisBuilder(clearanceSet);

            clearanceBuilder.ClearanceSetName = "CUSTOM";
            clearanceBuilder.ClearanceBetween = ClearanceAnalysisBuilder.ClearanceBetweenEntity.Components;
            clearanceBuilder.TotalCollectionCount = ClearanceAnalysisBuilder.NumberOfCollections.One;

            clearanceSet = clearanceBuilder.Commit() as ClearanceSet;
            clearanceBuilder.Destroy();

            clearanceSet.PerformAnalysis(ClearanceSet.ReanalyzeOutOfDateExcludedPairs.True);

            bool partSave;
            PartSaveStatus saveStatus;
            theSession.Parts.SaveAll(out partSave, out saveStatus);

            theUFSession.Part.CloseAll();
            theProgram.Dispose();
        }
        catch (NXOpen.NXException ex)
        {
            theUFSession.Part.CloseAll();

            theSession.ListingWindow.Open();
            theSession.ListingWindow.WriteLine(ex.ToString());

            // ---- Enter your exception handling code here -----

        }
        return retValue;
       
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

    public static int GetUnloadOptions(String arg)
    {
        return System.Convert.ToInt32(Session.LibraryUnloadOption.Explicitly);
    }

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

}

