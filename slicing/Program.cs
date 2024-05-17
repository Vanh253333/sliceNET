using slicingroslyn;

namespace slicing
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string metadataReferencePath = "C:/File_VA/c#/metadataReference";
            string fileKeywords = "C:\\File_VA\\c#\\keyfunction.json";
            string outputDirDecompile = "E:\\NETCleanSources";
            string inputDirDecompile = "E:\\cleans";

            string outputDirSlice = "E:/test";

            //var slice = new SemanticSlicing(fileKeywords, outputDirSlice, metadataReferencePath);

            ////string filename = "C:\\File_VA\\c#\\test2.cs";
            //string filename = "D:\\NETVirusSource/1a3efc0ff176cc7ba847af0fcd50cd428e2b199bd4d7a88fbc05b4ade610c428.cs";
            //slice.ProcessFile(filename);

            //slice.ProcessFiles(outputDirDecompile);

            var decomile = new Decompile();
            //decomile.DecompileFilesAndSave(inputDirDecompile, outputDirDecompile);
            //string filename = "D:\\error\\8358a894e4e496519c479dbf16d5929b2c09340122e78eda5ab756dec8d41b21";
            string filename = "E:\\cleans\\1626850d607084222653767c3139384d";
            decomile.DecompileFileAndSave(filename, outputDirSlice);
        }
    }
}
