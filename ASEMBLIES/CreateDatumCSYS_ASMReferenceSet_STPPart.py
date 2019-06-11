import NXOpen

leaf_list = list()
node_list = list()
session = NXOpen.Session.GetSession()


def traverse(root):
    children = root.GetChildren()
    if len(children) == 0:
        leaf_list.append(root)
    else:
        node_list.append(root)
        for child in children:
            traverse(child)


def check_datum(prototype):
    features =  list(prototype.Features)
    datum_found = False    
    for feature in features:
        this_type = feature.FeatureType
        if this_type == 'DATUM_CSYS':
            this_point = feature.Location
            point3 = NXOpen.Point3d()
            if point3.X == this_point.X and point3.Y == this_point.Y and point3.Z == this_point.Z:
                datum_found = True
                return datum_found                
    return datum_found


def datums_to_layer(component):
    prototype = component.Prototype
    datums =  list(prototype.Datums)
    display_mod = session.DisplayManager.NewDisplayModification()
    display_mod.ApplyToOwningParts = True
    display_mod.NewLayer = 31
    display_objects = list()
    proxy_list = list()
    for element in datums:
        display_objects.append(element)
        proxy_element = component.FindOccurrence(element)
        proxy_list.append(proxy_element)
        
    display_mod.Apply(display_objects)
    if not proxy_list[0] == None:
        display_mod.Apply(proxy_list)
    display_mod.Dispose()


def create_datums(component):
    prototype = component.Prototype
    datum_exists = check_datum(prototype)
    if not datum_exists:
        origin5 = NXOpen.Point3d(0.0, 0.0, 0.0)
        x_direction1 = NXOpen.Vector3d(1.0, 0.0, 0.0)
        y_direction1 = NXOpen.Vector3d(0.0, 1.0, 0.0)
        xform1 = prototype.Xforms.CreateXform(origin5, x_direction1, y_direction1, NXOpen.SmartObject.UpdateOption.WithinModeling, 1.0)
        csy = prototype.CoordinateSystems.CreateCoordinateSystem(xform1, NXOpen.SmartObject.UpdateOption.WithinModeling)
        coord_builder = prototype.Features.CreateDatumCsysBuilder(NXOpen.Features.Feature.Null)

        coord_builder.Csys = csy
        coord_builder.Commit()
        coord_builder.Destroy()
    datums_to_layer(component)


def create_reference_set(component):
    prototype = component.Prototype
    reference_set_asm = prototype.CreateReferenceSet()
    try:
        reference_set_asm.SetName("ASM")
        body_list = list(prototype.Bodies)
        if body_list:
            components1 = [NXOpen.NXObject.Null] * 1
            body1 = None
            for body in prototype.Bodies:
                body1 = body
            components1[0] = body1
            reference_set_asm.AddObjectsToReferenceSet(components1)
    except Exception:
        prototype.DeleteReferenceSet(reference_set_asm)


def process_file(stp_file_path):
    path = stp_file_path.strip()
    (base_part, load_status) = session.Parts.OpenActiveDisplay(path, NXOpen.DisplayPartOption.ReplaceExisting)
    (any_saved, save_status) = session.Parts.SaveAll()
    
    assembly = base_part.ComponentAssembly
    root_component = assembly.RootComponent
    
    traverse(root_component)
    create_datums(root_component)
    
    for node in node_list:
        (status1, load_status1) = session.Parts.SetDisplay(node.Prototype, False, True)
        create_reference_set(node)
        create_datums(node)
        (status2, load_status2) = session.Parts.SetDisplay(base_part, False, True)
    
    for leaf in leaf_list:
        (status3, load_status3) = session.Parts.SetDisplay(leaf.Prototype, False, True)
        create_reference_set(leaf)
        create_datums(leaf)
        (status4, load_status4) = session.Parts.SetDisplay(base_part, False, True)
    
    errorList2 = base_part.ComponentAssembly.ReplaceReferenceSetInOwners("ASM", node_list)    
    errorList1 = base_part.ComponentAssembly.ReplaceReferenceSetInOwners("ASM", leaf_list)
    
    (any_saved1, save_status1) = session.Parts.SaveAll()
    
    response = NXOpen.PartCloseResponses()
    session.Parts.CloseAll(NXOpen.BasePartCloseModified.DontCloseModified, None)


file_path = 'D:\ACCEPTED_NX_MODELS\Q5\FEED.txt'

with open(file_path) as this_file:
    for line in this_file:
        leaf_list.clear()
        node_list.clear()
        process_file(line)

print('Done!')
