using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using slicing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace slicingroslyn
{
    class CollectNode
    {
        private readonly SemanticModel _semanticModel;
        private readonly SyntaxNode _root;
        public readonly IEnumerable<InvocationExpressionSyntax> _invocations;
        public readonly Dictionary<string, List<string>> _keywords;
        public HashSet<SyntaxNode> collectedNode;
        //public HashSet<SyntaxNode> collectedNode1;
        private bool IsKeyNode = true;

        private readonly Logger logger;
        HashSet<IMethodSymbol> visitedMethods;
        HashSet<ISymbol> visitedPropertys;

        CancellationToken cancellationToken;


        public CollectNode(SemanticModel semanticModel, SyntaxNode keyNode, SyntaxNode root, IEnumerable<InvocationExpressionSyntax> invocations,
            Dictionary<string, List<string>> keywords, CancellationToken token)
        {
            logger = new Logger();
            _semanticModel = semanticModel;
            _root = root;
            _invocations = invocations;
            _keywords = keywords;
            collectedNode = new HashSet<SyntaxNode>();

            visitedMethods = new HashSet<IMethodSymbol>();
            visitedPropertys = new HashSet<ISymbol>();
            cancellationToken = token;
        }

        public void CollectDependencies(SyntaxNode keyNode)
        {
            logger.Information($"Start process node: {keyNode.ToString().Split('\n')[0]}");
            var ancestors = keyNode.Ancestors();
            collectedNode.UnionWith(keyNode.DescendantNodesAndSelf());
            collectedNode.UnionWith(ancestors);

            CollectArgumentDataDependency(keyNode);

            //CheckCancel();

            IsKeyNode = false;

            var methodSyntax = ancestors.OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (methodSyntax != null)
            {
                var methodSymbol = _semanticModel.GetDeclaredSymbol(methodSyntax) as IMethodSymbol;

                methodSyntax = null;

                if (!visitedMethods.Contains(methodSymbol))
                {
                    visitedMethods.Add(methodSymbol);
                    var callers = FindCallersFromMethod(methodSymbol);
                    foreach (var caller in callers)
                    {
                        //var callersmethodSyntax = caller.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
                        //if (callersmethodSyntax != null)
                        //{
                        //    var callersmethodSymbol = _semanticModel.GetDeclaredSymbol(callersmethodSyntax) as IMethodSymbol;
                        //    logger.Debug($"\tMethod {callersmethodSymbol.Name} contain {methodSymbol.Name}");
                        //}
                        CheckCancel();

                        CollectDependencies(caller);
                    }
                }
            }
            logger.Information($"End process node: {keyNode.ToString().Split('\n')[0]}");
        }


        private List<InvocationExpressionSyntax> FindCallersFromMethod(IMethodSymbol methodSymbol)
        {
            List<InvocationExpressionSyntax> callers = new List<InvocationExpressionSyntax>();

            foreach (var invocation in _invocations)
            {
                var candidateSymbol = _semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;

                if (candidateSymbol != null)
                {
                    if (methodSymbol.Equals(candidateSymbol))
                    {
                        callers.Add(invocation);
                    }
                }
            }
            return callers;
        }

        // invocation: invocationExpressionSyntax, MemberAccessExpressionSyntax
        private void CollectArgumentDataDependency(SyntaxNode invocation)
        {
            // collect statement that contain invocation 
            var statement = invocation.Ancestors().OfType<StatementSyntax>().FirstOrDefault();
            if (statement != null)
            {
                collectedNode.UnionWith(statement.DescendantNodesAndSelf());
            }

            var label = invocation.Ancestors().OfType<LabeledStatementSyntax>().FirstOrDefault();
            if (label != null)
            {
                FindArgumentLabeledDependency(label);
            }

            CheckCancel();

            var method = invocation.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (method != null)
            {
                var identifiers = CheckIfAssign(invocation).DescendantNodes().OfType<IdentifierNameSyntax>();
                foreach (var identifier in identifiers)
                {
                    var symbol = _semanticModel.GetSymbolInfo(identifier).Symbol;
                    if (symbol != null)
                    {
                        HandleSymbolKind(symbol, invocation, method);
                        CheckCancel();
                    }
                    else
                    {
                        logger.Warning("Identifier not found symbol: " + identifier.ToString());
                    }
                }
            }
        }

        private void FindArgumentLabeledDependency(LabeledStatementSyntax label)
        {
            var labelSymbol = _semanticModel.GetDeclaredSymbol(label);

            // collect gotoStatements is labeled
            var gotoStatements = _root.DescendantNodes().OfType<GotoStatementSyntax>();
            foreach (var gotoStatement in gotoStatements)
            {
                try
                {
                    var gotoLabel = _semanticModel.GetSymbolInfo(gotoStatement.Expression).Symbol;
                    if (Equals(gotoLabel, labelSymbol))
                    {
                        collectedNode.UnionWith(gotoStatement.DescendantNodesAndSelf());
                        collectedNode.UnionWith(gotoStatement.Ancestors());
                    }
                }
                catch (Exception e) { logger.Debug("Not found symbol: " + e.Message); }
            }

            gotoStatements = null;
        }

        private void HandleSymbolKind(ISymbol symbol, SyntaxNode invocation = null, MethodDeclarationSyntax method = null)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Local:
                    if (method != null && invocation != null)
                    {
                        FindArgumentLocalDependency(symbol, method, invocation);
                    }
                    break;
                case SymbolKind.Field:
                    FindArgumentFieldDependency(symbol);
                    break;
                case SymbolKind.Property:
                    FindArgumentPropertyDependency(symbol);
                    break;
                case SymbolKind.Method:
                    FindArgumentMethodDependency(symbol);
                    break;
                case SymbolKind.Parameter:
                    if (method != null)
                        FindArgumentParameterDependency(symbol, method);
                    break;
                case SymbolKind.NamedType:
                    break;
                default:
                    logger.Warning("Argument kind: " + symbol.Kind);
                    break;
            }
        }

        private void FindArgumentLocalDependency(ISymbol symbol, MethodDeclarationSyntax method, SyntaxNode invocation)
        {

            var syntaxReferences = symbol.DeclaringSyntaxReferences;

            //collect LocalDeclarationStatement depend on invocation in a method
            foreach (var reference in syntaxReferences)
            {
                var syntax = reference.GetSyntax();
                var declaration = syntax.FirstAncestorOrSelf<LocalDeclarationStatementSyntax>();
                if (declaration != null)
                {
                    collectedNode.UnionWith(declaration.DescendantNodesAndSelf());
                }
            }

            //collect statements that concains data depend on invocation in a method
            var identifierNodes = method.DescendantNodes().OfType<IdentifierNameSyntax>();
            foreach (var node in identifierNodes)
            {
                var symbolInfor = _semanticModel.GetSymbolInfo(node).Symbol;
                if (symbolInfor != null)
                {
                    if (symbolInfor.Equals(symbol))
                    {
                        var statement = node.Ancestors().OfType<StatementSyntax>().FirstOrDefault();
                        collectedNode.UnionWith(statement.DescendantNodesAndSelf());

                        //FindNestedInvocation(statement, invocation);
                    }
                }
            }
        }

        private void FindArgumentMethodDependency(ISymbol symbol)
        {
            if (IsKeyNode)
            {
                var syntaxReferences = symbol.DeclaringSyntaxReferences;
                foreach (var reference in syntaxReferences)
                {
                    var syntaxRef = reference.GetSyntax();
                    collectedNode.UnionWith(syntaxRef.DescendantNodesAndSelf());
                }
            }
        }

        private void FindArgumentPropertyDependency(ISymbol symbol)
        {
            if (visitedPropertys.Contains(symbol))
            {
                visitedPropertys.Add(symbol);

                var syntaxReferences = symbol.DeclaringSyntaxReferences;
                foreach (var reference in syntaxReferences)
                {
                    var property = reference.GetSyntax();
                    collectedNode.UnionWith(property.DescendantNodesAndSelf());
                    collectedNode.UnionWith(property.Ancestors());

                    var identifiers = property.DescendantNodes().OfType<IdentifierNameSyntax>();
                    foreach (var node in identifiers)
                    {
                        var symbolInfor = _semanticModel.GetSymbolInfo(node).Symbol;
                        if (symbolInfor != null)
                        {
                            HandleSymbolKind(symbolInfor);
                        }
                    }
                }
            }

        }

        private void FindArgumentFieldDependency(ISymbol symbol)
        {
            var syntaxReferences = symbol.DeclaringSyntaxReferences;
            foreach (var reference in syntaxReferences)
            {
                var syntaxRef = reference.GetSyntax();
                var field = syntaxRef.Ancestors().OfType<FieldDeclarationSyntax>().FirstOrDefault();
                if (field != null)
                {
                    collectedNode.UnionWith(field.DescendantNodesAndSelf());
                }
            }
        }

        private void FindArgumentParameterDependency(ISymbol symbol, MethodDeclarationSyntax method)
        {
            var descendantNodes = method.DescendantNodes();

            //collect statements depend on data on a method
            foreach (var node in descendantNodes)
            {
                if (node is IdentifierNameSyntax identifierNode && _semanticModel.GetSymbolInfo(identifierNode).Symbol?.Equals(symbol) == true)
                {
                    var statement = identifierNode.Ancestors().OfType<StatementSyntax>().FirstOrDefault();
                    if (statement != null)
                    {
                        collectedNode.UnionWith(statement.DescendantNodesAndSelf());
                    }
                }
            }
            descendantNodes = null;
        }

        private SyntaxNode CheckIfAssign(SyntaxNode invocation)
        {
            var parent = invocation.Parent;
            if (parent is AssignmentExpressionSyntax)
            {
                return parent;
            }
            else if (parent is EqualsValueClauseSyntax)
            {
                return parent.Parent;
            }
            return invocation;
        }


        private HashSet<ISymbol> FindArgumentDependOnInvocation(InvocationExpressionSyntax invocation)
        {
            HashSet<ISymbol> symbols = new HashSet<ISymbol>();

            var identifiers = CheckIfAssign(invocation).DescendantNodes().OfType<IdentifierNameSyntax>();
            foreach (var identifier in identifiers)
            {
                var symbol = _semanticModel.GetSymbolInfo(identifier).Symbol;
                if (symbol != null)
                {
                    if (symbol.Kind is SymbolKind.Local ||
                       symbol.Kind is SymbolKind.Parameter ||
                       symbol.Kind is SymbolKind.Field ||
                       symbol.Kind is SymbolKind.Property)
                    {
                        symbols.Add(symbol);
                    }
                }
            }
            return symbols;
        }

        private void CheckCancel()
        {
            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}


