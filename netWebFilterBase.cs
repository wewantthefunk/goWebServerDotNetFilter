using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using netWebFilterGlobal;

namespace netWebFilter
{
    public class NetWebFilterBase : INetFilter
    {
        private List<QueryStringArg> args = new List<QueryStringArg>();

        private StringBuilder buffer = new StringBuilder(4096);

        public void Init()
        {
            Stream inputStream = Console.OpenStandardInput();
            byte[] b = new byte[2048];
            int outputLength = inputStream.Read(b, 0, 2048);
            char[] chars = Encoding.UTF7.GetChars(b, 0, outputLength);
            string s = new string(chars);
            string[] url = s.Split('?');
            RequestedPage = url[0].Replace(Environment.NewLine, string.Empty);
            if (url.Length > 1)
            {
                string[] args1 = url[1].Replace(Environment.NewLine, string.Empty).Split('&');
                foreach (string arg in args1)
                {
                    string[] a = arg.Split('=');

                    args.Add(new QueryStringArg(a[0], a[1]));
                }
            }
        }

        public void AddToBuffer(string s)
        {
            buffer.Append(s);
        }

        public void ClearBuffer()
        {
            buffer.Remove(0, buffer.Length);
        }

        public void WriteBuffer()
        {
            Console.Write(buffer.ToString());
        }

        public QueryStringArg[] Args
        {
            get
            {
                return args.ToArray();
            }
        }

        public string RequestedPage
        {
            get;
            private set;
        }

        public bool ServeFile() 
        {
            bool result = false;

            AddToBuffer("_1200_");
            AddToBuffer(System.IO.File.ReadAllText(Environment.CurrentDirectory + RequestedPage));

            return result;
        }

        public bool ProcessPostBack()
        {
            bool result = true;

            string pluginModule = RequestedPage + ".dll";
            try
            {
                Assembly asm = Assembly.LoadFile(Environment.CurrentDirectory + pluginModule);

                INetCodeBehind ch = asm.GetTypes()[0].InvokeMember(null,
                                                BindingFlags.CreateInstance,
                                                null, null, null) as INetCodeBehind;

                ch.ProcessPostBack(Args, this);
                ch.DoPostBack();
            }
            catch (Exception e)
            {
                Log l = new Log();
                l.Write(e.ToString());
                l.Write(Environment.CurrentDirectory);
            }
            
            return result;
        }
    }
}
