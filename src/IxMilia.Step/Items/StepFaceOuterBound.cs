// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepFaceOuterBound : StepFaceBound
    {
        public override StepItemType ItemType => StepItemType.FaceOuterBound;

        private StepFaceOuterBound()
            : base()
        {
        }

        public StepFaceOuterBound(string name, StepLoop bound, bool orientation)
            : base(name, bound, orientation)
        {
        }

        internal static new StepFaceOuterBound CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            syntaxList.AssertListCount(3);
            var faceOuterBound = new StepFaceOuterBound();
            faceOuterBound.Name = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => faceOuterBound.Bound = v.AsType<StepLoop>());
            faceOuterBound.Orientation = syntaxList.Values[2].GetBooleanValue();
            return faceOuterBound;
        }
    }
}
