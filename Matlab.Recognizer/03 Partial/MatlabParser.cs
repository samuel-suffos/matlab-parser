/* 
 * [The BSD 3-Clause License]
 * Copyright (c) 2015, Samuel Suffos
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice, this list 
 *    of conditions and the following disclaimer.
 * 
 * 2. Redistributions in binary form must reproduce the above copyright notice, this 
 *    list of conditions and the following disclaimer in the documentation and/or other 
 *    materials provided with the distribution.
 * 
 * 3. Neither the name of the copyright holder nor the names of its contributors may be 
 *    used to endorse or promote products derived from this software without specific 
 *    prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY 
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT 
 * SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, 
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED 
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
 * BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH 
 * DAMAGE.
 *
 */

using Antlr.Runtime;
using Matlab.Nodes;
using Matlab.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matlab.Recognizer
{
    internal partial class MatlabParser : Parser
    {
        #region PROPERTIES:

        public Configuration Configuration { get; private set; }

        public ExtendedTokenStream Input
        {
            get
            {
                return ((ExtendedTokenStream)this.TokenStream);
            }
        }

        #endregion

        #region CONSTRUCTORS:

        public MatlabParser(ITokenStream input, Configuration configuration)
            : this(input)
        {
            Checker.CheckNotNull(configuration);

            this.Configuration = configuration;
        }

        #endregion

        #region METHODS:

        #region ERROR METHODS:

        public override void ReportError(RecognitionException e)
        {
            this.Configuration.Report.AddError(this.Configuration.Path, e.Line, e.CharPositionInLine + 1, string.Format("PARSER - {0}", this.GetErrorMessage(e, this.TokenNames)));

            if (this.Configuration.StopOnFirstError)
            {
                throw new StopException();
            }
        }

        #endregion

        #region BOOLEAN METHODS:

        private bool True()
        {
            return true;
        }

        private bool False()
        {
            return false;
        }

        #endregion

        #endregion

        #region GROUP MEMBERS:

        #region KEYWORD GROUP:

        private bool Keyword_EventsIdFollows()
        {
            IToken token1 = this.Input.LT(1);

            return (token1.Type == MatlabLexer.ID && token1.Text == "events");
        }

        private bool Keyword_PropertiesIdFollows()
        {
            IToken token1 = this.Input.LT(1);

            return (token1.Type == MatlabLexer.ID && token1.Text == "properties");
        }

        private bool Keyword_MethodsIdFollows()
        {
            IToken token1 = this.Input.LT(1);

            return (token1.Type == MatlabLexer.ID && token1.Text == "methods");
        }

        private bool Keyword_EnumerationIdFollows()
        {
            IToken token1 = this.Input.LT(1);

            return (token1.Type == MatlabLexer.ID && token1.Text == "enumeration");
        }

        #endregion

        #region COMMAND GROUP:

        private static int[] command_SpecialOperatorsSet = { MatlabLexer.PLUS, MatlabLexer.MINUS, MatlabLexer.MTIMES, MatlabLexer.TIMES, MatlabLexer.MRDIV, MatlabLexer.RDIV, MatlabLexer.MLDIV, MatlabLexer.LDIV, MatlabLexer.MPOW, MatlabLexer.POW, MatlabLexer.EQ, MatlabLexer.NOTEQ, MatlabLexer.LT, MatlabLexer.LTEQ, MatlabLexer.GT, MatlabLexer.GTEQ, MatlabLexer.AND, MatlabLexer.SHORTAND, MatlabLexer.OR, MatlabLexer.SHORTOR, MatlabLexer.AT, MatlabLexer.COLON, MatlabLexer.EXCLAMATION };

        private static int[] command_SpecialOperatorsFollowSet = { MatlabLexer.PLUS, MatlabLexer.MINUS, MatlabLexer.NOT, MatlabLexer.ID, MatlabLexer.REAL, MatlabLexer.IMAGINARY, MatlabLexer.STRING, MatlabLexer.LPAREN, MatlabLexer.LSQUARE, MatlabLexer.LCURLY, MatlabLexer.QUESTION, MatlabLexer.EXCLAMATION, MatlabLexer.BREAK, MatlabLexer.CASE, MatlabLexer.CATCH, MatlabLexer.CLASSDEF, MatlabLexer.CONTINUE, MatlabLexer.ELSE, MatlabLexer.ELSEIF, MatlabLexer.END, MatlabLexer.FOR, MatlabLexer.FUNCTION, MatlabLexer.GLOBAL, MatlabLexer.IF, MatlabLexer.OTHERWISE, MatlabLexer.PARFOR, MatlabLexer.PERSISTENT, MatlabLexer.RETURN, MatlabLexer.SPMD, MatlabLexer.SWITCH, MatlabLexer.TRY, MatlabLexer.WHILE };

        private void Command_Mark()
        {
            IToken token1 = this.Input.LT(1);

            this.Configuration.CommandMarker.Add(token1.StartIndex);
        }

        private void Command_Abort()
        {
            throw new CommandException();
        }

        private bool Command_IsEnabled()
        {
            IToken token1 = this.Input.LT(1);

            IToken token2 = this.Input.LT(2);

            IToken token3 = this.Input.LT(3);

            return (token1.Type == MatlabLexer.ID
                    && this.Input.GetOffChannelTokensToRight(token1.TokenIndex, (int)Channel.Spaces) != null
                    && token2.Type != MatlabLexer.LPAREN
                    && token2.Type != MatlabLexer.RPAREN
                    && token2.Type != MatlabLexer.ASSIGN
                    && !(MatlabParser.command_SpecialOperatorsSet.Contains(token2.Type)
                         && this.Input.GetOffChannelTokensToRight(token2.TokenIndex, (int)Channel.Spaces) != null
                         && MatlabParser.command_SpecialOperatorsFollowSet.Contains(token3.Type)
                        )
                    );
        }

        #endregion

        #region SEPARATOR GROUP:

        private bool Separator_CommaFollowsOrSpacesPrecede()
        {
            IToken token1 = this.Input.LT(1);

            return (token1.Type == MatlabLexer.COMMA || this.Input.GetOffChannelTokensToLeft(token1.TokenIndex, (int)Channel.Spaces) != null);
        }

        private bool Separator_SpacesPrecede()
        {
            IToken token1 = this.Input.LT(1);

            return (this.Input.GetOffChannelTokensToLeft(token1.TokenIndex, (int)Channel.Spaces) != null);
        }

        private bool Separator_EllipsisFollows()
        {
            IToken token1 = this.Input.LT(1);

            IList<IToken> offChannelTokensToLeft = this.Input.GetOffChannelTokensToLeft(token1.TokenIndex, (int)Channel.Spaces);

            return (offChannelTokensToLeft != null && offChannelTokensToLeft.FirstOrDefault(x => x.Type == MatlabLexer.ELLIPSIS) != null);
        }

        private bool Separator_EllipsisOrEolFollows()
        {
            IToken token1 = this.Input.LT(1);

            if (token1.Type == MatlabLexer.EOL)
            {
                return true;
            }
            else
            {
                IList<IToken> offChannelTokensToLeft = this.Input.GetOffChannelTokensToLeft(token1.TokenIndex, (int)Channel.Spaces);

                return (offChannelTokensToLeft != null && offChannelTokensToLeft.FirstOrDefault(x => x.Type == MatlabLexer.ELLIPSIS) != null);
            }
        }

        #endregion

        #region INDEX GROUP:

        private Stack<IndexOperator> index_Stack = new Stack<IndexOperator>();

        private IndexOperator Index_GetTop()
        {
            if (StackHelper.IsEmpty(this.index_Stack))
            {
                return IndexOperator.None;
            }
            else
            {
                return StackHelper.Peek(this.index_Stack);
            }
        }

        private void Index_EnterParenthesis()
        {
            StackHelper.Push(this.index_Stack, IndexOperator.Parenthesis);
        }

        private void Index_EnterCurlyBrace()
        {
            StackHelper.Push(this.index_Stack, IndexOperator.CurlyBrace);
        }

        private void Index_ExitParenthesis()
        {
            StackHelper.Pop(this.index_Stack, IndexOperator.Parenthesis);
        }

        private void Index_ExitCurlyBrace()
        {
            StackHelper.Pop(this.index_Stack, IndexOperator.CurlyBrace);
        }

        private bool Index_IsActive()
        {
            return !StackHelper.IsEmpty(this.index_Stack);
        }

        #endregion

        #region BALANCE GROUP:

        private Stack<BalanceOperator> balance_Stack = new Stack<BalanceOperator>();

        private void Balance_EnterCreationSquareBrace()
        {
            StackHelper.Push(this.balance_Stack, BalanceOperator.CreationSquareBrace);
        }

        private void Balance_EnterCreationCurlyBrace()
        {
            StackHelper.Push(this.balance_Stack, BalanceOperator.CreationCurlyBrace);
        }

        private void Balance_EnterStorageSquareBrace()
        {
            StackHelper.Push(this.balance_Stack, BalanceOperator.StorageSquareBrace);
        }

        private void Balance_EnterIndexCurlyBrace()
        {
            StackHelper.Push(this.balance_Stack, BalanceOperator.IndexCurlyBrace);
        }

        private void Balance_EnterParenthesis()
        {
            StackHelper.Push(this.balance_Stack, BalanceOperator.Parenthesis);
        }

        private void Balance_ExitCreationSquareBrace()
        {
            StackHelper.Pop(this.balance_Stack, BalanceOperator.CreationSquareBrace);
        }

        private void Balance_ExitCreationCurlyBrace()
        {
            StackHelper.Pop(this.balance_Stack, BalanceOperator.CreationCurlyBrace);
        }

        private void Balance_ExitStorageSquareBrace()
        {
            StackHelper.Pop(this.balance_Stack, BalanceOperator.StorageSquareBrace);
        }

        private void Balance_ExitIndexCurlyBrace()
        {
            StackHelper.Pop(this.balance_Stack, BalanceOperator.IndexCurlyBrace);
        }

        private void Balance_ExitParenthesis()
        {
            StackHelper.Pop(this.balance_Stack, BalanceOperator.Parenthesis);
        }

        private BalanceOperator Balance_GetTop()
        {
            if (StackHelper.IsEmpty(this.balance_Stack))
            {
                return BalanceOperator.None;
            }
            else
            {
                return StackHelper.Peek(this.balance_Stack);
            }
        }

        private bool Balance_TopIsCreationOrStoreOperator()
        {
            BalanceOperator balanceOperator = this.Balance_GetTop();

            switch (balanceOperator)
            {
                case BalanceOperator.CreationSquareBrace:
                case BalanceOperator.CreationCurlyBrace:
                case BalanceOperator.StorageSquareBrace:
                    return true;
                default:
                    return false;
            }
        }

        private bool Balance_InCreationOrStore_SpacesOnLeftButNotOnRight()
        {
            if (this.Balance_TopIsCreationOrStoreOperator())
            {
                IToken token1 = this.Input.LT(1);

                IList<IToken> offChannelTokensToLeft = this.Input.GetOffChannelTokensToLeft(token1.TokenIndex, (int)Channel.Spaces);

                IList<IToken> offChannelTokensToRight = this.Input.GetOffChannelTokensToRight(token1.TokenIndex, (int)Channel.Spaces);

                return (offChannelTokensToLeft != null && offChannelTokensToRight == null);
            }
            else
            {
                return false;
            }
        }

        private bool Balance_InCreationOrStore_SpacesOnLeft()
        {
            if (this.Balance_TopIsCreationOrStoreOperator())
            {
                IToken token1 = this.Input.LT(1);

                IList<IToken> offChannelTokensToLeft = this.Input.GetOffChannelTokensToLeft(token1.TokenIndex, (int)Channel.Spaces);

                return (offChannelTokensToLeft != null);
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region CHAIN GROUP:

        private Stack<ChainOperator> chain_Stack = new Stack<ChainOperator>();

        private void Chain_Begin()
        {
            StackHelper.Push(this.chain_Stack, ChainOperator.Start);
        }

        private void Chain_End()
        {
            while (!StackHelper.OnTop(this.chain_Stack, ChainOperator.Start))
            {
                StackHelper.Pop(this.chain_Stack);
            }

            StackHelper.Pop(this.chain_Stack, ChainOperator.Start);
        }

        private void Chain_AddedParenthesis()
        {
            StackHelper.Push(this.chain_Stack, ChainOperator.Parenthesis);
        }

        private void Chain_AddedCurlyBrace()
        {
            StackHelper.Push(this.chain_Stack, ChainOperator.CurlyBrace);
        }

        private void Chain_AddedDotName()
        {
            StackHelper.Push(this.chain_Stack, ChainOperator.DotName);
        }

        private void Chain_AddedDotExpression()
        {
            StackHelper.Push(this.chain_Stack, ChainOperator.DotExpression);
        }

        private void Chain_AddedAtBase()
        {
            StackHelper.Push(this.chain_Stack, ChainOperator.AtBase);
        }

        private ChainOperator Chain_GetTop()
        {
            if (StackHelper.IsEmpty(this.chain_Stack))
            {
                return ChainOperator.None;
            }
            else
            {
                return StackHelper.Peek(this.chain_Stack);
            }
        }

        private bool Chain_MayAddParenthesis()
        {
            ChainOperator chainOperator = this.Chain_GetTop();

            switch (chainOperator)
            {
                case ChainOperator.Start:
                case ChainOperator.CurlyBrace:
                case ChainOperator.DotName:
                case ChainOperator.DotExpression:
                case ChainOperator.AtBase:
                    return true;
                default:
                    return false;
            }
        }

        private bool Chain_MayAddCurlyBrace()
        {
            ChainOperator chainOperator = this.Chain_GetTop();

            switch (chainOperator)
            {
                case ChainOperator.Start:
                case ChainOperator.CurlyBrace:
                case ChainOperator.DotName:
                case ChainOperator.DotExpression:
                    return true;
                default:
                    return false;
            }
        }

        private bool Chain_MayAddDotName()
        {
            ChainOperator chainOperator = this.Chain_GetTop();

            switch (chainOperator)
            {
                case ChainOperator.Start:
                case ChainOperator.Parenthesis:
                case ChainOperator.CurlyBrace:
                case ChainOperator.DotName:
                case ChainOperator.DotExpression:
                    return true;
                default:
                    return false;
            }
        }

        private bool Chain_MayAddDotExpression()
        {
            ChainOperator chainOperator = this.Chain_GetTop();

            switch (chainOperator)
            {
                case ChainOperator.Start:
                case ChainOperator.Parenthesis:
                case ChainOperator.CurlyBrace:
                case ChainOperator.DotName:
                case ChainOperator.DotExpression:
                    return true;
                default:
                    return false;
            }
        }

        private bool Chain_MayAddAtBase()
        {
            ChainOperator chainOperator = this.Chain_GetTop();

            switch (chainOperator)
            {
                case ChainOperator.Start:
                case ChainOperator.DotName:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region METHOD GROUP:

        private bool method_Signature = false;

        private void Method_EnterSignature()
        {
            this.method_Signature = true;
        }

        private void Method_ExitSignature()
        {
            this.method_Signature = false;
        }

        private bool Method_SignatureIsActive()
        {
            return this.method_Signature;
        }

        #endregion

        #region TEXT GROUP:

        private string Text_UnquoteString(string text)
        {
            return Text.UnquoteString(text);
        }

        private string Text_QuoteString(string text)
        {
            return Text.QuoteString(text);
        }

        #endregion

        #endregion
    }
}
