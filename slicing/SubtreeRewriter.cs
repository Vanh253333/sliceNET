using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using slicing;

namespace slicingroslyn
{
    public class SubtreeRewriter : CSharpSyntaxRewriter
    {
        private readonly HashSet<SyntaxNode> _relatedNodes;
        private readonly Logger logger;

        public SubtreeRewriter(HashSet<SyntaxNode> relatedNodes)
        {
            _relatedNodes = relatedNodes;
            logger = new Logger();
        }

        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitCompilationUnit(node);
            }
            return null;
        }

        public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitUsingDirective(node);
            }
            return null;
        }


        public override SyntaxNode VisitFileScopedNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitFileScopedNamespaceDeclaration(node);
            }
            return null;
        }

        public override SyntaxNode VisitAttributeList(AttributeListSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitAttributeList(node);
            }
            return null;
        }


        public override SyntaxNode VisitAttribute(AttributeSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitAttribute(node);
            }
            return null;
        }

        public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitNamespaceDeclaration(node);
            }
            return null;
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                //var identifierToken = node.Identifier;
                //var newName = $"Class{++classCount}";

                //var newNode = node.WithIdentifier(SyntaxFactory.Identifier(newName).WithTriviaFrom(identifierToken));
                return base.VisitClassDeclaration(node);
            }
            return null;
        }

        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitInterfaceDeclaration(node);
            }
            return null;
        }

        public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitEnumDeclaration(node);
            }
            return null;
        }

        public override SyntaxNode VisitStructDeclaration(StructDeclarationSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitStructDeclaration(node);
            }
            return null;
        }

        public override SyntaxNode VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitDelegateDeclaration(node);
            }
            return null;
        }

        public override SyntaxNode VisitEventDeclaration(EventDeclarationSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitEventDeclaration(node);
            }
            return null;
        }

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitPropertyDeclaration(node);
            }
            return null;
        }

        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitConstructorDeclaration(node);
            }
            return null;
        }

        public override SyntaxNode VisitConstructorInitializer(ConstructorInitializerSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitConstructorInitializer(node);
            }
            return null;

        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitMethodDeclaration(node);
            }
            return null;
        }

        //public override SyntaxNode VisitBlock(BlockSyntax node)
        //{
        //    if (_relatedNodes.Contains(node))
        //    {
        //        return base.VisitBlock(node);
        //    }
        //    return null;
        //}


        public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitExpressionStatement(node);
            }
            return null;
        }


        public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitLocalDeclarationStatement(node);
            }
            return null;

        }

        public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitIfStatement(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.Condition.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }

            }
            return null;
        }

        public override SyntaxNode VisitElseClause(ElseClauseSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitElseClause(node);
            }
            return null;
        }



        public override SyntaxNode VisitSwitchStatement(SwitchStatementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitSwitchStatement(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.Expression.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;


        }

        public override SyntaxNode VisitSwitchSection(SwitchSectionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitSwitchSection(node);
            }
            return null;
        }


        public override SyntaxNode VisitWhileStatement(WhileStatementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitWhileStatement(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.Condition.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;
        }

        public override SyntaxNode VisitDoStatement(DoStatementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitDoStatement(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.Condition.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }

            }
            return null;
        }

        public override SyntaxNode VisitForStatement(ForStatementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitForStatement(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.Condition.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;
        }

        public override SyntaxNode VisitForEachStatement(ForEachStatementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitForEachStatement(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.Expression.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }

            }
            return null;
        }

        public override SyntaxNode VisitTryStatement(TryStatementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitTryStatement(node);
            }
            return null;
        }

        public override SyntaxNode VisitCatchClause(CatchClauseSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitCatchClause(node);
            }
            return null;
        }

        public override SyntaxNode VisitFinallyClause(FinallyClauseSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitFinallyClause(node);
            }
            return null;
        }

        public override SyntaxNode VisitReturnStatement(ReturnStatementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitReturnStatement(node);
            }
            return null;
        }

        public override SyntaxNode VisitContinueStatement(ContinueStatementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitContinueStatement(node);
            }
            return null;
        }

        public override SyntaxNode VisitBreakStatement(BreakStatementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitBreakStatement(node);
            }
            return null;
        }

        public override SyntaxNode VisitGotoStatement(GotoStatementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitGotoStatement(node);
            }
            return null;
        }

        public override SyntaxNode VisitGlobalStatement(GlobalStatementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitGlobalStatement(node);
            }
            return null;
        }

        public override SyntaxNode VisitDefaultExpression(DefaultExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitDefaultExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }

            }
            return null;
        }

        public override SyntaxNode VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitParenthesizedLambdaExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ExpressionBody.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }

            }
            return null;
        }

        public override SyntaxNode VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitSimpleLambdaExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ExpressionBody.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }

            }
            return null;
        }

        public override SyntaxNode VisitUsingStatement(UsingStatementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitUsingStatement(node);
            }
            return null;
        }

        public override SyntaxNode VisitLockStatement(LockStatementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitLockStatement(node);
            }
            return null;
        }

        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitFieldDeclaration(node);
            }
            return null;
        }

        public override SyntaxNode VisitThrowStatement(ThrowStatementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitThrowStatement(node);
            }
            return null;
        }

        public override SyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitVariableDeclarator(node);
            }
            return null;
        }

        public override SyntaxNode VisitLabeledStatement(LabeledStatementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitLabeledStatement(node);
            }
            return null;
        }

        public override SyntaxNode VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitAssignmentExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.Left.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }

            }
            return null;
        }


        public override SyntaxNode VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitParenthesizedExpression(node);
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    return node;
                }

            }
            return null;
        }

        public override SyntaxNode VisitArgument(ArgumentSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitArgument(node);
            }
            return null;
        }

        public override SyntaxNode VisitArgumentList(ArgumentListSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitArgumentList(node);
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    return node;
                }

            }
            return null;
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitInvocationExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.Expression.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;
        }

        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitBinaryExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.Right.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;
        }

        public override SyntaxNode VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitPrefixUnaryExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error(e.ToString());
                }

            }
            return null;
        }

        public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitMemberAccessExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error(e.ToString());
                }

            }
            return null;

        }

        public override SyntaxNode VisitCaseSwitchLabel(CaseSwitchLabelSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitCaseSwitchLabel(node);
            }
            return null;
        }

        public override SyntaxNode VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                return base.VisitEqualsValueClause(node);
            }
            return null;

        }

        public override SyntaxNode VisitInitializerExpression(InitializerExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitInitializerExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitArrayCreationExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitElementAccessExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitConditionalAccessExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitCollectionExpression(CollectionExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitCollectionExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitCheckedExpression(CheckedExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitCheckedExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitCastExpression(CastExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitCastExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitBaseExpression(BaseExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitBaseExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitAwaitExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitExpressionColon(ExpressionColonSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitExpressionColon(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitAnonymousMethodExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }

        public override SyntaxNode VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitAnonymousObjectCreationExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitExpressionElement(ExpressionElementSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitExpressionElement(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }

        public override SyntaxNode VisitThisExpression(ThisExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitThisExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitRefValueExpression(RefValueExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitRefValueExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitThrowExpression(ThrowExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitThrowExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitTupleExpression(TupleExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitTupleExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitArrowExpressionClause(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitConditionalExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitDeclarationExpression(DeclarationExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitDeclarationExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitElementBindingExpression(ElementBindingExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitElementBindingExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitImplicitArrayCreationExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitImplicitObjectCreationExpression(ImplicitObjectCreationExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitImplicitObjectCreationExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitImplicitStackAllocArrayCreationExpression(ImplicitStackAllocArrayCreationExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitImplicitStackAllocArrayCreationExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitInterpolatedStringExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitIsPatternExpression(IsPatternExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitIsPatternExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }

        public override SyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitLiteralExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitMakeRefExpression(MakeRefExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitMakeRefExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitMemberBindingExpression(MemberBindingExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {

                    return base.VisitMemberBindingExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;
        }


        public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitObjectCreationExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitOmittedArraySizeExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitPostfixUnaryExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitQueryExpression(QueryExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitQueryExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitRangeExpression(RangeExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitRangeExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitRefExpression(RefExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitRefExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitRefTypeExpression(RefTypeExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitRefTypeExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitSizeOfExpression(SizeOfExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitSizeOfExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitStackAllocArrayCreationExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }


        public override SyntaxNode VisitSwitchExpression(SwitchExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitSwitchExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }

        public override SyntaxNode VisitSwitchExpressionArm(SwitchExpressionArmSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitSwitchExpressionArm(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }

        public override SyntaxNode VisitTypeOfExpression(TypeOfExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitTypeOfExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }

        public override SyntaxNode VisitWithExpression(WithExpressionSyntax node)
        {
            if (_relatedNodes.Contains(node))
            {
                try
                {
                    return base.VisitWithExpression(node);
                }
                catch (Exception e)
                {
                    logger.Debug(node.ToString());
                    logger.Error($"Can't get it: {e.ToString()}");
                    return node;
                }
            }
            return null;

        }

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
                trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
                trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
                trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia) ||
                trivia.IsKind(SyntaxKind.DocumentationCommentExteriorTrivia))
            {
                return default(SyntaxTrivia);
            }
            return base.VisitTrivia(trivia);
        }
    }
}
