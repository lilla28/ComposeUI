using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WExmapleProgram.Fakes
{
    public class FakeService: IFakeService
    {
        public FakeService()
        {

        }
        public void DoSomething()
        {
            Console.WriteLine("just a bad service imitator");
            Thread.Sleep(500);
            Console.WriteLine("done");
        }
    }
}
