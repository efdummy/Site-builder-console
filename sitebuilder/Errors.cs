using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sitebuilder
{

    enum ExitCode : int
    {
        Success = 0,
        LessThanThreeArgs = 1,
        FileNotFound = 2,
        TargetFileExist = 4,
        InvalideReplacementArg = 8,
    }

    public static class Errors
    {
        public const string EM_SUCCESS = "No error";
        public const string EM_LESS_THAN_THREE = "Less than three parameters";
        public const string EM_FILE_NOT_FOUND = "File not found";
        public const string EM_TARGET_FILE_EXIST = "Target file exists";
        public const string EM_INVALID_REPLACEMENT_ARG = "Invalid #id=newvalue arg";
    }
}
