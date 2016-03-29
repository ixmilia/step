// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using IxMilia.Step.Syntax;

namespace IxMilia.Step.Entities
{
    public abstract class StepEntity
    {
        public abstract StepEntityType EntityType { get; }

        public string Label { get; set; }

        protected StepEntity(string label)
        {
            Label = label;
        }

        internal static StepEntity FromTypedParameter(StepBinder binder, StepEntitySyntax entitySyntax)
        {
            StepEntity entity = null;
            if (entitySyntax is StepSimpleEntitySyntax)
            {
                var simpleEntity = (StepSimpleEntitySyntax)entitySyntax;
                switch (simpleEntity.Keyword)
                {
                    case "CARTESIAN_POINT":
                        entity = StepCartesianPoint.CreateFromSyntaxList(simpleEntity.Parameters);
                        break;
                    case "DIRECTION":
                        entity = StepDirection.CreateFromSyntaxList(simpleEntity.Parameters);
                        break;
                    case "LINE":
                        entity = StepLine.CreateFromSyntaxList(binder, simpleEntity.Parameters);
                        break;
                    case "VECTOR":
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
