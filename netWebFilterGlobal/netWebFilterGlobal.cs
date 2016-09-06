using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace netWebFilterGlobal
{
    public interface INetCodeBehind
    {
        void ProcessPostBack(QueryStringArg[] args, INetFilter callingFilter);
        void DoPostBack();
    }

    public interface INetFilter
    {
        void AddToBuffer(string s);
    }

    public class NetFilterBase
    {
        protected StringBuilder _result = new StringBuilder();
        protected string parameters = string.Empty;
        protected string callbackFunction = string.Empty;
        protected string postBackFunction = string.Empty;
        protected INetFilter _callingFilter = null;

        public void ProcessPostBack(QueryStringArg[] args, INetFilter callingFilter)
        {
            _callingFilter = callingFilter;
            foreach (QueryStringArg arg in args)
            {
                switch (arg.Arg)
                {
                    case "p":
                        parameters = arg.Value;
                        break;
                    case "pf":
                        postBackFunction = arg.Value;
                        break;
                    case "cb":
                        callbackFunction = arg.Value;
                        break;
                }
            }
        }

        public virtual void DoPostBack()
        {
            _callingFilter.AddToBuffer("_0500_Nothing to do");
        }
    }

    public class QueryStringArg
    {
        public QueryStringArg(string arg, string value)
        {
            Arg = arg;
            Value = value;
        }

        public string Arg { get; private set; }
        public string Value { get; private set; }
    }

    public class Log
    {
        public void Write(string message)
        {
            using (StreamWriter w = File.AppendText("errors.txt"))
            {
                w.Write(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " ");
                w.WriteLine(message);
            }
        }
    }
}
