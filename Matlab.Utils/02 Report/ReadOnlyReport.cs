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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matlab.Utils
{
    [Serializable]
    public sealed class ReadOnlyReport : ReadOnlySequence<Message>, IReport
    {
        #region STATIC FIELDS:

        public static readonly IReport Empty;

        #endregion

        #region STATIC CONSTRUCTORS:

        static ReadOnlyReport()
        {
            ReadOnlyReport.Empty = new ReadOnlyReport(new Report());
        }

        #endregion

        #region PROPERTIES:

        public Severity Severity
        {
            get
            {
                return ((IReport)this.InnerCollection).Severity;
            }
        }

        public bool IsOk
        {
            get
            {
                return ((IReport)this.InnerCollection).IsOk;
            }
        }

        #endregion

        #region CONSTRUCTORS:

        public ReadOnlyReport(IReport innerCollection)
            : base(innerCollection)
        {
            return;
        }

        #endregion

        #region METHODS:

        public void Insert(int index, Severity severity, string path, int line, int column, string text)
        {
            this.CheckNotReadOnly();
        }

        public void Add(Severity severity, string path, int line, int column, string text)
        {
            this.CheckNotReadOnly();
        }

        public void AddInfo(string path, int line, int column, string text)
        {
            this.CheckNotReadOnly();
        }

        public void AddWarning(string path, int line, int column, string text)
        {
            this.CheckNotReadOnly();
        }

        public void AddError(string path, int line, int column, string text)
        {
            this.CheckNotReadOnly();
        }

        #endregion
    }
}
