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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matlab.Recognizer
{
    public static class MRecognizer
    {
        #region STATIC METHODS:

        #region MAIN METHODS:

        public static Result<UnitNode> RecognizeText(string text, bool buildTree)
        {
            Checker.CheckNotNull(text);

            Result<FileNode> result = MRecognizer.Recognize(string.Empty, text, buildTree);

            if (buildTree && result.IsOk)
            {
                UnitNode node = MRecognizer.CreateUnitNode(new[] { result.Value });

                return new Result<UnitNode>(node, result.Report);
            }
            else
            {
                return new Result<UnitNode>(null, result.Report);
            }
        }

        public static Result<UnitNode> RecognizeFile(string path, bool buildTree, Action<string, IReport> notifier = null)
        {
            Checker.CheckNotNull(path);

            return MRecognizer.RecognizeFiles(new[] { path }, buildTree, notifier);
        }

        public static Result<UnitNode> RecognizeFiles(IEnumerable<string> paths, bool buildTree, Action<string, IReport> notifier = null)
        {
            Checker.CheckNotNull(paths);

            Report report = new Report();

            LinkedList<FileNode> nodes = new LinkedList<FileNode>();

            foreach (string path in paths)
            {
                string text = File.ReadAllText(path);

                Result<FileNode> result = MRecognizer.Recognize(path, text, (buildTree && report.IsOk));

                if (notifier != null)
                {
                    notifier(path, result.Report);
                }

                report.AddRange(result.Report);

                if (buildTree && report.IsOk)
                {
                    nodes.AddLast(result.Value);
                }
                else
                {
                    nodes = null;
                }
            }

            if (buildTree && report.IsOk)
            {
                UnitNode node = MRecognizer.CreateUnitNode(nodes);

                return new Result<UnitNode>(node, new ReadOnlyReport(report));
            }
            else
            {
                return new Result<UnitNode>(null, new ReadOnlyReport(report));
            }
        }

        #endregion

        #region WORKER METHODS:

        private static Result<FileNode> Recognize(string path, string text, bool buildTree)
        {
            Report report = default(Report);

            CommandMarker commandMarker = new CommandMarker();

            bool stopOnFirstError = true;

            bool newCommand = default(bool);

            ITree tree = default(ITree);

            text = MRecognizer.FixText(text);

            do
            {
                try
                {
                    report = new Report();

                    newCommand = false;

                    ANTLRStringStream characters = new ANTLRStringStream(text);

                    MatlabLexer lexer = new MatlabLexer(characters, new Configuration(path, report, commandMarker, stopOnFirstError));

                    ExtendedTokenStream tokens = new ExtendedTokenStream(lexer, (int)Channel.Default);

                    MatlabParser parser = new MatlabParser(tokens, new Configuration(path, report, commandMarker, stopOnFirstError));

                    var scope = parser.file();

                    tree = (ITree)scope.Tree;
                }
                catch (CommandException)
                {
                    newCommand = true;
                }
                catch (StopException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    report.AddError(path, 0, 0, ex.Message ?? string.Empty);
                }
            }
            while (report.IsOk && newCommand);

            if (report.IsOk && buildTree)
            {
                FileNode buildNode = TreeToNodeBuilder.Build(path, tree);

                return new Result<FileNode>(buildNode, new ReadOnlyReport(report));
            }
            else
            {
                return new Result<FileNode>(null, new ReadOnlyReport(report));
            }
        }

        private static string FixText(string text)
        {
            char eof = Convert.ToChar(0x1a);

            int index = 0;

            for (; index < text.Length; index++)
            {
                if (text[index] == eof)
                {
                    break;
                }
            }

            if (index != text.Length)
            {
                text = text.Substring(0, text.Length - index);
            }

            string append = Environment.NewLine + Environment.NewLine;

            return (text + append);
        }

        private static UnitNode CreateUnitNode(IEnumerable<FileNode> nodes)
        {
            UnitNode node = new UnitNode();

            node.Children.AddRange(nodes);

            node.Freeze();

            return node;
        }

        #endregion

        #endregion
    }
}
