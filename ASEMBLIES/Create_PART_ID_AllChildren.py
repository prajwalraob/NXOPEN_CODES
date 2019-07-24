import NXOpen

leaf_list = list()
node_list = list()
session = None


def traverse(root):
    children = root.GetChildren()
    if len(children) == 0:
        leaf_list.append(root)
    else:
        node_list.append(root)
        for child in children:
            traverse(child)


def main():
    #'D:\ACCEPTED_NX_MODELS\Q5\SUPPORT_ASM'
    part_path = 'D:/ACCEPTED_NX_MODELS/Q5/SUPPORT_ASM/CASE_4_stp.prt'

    session = NXOpen.Session.GetSession()
    (base_part, load_status) = session.Parts.OpenBaseDisplay(part_path)

    assembly = base_part.ComponentAssembly
    root_component = assembly.RootComponent

    traverse(root_component)
    combined = node_list + leaf_list

    nx_objects = list([NXOpen.NXObject.Null, ])
    attrib_builder = session.AttributeManager.CreateAttributePropertiesBuilder(base_part, nx_objects,
                                                                               NXOpen.AttributePropertiesBuilder.OperationType.NotSet)

    count = 1
    for child in combined:
        (status1, load_status1) = session.Parts.SetDisplay(child.Prototype, False, True)
        nx_objects[0] = child.Prototype
        attrib_builder.SetAttributeObjects(nx_objects)
        attrib_builder.Title = "PART_ID"
        attrib_builder.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.Integer
        attrib_builder.IntegerValue = count
        obj2 = attrib_builder.CreateAttribute()
        attrib_builder.Commit()
        (status2, load_status2) = session.Parts.SetDisplay(base_part, False, True)
        count += 1

    save_status, num_saved = session.Parts.SaveAll()
    session.Parts.CloseAll(NXOpen.BasePartCloseModified.DontCloseModified, None)
    print(count)


if __name__ == '__main__':
    main()
