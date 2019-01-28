using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace petercashel.ModdersGearboxAPI.Internal.API
{
    public interface IAPIInstance
    {
        //Sub APIs Here






        //Intermod Comms

        void SendIntermodComms(string modKey, string message, object payload);




    }
}
