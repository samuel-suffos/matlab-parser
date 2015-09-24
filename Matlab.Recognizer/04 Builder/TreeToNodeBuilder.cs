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
using Antlr.Runtime.Tree;
using Matlab.Nodes;
using Matlab.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matlab.Recognizer
{
    internal static class TreeToNodeBuilder
    {
        #region STATIC METHODS:

        #region MAIN METHODS:

        public static FileNode Build(string path, ITree tree)
        {
            Checker.CheckNotNull(path);

            Checker.CheckNotNull(tree);

            FileNode node = (FileNode)TreeToNodeBuilder.BuildNode(tree);

            node.Path = path;

            return node;
        }

        #endregion

        #region BUILDER METHODS:

        private static MNode BuildNode(ITree tree)
        {
            MNode node = TreeToNodeBuilder.BuildNode(tree.Type);

            if (node is InternalNode)
            {
                InternalNode internalNode = (InternalNode)node;

                internalNode.Line = tree.Line;

                internalNode.Column = tree.CharPositionInLine + 1;

                internalNode.Text = tree.Text;
            }

            for (int i = 0; i < tree.ChildCount; i++)
            {
                MNode childNode = TreeToNodeBuilder.BuildNode(tree.GetChild(i));

                node.Children.Add(childNode);
            }

            return node;
        }

        private static MNode BuildNode(int type)
        {
            switch (type)
            {
                case MatlabParser.CLASSFILE:
                    return new ClassFileNode();
                case MatlabParser.FUNCTIONFILE:
                    return new FunctionFileNode();
                case MatlabParser.SCRIPTFILE:
                    return new ScriptFileNode();
                case MatlabParser.FUNCTION:
                    return new FunctionNode();
                case MatlabParser.CLASSDEF:
                    return new ClassNode();
                case MatlabParser.EVENTSECTION:
                    return new EventSectionNode();
                case MatlabParser.PROPERTYSECTION:
                    return new PropertySectionNode();
                case MatlabParser.METHODSECTION:
                    return new MethodSectionNode();
                case MatlabParser.ENUMERATIONSECTION:
                    return new EnumerationSectionNode();
                case MatlabParser.EVENT:
                    return new EventNode();
                case MatlabParser.PROPERTY:
                    return new PropertyNode();
                case MatlabParser.REGULARMETHOD:
                    return new RegularMethodNode();
                case MatlabParser.EXTERNALMETHOD:
                    return new ExternalMethodNode();
                case MatlabParser.ENUMERATION:
                    return new EnumerationNode();
                case MatlabParser.ATTRIBUTE:
                    return new AttributeNode();
                case MatlabParser.ACTION:
                    return new ActionNode();
                case MatlabParser.ASSIGN:
                    return new AssignmentNode();
                case MatlabParser.EXCLAMATION:
                    return new BangNode();
                case MatlabParser.BREAK:
                    return new BreakNode();
                case MatlabParser.CONTINUE:
                    return new ContinueNode();
                case MatlabParser.FOR:
                    return new ForNode();
                case MatlabParser.GLOBAL:
                    return new GlobalNode();
                case MatlabParser.IFELSE:
                    return new IfNode();
                case MatlabParser.NESTEDFUNCTION:
                    return new NestedFunctionNode();
                case MatlabParser.PARFOR:
                    return new ParforNode();
                case MatlabParser.PERSISTENT:
                    return new PersistentNode();
                case MatlabParser.RETURN:
                    return new ReturnNode();
                case MatlabParser.SPMD:
                    return new SpmdNode();
                case MatlabParser.SWITCHCASE:
                    return new SwitchNode();
                case MatlabParser.TRYCATCH:
                    return new TryNode();
                case MatlabParser.WHILE:
                    return new WhileNode();
                case MatlabParser.IF:
                    return new IfPartNode();
                case MatlabParser.ELSEIF:
                    return new ElseIfPartNode();
                case MatlabParser.ELSE:
                    return new ElsePartNode();
                case MatlabParser.SWITCH:
                    return new SwitchPartNode();
                case MatlabParser.CASE:
                    return new CasePartNode();
                case MatlabParser.OTHERWISE:
                    return new OtherwisePartNode();
                case MatlabParser.TRY:
                    return new TryPartNode();
                case MatlabParser.CATCH:
                    return new CatchPartNode();
                case MatlabParser.COLON:
                    return new ColonNode();
                case MatlabParser.HCAT:
                    return new HCatNode();
                case MatlabParser.VCAT:
                    return new VCatNode();
                case MatlabParser.PLUS:
                    return new PlusNode();
                case MatlabParser.MINUS:
                    return new MinusNode();
                case MatlabParser.TIMES:
                    return new TimesNode();
                case MatlabParser.MTIMES:
                    return new MTimesNode();
                case MatlabParser.LDIV:
                    return new LDivNode();
                case MatlabParser.MLDIV:
                    return new MLDivNode();
                case MatlabParser.RDIV:
                    return new RDivNode();
                case MatlabParser.MRDIV:
                    return new MRDivNode();
                case MatlabParser.POW:
                    return new PowNode();
                case MatlabParser.MPOW:
                    return new MPowNode();
                case MatlabParser.TRANS:
                    return new TransNode();
                case MatlabParser.CTRANS:
                    return new CTransNode();
                case MatlabParser.EQ:
                    return new EqNode();
                case MatlabParser.NOTEQ:
                    return new NotEqNode();
                case MatlabParser.LT:
                    return new LtNode();
                case MatlabParser.LTEQ:
                    return new LtEqNode();
                case MatlabParser.GT:
                    return new GtNode();
                case MatlabParser.GTEQ:
                    return new GtEqNode();
                case MatlabParser.AND:
                    return new AndNode();
                case MatlabParser.SHORTAND:
                    return new ShortAndNode();
                case MatlabParser.OR:
                    return new OrNode();
                case MatlabParser.SHORTOR:
                    return new ShortOrNode();
                case MatlabParser.POSITIVE:
                    return new PositiveNode();
                case MatlabParser.NEGATIVE:
                    return new NegativeNode();
                case MatlabParser.NOT:
                    return new NotNode();
                case MatlabParser.ALL:
                    return new AllNode();
                case MatlabParser.END:
                    return new EndNode();
                case MatlabParser.IMAGINARY:
                    return new ImaginaryNode();
                case MatlabParser.REAL:
                    return new RealNode();
                case MatlabParser.STRING:
                    return new StringNode();
                case MatlabParser.CELLARRAY:
                    return new CellArrayNode();
                case MatlabParser.REGULARARRAY:
                    return new RegularArrayNode();
                case MatlabParser.VAR:
                    return new VarNode();
                case MatlabParser.DOTEXPRESSION:
                    return new DotExpressionNode();
                case MatlabParser.DOTNAME:
                    return new DotNameNode();
                case MatlabParser.PARENTHESIS:
                    return new ParenthesisNode();
                case MatlabParser.CURLYBRACE:
                    return new CurlyBraceNode();
                case MatlabParser.ATBASE:
                    return new AtBaseNode();
                case MatlabParser.ANONYMOUSFUNCTION:
                    return new AnonymousFunctionNode();
                case MatlabParser.FUNCTIONHANDLE:
                    return new FunctionHandleNode();
                case MatlabParser.QUESTION:
                    return new MetaclassNode();
                case MatlabParser.STORAGE:
                    return new StorageNode();
                case MatlabParser.INPUT:
                    return new InputNode();
                case MatlabParser.OUTPUT:
                    return new OutputNode();
                case MatlabParser.PRINT:
                    return new PrintNode();
                case MatlabParser.NOPRINT:
                    return new NoPrintNode();
                case MatlabParser.CLASSREF:
                    return new ClassRefNode();
                case MatlabParser.FUNCTIONREF:
                    return new FunctionRefNode();
                case MatlabParser.ID:
                    return new IdNode();
                case MatlabParser.NAME:
                    return new NameNode();
                default:
                    throw new InternalException();
            }
        }

        #endregion

        #endregion
    }
}
