import NXOpen
import NXOpen_Features

session = NXOpen.Session.GetSession()
part_path = 'D:/ACCEPTED_NX_MODELS/POWER_ADAPTER/ADAPTER_BOX.prt'

work_part, load_status = session.Parts.OpenBaseDisplay(part_path)

if work_part:
    obj1 = work_part.GetUserAttributes()
    nx_objects = list([work_part, ])
    attrib_builder = session.AttributeManager.CreateAttributePropertiesBuilder(work_part, nx_objects, NXOpen.AttributePropertiesBuilder.OperationType.NotSet)

    attrib_builder.Title = "Part ID"
    attrib_builder.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.Integer
    attrib_builder.IntegerValue = 1
    obj2 = attrib_builder.CreateAttribute()

    nx_object1 = attrib_builder.Commit()

save_status, num_saved = session.Parts.SaveAll()
session.Parts.CloseAll(NXOpen.BasePartCloseModified.DontCloseModified, None)
print('Done')
