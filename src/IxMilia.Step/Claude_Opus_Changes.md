# IxMilia.Step Library Changes

## Summary

Changes were made to add support for **complex entity instances** in STEP files. Complex entity instances combine multiple entity types into a single instance using parentheses, which is common in STEP files containing B-spline curves and surfaces.

## Problem

When parsing STEP files containing B-spline curves (such as `Cam Pipe Clamp Sliced.stp`), the parser would fail with an error like:

```
Error: Cannot bind undefined pointer 1315 at [1317:33]
```

This occurred because complex entity instances like the following were not being parsed:

```step
#1315=(BOUNDED_CURVE()
B_SPLINE_CURVE(2,(#1306,#1307,...),.UNSPECIFIED.,.T.,.F.)
B_SPLINE_CURVE_WITH_KNOTS((3,2,2,2,3),(65.999...,155.999...,...),.UNSPECIFIED.)
CURVE()
GEOMETRIC_REPRESENTATION_ITEM()
RATIONAL_B_SPLINE_CURVE((1.,0.707106781,1.,...))
REPRESENTATION_ITEM(''));
```

The original code had a `// TODO:` comment where complex entity instances should have been handled, causing them to return `null` and not be added to the item map.

## Changes Made

### 1. `StepRepresentationItem_FromTypedParameter.cs`

**File:** `external/step/src/IxMilia.Step/Items/StepRepresentationItem_FromTypedParameter.cs`

#### Changes:

1. **Refactored `FromTypedParameter` method** to handle both simple and complex item syntax:
   - Simple items (`StepSimpleItemSyntax`) are handled by the new `CreateFromSimpleItem` method
   - Complex items (`StepComplexItemSyntax`) are handled by the new `CreateFromComplexItem` method

2. **Added `CreateFromSimpleItem` method** - Contains the original switch statement for parsing simple entity types (moved from `FromTypedParameter`)

3. **Added `CreateFromComplexItem` method** - Handles complex entity instances by:
   - Identifying the component types within the complex entity (e.g., `B_SPLINE_CURVE`, `B_SPLINE_CURVE_WITH_KNOTS`, `BOUNDED_SURFACE`, etc.)
   - For B-spline curves: Merges parameters from `B_SPLINE_CURVE` and `B_SPLINE_CURVE_WITH_KNOTS` to create a complete `StepBSplineCurveWithKnots` object
   - For B-spline surfaces: Returns a placeholder `StepUnsupportedSurface` object so they can be referenced without crashing
   - Falls back to trying each sub-item individually if no special handling is available

4. **Added `CreateBSplineCurveWithKnotsFromComplex` method** - Merges parameters from multiple sub-entities to construct a valid `StepBSplineCurveWithKnots`:
   - Takes parameters from `B_SPLINE_CURVE` (degree, control points, curve form, closed curve, self-intersect)
   - Takes parameters from `B_SPLINE_CURVE_WITH_KNOTS` (knot multiplicities, knots, knot spec)
   - Takes name from `REPRESENTATION_ITEM` if available
   - Combines them into a single syntax list matching the expected format

### 2. `StepUnsupportedSurface.cs` (New File)

**File:** `external/step/src/IxMilia.Step/Items/StepUnsupportedSurface.cs`

A new placeholder class for surface types that are not fully supported. This allows parsing to continue when encountering complex surface entities like:

- `BOUNDED_SURFACE`
- `B_SPLINE_SURFACE`
- `B_SPLINE_SURFACE_WITH_KNOTS`
- `RATIONAL_B_SPLINE_SURFACE`

These surfaces are used in `ADVANCED_FACE` entities via the `FaceGeometry` property. Without this placeholder, the parser would fail when trying to bind references to these surfaces.

```csharp
public class StepUnsupportedSurface : StepSurface
{
    public string OriginalType { get; }

    public StepUnsupportedSurface(string name, string originalType)
        : base(name)
    {
        OriginalType = originalType;
    }

    public override StepItemType ItemType => StepItemType.Plane;
}
```

## Why These Changes Were Needed

The LaserConvert application processes STEP files to extract 2D profiles from sheet metal parts for laser cutting. While B-spline surfaces aren't needed for flat sheet metal processing (they represent curved surfaces), the parser still needs to handle files that contain them without crashing.

The key requirements were:
1. **Parse B-spline curves** - These define edges in the model and are needed for accurate profile extraction
2. **Handle B-spline surfaces gracefully** - These represent curved surfaces that won't be part of a flat laser-cut panel, but the parser needs to not crash when encountering them
3. **Maintain backward compatibility** - All existing test cases continue to pass

## Testing

After these changes:
- The `Cam Pipe Clamp Sliced.stp` file parses successfully
- All previous test cases continue to pass (1box, 2boxes, 3boxes, KBox, CBox, KCBox, etc.)
- The bigbox test case continues to work correctly
