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
    public class Sequence<T> : ISequence<T>
    {
        #region EXPLICIT MEMBERS:

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.InnerList.GetEnumerator();
        }

        #endregion

        #region PROPERTIES:

        private List<T> InnerList { get; set; }

        public virtual bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public int Count
        {
            get
            {
                return this.InnerList.Count;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (this.InnerList.Count == 0);
            }
        }

        public int Capacity
        {
            get
            {
                return this.InnerList.Capacity;
            }
        }

        #endregion

        #region CONSTRUCTORS:

        public Sequence(int initialCapacity)
        {
            Checker.CheckNotNegative(initialCapacity);

            this.InnerList = new List<T>(initialCapacity);
        }

        public Sequence(IEnumerable<T> collection)
            : this(0)
        {
            this.AddRange(collection);
        }

        public Sequence()
            : this(0)
        {
            return;
        }

        #endregion

        #region INDEXERS:

        public T this[int index]
        {
            get
            {
                this.CheckInRangeForQuery(index);

                return this.InnerList[index];
            }
            set
            {
                this.CheckNotReadOnly();

                this.CheckInRangeForQuery(index);

                Checker.CheckNotNull(value);

                this.RemoveAt(index);

                this.Insert(index, value);
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

        protected void CheckInRangeForQuery(int index)
        {
            Checker.CheckInRange(index, 0, this.Count - 1);
        }

        protected void CheckInRangeForInsert(int index)
        {
            Checker.CheckInRange(index, 0, this.Count);
        }

        #endregion

        #region EXTRA METHODS:

        protected virtual void OnItemInserting(T item)
        {
            return;
        }

        protected virtual void OnItemInserted(T item)
        {
            return;
        }

        protected virtual void OnItemRemoving(T item)
        {
            return;
        }

        protected virtual void OnItemRemoved(T item)
        {
            return;
        }

        #endregion

        #region QUERY METHODS:

        public int IndexOf(T item)
        {
            return this.InnerList.IndexOf(item);
        }

        public bool Contains(T item)
        {
            return this.InnerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.InnerList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.InnerList.GetEnumerator();
        }

        #endregion

        #region ALTER METHODS:

        public void TrimExcess()
        {
            this.InnerList.TrimExcess();
        }

        public void Insert(int index, T item)
        {
            this.CheckNotReadOnly();

            this.CheckInRangeForInsert(index);

            Checker.CheckNotNull(item);

            this.OnItemInserting(item);

            this.InnerList.Insert(index, item);

            this.OnItemInserted(item);
        }

        public void Add(T item)
        {
            this.CheckNotReadOnly();

            this.Insert(this.Count, item);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            this.CheckNotReadOnly();

            Checker.CheckNotNull(collection);

            collection.ToArray().ForEach(x => this.Add(x));
        }

        public void RemoveAt(int index)
        {
            this.CheckNotReadOnly();

            this.CheckInRangeForQuery(index);

            T item = this.InnerList[index];

            this.OnItemRemoving(item);

            this.InnerList.RemoveAt(index);

            this.OnItemRemoved(item);
        }

        public bool Remove(T item)
        {
            this.CheckNotReadOnly();

            int index = this.IndexOf(item);

            if (index >= 0)
            {
                this.RemoveAt(index);

                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveRange(IEnumerable<T> collection)
        {
            this.CheckNotReadOnly();

            Checker.CheckNotNull(collection);

            collection.ToArray().ForEach(x => this.Remove(x));
        }

        public void Clear()
        {
            this.CheckNotReadOnly();

            while (!this.IsEmpty)
            {
                this.RemoveAt(this.Count - 1);
            }
        }

        #endregion

        #endregion
    }
}
