using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Metadata;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;


namespace slicing
{
    internal class Decompile
    {
        private string logDirectory = "E:\\PackageDownloader\\logDecompile";
        private string obfuscatedFile = "obfuscatedFile.txt";
        private string errorFile = "errorFile.txt";
        private string finishProcessFile = "finishProcessFile.txt";
        private string timeout = "timeout.txt";

        public Decompile()
        {
            Directory.CreateDirectory(logDirectory);
            obfuscatedFile = Path.Combine(logDirectory, obfuscatedFile);
            errorFile = Path.Combine(logDirectory, errorFile);
            finishProcessFile = Path.Combine(logDirectory, finishProcessFile);
            timeout = Path.Combine(logDirectory, timeout);
        }

        public string DecompileFile(string filePath, CancellationToken token)
        {
            Console.WriteLine("start decompile file: " + filePath);
            string sourceCode = "";
            try
            {   // Kiểm tra kích thước file
                var fileInfo = new FileInfo(filePath);
                long fileSizeInKilobytes = fileInfo.Length / 1024;
                if (fileSizeInKilobytes > 30000) 
                {
                    Console.Error.WriteLine($"File too large to decompile ({fileSizeInKilobytes} KB): {filePath}");
                    return sourceCode;
                }

                var assemblyResolver = new UniversalAssemblyResolver(filePath, false, null);

                var decompilerSettings = new DecompilerSettings
                {
                    ThrowOnAssemblyResolveErrors = false,
                };

                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var peFile = new PEFile(filePath, stream, PEStreamOptions.Default, metadataOptions: MetadataReaderOptions.None);
                    var decompiler = new CSharpDecompiler(peFile, assemblyResolver, decompilerSettings);

                    SyntaxTree syntaxTree = decompiler.DecompileWholeModuleAsSingleFile();

                    CheckCancel(token);

                    if (DetectObfuscatedVariableNames(syntaxTree, token))
                    {
                        FileWrite(obfuscatedFile, filePath);
                        Console.Error.WriteLine("obfuscation: " + filePath);
                    }
                    else
                    {
                        sourceCode = syntaxTree.ToString();
                    }
                }
                //sourceCode = syntaxTree.ToString();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error decompiling {filePath}: {e.Message}");
                FileWrite(errorFile, filePath);
            }

            return sourceCode;
        }

        public void DecompileFileAndSave(string filePath, string outputDirectory)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1)); // Set timeout for each file
            try
            {
                var source = DecompileFile(filePath, cts.Token);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                string outputFile = Path.Combine(outputDirectory, fileNameWithoutExtension + ".cs");
                SaveFileToCS(outputFile, outputDirectory, source);
            }
            catch (OperationCanceledException)
            {
                Console.Error.WriteLine($"Processing of the file {filePath} was cancelled due to timeout.");
                FileWrite(timeout, filePath);
            }
            finally { cts.Dispose(); }

        }

        public void DecompileFilesAndSave(string directoryPath, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            // Đọc file log để lấy danh sách các file đã được xử lý, nếu file log tồn tại
            var processedFiles = new HashSet<string>();
            if (File.Exists(finishProcessFile))
            {
                processedFiles = new HashSet<string>(File.ReadLines(finishProcessFile));
            }

            // Lấy danh sách tất cả files và lọc bỏ những file đã được xử lý
            var executableFiles = Directory.EnumerateFiles(directoryPath)
                .Where(file => !processedFiles.Contains(Path.GetFileNameWithoutExtension(file)))
                .ToList();

            int maxDegreeOfParallelism = 5;
            Parallel.ForEach(executableFiles, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, filePath =>
            //Parallel.ForEach(executableFiles, filePath =>
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                string outputFile = Path.Combine(outputDirectory, fileNameWithoutExtension + ".cs");

                FileWrite(finishProcessFile, fileNameWithoutExtension);

                var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1)); // Set timeout for each file
                try
                {
                    string sourceCode = DecompileFile(filePath, cts.Token);
                    SaveFileToCS(outputFile, outputDirectory, sourceCode);
                }
                catch (OperationCanceledException)
                {
                    Console.Error.WriteLine($"Processing of the file {filePath} was cancelled due to timeout.");
                    FileWrite(timeout, filePath);
                }
                finally { cts.Dispose(); }


                
            });
        }


        public void SaveFileToCS(string outputFile, string outputDirectory, string sourceCode)
        {
            if (sourceCode != "")
            {
                try
                {
                    FileWrite(outputFile, sourceCode);
                    Console.WriteLine($"Decompiled {Path.GetFileNameWithoutExtension(outputFile)} and saved");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }


        private bool DetectObfuscatedVariableNames(SyntaxTree syntaxTree, CancellationToken token)
        {
            // Kiểm tra tên biến
            var variables = syntaxTree.Descendants.OfType<VariableDeclarationStatement>();
            foreach (var variable in variables)
            {
                string? name = variable.Variables.FirstOrDefault()?.Name;
                if (LooksLikeObfuscated(name))
                {
                    return true;
                }
            }
            CheckCancel(token);
            // Kiểm tra tên phương thức
            var methods = syntaxTree.Descendants.OfType<MethodDeclaration>();
            foreach (var method in methods)
            {
                string name = method.Name;
                if (LooksLikeObfuscated(name))
                {
                    return true;
                }
            }
            CheckCancel(token);
            //Kiểm tra tên lớp
            var classes = syntaxTree.Descendants.OfType<TypeDeclaration>();
            foreach (var classType in classes)
            {
                string name = classType.Name;
                if (LooksLikeObfuscated(name))
                {
                    return true;
                }
            }
            CheckCancel(token);
            return false;
        }

        public bool LooksLikeObfuscated(string name)
        {
            bool containsSpecialCharacters = Regex.IsMatch(name, @"[^a-zA-Z0-9_<>*.]");
            if (containsSpecialCharacters)
            {
                Console.WriteLine("Looks Like Obfuscated: " + name);
                return true;
            }
            return false;
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


        private void CheckCancel(CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
        }
    }
}
