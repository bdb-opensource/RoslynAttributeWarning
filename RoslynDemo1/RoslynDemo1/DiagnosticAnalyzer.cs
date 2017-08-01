using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RoslynDemo1
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RoslynDemo1Analyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "RoslynDemo1";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction(AnalyzerInvocation, SyntaxKind.InvocationExpression);
        }

        private void AnalyzerInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            var methodDelcaration = (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol as IMethodSymbol);

            if (null == methodDelcaration)
            {
                return;
            }

            var methodAttributes = methodDelcaration.GetAttributes();
            var attributeData = methodAttributes.FirstOrDefault(attr =>
                        IsIDEMessageAttribute(context.SemanticModel, attr, typeof(IDEMessageAttribute)));

            var message = this.GetMessage(attributeData);

            var diagnoistic = Diagnostic.Create(Rule, invocation.GetLocation(), methodDelcaration.Name, message);
                    
        }

        private string GetMessage(AttributeData attributeData)
        {
            if(attributeData.ConstructorArguments.Length < 1)
            {
                return "this method is obsolete";
            }

            return (attributeData.ConstructorArguments[0].Value as string);
        }

        private bool IsIDEMessageAttribute(SemanticModel semanticModel, AttributeData attr, Type type)
        {
            var desiredTypeNamedSymbol = semanticModel.Compilation.GetTypeByMetadataName(type.FullName);

            var result = attr.AttributeClass.Equals(desiredTypeNamedSymbol);
            return result;
        }
    }
}
