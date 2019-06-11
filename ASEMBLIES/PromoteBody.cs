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
            string partPath = @"D:\ACCEPTED_NX_MODELS\Q5\TEST\TEST.prt";
            Part workPart = theSession.Parts.OpenBaseDisplay(partPath, out pld) as Part;

            ComponentAssembly assembly = workPart.ComponentAssembly;
            Component rootComponent = assembly.RootComponent;

            theProgram.Traverse(rootComponent);
            theSession.ListingWindow.Open();

            foreach (Component leaf in theProgram.leafList)
            {
                Part leafPart = leaf.Prototype as Part;
                Body[] leafBodies = leafPart.Bodies.ToArray();

                if (leafBodies.Length != 0)
                {
                    Body leafBody = leafBodies[0];
                    Promotion promoteBody = null;
                    PromotionBuilder promotionBuider1 = workPart.Features.CreatePromotionBuilder(promoteBody);
                    leafBody = leaf.FindOccurrence(leafBody) as Body;
                    bool added = promotionBuider1.Body.Add(leafBody);
                    bool validated = promotionBuider1.Validate();
                    NXObject objects = promotionBuider1.Commit();
                    promotionBuider1.Destroy();
                    theSession.ListingWindow.WriteLine(leaf.Name + "\t" + leafBodies.Length.ToString() + "\t" + leafBody.Name.ToString());
                }
            }

            PartSaveStatus ps;
            bool partSaved;
            theSession.Parts.SaveAll(out partSaved, out ps);

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

