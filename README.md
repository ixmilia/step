IxMilia.Step
============

A portable .NET library for reading and writing STEP CAD files.

### Usage

Open a STEP file:

``` C#
using System.IO;
using IxMilia.Step;
using IxMilia.Step.Entities;
// ...

//------------------------------------------------------------ read from a file
StepFile stepFile;
using (FileStream fs = new FileStream(@"C:\Path\To\File.stp", FileMode.Open))
{
    stepFile = StepFile.Load(fs);
}

//---------------------------------------------- or read directly from a string
StepFile stepFile = StepFile.Parse(@"ISO-10303-21;
HEADER
...
END-ISO-103030-21;");
//-----------------------------------------------------------------------------

foreach (StepEntity entity in stepFile.Entities)
{
    switch (entity.EntityType)
    {
        case StepEntityType.Line:
            StepLine line = (StepLine)entity;
            // ...
            break;
        // ...
    }
}
```

Save a STEP file:

``` C#
// TODO
```

### Specification

Using spec from steptools.com [here](http://www.steptools.com/library/standard/IS_final_p21e3.html).
STEP Application Protocols [here](http://www.steptools.com/support/stdev_docs/express/).
