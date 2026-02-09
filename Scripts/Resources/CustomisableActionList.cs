using Godot;
using System;
using Godot.Collections;

public partial class CustomisableActionList : Resource
{
    [Export] public string KeyboardPrefix;
    [Export] public string ControllerPrefix;
    [Export] public Array<string> ActionNames;
    [Export] public Array<string> ActionIDs;
}