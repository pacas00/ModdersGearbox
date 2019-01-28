using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace petercashel.ModdersGearboxAPI.Attributes
{
	/// <summary>
	/// Add this Attribute to a class containing methods annotated with SubscribeEventAttribute
	/// Or a field in your FortressCraftMod class, with an class instance containing methods annotated with SubscribeEventAttribute
	/// </summary>
	public class EventHandlersAttribute : Attribute
	{
        public EventHandlersAttribute()
        {

        }
	}

}
