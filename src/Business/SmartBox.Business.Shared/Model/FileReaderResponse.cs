using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Shared.Models
{
    public class FileReaderResponse
    {
        public bool isError { get; set; }
        public string ErrorMessage { get; set; }
        public string Content { get; set; }
    }
}
