using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using netWebFilterGlobal;

namespace netWebFilter
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                NetWebFilterBase filter = new NetWebFilterBase();

                filter.Init();

                if (filter.RequestedPage.EndsWith(".netf"))
                {
                    filter.ServeFile();
                }
                else if (filter.RequestedPage.EndsWith(".netf.p"))
                {
                    filter.ProcessPostBack();
                }

                filter.WriteBuffer();
            }
            catch (Exception e)
            {
                Log l = new Log();
                l.Write(e.ToString());
            }
        }
    }
}
