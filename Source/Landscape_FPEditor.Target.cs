using UnrealBuildTool;
using System.Collections.Generic;

public class Landscape_FPEditorTarget : TargetRules
{
	public Landscape_FPEditorTarget(TargetInfo Target) : base(Target)
	{
		Type = TargetType.Editor;
		DefaultBuildSettings = BuildSettingsVersion.Latest;
		IncludeOrderVersion = EngineIncludeOrderVersion.Latest;
		ExtraModuleNames.Add("Landscape_FP");
	}
}
