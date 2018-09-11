IxMilia.Step
============

A portable .NET library for reading and writing STEP CAD files.

[![Build Status](https://dev.azure.com/ixmilia/public/_apis/build/status/Step?branchName=master)](https://dev.azure.com/ixmilia/public/_build/latest?definitionId=25)

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

//------------------------------------------------------- or output as a string
string contents = stepFile.GetContentsAsString();
```

## Building locally

Requirements to build locally are:

- [Latest .NET Core SDK](https://github.com/dotnet/cli/releases)  As of this writing the following was also required on Ubuntu 14.04:

`sudo apt-get install dotnet-sharedframework-microsoft.netcore.app-1.0.3`

## Specification

Using spec from steptools.com [here](http://www.steptools.com/library/standard/IS_final_p21e3.html).

STEP Application Protocols [here](http://www.steptools.com/support/stdev_docs/express/).
