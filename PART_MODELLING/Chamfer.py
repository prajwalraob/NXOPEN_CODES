import NXOpen

part_path = 'D:\ACCEPTED_NX_MODELS\FINGER\MCPJ_catpart.prt'
#'D:\ACCEPTED_NX_MODELS\Q5\OIL_PIPE\FLANGE(10K_150A_N)_K3034074#1.prt'
#'D:\ACCEPTED_NX_MODELS\FINGER\MCPJ_catpart.prt'
session = NXOpen.Session.GetSession()
work_part, load_status = session.Parts.OpenBaseDisplay(part_path)

if(work_part):
    body_list = list(work_part.Bodies)
    body = body_list[0]
    edge_list = body.GetEdges()
    select_edge = None
    for edge in edge_list:
        #print(str(edge.Name))
        if(edge.Name == 'EDGE0.5'):
            select_edge = edge
            break
    collector = work_part.ScCollectors.CreateCollector()
    edges = list()
    edges.append(select_edge)
    tangent_rule = work_part.ScRuleFactory.CreateRuleEdgeMultipleSeedTangent(edges,0.001,False)
    rules = list()
    rules.append(tangent_rule)
    collector.ReplaceRules(rules,False)
    
    chamfer_builder = work_part.Features.CreateChamferBuilder(NXOpen.Features.Feature.Null)
    chamfer_builder.Option = NXOpen.Features.ChamferBuilder.ChamferOption.TwoOffsets
    chamfer_builder.Method = NXOpen.Features.ChamferBuilder.OffsetMethod.EdgesAlongFaces
    chamfer_builder.FirstOffset = "0.5"
    chamfer_builder.SecondOffset = "0.5"
    chamfer_builder.Angle = "60"
    chamfer_builder.SmartCollector = collector
    
    vallid = chamfer_builder.Validate()
    chamfer_builder.Commit()
    
(any_saved1, save_status1) = session.Parts.SaveAll()
print('Done!')