// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using IxMilia.Step.Items;
using IxMilia.Step.Syntax;

namespace IxMilia.Step
{
    internal class StepBoundItem
    {
        public StepSyntax CreatingSyntax { get; }
        public StepRepresentationItem Item { get; }
        public bool IsAuto { get; private set; }

        public StepBoundItem(StepRepresentationItem item, StepSyntax creatingSyntax)
        {
            CreatingSyntax = creatingSyntax;
            Item = item;
        }

        public TItemType AsType<TItemType>() where TItemType : StepRepresentationItem
        {
            TItemType result = null;
            if (IsAuto)
            {
                // do nothing; null is expected
            }
            else
            {
                result = Item as TItemType;
                if (result == null)
                {
                    throw new StepReadException("Unexpected type", CreatingSyntax.Line, CreatingSyntax.Column);
                }
            }

            return result;
        }

        public static StepBoundItem AutoItem(StepSyntax creatingSyntax)
        {
            var boundItem = new StepBoundItem(null, creatingSyntax);
            boundItem.IsAuto = true;
            return boundItem;
        }
    }
}
