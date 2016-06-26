// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Linq;

namespace IxMilia.Step.Syntax
{
    internal static class StepSyntaxExtensions
    {
        public static void AssertListCount(this StepSyntaxList syntaxList, int count)
        {
            if (syntaxList.Values.Count != count)
            {
                ReportError($"Expected list to contain {count} items but it contained {syntaxList.Values.Count}", syntaxList);
            }
        }

        public static void AssertListCount(this StepSyntaxList syntaxList, int minCount, int maxCount)
        {
            if (syntaxList.Values.Count < minCount || syntaxList.Values.Count > maxCount)
            {
                ReportError($"Expected list to contain between {minCount} and {maxCount} items but it contained {syntaxList.Values.Count}", syntaxList);
            }
        }

        public static string GetStringValue(this StepSyntax syntax)
        {
            if (syntax.SyntaxType != StepSyntaxType.String)
            {
                ReportError("Expected string value", syntax);
            }

            return ((StepStringSyntax)syntax).Value;
        }

        public static DateTime GetDateTimeValue(this StepSyntax syntax)
        {
            var str = syntax.GetStringValue();
            return DateTime.Parse(str, CultureInfo.InvariantCulture);
        }

        public static double GetRealVavlue(this StepSyntax syntax)
        {
            if (syntax.SyntaxType != StepSyntaxType.Real)
            {
                ReportError("Expected real value", syntax);
            }

            return ((StepRealSyntax)syntax).Value;
        }

        public static string GetEnumerationValue(this StepSyntax syntax)
        {
            if (syntax.SyntaxType != StepSyntaxType.Enumeration)
            {
                ReportError("Expected enumeration value", syntax);
            }

            return ((StepEnumerationValueSyntax)syntax).Value;
        }

        public static bool GetBooleanValue(this StepSyntax syntax)
        {
            switch (syntax.GetEnumerationValue().ToUpperInvariant())
            {
                case "T":
                case "TRUE":
                    return true;
                case "F":
                case "FALSE":
                    return false;
                default:
                    ReportError("Expected boolean value", syntax);
                    return false; // unreachable
            }
        }

        public static double GetRealValueOrDefault(this StepSyntaxList syntaxList, int index)
        {
            return syntaxList.GetRealValueOrDefault(index, 0.0);
        }

        public static double GetRealValueOrDefault(this StepSyntaxList syntaxList, int index, double defaultValue)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (index < syntaxList.Values.Count)
            {
                return syntaxList.Values[index].GetRealVavlue();
            }
            else
            {
                return defaultValue;
            }
        }

        public static StepSyntaxList GetValueList(this StepSyntax syntax)
        {
            if (syntax.SyntaxType != StepSyntaxType.List)
            {
                ReportError("Expected list value", syntax);
            }

            return (StepSyntaxList)syntax;
        }

        public static string GetConcatenatedStringValue(this StepSyntax syntax)
        {
            return string.Join(string.Empty, syntax.GetValueList().Values.Select(v => v.GetStringValue()));
        }

        private static void ReportError(string message, StepSyntax location)
        {
            ReportError(message, location.Line, location.Column);
        }

        private static void ReportError(string message, int line, int column)
        {
            throw new StepReadException(message, line, column);
        }
    }
}
