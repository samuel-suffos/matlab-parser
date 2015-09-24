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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matlab.Utils
{
    [Serializable]
    public class ReadOnlySequence<T> : ISequence<T>
    {
        #region EXPLICIT MEMBERS:

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.InnerCollection.GetEnumerator();
        }

        #endregion

        #region PROPERTIES:

        protected ISequence<T> InnerCollection { get; private set; }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public int Count
        {
            get
            {
                return this.InnerCollection.Count;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (this.InnerCollection.Count == 0);
            }
        }

        #endregion

        #region CONSTRUCTORS:

        public ReadOnlySequence(ISequence<T> innerCollection)
        {
            Checker.CheckNotNull(innerCollection);

            this.InnerCollection = innerCollection;
        }

        #endregion

        #region INDEXERS:

        public T this[int index]
        {
            get
            {
                return this.InnerCollection[index];
            }
            set
            {
                this.CheckNotReadOnly();
            }
        }

        #endregion

        #region METHODS:

        #region CHECK METHODS:

        protected void CheckNotReadOnly()
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException();
            }
        }

        #endregion

        #region QUERY METHODS:

        public int IndexOf(T item)
        {
            return this.InnerCollection.IndexOf(item);
        }

        public bool Contains(T item)
        {
            return this.InnerCollection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.InnerCollection.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.InnerCollection.GetEnumerator();
        }

        #endregion

        #region ALTER METHODS:

        public void Insert(int index, T item)
        {
            this.CheckNotReadOnly();
        }

        public void Add(T item)
        {
            this.CheckNotReadOnly();
        }

        public void AddRange(IEnumerable<T> collection)
        {
            this.CheckNotReadOnly();
        }

        public void RemoveAt(int index)
        {
            this.CheckNotReadOnly();
        }

        public bool Remove(T item)
        {
            this.CheckNotReadOnly();

            return false;
        }

        public void RemoveRange(IEnumerable<T> collection)
        {
            this.CheckNotReadOnly();
        }

        public void Clear()
        {
            this.CheckNotReadOnly();
        }

        #endregion

        #endregion
    }
}
