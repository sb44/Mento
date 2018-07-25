using MentoratNetCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MentoratNetCore.Extensions.Classes
{
    public class RemoteCSCAttribute : RemoteAttribute
    {
        private string nomController;
        private string nomAction;

        public RemoteCSCAttribute()
        {

        }

        public RemoteCSCAttribute(string action, string controller) : base(action, controller)
        {
            nomController = controller;
            nomAction = action;
        }

        private static List<Type> GetControllerList()
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(Controller))).ToList();
        }

        public override bool IsValid(object value)
        {
            //nomController = nomController == "" ? this.RouteData["controller"].ToString() : nomController;
            //nomAction = nomAction == "" ? this.RouteData["action"].ToString() : nomAction;

            Type controller = GetControllerList().FirstOrDefault(x => x.Name == string.Format("{0}Controller", nomController));
            if (controller == null)
            {
                // Default behavior of IsValid when no controller is found.
                return true;
            }

            // Find the Method passed in constructor
            MethodInfo mi = controller.GetMethod(nomAction);
            if (mi == null)
            {
                // Default behavior of IsValid when action not found
                return true;
            }

            object instance;
            switch (controller.Name)
            {
                case "InscriptionsController":
                    //object[] myObjArr = { null, null };
                    instance = Activator.CreateInstance(controller, new object[] { null, null });
                    break;
                case "AssignationController":
                    instance = Activator.CreateInstance(controller, new object[] { null, null,null });
                    break;
                case "MentorsController":
                    instance = Activator.CreateInstance(controller);
                    break;
                case "InterventionsAdmController":
                    instance = Activator.CreateInstance(controller);
                    break;
                case "InterventionsController":
                    instance = Activator.CreateInstance(controller);
                    break;
                case "AccountController":
                    instance = Activator.CreateInstance(controller, new object[] { null, null,null,null,null,null });
                    break;
                default:
                    instance = Activator.CreateInstance(controller);
                    break;
            }
            // Create instance of the controller to be able to call non static validation method
            // object instance = Activator.CreateInstance(controller);

            var result = (JsonResult)mi.Invoke(instance, new object[] { value });

            // Return success or the error message string from CustomRemoteAttribute
            return (bool)result.Value;

        }

    }
}