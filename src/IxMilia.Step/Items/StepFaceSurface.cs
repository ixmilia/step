// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public abstract class StepFaceSurface : StepFace
    {
        public StepSurface FaceGeometry { get; set; }

        public bool SameSense { get; set; }

        public StepFaceSurface(string name)
            : base(name)
        {
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return writer.GetItemSyntax(FaceGeometry);
            yield return StepWriter.GetBooleanSyntax(!SameSense);
        }
    }
}
