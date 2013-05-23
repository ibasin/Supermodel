using System;
using System.Web.Mvc;

namespace Supermodel.Mvc.Extensions
{
    public class SupermodelNamespaceControllerTypeExtensions
    {
        public SupermodelNamespaceControllerTypeExtensions(Type type)
        {
            _type = type;
        }
        
        public string GetControllerName()
        {
            if (!typeof(IController).IsAssignableFrom(_type)) throw new SupermodelException("Type passsed is not a valid MVC controller");

            var controllerName = _type.Name;
            if (controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)) controllerName = controllerName.Remove(controllerName.Length - 10, 10);
            return controllerName;
        }
        
        #region Private Context
        private readonly Type _type;
        #endregion
    }
}
