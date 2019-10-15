IxMilia.Step
============

A portable .NET library for reading and writing STEP CAD files.

## Usage

Open a STEP file:

``` C#
using System.IO;
using IxMilia.Step;
using IxMilia.Step.Items;
// ...

//------------------------------------------------------------ read from a file
StepFile stepFile;
using (FileStream fs = new FileStream(@"C:\Path\To\File.stp", FileMode.Open))
{
    stepFile = StepFile.Load(fs);
}

// if on >= NETStandard1.3 you can use:
// StepFile stepFile = StepFile.Load(@"C:\Path\To\File.stp");

//---------------------------------------------- or read directly from a string
StepFile stepFile = StepFile.Parse(@"ISO-10303-21;
HEADER;
...
END-ISO-103030-21;");
//-----------------------------------------------------------------------------

foreach (StepRepresentationItem item in stepFile.Items)
{
    switch (item.ItemType)
    {
        case StepItemType.Line:
            StepLine line = (StepLine)item;
            // ...
            break;
        // ...
    }
}
```

Save a STEP file:

``` C#
using System.IO;
using IxMilia.Step;
using IxMilia.Step.Items;
// ...

StepFile stepFile = new StepFile();
stepFile.Items.Add(new StepDirection("direction-label", 1.0, 0.0, 0.0));
// ...

//------------------------------------------------------------- write to a file
using (FileStream fs = new FileStream(@"C:\Path\To\File.stp", FileMode.Create))
{
    stepFile.Save(fs);
}

// if on >= NETStandard1.3 you can use
// stepFile.Save(@"C:\Path\To\File.stp");

//------------------------------------------------------- or output as a string
string contents = stepFile.GetContentsAsString();
```

## Building locally

To build locally, install the [latest .NET Core 3.0 SDK](https://dotnet.microsoft.com/download).

## Specification

Using spec from steptools.com [here](http://www.steptools.com/library/standard/IS_final_p21e3.html).

STEP Application Protocols [here](http://www.steptools.com/support/stdev_docs/express/).
