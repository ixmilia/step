// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using IxMilia.Step.Entities;
using IxMilia.Step.Syntax;

namespace IxMilia.Step
{
    internal class StepBinder
    {
        private Dictionary<int, StepEntity> _entityMap;
        private Dictionary<int, List<Tuple<StepSyntax, Action<StepBoundEntity>>>> _unboundPointers = new Dictionary<int, List<Tuple<StepSyntax, Action<StepBoundEntity>>>>();

        public StepBinder(Dictionary<int, StepEntity> entityMap)
        {
            _entityMap = entityMap;
        }

        public void BindValue(StepSyntax syntax, Action<StepBoundEntity> bindAction)
        {
            if (syntax is StepTypedParameterSyntax)
            {
                var typedParameter = (StepTypedParameterSyntax)syntax;
                var entity = StepEntity.FromTypedParameter(this, typedParameter);
                var boundEntity = new StepBoundEntity(entity, syntax);
                bindAction(boundEntity);
            }
            else if (syntax is StepEntityInstanceReferenceSyntax)
            {
                var entityInstance = (StepEntityInstanceReferenceSyntax)syntax;
                if (_entityMap.ContainsKey(entityInstance.Id))
                {
                    // pointer already defined, bind immediately
                    var boundEntity = new StepBoundEntity(_entityMap[entityInstance.Id], syntax);
                    bindAction(boundEntity);
                }
                else
                {
                    // not already defined, save it for later
                    if (!_unboundPointers.ContainsKey(entityInstance.Id))
                    {
                        _unboundPointers.Add(entityInstance.Id, new List<Tuple<StepSyntax, Action<StepBoundEntity>>>());
                    }

                    _unboundPointers[entityInstance.Id].Add(Tuple.Create(syntax, bindAction));
                }
            }
            else
            {
                throw new StepReadException("Unable to bind pointer, this should be unreachable", syntax.Line, syntax.Column);
            }
        }

        public void BindRemainingValues()
        {
            foreach (var id in _unboundPointers.Keys)
            {
                if (!_entityMap.ContainsKey(id))
                {
                    var syntax = _unboundPointers[id].First().Item1;
                    throw new StepReadException($"Cannot bind undefined pointer {id}", syntax.Line, syntax.Column);
                }

                var entity = _entityMap[id];
                foreach (var binder in _unboundPointers[id])
                {
                    var boundEntity = new StepBoundEntity(entity, binder.Item1);
                    binder.Item2(boundEntity);
                }
            }
        }
    }
}
