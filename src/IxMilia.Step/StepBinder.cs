using System;
using System.Collections.Generic;
using System.Linq;
using IxMilia.Step.Items;
using IxMilia.Step.Syntax;

namespace IxMilia.Step
{
    internal class StepBinder
    {
        private Dictionary<int, StepRepresentationItem> _itemMap;
        private Dictionary<int, List<Tuple<StepSyntax, Action<StepBoundItem>>>> _unboundPointers = new Dictionary<int, List<Tuple<StepSyntax, Action<StepBoundItem>>>>();

        public StepBinder(Dictionary<int, StepRepresentationItem> itemMap)
        {
            _itemMap = itemMap;
        }

        public void BindValue(StepSyntax syntax, Action<StepBoundItem> bindAction)
        {
            if (syntax is StepSimpleItemSyntax)
            {
                var typedParameter = (StepSimpleItemSyntax)syntax;
                var item = StepRepresentationItem.FromTypedParameter(this, typedParameter);
                var boundItem = new StepBoundItem(item, syntax);
                bindAction(boundItem);
            }
            else if (syntax is StepEntityInstanceReferenceSyntax)
            {
                var itemInstance = (StepEntityInstanceReferenceSyntax)syntax;
                if (_itemMap.ContainsKey(itemInstance.Id))
                {
                    // pointer already defined, bind immediately
                    var boundItem = new StepBoundItem(_itemMap[itemInstance.Id], syntax);
                    bindAction(boundItem);
                }
                else
                {
                    // not already defined, save it for later
                    if (!_unboundPointers.ContainsKey(itemInstance.Id))
                    {
                        _unboundPointers.Add(itemInstance.Id, new List<Tuple<StepSyntax, Action<StepBoundItem>>>());
                    }

                    _unboundPointers[itemInstance.Id].Add(Tuple.Create(syntax, bindAction));
                }
            }
            else if (syntax is StepAutoSyntax)
            {
                bindAction(StepBoundItem.AutoItem(syntax));
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
                if (!_itemMap.ContainsKey(id))
                {
                    var syntax = _unboundPointers[id].First().Item1;
                    throw new StepReadException($"Cannot bind undefined pointer {id}", syntax.Line, syntax.Column);
                }

                var item = _itemMap[id];
                foreach (var binder in _unboundPointers[id])
                {
                    var boundItem = new StepBoundItem(item, binder.Item1);
                    binder.Item2(boundItem);
                }
            }
        }
    }
}
