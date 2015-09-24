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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matlab.Recognizer
{
    internal class ExtendedTokenStream : CommonTokenStream
    {
        #region CONSTRUCTORS:

        public ExtendedTokenStream()
            : base()
        {
            return;
        }

        public ExtendedTokenStream(ITokenSource tokenSource)
            : base(tokenSource, 0)
        {
            return;
        }

        public ExtendedTokenStream(ITokenSource tokenSource, int channel)
            : base(tokenSource)
        {
            return;
        }

        #endregion

        #region METHODS:

        public IList<IToken> GetOffChannelTokensToLeft(int tokenIndex)
        {
            int stop = tokenIndex - 1;

            if (stop < 0 || this.Get(stop).Channel == this.Channel)
            {
                return null;
            }
            else
            {
                int start = this.SkipOffTokenChannelsReverse(stop);

                return this.GetTokens(start, stop);
            }
        }

        public IList<IToken> GetOffChannelTokensToLeft(int tokenIndex, int channel)
        {
            IList<IToken> list = this.GetOffChannelTokensToLeft(tokenIndex);

            return (list == null ? list : list.Where(x => x.Channel == channel).ToList());
        }

        public IList<IToken> GetOffChannelTokensToRight(int tokenIndex)
        {
            int start = tokenIndex + 1;

            if (start >= this.Count || this.Get(start).Channel == this.Channel)
            {
                return null;
            }
            else
            {
                int stop = this.SkipOffTokenChannels(start);

                return this.GetTokens(start, stop);
            }
        }

        public IList<IToken> GetOffChannelTokensToRight(int tokenIndex, int channel)
        {
            IList<IToken> list = this.GetOffChannelTokensToRight(tokenIndex);

            return (list == null ? list : list.Where(x => x.Channel == channel).ToList());
        }

        #endregion
    }
}
