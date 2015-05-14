using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuickSearch.Models
{
    public class FileSystemItemError
    {
        public string Path { get; private set; }
        public string ErrorMessage { get; private set; }
        public string ErrorDetails { get; private set; }

        public FileSystemItemError(string path, string errorMessage, string errorDetails)
        {
            Path = path;
            ErrorMessage = errorMessage;
            ErrorDetails = errorDetails;
        }

        public FileSystemItemError(string path, string errorMessage) : this (path, errorMessage, string.Empty)
        {
        }

        public FileSystemItemError(string path, Exception exception) : this(path, exception.Message, exception.StackTrace)
        {
        }

        public FileSystemItemError(FileSystemInfo info, Exception exception) : this(info.FullName, exception)
        {
        }
    }
}
