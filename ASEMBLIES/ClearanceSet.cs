ClearanceSet clearanceSet = null;
ClearanceAnalysisBuilder clearanceBuilder = workPart.AssemblyManager.CreateClearanceAnalysisBuilder(clearanceSet);

clearanceBuilder.ClearanceSetName = "CUSTOM";
clearanceBuilder.ClearanceBetween = ClearanceAnalysisBuilder.ClearanceBetweenEntity.Components;
clearanceBuilder.TotalCollectionCount = ClearanceAnalysisBuilder.NumberOfCollections.One;

bool status = clearanceBuilder.Validate();
if (status)
{
	clearanceSet = clearanceBuilder.Commit() as ClearanceSet;
}
else
{
	theUI.NXMessageBox.Show("Error", NXMessageBox.DialogType.Information, "Builder could not be committed");
}
clearanceBuilder.Destroy();

clearanceSet.PerformAnalysis(ClearanceSet.ReanalyzeOutOfDateExcludedPairs.True);

int version = clearanceSet.GetVersion();
ClearanceSet.Summary summary = clearanceSet.GetResults();