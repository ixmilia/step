// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public abstract class StepFace : StepTopologicalRepresentationItem
    {
        private List<StepFaceBound> _bounds;

        public List<StepFaceBound> Bounds {
            get => _bounds;
            set
            {
                if ( value == null )
                {
                    throw new ArgumentNullException();
                }

                _bounds = value;
            }
        }

        public StepFace( string name ) : base( name )
        {
        }

        internal override IEnumerable<StepSyntax> GetParameters( StepWriter writer )
        {
            foreach ( var parameter in base.GetParameters( writer ) )
            {
                yield return parameter;
            }
            if ( _bounds != null )
            {
                yield return new StepSyntaxList( Bounds.Select( b => writer.GetItemSyntax( b ) ) );
            }
            else
            {
                yield return new StepSyntaxList();
            }
        }
    }
}
