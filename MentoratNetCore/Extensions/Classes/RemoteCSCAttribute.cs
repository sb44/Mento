using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MentoratNetCore.Extensions.Classes
{
    public class RemoteCSCAttribute : RemoteAttribute
    {
        public RemoteCSCAttribute()
        {

        }

        public RemoteCSCAttribute(string action, string controller) : base(action, controller)
        {
        }

        private static List<Type> GetControllerList()
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(Controller))).ToList();
        }

        public override bool IsValid(object value)
        {
            Type controller = GetControllerList().FirstOrDefault(x => x.Name == string.Format("{0}Controller", this.RouteData["controller"]));
            if (controller == null)
            {
                // Default behavior of IsValid when no controller is found.
                return true;
            }

            // Find the Method passed in constructor
            MethodInfo mi = controller.GetMethod(this.RouteData["action"].ToString());
            if (mi == null)
            {
                // Default behavior of IsValid when action not found
                return true;
            }

            // Create instance of the controller to be able to call non static validation method
            object instance = Activator.CreateInstance(controller);

            // invoke the method on the controller with value
            var result = (JsonResult)mi.Invoke(instance, new object[] { value });

            // Return success or the error message string from CustomRemoteAttribute
            return (bool)result.Value;

        }

    }
}