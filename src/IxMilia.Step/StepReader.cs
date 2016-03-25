// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using IxMilia.Step.Entities;
using IxMilia.Step.Syntax;

namespace IxMilia.Step
{
    internal class StepReader
    {
        public const string DateTimeFormat = "yyyy-MM-ddT";

        private StepLexer _lexer;
        private StepFile _file;

        public StepReader(Stream stream)
        {
            _file = new StepFile();
            var tokenizer = new StepTokenizer(stream);
            _lexer = new StepLexer(tokenizer.GetTokens());
        }

        public StepFile ReadFile()
        {
            var fileSyntax = _lexer.LexFileSyntax();
            foreach (var headerMacro in fileSyntax.Header.Macros)
            {
                ApplyHeaderMacro(headerMacro);
            }

            var entityMap = new Dictionary<int, StepEntity>();
            var binder = new StepBinder(entityMap);
            foreach (var entityInstance in fileSyntax.Data.EntityInstances)
            {
                if (entityMap.ContainsKey(entityInstance.Id))
                {
                    throw new StepReadException("Duplicate entity instance", entityInstance.Line, entityInstance.Column);
                }

                var entity = StepEntity.FromTypedParameter(binder, entityInstance.SimpleEntityInstance);
                if (entity != null)
                {
                    entityMap.Add(entityInstance.Id, entity);
                    _file.Entities.Add(entity);
                }
            }

            binder.BindRemainingValues();

            return _file;
        }

        private void ApplyHeaderMacro(StepHeaderMacroSyntax macro)
        {
            switch (macro.Name)
            {
                case StepFile.FileDescriptionText:
                    ApplyFileDescription(macro.Values);
                    break;
                case StepFile.FileNameText:
                    ApplyFileName(macro.Values);
                    break;
                case StepFile.FileSchemaText:
                    ApplyFileSchema(macro.Values);
                    break;
                default:
                    // TODO:
                    break;
            }
        }

        private void ApplyFileDescription(StepSyntaxList valueList)
        {
            valueList.AssertListCount(2);
            _file.Description = valueList.Values[0].GetConcatenatedStringValue();
            _file.ImplementationLevel = valueList.Values[1].GetStringValue(); // TODO: handle appropriate values
        }

        private void ApplyFileName(StepSyntaxList valueList)
        {
            valueList.AssertListCount(7);
            _file.Name = valueList.Values[0].GetStringValue();
            _file.Timestamp = valueList.Values[1].GetDateTimeValue();
            _file.Author = valueList.Values[2].GetConcatenatedStringValue();
            _file.Organization = valueList.Values[3].GetConcatenatedStringValue();
            _file.PreprocessorVersion = valueList.Values[4].GetStringValue();
            _file.OriginatingSystem = valueList.Values[5].GetStringValue();
            _file.Authorization = valueList.Values[6].GetStringValue();
        }

        private void ApplyFileSchema(StepSyntaxList valueList)
        {
            valueList.AssertListCount(1);
            foreach (var schemaType in valueList.Values[0].GetValueList().Values.Select(v => StepSchemaTypeExtensions.SchemaTypeFromName(v.GetStringValue())))
            {
                _file.Schemas.Add(schemaType);
            }
        }
    }
}
