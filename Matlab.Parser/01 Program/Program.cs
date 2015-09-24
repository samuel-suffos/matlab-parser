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

using Matlab.Info;
using Matlab.Nodes;
using Matlab.Recognizer;
using Matlab.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Matlab.Parser
{
    public static class Program
    {
        #region STATIC METHODS:

        #region MAIN METHOD:

        public static int Main(string[] args)
        {
            try
            {
                Console.WriteLine();

                Program.PrintHeader();

                if (args.Length == 0)
                {
                    return 0;
                }
                else if (args.Length == 1 && args[0] == "/help")
                {
                    Program.PrintHelp();

                    return 0;
                }
                else if (args.Length >= 4 && args[0] == "/out:" && args[2] == "/files:")
                {
                    Output output = Program.GetOutput(args[1]);

                    return Program.ParseFilesByName(output, (output == Output.SingleFile ? args[1] : null), args.Skip(3));
                }
                else if (args.Length == 4 && args[0] == "/out:" && args[2] == "/pattern:")
                {
                    Output output = Program.GetOutput(args[1]);

                    return Program.ParseFilesByPattern(output, (output == Output.SingleFile ? args[1] : null), args[3], true);
                }
                else
                {
                    Program.PrintArgumentErrorMessage();

                    return 1;
                }
            }
            catch (Exception ex)
            {
                Program.PrintExecutionErrorMessage(ex.Message);

                return 1;
            }
        }

        #endregion

        #region OUTPUT METHODS:

        private static Output GetOutput(string text)
        {
            switch (text)
            {
                case "?":
                    return Output.NoFile;
                case "*":
                    return Output.MultiFile;
                default:
                    return Output.SingleFile;
            }
        }

        #endregion

        #region PRINT METHODS:

        private static void PrintHeader()
        {
            Console.WriteLine("{0} (version {1})", Information.MatlabParser, Information.Version);

            Console.WriteLine(Information.Copyright);

            Console.WriteLine(Information.AllRightsReserved);
        }

        private static void PrintHelp()
        {
            Console.WriteLine();

            Console.WriteLine("- To display help, type:");

            Console.WriteLine("  {0} /help", Information.MatlabParser);

            Console.WriteLine();

            Console.WriteLine("- To parse <file1> ... <fileN> and generate <file>, type:");

            Console.WriteLine("  {0} /out: <file> /files: <file1> ... <fileN>", Information.MatlabParser);

            Console.WriteLine("  Let <file> = ? to generate no output file.");

            Console.WriteLine("  Let <file> = * to generate an output file for each processed file.");

            Console.WriteLine();

            Console.WriteLine("- To parse files whose names match <pattern> in current directory (and subdirectories) and generate <file>, type:");

            Console.WriteLine("  {0} /out: <file> /pattern: <pattern>", Information.MatlabParser);

            Console.WriteLine("  Let <file> = ? to generate no output file.");

            Console.WriteLine("  Let <file> = * to generate an output file for each processed file.");
        }

        private static void PrintArgumentErrorMessage()
        {
            Console.WriteLine();

            Console.WriteLine("Argument error.");

            Console.WriteLine();

            Console.WriteLine("- To display help, type:");

            Console.WriteLine("  {0} /help", Information.MatlabParser);
        }

        private static void PrintExecutionErrorMessage(string message)
        {
            Console.WriteLine();

            Console.WriteLine("Execution error.");

            Console.WriteLine();

            Console.WriteLine(message);
        }

        #endregion

        #region FIND METHODS:

        private static IEnumerable<string> FindPathsByPattern(string pattern, bool recurse)
        {
            LinkedList<string> paths = new LinkedList<string>();

            SearchOption searchOption = (recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            foreach (string path in Directory.GetFiles(Environment.CurrentDirectory, pattern, searchOption))
            {
                paths.AddLast(path);
            }

            return paths;
        }

        #endregion

        #region PARSE METHODS:

        private static int ParseFilesByName(Output output, string outputName, IEnumerable<string> inputNames)
        {
            HashSet<string> inputNameSet = new HashSet<string>();

            LinkedList<string> inputPaths = new LinkedList<string>();

            foreach (string inputName in inputNames)
            {
                if (inputNameSet.Add(inputName))
                {
                    inputPaths.AddLast(Path.GetFullPath(inputName));
                }
            }

            string outputPath = (outputName == null ? null : Path.GetFullPath(outputName));

            return Program.ParseFiles(output, outputPath, inputPaths);
        }

        private static int ParseFilesByPattern(Output output, string outputName, string pattern, bool recurse)
        {
            IEnumerable<string> inputPaths = Program.FindPathsByPattern(pattern, recurse);

            string outputPaths = (outputName == null ? null : Path.GetFullPath(outputName));

            return Program.ParseFiles(output, outputPaths, inputPaths);
        }

        #endregion

        #region WORKER METHODS:

        private static int ParseFiles(Output output, string outputPath, IEnumerable<string> inputPaths)
        {
            Console.WriteLine();

            Console.WriteLine("Parsing...");

            int exitCode;

            if (output == Output.SingleFile || output == Output.NoFile)
            {
                exitCode = Program.ParseFilesToSingleOrNoFile(outputPath, inputPaths);
            }
            else
            {
                exitCode = Program.ParseFilesToMultipleFiles(inputPaths);
            }

            if (exitCode == 0)
            {
                Console.WriteLine();

                Console.WriteLine("Parsing completed successfully.");
            }
            else
            {
                Console.WriteLine();

                Console.WriteLine("Errors occurred during parsing.");
            }

            return exitCode;
        }

        private static int ParseFilesToSingleOrNoFile(string outputPath, IEnumerable<string> inputPaths)
        {
            bool buildTree = (outputPath != null);

            Result<UnitNode> result = MRecognizer.RecognizeFiles(inputPaths, buildTree, Program.NotifyProgress);

            if (result.Report.IsOk)
            {
                if (buildTree)
                {
                    XDocument document = NodeToXmlBuilder.Build(result.Value);

                    document.Save(outputPath);
                }

                return 0;
            }
            else
            {
                return 1;
            }
        }

        private static int ParseFilesToMultipleFiles(IEnumerable<string> inputPaths)
        {
            bool buildTree = true;

            bool ok = true;

            foreach (string inputPath in inputPaths)
            {
                Result<UnitNode> result = MRecognizer.RecognizeFile(inputPath, buildTree, Program.NotifyProgress);

                if (result.Report.IsOk)
                {
                    if (buildTree)
                    {
                        XDocument document = NodeToXmlBuilder.Build(result.Value);

                        document.Save(inputPath + ".xml");
                    }
                }
                else
                {
                    ok = false;
                }
            }

            return (ok ? 0 : 1);
        }

        private static void NotifyProgress(string path, IReport report)
        {
            Console.WriteLine();

            Console.WriteLine("File: {0}", path);

            foreach (Message message in report)
            {
                Console.WriteLine("[{0}] Line: [{1}] Column: [{2}] Text: [{3}]", message.Severity, message.Line, message.Column, message.Text);
            }
        }

        #endregion

        #endregion
    }
}
