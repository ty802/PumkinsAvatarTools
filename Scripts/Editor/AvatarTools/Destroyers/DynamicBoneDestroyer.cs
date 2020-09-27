﻿using Pumkin.AvatarTools.Base;
using Pumkin.AvatarTools.Modules;
using Pumkin.Core.Attributes;

namespace Pumkin.AvatarTools.Destroyers
{
    [AutoLoad("destroyers_dynamicBone", ParentModuleID = DefaultModuleIDs.DESTROYER)]
    class DynamicBoneDestroyer : ComponentDestroyerBase
    {
        public override string ComponentTypeNameFull => "DynamicBone";
    }
}