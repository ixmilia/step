// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using IxMilia.Step.Entities;
using IxMilia.Step.Syntax;

namespace IxMilia.Step
{
    internal class StepBoundEntity
    {
        public StepSyntax CreatingSyntax { get; }
        public StepEntity Entity { get; }

        public StepBoundEntity(StepEntity entity, StepSyntax creatingSyntax)
        {
            CreatingSyntax = creatingSyntax;
            Entity = entity;
        }

        public TEntityType AsType<TEntityType>() where TEntityType : StepEntity
        {
            var result = Entity as TEntityType;
            if (result == null)
            {
                throw new StepReadException("Unexpected type", CreatingSyntax.Line, CreatingSyntax.Column);
            }

            return result;
        }
    }
}
