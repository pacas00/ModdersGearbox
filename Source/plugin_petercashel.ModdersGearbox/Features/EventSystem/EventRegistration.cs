using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using petercashel.ModdersGearboxAPI.Attributes;

namespace plugin_petercashel_ModdersGearbox.Features.EventSystem
{
	public static class EventRegistration
    {
        public static bool EventsRegistered = false;
        public static bool EventsRegisteredTest = false;


		public static void RegisterAllMods()
        {
            if (EventsRegistered) return;

			foreach (FortressCraftMod instanceLoadedPlugin in ModManager.instance.mLoadedPlugins)
            {
                //Find all static classes in mod assemblies with EventHandlersAttribute
                //Process static members with SubscribeEventAttribute

				var classes = GetTypesWithEventHandlersAttribute(instanceLoadedPlugin.GetType().Assembly);

                foreach (Type @class in classes)
                {
                    var methods = GetMethodsWithSubscribeEventAttribute(@class, BindingFlags.Static | BindingFlags.Public);

                    foreach (MethodInfo method in methods)
                    {
                        SubscribeEventAttribute attribute = (SubscribeEventAttribute) method.GetCustomAttributes(typeof(SubscribeEventAttribute), false).First();
                        if (attribute != null) SubscribeEventHandler(attribute.value, null, method);
					}
                }

                //Find all members in FortressCraftMod classes with EventHandlersAttribute
                //Process instance members with SubscribeEventAttribute

				var fields = GetFieldsWithEventHandlersAttribute(instanceLoadedPlugin.GetType());

                foreach (FieldInfo fieldInfo in fields)
                {
                    var methods = GetMethodsWithSubscribeEventAttribute(fieldInfo.FieldType);

                    foreach (MethodInfo method in methods)
                    {
                        SubscribeEventAttribute attribute = (SubscribeEventAttribute)method.GetCustomAttributes(typeof(SubscribeEventAttribute), false).First();
                        if (attribute != null) SubscribeEventHandler(attribute.value, fieldInfo.GetValue(instanceLoadedPlugin), method);
                    }
				}



                //Find and Process all members in FortressCraftMod classes with SubscribeEventAttribute
				{
					var methods = GetMethodsWithSubscribeEventAttribute(instanceLoadedPlugin.GetType(), BindingFlags.Instance | BindingFlags.Public);

                    foreach (MethodInfo method in methods)
                    {
                        SubscribeEventAttribute attribute = (SubscribeEventAttribute)method.GetCustomAttributes(typeof(SubscribeEventAttribute), false).First();
                        if (attribute != null) SubscribeEventHandler(attribute.value, instanceLoadedPlugin, method);
                    }
				}
            }

            EventsRegistered = true;
        }

        static void SubscribeEventHandler(SubscribableEvents @event, object TargetClass, MethodInfo method)
        {
            try
            {
                switch (@event)
                {
                    case SubscribableEvents.IntermodComm:
                    {
                        EventInfo eventInfo = typeof(EventRegistration).GetEvent("OnIntermodCommunication");
                        Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, TargetClass, method);
                        eventInfo.AddEventHandler(TargetClass, handler);
                        break;
                    }
                }
			}
            catch (Exception e)
            {
                try
                {
                    UtilClass.WriteLine("Exception registering " + @event.ToString() + " event handler for " + method.Name);
                }
                catch
                {
                    //Discard error here. Most likely string or null related
                }

                UtilClass.WriteLine(e);
            }
		}

        static IEnumerable<Type> GetTypesWithEventHandlersAttribute(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(EventHandlersAttribute), true).Length > 0)
                {
                    yield return type;
                }
            }
        }

        static IEnumerable<FieldInfo> GetFieldsWithEventHandlersAttribute(Type type, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
        {
            foreach (FieldInfo field in type.GetFields(bindingFlags))
            {
                if (field.GetCustomAttributes(typeof(EventHandlersAttribute), true).Length > 0)
                {
                    yield return field;
                }
            }
        }

		static IEnumerable<MethodInfo> GetMethodsWithSubscribeEventAttribute(Type type, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
        {
            foreach (MethodInfo method in type.GetMethods(bindingFlags))
            {
                if (method.GetCustomAttributes(typeof(SubscribeEventAttribute), true).Length > 0)
                {
                    yield return method;
                }
            }
        }


		public delegate void IntermodCommunicationHander(string ModKey, string Message, object Payload);
        public static event IntermodCommunicationHander OnIntermodCommunication;
        public static void SendIntermodComms(string ModKey, string message, object payload)
        {
            if (EventRegistration.OnIntermodCommunication != null)
            {
                EventRegistration.OnIntermodCommunication(ModKey, message, payload);
            }
        }


	}
}
