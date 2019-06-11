
Option Strict Off  
Imports System  
Imports NXOpen  
Imports NXOpen.UF  

Module Module1  

    Sub Main()  

        Dim theSession As Session = Session.GetSession()  
        Dim workPart As Part = theSession.Parts.Work  

        Dim mySolid As NXObject  
        If SelectSolid("Select a solid", mySolid) = Selection.Response.Cancel Then  
            Exit Sub  
        End If  

        Dim markId1 As Session.UndoMarkId  
        markId1 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start")  

        Dim nullFeatures_Feature As Features.Feature = Nothing  

        If Not workPart.Preferences.Modeling.GetHistoryMode Then  
            Throw (New Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode."))  
        End If  

        Dim waveLinkBuilder1 As Features.WaveLinkBuilder  
        waveLinkBuilder1 = workPart.BaseFeatures.CreateWaveLinkBuilder(nullFeatures_Feature)  

        Dim extractFaceBuilder1 As Features.ExtractFaceBuilder  
        extractFaceBuilder1 = waveLinkBuilder1.ExtractFaceBuilder  

        extractFaceBuilder1.FaceOption = Features.ExtractFaceBuilder.FaceOptionType.FaceChain  

        waveLinkBuilder1.Type = Features.WaveLinkBuilder.Types.BodyLink  

        extractFaceBuilder1.FaceOption = Features.ExtractFaceBuilder.FaceOptionType.FaceChain  

        waveLinkBuilder1.CopyThreads = False  

        extractFaceBuilder1.ParentPart = Features.ExtractFaceBuilder.ParentPartType.OtherPart  

        theSession.SetUndoMarkName(markId1, "WAVE Geometry Linker Dialog")  

        extractFaceBuilder1.Associative = True  

        extractFaceBuilder1.FixAtCurrentTimestamp = False  

        extractFaceBuilder1.HideOriginal = False  

        extractFaceBuilder1.InheritDisplayProperties = False  

        Dim selectObjectList1 As SelectObjectList  
        selectObjectList1 = extractFaceBuilder1.BodyToExtract  

        extractFaceBuilder1.CopyThreads = False  

        Dim added1 As Boolean  
        added1 = selectObjectList1.Add(mySolid)  

        Dim nXObject1 As NXObject  
        nXObject1 = waveLinkBuilder1.Commit()  

        theSession.SetUndoMarkName(markId1, "WAVE Geometry Linker")  

        waveLinkBuilder1.Destroy()  

    End Sub  

    Function SelectSolid(ByVal prompt As String, ByRef selObj As NXObject) As Selection.Response  

        Dim theUI As UI = UI.GetUI  
        Dim title As String = "Select a solid"  
        Dim includeFeatures As Boolean = False  
        Dim keepHighlighted As Boolean = False  
        Dim selAction As Selection.SelectionAction = Selection.SelectionAction.ClearAndEnableSpecific  
        Dim cursor As Point3d  
        Dim scope As Selection.SelectionScope = Selection.SelectionScope.AnyInAssembly  
        Dim selectionMask_array(0) As Selection.MaskTriple  

        With selectionMask_array(0)  
            .Type = UFConstants.UF_solid_type  
            .SolidBodySubtype = UFConstants.UF_UI_SEL_FEATURE_SOLID_BODY  
        End With  

        Dim resp As Selection.Response = theUI.SelectionManager.SelectObject(prompt, _  
         title, scope, selAction, _  
         includeFeatures, keepHighlighted, selectionMask_array, _  
         selobj, cursor)  
        If resp = Selection.Response.ObjectSelected OrElse resp = Selection.Response.ObjectSelectedByName Then  
            Return Selection.Response.Ok  
        Else  
            Return Selection.Response.Cancel  
        End If  

    End Function  

End Module 