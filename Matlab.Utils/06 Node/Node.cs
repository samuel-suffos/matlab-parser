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
using System.Threading.Tasks;

namespace Matlab.Utils
{
    [Serializable]
    public abstract class Node : INode, IFreezable
    {
        #region EXPLICIT MEMBERS:

        INode INode.Parent
        {
            get
            {
                return this.Parent;
            }
        }

        IEnumerable<INode> INode.Children
        {
            get
            {
                return this.Children;
            }
        }

        bool INode.IsRoot
        {
            get
            {
                return this.IsRoot;
            }
        }

        bool INode.IsLeaf
        {
            get
            {
                return this.IsLeaf;
            }
        }

        #endregion

        #region FIELDS:

        #region FREEZE FIELDS:

        private bool isFrozen;

        #endregion

        #endregion

        #region PROPERTIES:

        #region TREE PROPERTIES:

        public Node Parent { get; internal set; }

        public NodeCollection Children { get; private set; }

        public bool IsRoot
        {
            get
            {
                return (this.Parent == null);
            }
        }

        public bool IsLeaf
        {
            get
            {
                return (this.Children.IsEmpty);
            }
        }

        #endregion

        #region FREEZE PROPERTIES:

        public bool IsFrozen
        {
            get
            {
                return (!this.IsRoot ? this.Parent.IsFrozen : this.isFrozen);
            }
        }

        public bool CanFreeze
        {
            get
            {
                return (this.IsRoot && !this.IsFrozen);
            }
        }

        #endregion

        #endregion

        #region CONSTRUCTORS:

        protected Node()
        {
            this.isFrozen = false;

            this.Parent = null;

            this.Children = new NodeCollection(this);
        }

        #endregion

        #region METHODS:

        #region CHECK METHODS:

        protected void CheckCanFreeze()
        {
            if (!this.CanFreeze)
            {
                throw new InvalidOperationException();
            }
        }

        protected void CheckNotFrozen()
        {
            if (this.IsFrozen)
            {
                throw new InvalidOperationException();
            }
        }

        #endregion

        #region FREEZE METHODS:

        public void Freeze()
        {
            this.CheckCanFreeze();

            this.isFrozen = true;
        }

        #endregion

        #region VISITOR METHODS:

        public virtual object Accept(Visitor visitor)
        {
            Checker.CheckNotNull(visitor);

            return visitor.Visit(this);
        }

        #endregion

        #endregion
    }
}
