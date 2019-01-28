using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace petercashel.ModdersGearboxAPI.Attributes
{
	/// <summary>
	/// Methods annotated with this Attribute will be auto subscribed to events.
	/// Also works on methods placed directly in your FortressCraftMod class.
	/// </summary>
	public class SubscribeEventAttribute : Attribute
	{
        ///<exclude/>
		public SubscribableEvents value;

		/// <summary>
		/// Methods annotated with this Attribute will be auto subscribed to events.
		/// </summary>
		/// <param name="_event">Event to be subscribed to</param>
		public SubscribeEventAttribute(SubscribableEvents _event)
        {
            this.value = _event;
        }
	}

    /// <summary>
	/// Events that can be subscribed to.
	/// </summary>
    public enum SubscribableEvents
    {
        /// <summary>
		/// Intermod Communication
		/// </summary>
        IntermodComm,

    }
}
