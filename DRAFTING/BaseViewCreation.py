import NXOpen
import NXOpen_Drawings

part_path = 'D:\ACCEPTED_NX_MODELS\Q5\TEST\FLANGE(10K_150A_G)_K3029930#1.prt'
session = NXOpen.Session.GetSession()
work_part, load_status = session.Parts.OpenBaseDisplay(part_path)

sheet = list(work_part.DrawingSheets)[0]
sheet.Open()
builder = work_part.DraftingViews.CreateBaseViewBuilder(NXOpen_Drawings.BaseView.Null)

top_view = work_part.ModelingViews.FindObject("Top")
builder.SelectModelView.SelectedView = top_view
builder.Scale.Denominator = 1.0;

point1 = NXOpen.Point3d(sheet.Length/2, sheet.Height/2, 0.0)
builder.Placement.Placement.SetValue(NXOpen.TaggedObject.Null, work_part.Views.WorkView, point1)

val = builder.Validate()
view = builder.Commit()
builder.Destroy()

save_status, saved = session.Parts.SaveAll()
print("Done!")