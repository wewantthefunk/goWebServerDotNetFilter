using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace netWebFilter
{
    class Program
    {
        static void Main(string[] args)
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
    }
}
