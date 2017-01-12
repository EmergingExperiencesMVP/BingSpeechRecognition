using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechRecognitionProxy
{
    public class Properties
    {
        public string requestid { get; set; }
        public string HIGHCONF { get; set; }
    }

    public class Header
    {
        public string status { get; set; }
        public string scenario { get; set; }
        public string name { get; set; }
        public string lexical { get; set; }
        public Properties properties { get; set; }
    }

    public class Properties2
    {
        public string HIGHCONF { get; set; }
    }

    public class Result
    {
        public string scenario { get; set; }
        public string name { get; set; }
        public string lexical { get; set; }
        public string confidence { get; set; }
        public Properties2 properties { get; set; }
    }

    public class RootObject
    {
        public string version { get; set; }
        public Header header { get; set; }
        public List<Result> results { get; set; }
    }
}
