using System;
using System.Collections.Generic;
using System.Text;
using netWebFilterGlobal;

namespace netFilterTest
{
    public class NetFilterTest : NetFilterBase, INetCodeBehind
    {
        public override void DoPostBack()
        {
            switch (postBackFunction)
            {
                case "testFunc":
                    string[] p = parameters.Split(',');
                    if (p.Length == 2)
                    {
                        testFunc(p[0], p[1]);
                    }
                    break;
            }
        }

        private void testFunc(string a, string b)
        {
            if (string.IsNullOrEmpty(callbackFunction))
                _callingFilter.AddToBuffer("_0200_'you sent up " + a + " and " + b + "'");
            else
                _callingFilter.AddToBuffer("_5200_" + callbackFunction + "('you sent up " + a + " and " + b + "');");
        }
    }
}
