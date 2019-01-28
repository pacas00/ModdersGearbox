using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace petercashel.ModdersGearboxAPI.Attributes
{
	public class SubscribeEventAttribute : Attribute
	{
        public SubscribableEvents value;

        public SubscribeEventAttribute(SubscribableEvents _event)
        {
            this.value = _event;
        }
	}

    public enum SubscribableEvents
    {
        IntermodComm,

    }
}
