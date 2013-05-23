using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace Supermodel.Mvc.Extensions
{
    public class SupermodelNamespaceControllerExtensions<ControllerT> where ControllerT : Controller
    {
        #region Constructors
        public SupermodelNamespaceControllerExtensions(Controller controller)
        {
            _controller = controller;
        }
	    #endregion

        #region RedirectToActionStrong
        public RedirectToRouteResult RedirectToActionStrong(Expression<Action<ControllerT>> action)
        {
            return RedirectToActionStrongInternal(action);
        }
        public RedirectToRouteResult RedirectToActionStrong<T>(Expression<Action<T>> action) where T : Controller
        {
            return RedirectToActionStrongInternalGeneric(action);
        }
        private RedirectToRouteResult RedirectToActionStrongInternalGeneric<T>(Expression<Action<T>> action)
        {
            var routeValues = new RouteValueDictionary();

            var methodExpression = action.Body as MethodCallExpression;
            if (methodExpression == null) throw new InvalidOperationException("Expression must be a method call.");
            if (methodExpression.Object != action.Parameters[0]) throw new InvalidOperationException("Method call must target lambda argument.");

            var actionName = methodExpression.Method.Name;
            var controllerName = typeof(T).Supermodel().GetControllerName();

            routeValues.Add("controller", controllerName);

            var parameters = methodExpression.Method.GetParameters();
            for (var i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                var argumentExpression = methodExpression.Arguments[i];
                var argument = Expression.Lambda(argumentExpression).Compile().DynamicInvoke();
                if (param.Name != "ignore") routeValues.Add(param.Name, argument);
            }
            return new RedirectToRouteResult(MergeRouteValues(actionName, controllerName, null, routeValues, true));
        }
        private RedirectToRouteResult RedirectToActionStrongInternal(Expression<Action<ControllerT>> action)
        {
            var routeValues = new RouteValueDictionary();

            var methodExpression = action.Body as MethodCallExpression;
            if (methodExpression == null) throw new InvalidOperationException("Expression must be a method call.");
            if (methodExpression.Object != action.Parameters[0]) throw new InvalidOperationException("Method call must target lambda argument.");

            var actionName = methodExpression.Method.Name;
            var controllerName = typeof(ControllerT).Supermodel().GetControllerName();
            
            routeValues.Add("controller", controllerName);

            var parameters = methodExpression.Method.GetParameters();
            for (var i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                var argumentExpression = methodExpression.Arguments[i];
                var argument = Expression.Lambda(argumentExpression).Compile().DynamicInvoke();
                if (param.Name != "ignore") routeValues.Add(param.Name, argument);
            }
            return new RedirectToRouteResult(MergeRouteValues(actionName, controllerName, null, routeValues, true));
        }
        #endregion

        #region Private helpers
        private static RouteValueDictionary MergeRouteValues(string actionName, string controllerName, RouteValueDictionary implicitRouteValues, RouteValueDictionary routeValues, bool includeImplicitMvcValues)
        {
            var routeValueDictionary = new RouteValueDictionary();
            if (includeImplicitMvcValues)
            {
                object obj;
                if (implicitRouteValues != null && implicitRouteValues.TryGetValue("action", out obj)) routeValueDictionary["action"] = obj;
                if (implicitRouteValues != null && implicitRouteValues.TryGetValue("controller", out obj)) routeValueDictionary["controller"] = obj;
            }
            if (routeValues != null)
            {
                foreach (KeyValuePair<string, object> keyValuePair in GetRouteValues(routeValues)) routeValueDictionary[keyValuePair.Key] = keyValuePair.Value;
            }
            if (actionName != null) routeValueDictionary["action"] = actionName;
            if (controllerName != null) routeValueDictionary["controller"] = controllerName;
            return routeValueDictionary;
        }
        private static RouteValueDictionary GetRouteValues(RouteValueDictionary routeValues)
        {
            return routeValues == null ? new RouteValueDictionary() : new RouteValueDictionary(routeValues);
        }
        #endregion

        #region Private Context
        // ReSharper disable NotAccessedField.Local
        private readonly Controller _controller;
        // ReSharper restore NotAccessedField.Local
        #endregion
    }
}
