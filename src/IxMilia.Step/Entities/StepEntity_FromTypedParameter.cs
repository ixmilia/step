// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using IxMilia.Step.Syntax;

namespace IxMilia.Step.Entities
{
    public abstract partial class StepEntity
    {
        internal static StepEntity FromTypedParameter(StepBinder binder, StepEntitySyntax entitySyntax)
        {
            StepEntity entity = null;
            if (entitySyntax is StepSimpleEntitySyntax)
            {
                var simpleEntity = (StepSimpleEntitySyntax)entitySyntax;
                switch (simpleEntity.Keyword)
                {
                    case StepEntityTypeExtensions.AxisPlacement2DText:
                        entity = StepAxisPlacement2D.CreateFromSyntaxList(binder, simpleEntity.Parameters);
                        break;
                    case StepEntityTypeExtensions.CartesianPointText:
                        entity = StepCartesianPoint.CreateFromSyntaxList(simpleEntity.Parameters);
                        break;
                    case StepEntityTypeExtensions.CircleText:
                        entity = StepCircle.CreateFromSyntaxList(binder, simpleEntity.Parameters);
                        break;
                    case StepEntityTypeExtensions.DirectionText:
                        entity = StepDirection.CreateFromSyntaxList(simpleEntity.Parameters);
                        break;
                    case StepEntityTypeExtensions.LineText:
                        entity = StepLine.CreateFromSyntaxList(binder, simpleEntity.Parameters);
                        break;
                    case StepEntityTypeExtensions.VectorText:
                        entity = StepVector.CreateFromSyntaxList(binder, simpleEntity.Parameters);
                        break;
                }
            }
            else
            {
                // TODO:
            }

            return entity;
        }
    }
}
