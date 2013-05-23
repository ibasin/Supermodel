using System;
using System.Web.Mvc;

namespace Supermodel.Mvc.Extensions
{
    public static class SupermodelNamespaceGateway
    {
        #region Supermodel namespace gateway
        public static SupermodelNamespaceControllerTypeExtensions Supermodel(this Type type)
        {
            return new SupermodelNamespaceControllerTypeExtensions(type);
        }
        
        public static SupermodelNamespaceControllerExtensions<ControllerT> Supermodel<ControllerT>(this ControllerT controller) where ControllerT : Controller
        {
            return new SupermodelNamespaceControllerExtensions<ControllerT>(controller);
        }
        public static SupermodelNamespaceControllerExtensions<ControllerT> Supermodel<ControllerT>(this Controller controller) where ControllerT : Controller
        {
            return new SupermodelNamespaceControllerExtensions<ControllerT>(controller);
        }
        
        public static SupermodelNamespaceHtmlHelperExtensions<dynamic> Supermodel(this HtmlHelper html)
        {
            return new SupermodelNamespaceHtmlHelperExtensions<dynamic>(html);
        }
        public static SupermodelNamespaceHtmlHelperExtensions<ModelT> Supermodel<ModelT>(this HtmlHelper<ModelT> html)
        {
            return new SupermodelNamespaceHtmlHelperExtensions<ModelT>(html);
        }

        public static SupermodelNamespaceStringExtensions Supermodel(this string str)
        {
            return new SupermodelNamespaceStringExtensions(str);
        }
        public static SupermodelNamespaceMvcHtmlStringExtensions Supermodel(this MvcHtmlString mvcStr)
        {
            return new SupermodelNamespaceMvcHtmlStringExtensions(mvcStr);
        }

        public static SupermodelNamespaceTempDataDictionaryExtensions Supermodel(this TempDataDictionary tempData)
        {
            return new SupermodelNamespaceTempDataDictionaryExtensions(tempData);
        }
        #endregion
    }
}
