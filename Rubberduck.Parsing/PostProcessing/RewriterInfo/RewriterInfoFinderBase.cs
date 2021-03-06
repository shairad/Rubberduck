using System.Collections.Generic;
using Antlr4.Runtime;
using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing.PostProcessing.RewriterInfo
{
    public abstract class RewriterInfoFinderBase : IRewriterInfoFinder
    {
        public abstract RewriterInfo GetRewriterInfo(ParserRuleContext context);
        protected static RewriterInfo GetRewriterInfoForTargetRemovedFromListStmt(IToken targetStartToken, int itemIndex, IReadOnlyList<ParserRuleContext> items)
        {
            var count = items.Count;
            var startIndex = itemIndex < count - 1
                ? targetStartToken.TokenIndex
                : items[itemIndex - 1].Stop.TokenIndex + 1;

            var stopIndex = itemIndex < count - 1
                ? items[itemIndex + 1].Start.TokenIndex - 1
                : items[itemIndex].Stop.TokenIndex;

            return new RewriterInfo(startIndex, stopIndex);
        }

        protected static int FindStopTokenIndex<TParent>(IReadOnlyList<ParserRuleContext> items, ParserRuleContext item, TParent parent)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i] != item)
                {
                    continue;
                }
                return FindStopTokenIndex((dynamic)parent, i);
            }

            return item.Stop.TokenIndex;
        }

        private static int FindStopTokenIndex(VBAParser.BlockContext context, int index)
        {
            return context.endOfStatement(index).Stop.TokenIndex;
        }

        private static int FindStopTokenIndex(VBAParser.ModuleDeclarationsContext context, int index)
        {
            return context.endOfStatement(index).Stop.TokenIndex;
        }
    }
}