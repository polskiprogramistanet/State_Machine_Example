using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace State_Machine_Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var phoneCall = new PhoneCall("Stefan");//połaczenie telefoniczne
            phoneCall.Print();
            phoneCall.Dialed("Karol");
            phoneCall.Print();
            phoneCall.Connected();
            phoneCall.Print();

            //Console.WriteLine(phoneCall.ToDotGraph());

            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
        }
    }
}
