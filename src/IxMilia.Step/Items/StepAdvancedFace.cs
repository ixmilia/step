// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using IxMilia.Step.Syntax;
using System.Collections.Generic;

namespace IxMilia.Step.Items
{
    public class StepAdvancedFace : StepFaceSurface
    {
        public StepAdvancedFace( string name ) : base( name )
        {
        }

        private StepAdvancedFace() : base( string.Empty ) { }

        public override StepItemType ItemType => StepItemType.AdvancedFace;

        internal static StepAdvancedFace CreateFromSyntaxList( StepBinder binder, StepSyntaxList syntaxList )
        {
            var face = new StepAdvancedFace();
            syntaxList.AssertListCount( 4 );
            face.Name = syntaxList.Values[0].GetStringValue();

            var boundsList = syntaxList.Values[1].GetValueList();
            face.Bounds = new List<StepFaceBound>( new StepFaceBound[boundsList.Values.Count] );
            for ( int i = 0; i < boundsList.Values.Count; i++ )
            {
                var j = i; // capture to avoid rebinding.
                binder.BindValue( boundsList.Values[j], v => face.Bounds[j] = v.AsType<StepFaceBound>() );
            }
            binder.BindValue( syntaxList.Values[2], v => face.FaceGeometry = v.AsType<StepSurface>() );
            face.SameSense = syntaxList.Values[3].GetBooleanValue();

            return face;
        }
    }
}
