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
            var phoneCall = new PhoneCall("Rafał");//połaczenie telefoniczne
            phoneCall.Print();
            phoneCall.Dialed("Karol");
            phoneCall.Print();
            phoneCall.Connected();
            phoneCall.Print();
            phoneCall.SetVolume(2);
            phoneCall.Print();
            phoneCall.Hold();
            phoneCall.Print();
            phoneCall.Mute();
            phoneCall.Print();
            phoneCall.UnMute();
            phoneCall.Print();
            phoneCall.Resume();
            phoneCall.Print();
            phoneCall.SetVolume(11);
            phoneCall.Print();
            //Console.WriteLine(phoneCall.ToDotGraph());

            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
        }
    }
}
