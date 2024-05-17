using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using slicing;

namespace slicingroslyn
{
    class SemanticSlicing
    {
        private List<MetadataReference> _references;
        private Dictionary<string, List<string>> _keywords;
        private string outputDirectory;
        private string assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

        private readonly Logger logger;

        private string logDirectory = "log";
        private string namespaceNotFoundFiles = "namespaceNotFoundFiles.txt";
        //private string namespaceNotFound = "namespaceNotFound.txt";
        private string errorFiles = "errorFiles.txt";
        //private string logtime = "logtime.txt";
        private string emptySliceFiles = "emptySliceFiles.txt";
        private string finishProcessFile = "finishProcessFile.txt";
        private string timeout = "timeout.txt";

        public SemanticSlicing(string fileKeyWordPath, string outputDir, string metadataReferencePath)
        {
            logger = new Logger();

            //_references = Directory.EnumerateFiles(metadataReferencePath)
            //    .Select(path => MetadataReference.CreateFromFile(path) as MetadataReference)
            //    .ToList();

            _references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                //MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Security.Cryptography.Algorithms.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Security.Cryptography.Primitives.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Text.Encoding.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "Microsoft.Win32.Registry.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Diagnostics.Process.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath,"System.IO.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Reflection.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.InteropServices.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath,"System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "Microsoft.VisualBasic.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Windows.Forms.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "PInvoke.AdvApi32.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "PInvoke.Crypt32.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "PInvoke.Gdi32.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "PInvoke.Kernel32.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "PInvoke.NTDll.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "PInvoke.Shell32.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "PInvoke.User32.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Net.Sockets.dll")),
            };

            _keywords = LoadKeywordsFromFile(fileKeyWordPath);

            Directory.CreateDirectory(outputDir);
            outputDirectory = outputDir;

            Directory.CreateDirectory(logDirectory);
            namespaceNotFoundFiles = Path.Combine(logDirectory, namespaceNotFoundFiles);
            //namespaceNotFound = Path.Combine(logDirectory, namespaceNotFound);
            errorFiles = Path.Combine(logDirectory, errorFiles);
            //logtime = Path.Combine(logDirectory, logtime);
            emptySliceFiles = Path.Combine(logDirectory, emptySliceFiles);
            finishProcessFile = Path.Combine(logDirectory, finishProcessFile);
            timeout = Path.Combine(logDirectory, timeout);
        }

        public Dictionary<string, List<string>> LoadKeywordsFromFile(string filePath)
        {
            Dictionary<string, List<string>> jsonObj = null;
            try
            {
                var fileContent = File.ReadAllText(filePath);
                jsonObj = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(fileContent);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error while processing JSON file: " + e.Message);
            }
            return jsonObj;
        }

        public void ProcessFiles(string directorypath)
        {
            // Đọc file log để lấy danh sách các file đã được xử lý, nếu file log tồn tại
            var processedFiles = new HashSet<string>();
            if (File.Exists(finishProcessFile))
            {
                processedFiles = new HashSet<string>(File.ReadLines(finishProcessFile));
            }

            // Lấy danh sách tất cả files và lọc bỏ những file đã được xử lý
            var filesToProcess = Directory.EnumerateFiles(directorypath)
                .Where(file => !processedFiles.Contains(Path.GetFileNameWithoutExtension(file)))
                .ToList();

            Parallel.ForEach(filesToProcess, new ParallelOptions { MaxDegreeOfParallelism = 5 }, file =>
            {
                ProcessFile(file);

            });
        }

        public void ProcessFile(string file)
        {
            logger.Information("Start process file: " + file);
            //FileWrite(logtime, $"Started processing {file} at {DateTime.Now}");
            string filePath = Path.GetFileNameWithoutExtension(file);
            try
            {
                string sourceCode = File.ReadAllText(file);

                var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1)); // Set timeout for each file
                try
                {
                    ProcessSourceCode(sourceCode, filePath, cts.Token);
                }
                catch (OperationCanceledException)
                {
                    Console.Error.WriteLine($"Processing of the file {file} was cancelled due to timeout.");
                    FileWrite(timeout, filePath);
                }
                finally { cts.Dispose(); }
            }
            catch (IOException) { FileWrite(errorFiles, "IOException, " + file); }
            catch (Exception e) { FileWrite(errorFiles, "Exception, " + file + "\n" + e.Message); }
            FileWrite(finishProcessFile, filePath);
            //FileWrite(logtime, $"Finished processing {file} at {DateTime.Now}");
            logger.Information("End process file: " + file);
        }

        public void ProcessSourceCode(string sourceCode, string filePath, CancellationToken token)
        {
            List<HashSet<SyntaxNode>> collectedNodes = new List<HashSet<SyntaxNode>>();

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            CSharpCompilation compilation = CSharpCompilation.Create("Analysis").AddReferences(_references).AddSyntaxTrees(syntaxTree);
            SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
            var invocations = syntaxTree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>();
            CheckCancel(token);
            ProcessMemberAccessKeyNode(syntaxTree, semanticModel, invocations, collectedNodes, filePath, token);
            ProcessKeyNode(syntaxTree, semanticModel, invocations, collectedNodes, filePath, token);
            ProcessFieldDeclarationKeyNode(syntaxTree, semanticModel, invocations, collectedNodes, filePath, token);
            ProcessLocalDeclarationKeyNode(syntaxTree, semanticModel, invocations, collectedNodes, filePath, token);

            CheckCancel(token);

            CheckRemoveNestedSubtree(collectedNodes);
            SaveSlice(syntaxTree.GetRoot(), collectedNodes, filePath, token);

        }

        public void ProcessKeyNode(SyntaxTree syntaxTree, SemanticModel semanticModel, IEnumerable<InvocationExpressionSyntax> invocations,
            List<HashSet<SyntaxNode>> collectedNodes, string filePath, CancellationToken token)
        {
            var keys = MergeListsFromDictionary(_keywords);

            foreach (var invocation in invocations)
            {
                var symbolInfo = SymbolInformation(invocation, semanticModel);
                if (symbolInfo != "")
                {
                    if (CheckInvocationKeyNode(symbolInfo) ||
                        CheckInvokeKeyNode(symbolInfo, invocation, semanticModel) ||
                        CheckStaticClassKeyNode(symbolInfo))
                    {
                        //logger.Information("Start slice the number " + sliceNumber);
                        logger.Debug($"\tFull Method Name : {symbolInfo}");
                        logger.Debug($"\tStatement: {invocation.ToString().Trim()}");

                        CollectingNode(semanticModel, invocation, syntaxTree.GetRoot(), invocations, collectedNodes, filePath, token);
                    }
                }
                else
                {
                    string invocationName = invocation.Expression.ToString();
                    foreach (var key in keys)
                    {
                        if (invocationName.Contains(key))
                        {
                            logger.Warning($"Attention namespace not found: {invocationName}");
                            FileWrite(namespaceNotFoundFiles, filePath);
                            //FileWrite(namespaceNotFound, $"Attention namespace not found: {invocationName}, {filePath}");
                        }
                    }
                }
            }
        }

        public void ProcessFieldDeclarationKeyNode(SyntaxTree syntaxTree, SemanticModel semanticModel, IEnumerable<InvocationExpressionSyntax> invocations,
            List<HashSet<SyntaxNode>> collectedNodes, string filePath, CancellationToken token)
        {
            HashSet<SyntaxNode> collectFieldNode = new HashSet<SyntaxNode>();
            var fieldDeclarations = syntaxTree.GetRoot().DescendantNodes().OfType<BaseFieldDeclarationSyntax>();

            CheckCancel(token);

            foreach (var fieldDeclaration in fieldDeclarations)
            {
                var variableDeclaration = fieldDeclaration.Declaration;

                var type = semanticModel.GetTypeInfo(variableDeclaration.Type).Type;
                if (type != null)
                {
                    foreach (var key in _keywords["class"])
                    {
                        if (key == type.ToString())
                        {
                            collectFieldNode.UnionWith(fieldDeclaration.AncestorsAndSelf());
                            collectFieldNode.UnionWith(fieldDeclaration.DescendantNodes());
                            var identifiers = fieldDeclaration.Parent.DescendantNodes().OfType<IdentifierNameSyntax>();
                            foreach (var variable in variableDeclaration.Variables)
                            {
                                var fieldSymbol = semanticModel.GetDeclaredSymbol(variable);
                                foreach (var id in identifiers)
                                {
                                    var symbol = semanticModel.GetSymbolInfo(id).Symbol;
                                    if (symbol != null && symbol.Equals(fieldSymbol))
                                    {
                                        try
                                        {
                                            CollectNode collectNode = new CollectNode(semanticModel, id, syntaxTree.GetRoot(), invocations, _keywords, token);
                                            collectNode.CollectDependencies(id);
                                            collectFieldNode.UnionWith(collectNode.collectedNode);
                                        }
                                        catch (OperationCanceledException) { }
                                        catch (Exception e)
                                        {
                                            logger.Error("Error when processing KeyNode: " + e.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var key in _keywords["class"])
                    {
                        var lastIndex = key.LastIndexOf(".");
                        var suffix = key.Substring(lastIndex + 1);
                        if (fieldDeclaration.ToString().Contains(suffix))
                        {
                            logger.Warning($"Attention namespace not found: {suffix}");
                            FileWrite(namespaceNotFoundFiles, filePath);
                            //FileWrite(namespaceNotFound, $"Attention namespace not found: {suffix}");
                        }
                    }

                }
            }
            collectedNodes.Add(collectFieldNode);
        }

        public void ProcessLocalDeclarationKeyNode(SyntaxTree syntaxTree, SemanticModel semanticModel, IEnumerable<InvocationExpressionSyntax> invocations,
            List<HashSet<SyntaxNode>> collectedNodes, string filePath, CancellationToken token)
        {
            var localDeclarations = syntaxTree.GetRoot().DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
            CheckCancel(token);
            foreach (var localDeclaration in localDeclarations)
            {
                var variableDeclaration = localDeclaration.Declaration;
                var type = semanticModel.GetTypeInfo(variableDeclaration.Type).Type;
                if (type != null)
                {
                    foreach (var key in _keywords["class"])
                    {
                        if (key == type.ToString())
                        {
                            logger.Debug($"\tFull Method Name : {type.ToString()}");
                            logger.Debug($"\tStatement: {localDeclaration.ToString().Trim()}");

                            CollectingNode(semanticModel, localDeclaration, syntaxTree.GetRoot(), invocations, collectedNodes, filePath, token);
                        }
                    }
                }
                else
                {
                    foreach (var key in _keywords["class"])
                    {
                        var lastIndex = key.LastIndexOf(".");
                        var suffix = key.Substring(lastIndex + 1);
                        if (localDeclaration.ToString().Contains(suffix))
                        {
                            logger.Warning($"Attention namespace not found: {suffix}");
                            FileWrite(namespaceNotFoundFiles, filePath);
                            //FileWrite(namespaceNotFound, $"Attention namespace not found: {suffix}");
                        }
                    }
                }
            }
        }

        public void ProcessMemberAccessKeyNode(SyntaxTree syntaxTree, SemanticModel semanticModel, IEnumerable<InvocationExpressionSyntax> invocations,
            List<HashSet<SyntaxNode>> collectedNodes, string filePath, CancellationToken token)
        {
            var members = syntaxTree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();

            CheckCancel(token);

            foreach (var mem in members)
            {
                var symbolInfo = SymbolInformation(mem, semanticModel);
                if (symbolInfo != "")
                {
                    if (CheckMemberAccessKeyNode(symbolInfo))
                    {
                        CollectingNode(semanticModel, mem, syntaxTree.GetRoot(), invocations, collectedNodes, filePath, token);
                    }
                }
                else
                {
                    string memName = mem.Expression.ToString();
                    foreach (var key in _keywords["property"])
                    {
                        if (memName.Contains(key))
                        {
                            logger.Warning($"Attention namespace not found: {memName}");
                            FileWrite(namespaceNotFoundFiles, filePath);
                            //FileWrite(namespaceNotFound, $"Attention namespace not found: {memName}");
                        }
                    }
                }
            }
        }

        private bool CheckInvocationKeyNode(string fullMethodName)
        {
            bool condition = false;
            foreach (var key in _keywords["invocation"])
            {
                if (!key.Contains("."))
                {
                    condition = fullMethodName.EndsWith(key);
                }
                else
                {
                    var lastIndex = key.LastIndexOf(".");
                    var prefix = key.Substring(0, lastIndex);
                    var suffix = key.Substring(lastIndex + 1);
                    condition = fullMethodName.Contains(prefix) && fullMethodName.EndsWith(suffix);
                }

                if (condition == true) break;
            }
            return condition;
        }

        private bool CheckMemberAccessKeyNode(string fullMethodName)
        {
            bool condition = false;
            foreach (var key in _keywords["property"])
            {
                if (fullMethodName.EndsWith(key))
                {
                    condition = true;
                    break;
                }
            }
            return condition;
        }

        private bool CheckStaticClassKeyNode(string fullMethodName)
        {
            bool condition = false;
            foreach (var key in _keywords["staticclass"])
            {
                if (fullMethodName.Contains(key))
                {
                    condition = true;
                    break;
                }
            }
            return condition;
        }

        private bool CheckInvokeKeyNode(string fullMethodName, InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            bool condition = false;
            foreach (var key in _keywords["invoke"])
            {
                if (fullMethodName.EndsWith(key))
                {
                    var symbol = semanticModel.GetSymbolInfo(invocation).Symbol;
                    if (symbol != null)
                    {
                        MethodDeclarationSyntax declaration = symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;

                        if (declaration != null && IsDllImportMethod(declaration))
                        {
                            condition = true;
                        }
                    }
                }
                if (condition == true) break;
            }
            return condition;
        }

        private void CollectingNode(SemanticModel semanticModel, SyntaxNode node, SyntaxNode root, IEnumerable<InvocationExpressionSyntax> invocations,
            List<HashSet<SyntaxNode>> collectedNodes, string filePath, CancellationToken token)
        {
            try
            {
                CollectNode collectNode = new CollectNode(semanticModel, node, root, invocations, _keywords, token);
                collectNode.CollectDependencies(node);
                collectedNodes.Add(collectNode.collectedNode);

                CheckCancel(token);
            }
            catch (OutOfMemoryException ex)
            {
                FileWrite(errorFiles, "OutOfMemoryException, " + filePath);
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                logger.Error("Error when processing KeyNode: " + e.ToString());
            }
        }

        private string SymbolInformation(SyntaxNode syntaxNode, SemanticModel semanticModel)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(syntaxNode).Symbol;
            string containingType = "";
            if (symbolInfo != null)
            {
                string x = symbolInfo.ContainingNamespace.ToString();
                string y = "";
                try
                {
                    y = symbolInfo.ContainingType.Name;
                }
                catch (Exception e)
                { //logger.Debug(e.Message); 
                }
                if (y != "") { containingType = x + "." + y + "."; }
                else { containingType = x + "."; }
                string fullMethodName = containingType + symbolInfo.Name;
                //Console.WriteLine("yyyy: "+symbolInfo.ToDisplayString());
                return fullMethodName;
            }
            return "";
        }

        private void CheckRemoveNestedSubtree(List<HashSet<SyntaxNode>> collectedNodes)
        {
            var setsToRemove = new List<HashSet<SyntaxNode>>();
            for (int i = 0; i < collectedNodes.Count; i++)
            {
                for (int j = 0; j < collectedNodes.Count; j++)
                {
                    if (i != j && collectedNodes[j].IsSupersetOf(collectedNodes[i]))
                    {
                        setsToRemove.Add(collectedNodes[i]);
                        break;
                    }
                }
            }
            foreach (var set in setsToRemove)
            {
                collectedNodes.Remove(set);
            }

        }

        private void SaveSlice(SyntaxNode root, List<HashSet<SyntaxNode>> collectedNodes, string filePath, CancellationToken token)
        {
            bool empty = true;
            foreach (var collectedNode in collectedNodes)
            {
                CheckCancel(token, filePath);

                try
                {
                    var subtreeRewriter = new SubtreeRewriter(collectedNode);
                    var subtree = subtreeRewriter.Visit(root);

                    if (subtree != null)
                    {
                        string path = Path.Combine(outputDirectory, $"{filePath}.txt");

                        bool check = SaveStringToFile(subtree.ToFullString(), path, token);
                        if (check)
                        {
                            logger.Sucess("Done and saved to: " + path);
                            empty = false;
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Error rewrite syntaxtree: " + e.ToString());
                    FileWrite(errorFiles, "mising slice, " + filePath);
                }


            }

            if (empty)
            {
                FileWrite(emptySliceFiles, filePath);
            }
        }

        public bool SaveStringToFile(string sb, string filePath, CancellationToken token)
        {
            if (sb == "")
            {
                return false;
            }
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        CheckCancel(token, filePath);

                        streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                        streamWriter.WriteLine(sb);
                        streamWriter.Flush();
                    }
                }
                return true;
            }
            catch (IOException e)
            {
                logger.Error("An error occurred while writing to the file: " + e.Message);
                return false;
            }
        }

        private bool IsDllImportMethod(MethodDeclarationSyntax methodDeclaration)
        {
            var dllImportAttribute = methodDeclaration.AttributeLists
                .SelectMany(a => a.Attributes)
                .FirstOrDefault(a => a.Name.ToString() == "DllImport" || a.Name.ToString().EndsWith(".DllImport"));
            return dllImportAttribute != null;
        }

        public List<string> MergeListsFromDictionary(Dictionary<string, List<string>> dictionary)
        {
            var mergedList = new List<string>();

            if (dictionary.ContainsKey("invocation"))
            {
                mergedList.AddRange(dictionary["invocation"]);
            }

            if (dictionary.ContainsKey("staticclass"))
            {
                mergedList.AddRange(dictionary["staticclass"]);
            }

            if (dictionary.ContainsKey("invoke"))
            {
                mergedList.AddRange(dictionary["invoke"]);
            }

            return mergedList;
        }


        private void FileWrite(string filePath, string text)
        {
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                        streamWriter.WriteLine(text);
                        streamWriter.Flush();
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("An error occurred while writing to the file: " + e.Message);
            }
        }

        private void CheckCancel(CancellationToken token, string filePath = null)
        {
            if (token.IsCancellationRequested)
            {
                if (File.Exists(filePath)) { File.Delete(filePath); }
                token.ThrowIfCancellationRequested();
            }
        }
    }
}

