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
using Matlab.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matlab.Recognizer
{
    internal partial class MatlabLexer : Lexer
    {
        #region PROPERTIES:

        public Configuration Configuration { get; private set; }

        private ICharStream Input
        {
            get
            {
                return this.CharStream;
            }
        }

        #endregion

        #region CONSTRUCTORS:

        public MatlabLexer(ICharStream input, Configuration configuration)
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
            this.Configuration.Report.AddError(this.Configuration.Path, e.Line, e.CharPositionInLine + 1, string.Format("LEXER - {0}", this.GetErrorMessage(e, this.TokenNames)));

            if (this.Configuration.StopOnFirstError)
            {
                throw new StopException();
            }
        }

        #endregion

        #region MATCH METHODS:

        public override IToken NextToken()
        {
            IToken token = base.NextToken();

            this.Quotation_AddTokenIfMeaningful(token);

            return token;
        }

        #endregion

        #endregion

        #region GROUP MEMBERS:

        #region MACHINE GROUP:

        private int machine_TokenType = int.MinValue;

        private Channel machine_TokenChannel = Channel.Default;

        private Mode machine_LexerMode = Mode.Default;

        private void Machine_Update(int tokenType, Channel tokenChannel, Mode lexerMode)
        {
            this.machine_TokenType = tokenType;

            this.machine_TokenChannel = tokenChannel;

            this.machine_LexerMode = lexerMode;
        }

        private void Machine_Update(string text, int tokenType, Channel tokenChannel, Mode lexerMode)
        {
            if (tokenType == MatlabLexer.ID)
            {
                switch (text)
                {
                    case "break":
                        tokenType = MatlabLexer.BREAK;
                        break;
                    case "case":
                        tokenType = MatlabLexer.CASE;
                        break;
                    case "catch":
                        tokenType = MatlabLexer.CATCH;
                        break;
                    case "classdef":
                        tokenType = MatlabLexer.CLASSDEF;
                        break;
                    case "continue":
                        tokenType = MatlabLexer.CONTINUE;
                        break;
                    case "else":
                        tokenType = MatlabLexer.ELSE;
                        break;
                    case "elseif":
                        tokenType = MatlabLexer.ELSEIF;
                        break;
                    case "end":
                        tokenType = MatlabLexer.END;
                        break;
                    case "for":
                        tokenType = MatlabLexer.FOR;
                        break;
                    case "function":
                        tokenType = MatlabLexer.FUNCTION;
                        break;
                    case "global":
                        tokenType = MatlabLexer.GLOBAL;
                        break;
                    case "if":
                        tokenType = MatlabLexer.IF;
                        break;
                    case "otherwise":
                        tokenType = MatlabLexer.OTHERWISE;
                        break;
                    case "parfor":
                        tokenType = MatlabLexer.PARFOR;
                        break;
                    case "persistent":
                        tokenType = MatlabLexer.PERSISTENT;
                        break;
                    case "return":
                        tokenType = MatlabLexer.RETURN;
                        break;
                    case "spmd":
                        tokenType = MatlabLexer.SPMD;
                        break;
                    case "switch":
                        tokenType = MatlabLexer.SWITCH;
                        break;
                    case "try":
                        tokenType = MatlabLexer.TRY;
                        break;
                    case "while":
                        tokenType = MatlabLexer.WHILE;
                        break;
                    default:
                        tokenType = (this.Configuration.CommandMarker.Contains(this.CharIndex - text.Length) ? MatlabLexer.COMMAND : tokenType);
                        break;
                }

                lexerMode = (tokenType == MatlabLexer.COMMAND ? Mode.Command : lexerMode);

                this.Machine_Update(tokenType, tokenChannel, lexerMode);
            }
            else
            {
                throw new InternalException();
            }
        }

        private bool Machine_InMode(Mode mode)
        {
            return (this.machine_LexerMode == mode);
        }

        #endregion

        #region LINE GROUP:

        private bool Line_StartPrecedes()
        {
            int value = this.Input.LA(-1);

            if (value >= 0)
            {
                char character = Convert.ToChar(value);

                return (character == '\r' || character == '\n');
            }
            else
            {
                return true;
            }
        }

        private bool Line_EndFollows()
        {
            int value = this.Input.LA(1);

            if (value >= 0)
            {
                char character = Convert.ToChar(value);

                return (character == '\r' || character == '\n');
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region SPACES GROUP:

        private bool Spaces_SpacesPrecedeInLine()
        {
            int index = -1;

            int value;

            while ((value = this.Input.LA(index)) >= 0)
            {
                char character = Convert.ToChar(value);

                if (character == '\r' || character == '\n')
                {
                    return true;
                }
                else if (character != ' ' && character != '\t')
                {
                    return false;
                }
                else
                {
                    index--;
                }
            }

            return true;
        }

        #endregion

        #region TEXT GROUP:

        private bool Text_TextFollows(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                int value = this.Input.LA(i + 1);

                if (value >= 0)
                {
                    char character = Convert.ToChar(value);

                    if (character != text[i])
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private bool Text_ThreeDotsFollow()
        {
            return this.Text_TextFollows("...");
        }

        #endregion

        #region QUOTATION GROUP:

        private List<IToken> quotation_MeaningfulTokenList = new List<IToken>();

        private void Quotation_AddTokenIfMeaningful(IToken token)
        {
            if (token != null && token.Channel != (int)Channel.Skipped)
            {
                this.quotation_MeaningfulTokenList.Add(token);
            }
        }

        private int Quotation_FindLastOrDefaultIndex(int upToIndex, Predicate<IToken> predicate)
        {
            int max = Math.Min(upToIndex, this.quotation_MeaningfulTokenList.Count - 1);

            for (int i = max; i >= 0; i--)
            {
                if (predicate(this.quotation_MeaningfulTokenList[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        private bool Quotation_TransposeIsEnabledAfterKeyword(int keywordIndex)
        {
            int indexOfPreviousDefaultToken = this.Quotation_FindLastOrDefaultIndex(keywordIndex - 1, x => x.Channel == (int)Channel.Default);

            IToken previousDefaultToken = (indexOfPreviousDefaultToken < 0 ? null : this.quotation_MeaningfulTokenList[indexOfPreviousDefaultToken]);

            return (previousDefaultToken != null && previousDefaultToken.Type == MatlabLexer.DOT);
        }

        private bool Quotation_TransposeIsEnabledAfterClosingParenthesis(int closingParenthesisIndex)
        {
            int closing = 1;

            int index = closingParenthesisIndex - 1;

            for (; index >= 0; index--)
            {
                IToken token = this.quotation_MeaningfulTokenList[index];

                switch (token.Type)
                {
                    case MatlabLexer.LPAREN:
                    case MatlabLexer.LSQUARE:
                    case MatlabLexer.LCURLY:
                        closing--;
                        break;
                    case MatlabLexer.RPAREN:
                    case MatlabLexer.RSQUARE:
                    case MatlabLexer.RCURLY:
                        closing++;
                        break;
                }

                if (closing == 0)
                {
                    break;
                }
            }

            if (closing == 0)
            {
                int indexOfPreviousDefaultToken = this.Quotation_FindLastOrDefaultIndex(index - 1, x => x.Channel == (int)Channel.Default);

                IToken previousDefaultToken = (indexOfPreviousDefaultToken < 0 ? null : this.quotation_MeaningfulTokenList[indexOfPreviousDefaultToken]);

                return (previousDefaultToken == null || previousDefaultToken.Type != MatlabLexer.AT);
            }
            else
            {
                return true;
            }
        }

        private bool Quotation_TransposeIsEnabled()
        {
            int indexOfLastDefaultToken = this.Quotation_FindLastOrDefaultIndex(this.quotation_MeaningfulTokenList.Count - 1, x => x.Channel == (int)Channel.Default);

            if (indexOfLastDefaultToken < 0 || indexOfLastDefaultToken != this.quotation_MeaningfulTokenList.Count - 1)
            {
                return false;
            }
            else
            {
                IToken lastDefaultToken = this.quotation_MeaningfulTokenList[indexOfLastDefaultToken];

                switch (lastDefaultToken.Type)
                {
                    case MatlabLexer.RPAREN:
                        return this.Quotation_TransposeIsEnabledAfterClosingParenthesis(indexOfLastDefaultToken);
                    case MatlabLexer.RSQUARE:
                    case MatlabLexer.RCURLY:
                    case MatlabLexer.CTRANS:
                    case MatlabLexer.REAL:
                    case MatlabLexer.IMAGINARY:
                    case MatlabLexer.ID:
                        return true;
                    case MatlabLexer.BREAK:
                    case MatlabLexer.CASE:
                    case MatlabLexer.CATCH:
                    case MatlabLexer.CLASSDEF:
                    case MatlabLexer.CONTINUE:
                    case MatlabLexer.ELSE:
                    case MatlabLexer.ELSEIF:
                    case MatlabLexer.END:
                    case MatlabLexer.FOR:
                    case MatlabLexer.FUNCTION:
                    case MatlabLexer.GLOBAL:
                    case MatlabLexer.IF:
                    case MatlabLexer.OTHERWISE:
                    case MatlabLexer.PARFOR:
                    case MatlabLexer.PERSISTENT:
                    case MatlabLexer.RETURN:
                    case MatlabLexer.SPMD:
                    case MatlabLexer.SWITCH:
                    case MatlabLexer.TRY:
                    case MatlabLexer.WHILE:
                        return this.Quotation_TransposeIsEnabledAfterKeyword(indexOfLastDefaultToken);
                    default:
                        return false;
                }
            }
        }

        #endregion

        #endregion
    }
}
