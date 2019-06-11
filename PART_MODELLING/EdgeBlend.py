import NXOpen

part_path = 'D:\ACCEPTED_NX_MODELS\FINGER\MCPJ_catpart.prt'
#'D:\ACCEPTED_NX_MODELS\Q5\OIL_PIPE\FLANGE(10K_150A_N)_K3034074#1.prt'
#'D:\ACCEPTED_NX_MODELS\FINGER\MCPJ_catpart.prt'
session = NXOpen.Session.GetSession()
part, load_status = session.Parts.OpenBaseDisplay(part_path)

if(part):
    body_list = list(part.Bodies)
    body = body_list[0]
    edge_list = body.GetEdges()
    select_edge = None
    for edge in edge_list:
        print(str(edge.Name))
        if(edge.Name == 'EDGE0.5'):
            select_edge = edge
            break
    collector = part.ScCollectors.CreateCollector()
    edges = list()
    edges.append(select_edge)
    tangent_rule = part.ScRuleFactory.CreateRuleEdgeMultipleSeedTangent(edges,0.001,False)
    rules = list()
    rules.append(tangent_rule)
    collector.ReplaceRules(rules,False)
    
    blend_builder = part.Features.CreateEdgeBlendBuilder(NXOpen.Features.Feature.Null)
    blend_builder.AddChainset(collector,'0.5')
    if(blend_builder.Validate()):
        blend_builder.CommitFeature()
    
(any_saved1, save_status1) = session.Parts.SaveAll()
print('Done!')