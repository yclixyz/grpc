using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Progress.Client
{
    public class ReponseReport : Progress<int>
    {
        private readonly IProgress<int> _progress;

        public ReponseReport(IProgress<int> progress)
        {
            _progress = progress;
        }

        public void Report(int value)
        {
            _progress.Report(value);
        }
    }
}
