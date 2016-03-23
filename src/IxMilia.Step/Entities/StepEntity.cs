// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;

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

        protected virtual void AfterEntitiesRead(Dictionary<int, StepEntity> entityMap)
        {
        }

        internal static StepEntity FromMacro(StepMacro macro)
        {
            StepEntity entity;
            switch (macro.Keyword.Value)
            {
                case "CARTESIAN_POINT":
                    entity = new StepCartesianPoint(macro);
                    break;
                default:
                    entity = null;
                    break;
            }

            return entity;
        }
    }
}
