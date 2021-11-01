using System;
using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Schemas.ExplicitDraughting
{
    public abstract class StepItem
    {
        public abstract string ItemTypeString { get; }

        protected bool SuppressValidation { get; set; } = false;

        internal virtual void ValidateDomainRules()
        {
        }

        internal virtual IEnumerable<StepItem> GetReferencedItems()
        {
            yield break;
        }

        internal virtual IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            yield break;
        }

        internal static StepItem CreateFromSyntaxList(StepBinder _binder, StepSyntaxList _syntaxList)
        {
            throw new InvalidOperationException("This method must be called on sub-types, only.");
        }

        protected static long DimensionOf(StepItem item)
        {
            // TODO: this function should come from the schema when supported
            return 0;
        }
    }
}
