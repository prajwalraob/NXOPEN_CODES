import NXOpen
import NXOpen_Assemblies

part_path = 'D:\ACCEPTED_NX_MODELS\Q5\TEST\TEST.prt'
session = NXOpen.Session.GetSession()
work_part, load_status = session.Parts.OpenBaseDisplay(part_path)

clearance_set = NXOpen_Assemblies.ClearanceSet.Null
builder = work_part.AssemblyManager.CreateClearanceAnalysisBuilder(clearance_set)

builder.ClearanceSetName = "CUSTOM";
builder.ClearanceBetween = NXOpen_Assemblies.ClearanceAnalysisBuilder.ClearanceBetweenEntity.Components;
builder.TotalCollectionCount = NXOpen_Assemblies.ClearanceAnalysisBuilder.NumberOfCollections.One;

status = builder.Validate()
if status:    
    clearance_set = builder.Commit()
    builder.Destroy()
    clearance_set.PerformAnalysis(NXOpen_Assemblies.ClearanceSet.ReanalyzeOutOfDateExcludedPairs.TrueValue);

    version = clearance_set.GetVersion();
    summary = clearance_set.GetResults();

saved, save_status = session.Parts.SaveAll()

print('Done!')
