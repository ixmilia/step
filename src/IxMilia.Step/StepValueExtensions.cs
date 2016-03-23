// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using IxMilia.Step.Tokens;

namespace IxMilia.Step
{
    public static class StepValueExtensions
    {
        private static void ReportError(string message, int line, int column)
        {
            throw new StepReadException(message, line, column);
        }

        public static string GetStringValue(this StepValue value)
        {
            if (value is StepIndividualValue && ((StepIndividualValue)value).Value.Kind == StepTokenKind.String)
            {
                return ((StepStringToken)(((StepIndividualValue)value).Value)).Value;
            }
            else
            {
                ReportError("Expected string token", value.Line, value.Column);
                return null; // unreachable
            }
        }

        public static double GetDoubleValue(this StepValue value)
        {
            if (value is StepIndividualValue && ((StepIndividualValue)value).Value.Kind == StepTokenKind.Real)
            {
                return ((StepRealToken)(((StepIndividualValue)value).Value)).Value;
            }
            else
            {
                ReportError("Expected string token", value.Line, value.Column);
                return 0.0; // unreachable
            }
        }

        public static StepValueList GetValueList(this StepValue value)
        {
            if (value is StepValueList)
            {
                return (StepValueList)value;
            }
            else
            {
                ReportError("Expected value list", value.Line, value.Column);
                return null; // unreachable
            }
        }

        public static double GetDoubleValueOrDefault(this StepValueList valueList, int index, double defaultValue)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (index < valueList.Values.Count)
            {
                return valueList.Values[index].GetDoubleValue();
            }
            else
            {
                return defaultValue;
            }
        }

        public static double GetDoubleValueOrDefault(this StepValueList valueList, int index)
        {
            return valueList.GetDoubleValueOrDefault(index, 0.0);
        }

        public static void AssertValueListCount(this StepValueList valueList, int expectedCount)
        {
            if (valueList.Values.Count != expectedCount)
            {
                ReportError($"Expected {expectedCount} values but got {valueList.Values.Count}", valueList.Line, valueList.Column);
            }
        }

        public static void AssertValueListCount(this StepValueList valueList, int minExpectedCount, int maxExpectedCount)
        {
            if (valueList.Values.Count < minExpectedCount || valueList.Values.Count > maxExpectedCount)
            {
                ReportError($"Expected {minExpectedCount} to {maxExpectedCount} values but got {valueList.Values.Count}", valueList.Line, valueList.Column);
            }
        }
    }
}
