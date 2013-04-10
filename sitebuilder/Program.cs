using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sitebuilder
{
    class Program
    {
        static void DisplayHelp()
        {
            Console.WriteLine("\nDoc :\n");
            Console.WriteLine("Sitebuilder concats utf-8 or utf-16 or utf-32 (little endian) text files");
            Console.WriteLine("without interpoling the BOM.");
            Console.WriteLine("After concatenation, the lines in the result file with <!--#delete--> comment");
            Console.WriteLine("are suppressed and if \"#placeholder=newvalue\" parameters are provided, all");
            Console.WriteLine("occurrences of #placeholder in the result file are replaced by newvalue.");
            Console.WriteLine("/utf8 is the default option value.");
            Console.WriteLine("\nSyntax :\n");
            Console.WriteLine("sitebuilder <txtfile1> <txtfile2> [<txtfileN>...] <resfile> [#var=newval]*");
            Console.WriteLine("sitebuilder /utf16 <file1> <file2> [<fileN>...] <resfile> [#var=newval]*");
            Console.WriteLine("sitebuilder /utf32 <file1> <file2> [<fileN>...] <resfile> [#var=newval]*");
            Console.WriteLine("\nSample :\n");
            Console.WriteLine("sitebuilder header.htm content12.htm footer.htm page12.html #menuid=menu12 #backhref=page11.html");
            Console.WriteLine("");
            Console.WriteLine("will produce page12.html file by concatening header.htm, content12.htm and");
            Console.WriteLine("footer.htm files. Then it will delete all lines containing <!--#delete-->");
            Console.WriteLine("comment. Then, all occurrences of #menuidand and #backhref patterns in the");
            Console.WriteLine("result file are replaced by the provided values.");
        }
        static void DisplayParams(Params p)
        {
            Console.WriteLine("\nI will append following files :");
            foreach (string f in p.FilesToConcat) { Console.WriteLine(String.Format(" {0} ", f)); }
            switch (p.UTF_OPTION)
            {
                case Params.UTF8_ON:
                    Console.WriteLine("with only one UTF-8 BOM (Dec 239 187 191 Hex EF BB BF)");
                    break;
                case Params.UTF16_ON:
                    Console.WriteLine("with only one UTF-16 BOM (Dec 255 254 Hex FF FE)");
                    break;
                case Params.UTF32_ON:
                    Console.WriteLine("with only one UTF-32 BOM (Dec 255 254 0 0 Hex FF FE 00 00)");
                    break;
                default:
                    Console.WriteLine("with only one UTF-8 BOM (Dec 239 187 191 Hex EF BB BF)");
                    break;
            }
            Console.WriteLine("to produce " + p.ResultFile);
            if (p.ToReplaceList.Count > 0)
            {
                Console.WriteLine("then I will process following replacements");
                foreach (Replacement repl in p.ToReplaceList) { Console.WriteLine(String.Format(" replace {0} with {1}", repl.pattern, repl.replacement)); }
            }
        }

        // Replace
        private static void ReplaceTextOrDeleteLine(string originalFile, string outputFile, List<Replacement> toReplace)
        {
            string tempLineValue;
            using (FileStream inputStream = File.OpenRead(originalFile))
            {
                using (StreamReader inputReader = new StreamReader(inputStream))
                {
                    using (StreamWriter outputWriter = File.AppendText(outputFile))
                    {
                        while (null != (tempLineValue = inputReader.ReadLine()))
                        {
                            if (!tempLineValue.Contains(Params.CMD_DELETELINE))
                            {
                                string buffer=tempLineValue;
                                foreach (Replacement repl in toReplace)
                                {
                                    buffer=buffer.Replace(repl.pattern, repl.replacement);
                                }
                                outputWriter.WriteLine(buffer);
                            }
                        }
                    }
                }
            }
        }

        static ExitCode processFiles(List<string> files, string resultFile, List<Replacement> toReplace, out string errorMsg, int utfOption)
        {
            ExitCode rc = ExitCode.Success;
            errorMsg = Errors.EM_SUCCESS;

            // Create temporary file
            string tempfile=Path.GetTempFileName();
            if (File.Exists(tempfile)) File.Delete(tempfile);
            TargetFileWriter writer = new TargetFileWriter(tempfile, FileMode.CreateNew);
            // Open each input file in read mode, and copy its content to the destination file
            bool isFirst= true;
            foreach (string file in files)
            {
                if (isFirst) writer.Append(file);
                else writer.AppendWithoutBom(file, utfOption);
                isFirst=false;
            }
            writer.Close();

            // Replace all placeholders
            ReplaceTextOrDeleteLine(tempfile, resultFile, toReplace);

            if (File.Exists(tempfile)) File.Delete(tempfile);

            return rc;
        }

        static int Main(string[] args)
        {
            Params parameters = new Params(args);
            ExitCode rc=parameters.CurrentError;
            string errorMsg;

            if (parameters.CurrentError == ExitCode.Success)
            {
                DisplayParams(parameters);
                rc = processFiles(parameters.FilesToConcat, parameters.ResultFile, parameters.ToReplaceList, out errorMsg, parameters.UTF_OPTION);
                if (rc == ExitCode.Success) { Console.WriteLine("done."); }
                else { Console.WriteLine(String.Format("\n*** {0} {1}", rc, errorMsg)); }
            }

            if (parameters.CurrentError != ExitCode.Success) {
                DisplayHelp();
                Console.WriteLine(String.Format("\n*** {0} {1}", parameters.CurrentError, parameters.CurrentErrorMsg));
            }
            return (int)parameters.CurrentError;
        }
    }
}
