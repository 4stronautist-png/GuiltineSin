GuiltineSin.Ui.SysMenu.SuspendRefresh()

GuiltineSin.Ui.SysMenu.RemoveButton("BtnInstantDungeon")
GuiltineSin.Ui.SysMenu.RemoveButton("BtnParty")
GuiltineSin.Ui.SysMenu.RemoveButton("BtnAdvancement")
GuiltineSin.Ui.SysMenu.RemoveButton("BtnFishing")
GuiltineSin.Ui.SysMenu.RemoveButton("BtnGuildPromo")
GuiltineSin.Ui.SysMenu.RemoveButton("BtnPcBang")
GuiltineSin.Ui.SysMenu.InsertButton(2, "BtnAdvancement", "sysmenu_jobinfo", DicID("UI_20180208_003013"), "OPEN_RANKROLLBACK_UI_BY_SYSMENU()")
GuiltineSin.Ui.SysMenu.InsertButton(6, "BtnParty", "sysmenu_party", DicID("UI_20220204_005169"), "ui.ToggleFrame('party')")

GuiltineSin.Ui.SysMenu.ResumeRefresh()
