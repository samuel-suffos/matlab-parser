# `Matlab.Parser` 

## Introduction: 

This project is a parser for Matlab 8.1 (R2013a). The parser processes M-files written in Matlab and builds ASTs from their content. The ASTs are serialized to XML for later inspection. 

## Development: 

The project has been written in the C# programming language version 5.0, and targets Microsoft's .NET Framework version 4.5. The project is also partly written in ANTLR. The ANTLR code was written for the C# port of the ANTLR Tool, version 3.5.0.2. 

## Contents: 

The contents of this repository is a Visual Studio 2013 solution, made of five C# projects: 
* `Matlab.Info` (class library) - Contains information about the project. 
* `Matlab.Utils` (class library) - Contains utility classes for the project. 
* `Matlab.Nodes` (class library) - Contains the nodes that ASTs are made of. 
* `Matlab.Recognizer` (class library) - The reusable parsing API. 
* `Matlab.Parser` (console application) - The parsing tool, which uses the parsing API. 

## How to build the project: 

Just open the solution in Visual Studio and compile it! Once compiled, the application will be located in `Matlab.Parser\bin\Release`. 

The application is made of the following files: 
* `Antlr3.Runtime.dll`
* `Matlab.Info.dll`
* `Matlab.Utils.dll` 
* `Matlab.Nodes.dll`
* `Matlab.Recognizer.dll` 
* `Matlab.Parser.exe` 
* `Matlab.Parser.exe.config` 

## How to use the project: 

* Copy the above mentioned files into the directory of your choice, and open a command prompt in that directory. Next, type `Matlab.Parser` and press `Enter`. Information about the program version and a short copyright notice should be printed on the screen. 

* To display help, type `Matlab.Parser /help`. 

* To parse `<file1>` ... `<fileN>` and generate `<file>`, type `Matlab.Parser /out: <file> /files: <file1> ... <fileN>`. Let `<file>` = `?` to generate no output file. Let `<file>` = `*` to generate an output file for each processed file. 

* To parse files whose names match `<pattern>` in current directory (and subdirectories) and generate `<file>`, type `Matlab.Parser /out: <file> /pattern: <pattern>`. Let `<file>` = `?` to generate no output file. Let `<file>` = `*` to generate an output file for each processed file.  

## Examples: 

* To process files `file1.m` and `file2.m` and generate file `files.xml` containing the XML serialization of a combined AST built from their content, type `Matlab.Parser /out: files.xml /files: file1.m file2.m`. 

* To analyze file `file1.m` and file `"D:\file2.m"` for syntactic correctness but without generating any output file (that is, just to be notified of any syntactic problems with the files' contents), type `Matlab.Parser /out: ? /files: file1.m "D:\file2.m"`. 

* To process files `file1.m` and `file2.m` and generate new files (that is, files `file1.m.xml` and `file2.m.xml`) with the XML serialization of each input file's AST, type `Matlab.Parser /out: * /files: file1.m file2.m`. 

* To process all files whose names match pattern `*.m` in the current working directory (and its subdirectories) and generate file `files.xml` containing the XML serialization of a combined AST built from their content, type `Matlab.Parser /out: files.xml /pattern: *.m`. 

* To analyze all files whose names match pattern `*.m` in the current working directory (and its subdirectories) for syntactic correctness but without generating any output file (that is, just to be notified of any syntactic problems with the files' contents), type `Matlab.Parser /out: ? /pattern: *.m`. 

* To process all files whose names match pattern `*.m` in the current working directory (and its subdirectories) and generate new files with the XML serialization of each input file's AST, type `Matlab.Parser /out: * /pattern: *.m`. 

## More about the project: 

To the best of my current knowledge, this program deals correctly with Matlab's tricky command syntax. Also, to the best of my current knowledge, this program correctly identifies the complex conjugate transpose operator (`'`). Properly handling Matlab's command syntax and properly identifying the complex conjugate transpose operator is where Matlab parsers have their most significant problems. Again, to the best of my knowledge, this parser deals appropriately with white-space separators in the contexts where they are significant. It should be pointed out, however, that Matlab's documentaion doesn't go into too much detail when talking about the syntax of the above mentioned elements. So, let me know if you find any bugs with my parser! In Matlab, you may want to use the `pcode` function as a means to test a file for syntactic correctness. 

I have tested this project on a collection of close to 75,000 valid M-files that are distributed as part of the Matlab suite, version R2013a. (Note that not all files with `.m` extension distributed with Matlab are intended to be valid M-files. Some of them, for instance, are code templates, and contain invalid characters surrounding identifiers.) The parser is able to parse 100% of those valid M-files, without complaining. Evidence such as this suggests that, at least on valid input, `Matlab.Parser` is quite robust. (And believe me, some of those valid M-files use quite tricky Matlab syntax!) 

You may find it interesting to know that, during testing, I discovered that a small set of the M-files distributed with Matlab R2013a that are clearly intended to be valid M-files contain syntax errors. Even Matlab's own `pcode` function rejects them! 

## License: 

[The BSD 3-Clause License]
Copyright (c) 2015, Samuel Suffos
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
