using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sitebuilder
{
    class Replacement
    {
        public string pattern;
        public string replacement;
    }
    class Params
    {

        public const string CMD_DELETELINE = "<!--#delete-->";

        public const int UTF8_ON = 1;
        public const int UTF16_ON = 2;
        public const int UTF32_ON = 3;

        public int UTF_OPTION = UTF8_ON;

        private string[] originalArgs;
        public string CurrentErrorMsg;
        public ExitCode CurrentError;
        public List<string> FilesToConcat;
        public string ResultFile;
        public List<Replacement> ToReplaceList;

        public Params(string[] parameters)
        {
            CurrentError = ExitCode.Success;
            CurrentErrorMsg = Errors.EM_SUCCESS;

            originalArgs = parameters;

            FilesToConcat = new List<string>();
            ToReplaceList = new List<Replacement>();

            if (parameters.Length < 3)
            {
                CurrentError = ExitCode.LessThanThreeArgs; CurrentErrorMsg = Errors.EM_LESS_THAN_THREE;
            }
            if (CurrentError == ExitCode.Success)
            {
                int maxIndex = parameters.Length - 1;
                string arg;
                // Check first args
                for (int i = 0; i <= maxIndex; i++)
                {
                    arg = parameters[i];
                    if (CheckReplacementParam(arg))
                    {
                        // #id=newvalue argument
                        Replacement repl = new Replacement();
                        string[] splitted = arg.Split('=');
                        if (splitted.Length == 2)
                        {
                            repl.pattern = arg.Split('=')[0];
                            repl.replacement = arg.Split('=')[1];
                            ToReplaceList.Add(repl);
                        }
                        else
                        {
                            CurrentError = ExitCode.InvalideReplacementArg; CurrentErrorMsg = Errors.EM_FILE_NOT_FOUND + " " + arg; 
                        }
                    }
                    else
                    {
                        if (!CheckAndSetUtfparam(arg)) // utf* argument processed by the test
                            FilesToConcat.Add(arg);
                    }
                }
                // Set result file (last file arg) and remove it from the lits to process
                ResultFile = FilesToConcat.Last<string>();
                FilesToConcat.RemoveAt(FilesToConcat.Count - 1);

                // Check if files exists
                foreach (string f in FilesToConcat)
                {
                    if (!File.Exists(f)) { CurrentError = ExitCode.FileNotFound; CurrentErrorMsg = Errors.EM_FILE_NOT_FOUND + " " + f; }
                }
                // Check if result file does not exist
                if (File.Exists(ResultFile)) { CurrentError = ExitCode.TargetFileExist; CurrentErrorMsg = Errors.EM_TARGET_FILE_EXIST + " " + ResultFile; }

            }
        }

        bool CheckReplacementParam(string arg)
        {
            bool isReplacement = false;
            if (arg.Length > 3)
            {
                if ((arg.IndexOf('#')==0) && (arg.IndexOf('=')>1)) isReplacement = true;
            }
            return isReplacement;
        }
        bool CheckAndSetUtfparam(string arg)
        {
            bool isUtf = false;
            string minArg = arg.ToLower();
            switch (minArg)
            {
                case "utf8":
                case "-utf8":
                case "/utf8":
                case "utf-8":
                case "-utf-8":
                case "/utf-8":
                    UTF_OPTION = UTF16_ON;
                    isUtf = true;
                    break;
                case "utf16":
                case "-utf16":
                case "/utf16":
                case "utf-16":
                case "-utf-16":
                case "/utf-16":
                    UTF_OPTION = UTF16_ON;
                    isUtf = true;
                    break;
                case "utf32":
                case "-utf32":
                case "/utf32":
                case "utf-32":
                case "-utf-32":
                case "/utf-32":
                    UTF_OPTION = UTF32_ON;
                    isUtf = true;
                    break;
            }
            return isUtf;
        }


    }
}
