﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.VBA;
using Rubberduck.VBA.Grammar;
using Rubberduck.VBA.Nodes;

namespace Rubberduck.Inspections
{
    [ComVisible(false)]
    public class VariableTypeNotDeclaredInspection : IInspection
    {
        public VariableTypeNotDeclaredInspection()
        {
            Severity = CodeInspectionSeverity.Warning;
        }

        public string Name { get { return InspectionNames.VariableTypeNotDeclared; } }
        public CodeInspectionType InspectionType { get { return CodeInspectionType.CodeQualityIssues; } }
        public CodeInspectionSeverity Severity { get; set; }

        public IEnumerable<CodeInspectionResultBase> GetInspectionResults(IEnumerable<VBComponentParseResult> parseResult)
        {
            foreach (var result in parseResult)
            {
                var declarations = result.ParseTree.GetDeclarations().ToList();
                var module = result; // to avoid access to modified closure in below lambdas

                var constants = declarations.Where(declaration => declaration is VisualBasic6Parser.ConstSubStmtContext)
                                            .Cast<VisualBasic6Parser.ConstSubStmtContext>()
                                            .Where(constant => constant.asTypeClause() == null)
                                            .Select(constant => new VariableTypeNotDeclaredInspectionResult(Name, Severity, constant, module.QualifiedName));

                var variables = declarations.Where(declaration => declaration is VisualBasic6Parser.VariableSubStmtContext)
                                            .Cast<VisualBasic6Parser.VariableSubStmtContext>()
                                            .Where(variable => variable.asTypeClause() == null)
                                            .Select(variable => new VariableTypeNotDeclaredInspectionResult(Name, Severity, variable, module.QualifiedName));

                foreach (var inspectionResult in constants.Concat(variables))
                {
                    yield return inspectionResult;
                }
            }
        }
    }
}