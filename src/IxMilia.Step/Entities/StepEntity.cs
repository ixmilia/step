// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using IxMilia.Step.Syntax;

namespace IxMilia.Step.Entities
{
    public abstract class StepEntity
    {
        public abstract StepEntityType EntityType { get; }

        public string Name { get; set; }

        protected StepEntity(string name)
        {
            Name = name;
        }

        internal static StepEntity FromTypedParameter(StepTypedParameterSyntax typedParameter)
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
                default:
                    entity = null;
                    break;
            }

            return entity;
        }
    }
}
