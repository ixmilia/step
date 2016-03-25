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

        internal static StepEntity FromTypedParameter(StepBinder binder, StepTypedParameterSyntax typedParameter)
        {
            StepEntity entity;
            switch (typedParameter.Keyword)
            {
                case "CARTESIAN_POINT":
                    entity = StepCartesianPoint.CreateFromSyntaxList(typedParameter.Parameters);
                    break;
                case "DIRECTION":
                    entity = StepDirection.CreateFromSyntaxList(typedParameter.Parameters);
                    break;
                case "LINE":
                    entity = StepLine.CreateFromSyntaxList(binder, typedParameter.Parameters);
                    break;
                case "VECTOR":
                    entity = StepVector.CreateFromSyntaxList(binder, typedParameter.Parameters);
                    break;
                default:
                    entity = null;
                    break;
            }

            return entity;
        }
    }
}
