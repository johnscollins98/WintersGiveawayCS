using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WintersGiveaway.Interfaces;

namespace WintersGiveaway.Services
{
    internal class BasicFile : IFile
    {
        private readonly string filename;

        public BasicFile(string filename)
        {
            this.filename = filename;
        }

        public string GetText()
        {
            return File.ReadAllText(filename);
        }
    }
}
