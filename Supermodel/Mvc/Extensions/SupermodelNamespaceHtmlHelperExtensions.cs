using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using ReflectionMapper;
using Supermodel.DDD.Models.Domain;
using Supermodel.DDD.Models.View.Mvc;
using Supermodel.DDD.Models.View.Mvc.JQMobile;
using Supermodel.DDD.Models.View.Mvc.UIComponents;
using Supermodel.EmbeddedResources;

namespace Supermodel.Mvc.Extensions
{
    public class SupermodelNamespaceHtmlHelperExtensions<ModelT>
    {
        #region Constructors
		public SupermodelNamespaceHtmlHelperExtensions(HtmlHelper html)
        {
            _html = html;
        }
	    #endregion

        #region Strongly-typed ActionLink
        public MvcHtmlString ActionLinkStrong<ControllerT>(string linkText, Expression<Func<ControllerT, ActionResult>> action, object htmlAttributes = null) where ControllerT : IController
        {
            var routeValues = new RouteValueDictionary();

            var methodExpression = action.Body as MethodCallExpression;
            if (methodExpression == null) throw new InvalidOperationException("Expression must be a method call.");
            
            var actionName = methodExpression.Method.Name;

            var controllerName = typeof(ControllerT).Supermodel().GetControllerName();
            routeValues.Add("controller", controllerName);

            var parameters = methodExpression.Method.GetParameters();
            for (var i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                var argumentExpression = methodExpression.Arguments[i];
                var argument = Expression.Lambda(argumentExpression).Compile().DynamicInvoke();
                routeValues.Add(param.Name, argument);
            }
            return _html.ActionLink(linkText, actionName, routeValues, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }
        #endregion

        #region RESTful ActionLink helpers
        public MvcHtmlString RESTfulActionLink(HttpVerbs actionVerb, string linkText, object htmlAttributes, string actionName, object routeValues, string confMsg)
        {
            var result = new StringBuilder();
            result.AppendLine(_html.Supermodel().BeginMultipartFormMvcHtmlString(actionName, null /*controllerName*/, routeValues).ToString());
            result.AppendLine(ActionLinkFormContent(_html, linkText, actionVerb, htmlAttributes, confMsg).ToString());
            result.AppendLine(_html.Supermodel().EndFormMvcHtmlString().ToString());
            return MvcHtmlString.Create(result.ToString());
        }
        public MvcHtmlString RESTfulActionLink(HttpVerbs actionVerb, string linkText, object htmlAttributes, string actionName, object routeValues, string controllerName, string confMsg)
        {
            var result = new StringBuilder();
            result.AppendLine(_html.Supermodel().BeginMultipartFormMvcHtmlString(actionName, controllerName, routeValues).ToString());
            result.AppendLine(ActionLinkFormContent(_html, linkText, actionVerb, htmlAttributes, confMsg).ToString());
            result.AppendLine(_html.Supermodel().EndFormMvcHtmlString().ToString());
            return MvcHtmlString.Create(result.ToString());
        }
        public MvcHtmlString RESTfulActionLink(HttpVerbs actionVerb, string linkText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, string confMsg)
        {
            var result = new StringBuilder();
            result.AppendLine(_html.Supermodel().BeginMultipartFormMvcHtmlString(actionName, null /*controllerName*/, routeValues).ToString());
            result.AppendLine(ActionLinkFormContent(_html, linkText, actionVerb, htmlAttributes, confMsg).ToString());
            result.AppendLine(_html.Supermodel().EndFormMvcHtmlString().ToString());
            return MvcHtmlString.Create(result.ToString());
        }
        public MvcHtmlString RESTfulActionLink(HttpVerbs actionVerb, string linkText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, IDictionary<string, object> formHtmlAttributes, string confMsg)
        {
            var result = new StringBuilder();
            result.AppendLine(_html.Supermodel().BeginMultipartFormMvcHtmlString(actionName, null /*controllerName*/, routeValues, FormMethod.Post, formHtmlAttributes).ToString());
            result.AppendLine(ActionLinkFormContent(_html, linkText, actionVerb, htmlAttributes, confMsg).ToString());
            result.AppendLine(_html.Supermodel().EndFormMvcHtmlString().ToString());
            return MvcHtmlString.Create(result.ToString());
        }
        public MvcHtmlString RESTfulActionLink(HttpVerbs actionVerb, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, string confMsg)
        {
            return RESTfulActionLink(actionVerb, linkText, actionName, controllerName, routeValues, htmlAttributes, null, confMsg);
        }
        public MvcHtmlString RESTfulActionLink(HttpVerbs actionVerb, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, IDictionary<string, object> formHtmlAttributes, string confMsg)
        {
            var result = new StringBuilder();
            result.AppendLine(_html.Supermodel().BeginMultipartFormMvcHtmlString(actionName, controllerName, routeValues, FormMethod.Post, formHtmlAttributes).ToString());
            result.AppendLine(ActionLinkFormContent(_html, linkText, actionVerb, htmlAttributes, confMsg).ToString());
            result.AppendLine(_html.Supermodel().EndFormMvcHtmlString().ToString());
            return MvcHtmlString.Create(result.ToString());
        }

        private static MvcHtmlString ActionLinkFormContent(HtmlHelper htmlHelper, string linkText, HttpVerbs actionVerb, object htmlAttributes, string confMsg)
        {
            return ActionLinkFormContent(htmlHelper, linkText, actionVerb, UtilsLib.ObjectToCaseSensitiveDictionary(htmlAttributes), confMsg);
        }
        private static MvcHtmlString ActionLinkFormContent(HtmlHelper htmlHelper, string linkText, HttpVerbs actionVerb, IDictionary<string, object> htmlAttributes, string confMsg)
        {
            var result = new StringBuilder();

            //HttpMethodOverride
            switch (actionVerb)
            {
                case HttpVerbs.Delete:
                case HttpVerbs.Head:
                case HttpVerbs.Put:
                    result.AppendLine(htmlHelper.HttpMethodOverride(actionVerb).ToHtmlString());
                    break;
                case HttpVerbs.Post:
                    break;
                case HttpVerbs.Get:
                    throw new SupermodelException("HttpVerbs.Get is not supported in ActionLinkFormContent");
            }

            //submit
            var aTag = new TagBuilder("input");
            aTag.MergeAttribute("link", "");
            aTag.MergeAttribute("type", "submit");
            aTag.MergeAttribute("Value", linkText);
            aTag.MergeAttributes(htmlAttributes);
            if (confMsg != null) aTag.MergeAttribute("data-sm-ConfirmMsg", confMsg);
            result.AppendLine(aTag.ToString());

            return MvcHtmlString.Create(result.ToString());
        }
        #endregion

        #region Form Method Extensions
        public MvcForm BeginMultipartForm()
        {
            var rawUrl = _html.ViewContext.HttpContext.Request.RawUrl;
            return MultipartFormHelper(rawUrl, FormMethod.Post, new RouteValueDictionary());
        }
        //public MvcForm BeginMultipartForm(string formId)
        //{
        //    return BeginMultipartForm(new Dictionary<string, object>{ { "id", formId } });
        //}
        public MvcForm BeginMultipartForm(IDictionary<string, object> htmlAttributes)
        {
            var rawUrl = _html.ViewContext.HttpContext.Request.RawUrl;
            return MultipartFormHelper(rawUrl, FormMethod.Post, htmlAttributes);
        }
        public MvcForm BeginMultipartForm(string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return BeginMultipartForm(actionName, controllerName, routeValues, FormMethod.Post, new RouteValueDictionary());
        }
        public MvcForm BeginMultipartForm(string actionName, string controllerName, object routeValues)
        {
            return BeginMultipartForm(actionName, controllerName, new RouteValueDictionary(routeValues), FormMethod.Post, new RouteValueDictionary());
        }
        public MvcForm BeginMultipartForm(string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            string formAction = UrlHelper.GenerateUrl(null, actionName, controllerName, routeValues, _html.RouteCollection, _html.ViewContext.RequestContext, true);
            return MultipartFormHelper(formAction, method, htmlAttributes);
        }
        private MvcForm MultipartFormHelper(string formAction, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            htmlAttributes.Add("enctype", "multipart/form-data");
            return (MvcForm)ReflectionHelper.ExecuteNonPublicStaticMethod(typeof(FormExtensions), "FormHelper", _html, formAction, method, htmlAttributes);
        }

        public MvcHtmlString BeginMultipartFormMvcHtmlString()
        {
            var rawUrl = _html.ViewContext.HttpContext.Request.RawUrl;
            return MultipartFormHelperMvcHtmlString(rawUrl, FormMethod.Post, new RouteValueDictionary());
        }
        //public MvcHtmlString BeginMultipartFormMvcHtmlString(string formId)
        //{
        //    return BeginMultipartFormMvcHtmlString(new Dictionary<string, object> { { "id", formId } });
        //}
        public MvcHtmlString BegingMultipartFormMvcHtmlString(IDictionary<string, object> htmlAttributes)
        {
            var rawUrl = _html.ViewContext.HttpContext.Request.RawUrl;
            return MultipartFormHelperMvcHtmlString(rawUrl, FormMethod.Post, htmlAttributes);
        }
        public MvcHtmlString BeginMultipartFormMvcHtmlString(string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return BeginMultipartFormMvcHtmlString(actionName, controllerName, routeValues, FormMethod.Post, new RouteValueDictionary());
        }
        public MvcHtmlString BeginMultipartFormMvcHtmlString(string actionName, string controllerName, object routeValues)
        {
            return BeginMultipartFormMvcHtmlString(actionName, controllerName, new RouteValueDictionary(routeValues), FormMethod.Post, new RouteValueDictionary());
        }
        public MvcHtmlString BeginMultipartFormMvcHtmlString(string actionName, string controllerName, RouteValueDictionary routeValues, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            string formAction = UrlHelper.GenerateUrl(null, actionName, controllerName, routeValues, _html.RouteCollection, _html.ViewContext.RequestContext, true);
            return MultipartFormHelperMvcHtmlString(formAction, method, htmlAttributes);
        }
        private MvcHtmlString MultipartFormHelperMvcHtmlString(string formAction, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            var tagBuilder = new TagBuilder("form");
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("action", formAction);
            tagBuilder.MergeAttribute("method", HtmlHelper.GetFormMethodString(method), true);
            tagBuilder.MergeAttribute("enctype", "multipart/form-data"); //This diffres for standard implemetation but we always want to have an ability to upload files
            var flag = _html.ViewContext.ClientValidationEnabled && !_html.ViewContext.UnobtrusiveJavaScriptEnabled;
            if (flag) tagBuilder.GenerateId((string)_html.ViewContext.ExecuteNonPublicMethod("FormIdGenerator"));
            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.StartTag));
        }

        public MvcHtmlString EndFormMvcHtmlString()
        {
            var result = new StringBuilder();
            result.AppendLine("</form>");
            result.AppendLine(OutputClientValidation(_html.ViewContext).ToString());
            _html.ViewContext.FormContext = null;
            return MvcHtmlString.Create(result.ToString());
        }
        private static MvcHtmlString OutputClientValidation(ViewContext viewContext)
        {
            string result;

            var clientValidation = (FormContext)viewContext.ExecuteNonPublicMethod("GetFormContextForClientValidation");
            if (clientValidation == null || viewContext.UnobtrusiveJavaScriptEnabled) result = "";
            else result = string.Format(CultureInfo.InvariantCulture, "<script type=\"text/javascript\">\r\n//<![CDATA[\r\nif (!window.mvcClientValidationMetadata) {{ window.mvcClientValidationMetadata = []; }}\r\nwindow.mvcClientValidationMetadata.push({0});\r\n//]]>\r\n</script>".Replace("\r\n", Environment.NewLine), new object[] { clientValidation.GetJsonValidationMetadata() });

            return MvcHtmlString.Create(result);
        }
        #endregion

        #region CRUD Edit helpers
        public MvcHtmlString CRUDEdit<EntityT, MvcModelT>(string pageTitle, bool readOnly = false, bool skipBackButton = false)
            where EntityT : class, IEntity, new()
            where MvcModelT : MvcModelForEntity<EntityT>, new()
        {
            return CRUDEdit<EntityT, MvcModelT>(new MvcHtmlString(pageTitle), readOnly, skipBackButton);
        }

        public MvcHtmlString CRUDEditJQMobile<EntityT, MvcModelT>(string pageTitle, bool readOnly = false, bool skipBackButton = false, bool forceNoDataAjax = false)
            where EntityT : class, IEntity, new()
            where MvcModelT : JQMobile.MvcModelForEntity<EntityT>, new()
        {
            return CRUDEditJQMobile<EntityT, MvcModelT>(new MvcHtmlString(pageTitle), readOnly, skipBackButton, forceNoDataAjax);
        }

        public MvcHtmlString CRUDEdit<EntityT, MvcModelT>(MvcHtmlString pageTitle = null, bool readOnly = false, bool skipBackButton = false)
            where EntityT : class, IEntity, new()
            where MvcModelT : MvcModelForEntity<EntityT>, new()
        {
            return CRUDEditHelper<EntityT, MvcModelT>(pageTitle, readOnly, skipBackButton);
        }

        public MvcHtmlString CRUDEditJQMobile<EntityT, MvcModelT>(MvcHtmlString pageTitle = null, bool readOnly = false, bool skipBackButton = false, bool forceNoDataAjax = false)
            where EntityT : class, IEntity, new()
            where MvcModelT : MvcModelForEntity<EntityT>, new()
        {
            return CRUDEditHelperJQMobile<EntityT, MvcModelT>(pageTitle, readOnly, skipBackButton, forceNoDataAjax);
        }

        private MvcHtmlString CRUDEditHelper<EntityT, MvcModelT>(MvcHtmlString pageTitle, bool readOnly, bool skipBackButton)
            where EntityT : class, IEntity, new()
            where MvcModelT : MvcModelForEntity<EntityT>, new()
        {
            var result = new StringBuilder();
            var model = (MvcModelT)_html.ViewData.Model;

            result.AppendLine(_html.Supermodel().BeginMultipartFormMvcHtmlString().ToString());
            if (!MvcHtmlString.IsNullOrEmpty(pageTitle)) result.AppendLine("<h1>" + pageTitle + "</h1>");

            if (model.Id != 0) result.AppendLine(_html.HttpMethodOverride(HttpVerbs.Put).ToString());
            result.AppendLine(_html.Supermodel().EditorForModel().ToString());

            if (!skipBackButton)
            {
                int? parentId = null;
                if (ReflectionHelper.IsClassADerivedFromClassB(model.GetType(), typeof(ChildMvcModelForEntity<,>))) parentId = (int?)model.PropertyGet("ParentId");
                // ReSharper disable Mvc.ActionNotResolved
                result.AppendLine(_html.ActionLink("Back", "List", new { parentId }, new { id = SupermodelSettings.Scaffolding.BackButtonId, @class = SupermodelSettings.Scaffolding.BackButtonCssClass }).ToString());
                // ReSharper restore Mvc.ActionNotResolved
            }
            if (!readOnly) result.AppendLine("<input type='submit' value='Save' " + SupermodelSettings.Scaffolding.MakeIdAndClassAttribues(SupermodelSettings.Scaffolding.SaveButtonId, SupermodelSettings.Scaffolding.SaveButtonCssClass) + "/>");
            result.AppendLine(_html.Supermodel().EndFormMvcHtmlString().ToString());

            return MvcHtmlString.Create(readOnly ? result.ToString().Supermodel().DisableAllControls() : result.ToString());
        }

        private MvcHtmlString CRUDEditHelperJQMobile<EntityT, MvcModelT>(MvcHtmlString pageTitle, bool readOnly, bool skipBackButton, bool forceNoDataAjax)
            where EntityT : class, IEntity, new()
            where MvcModelT : MvcModelForEntity<EntityT>, new()
        {
            var result = new StringBuilder();
            var model = (MvcModelT)_html.ViewData.Model;

            int? parentId = null;
            if (ReflectionHelper.IsClassADerivedFromClassB(model.GetType(), typeof(ChildMvcModelForEntity<,>))) parentId = (int?)model.PropertyGet("ParentId");

            //Start form
            var formHtmlAttributes = new Dictionary<string, object> { { "class", "sm-form" } };
            if (!model.IsNewModel)
            {
                formHtmlAttributes.Add("data-transition", "slide");
                formHtmlAttributes.Add("data-direction", "reverse");
            }
            if (forceNoDataAjax) formHtmlAttributes.Add("data-ajax", "false");
            result.AppendLine(_html.Supermodel().BegingMultipartFormMvcHtmlString(formHtmlAttributes).ToString());

            //Override Http Verb if needed
            if (model.Id != 0) result.AppendLine(_html.HttpMethodOverride(HttpVerbs.Put).ToString());

            //Header
            result.AppendLine("<div data-role='header' data-position='fixed'>");

            //Back button
            if (!skipBackButton)
            {
                var htmlAttributes = new Dictionary<string, object> { { "data-icon", "arrow-l" }, { "data-theme", "b" }, { "data-direction", "reverse" } };
                htmlAttributes.Add("data-transition", model.IsNewModel ? "flip" : "slide");
                // ReSharper disable Mvc.ActionNotResolved
                result.AppendLine(_html.ActionLink("Back", "List", new RouteValueDictionary { { "parentId", parentId }, { "_", DateTime.Now.Ticks } }, htmlAttributes).ToString());
                // ReSharper restore Mvc.ActionNotResolved
            }

            //Title
            var titleText = "New Record";
            // ReSharper disable PossibleNullReferenceException
            if (!model.IsNewModel) titleText = !MvcHtmlString.IsNullOrEmpty(pageTitle) ? pageTitle.ToString() : model.Label;
            // ReSharper restore PossibleNullReferenceException
            result.AppendLine("<h1>" + titleText + "</h1>");

            //Update button
            if (!readOnly)
            {
                var submitText = model.IsNewModel ? "Create" : "Update";
                result.AppendLine("<button type='submit' data-icon='check' data-theme='b'>" + submitText + "</button>");
            }
            result.AppendLine("</div>");

            //Form fields
            result.AppendLine(_html.Supermodel().EditorForModel().ToString());

            //End form
            result.AppendLine(_html.Supermodel().EndFormMvcHtmlString().ToString());

            //Delete button
            if (!model.IsNewModel)
            {
                result.AppendLine("<div class='sm-form'>");
                var htmlAttributes = new Dictionary<string, object> { { "data-role", "button" }, { "data-icon", "delete" }, { "data-theme", "f" } };
                var deleteFormHtmlAttributes = new Dictionary<string, object> { { "data-transition", "slide" }, { "data-direction", "reverse" } };
                var routeValues = new RouteValueDictionary { { "id", model.Id }, { "parentId", parentId }, { "_", DateTime.Now.Ticks } };
                result.AppendLine(_html.Supermodel().RESTfulActionLink(HttpVerbs.Delete, "Delete", "Edit", routeValues, htmlAttributes, deleteFormHtmlAttributes, "Are you sure?").ToString());
                result.AppendLine("</div>");
            }

            return MvcHtmlString.Create(readOnly ? result.ToString().Supermodel().DisableAllControls() : result.ToString());
        }
        #endregion

        #region CRUD List helpers
        public MvcHtmlString CRUDList<EntityT, MvcModelT>(ICollection<MvcModelT> items, Type controllerType, string pageTitle, bool skipAddNew = false)
            where MvcModelT : MvcModelForEntity<EntityT>, new()
            where EntityT : class, IEntity, new()
        {
            return CRUDListHelper<EntityT, MvcModelT>(items, controllerType, new MvcHtmlString(pageTitle), null, skipAddNew);
        }

        public MvcHtmlString CRUDListJQMobile<EntityT, MvcModelT>(ICollection<MvcModelT> items, Type controllerType, string pageTitle, bool skipAddNew = false)
            where MvcModelT : MvcModelForEntity<EntityT>, new()
            where EntityT : class, IEntity, new()
        {
            return CRUDListHelperJQMobile<EntityT, MvcModelT>(items, controllerType, new MvcHtmlString(pageTitle), null, skipAddNew);
        }

        public MvcHtmlString CRUDList<EntityT, MvcModelT>(ICollection<MvcModelT> items, Type controllerType, MvcHtmlString pageTitle = null, bool skipAddNew = false)
            where MvcModelT : MvcModelForEntity<EntityT>, new()
            where EntityT : class, IEntity, new()
        {
            return CRUDListHelper<EntityT, MvcModelT>(items, controllerType, pageTitle, null, skipAddNew);
        }

        public MvcHtmlString CRUDListJQMobile<EntityT, MvcModelT>(ICollection<MvcModelT> items, Type controllerType, MvcHtmlString pageTitle = null, bool skipAddNew = false)
            where MvcModelT : MvcModelForEntity<EntityT>, new()
            where EntityT : class, IEntity, new()
        {
            return CRUDListHelperJQMobile<EntityT, MvcModelT>(items, controllerType, pageTitle, null, skipAddNew);
        }

        public MvcHtmlString CRUDChildrenList<EntityT, MvcModelT>(ICollection<MvcModelT> items, Type controllerType, string pageTitle, int parentId, bool skipAddNew = false)
            where MvcModelT : MvcModelForEntity<EntityT>, new()
            where EntityT : class, IEntity, new()
        {
            return CRUDListHelper<EntityT, MvcModelT>(items, controllerType, new MvcHtmlString(pageTitle), parentId, skipAddNew);
        }

        public MvcHtmlString CRUDChildrenListJQMobile<EntityT, MvcModelT>(ICollection<MvcModelT> items, Type controllerType, string pageTitle, int parentId, bool skipAddNew = false)
            where MvcModelT : MvcModelForEntity<EntityT>, new()
            where EntityT : class, IEntity, new()
        {
            return CRUDListHelperJQMobile<EntityT, MvcModelT>(items, controllerType, new MvcHtmlString(pageTitle), parentId, skipAddNew);
        }

        
        public MvcHtmlString CRUDChildrenList<EntityT, MvcModelT>(ICollection<MvcModelT> items, Type controllerType, MvcHtmlString pageTitle, int parentId, bool skipAddNew = false)
            where MvcModelT : MvcModelForEntity<EntityT>, new()
            where EntityT : class, IEntity, new()
        {
            return CRUDListHelper<EntityT, MvcModelT>(items, controllerType, pageTitle, parentId, skipAddNew);
        }

        public MvcHtmlString CRUDChildrenListJQMobile<EntityT, MvcModelT>(ICollection<MvcModelT> items, Type controllerType, MvcHtmlString pageTitle, int parentId, bool skipAddNew = false)
            where MvcModelT : MvcModelForEntity<EntityT>, new()
            where EntityT : class, IEntity, new()
        {
            return CRUDListHelperJQMobile<EntityT, MvcModelT>(items, controllerType, pageTitle, parentId, skipAddNew);
        }

        private MvcHtmlString CRUDListHelper<EntityT, MvcModelT>(IEnumerable<MvcModelT> items, Type controllerType, MvcHtmlString pageTitle, int? parentId, bool skipAddNew)
            where MvcModelT : MvcModelForEntity<EntityT>, new()
            where EntityT : class, IEntity, new()
        {
            var controllerName = controllerType.Supermodel().GetControllerName();

            var result = new StringBuilder();
            if (parentId == null || parentId > 0)
            {
                if (!MvcHtmlString.IsNullOrEmpty(pageTitle)) result.AppendLine("<h1>" + pageTitle + "</h1>");
                result.AppendLine("<div" + SupermodelSettings.Scaffolding.MakeIdAndClassAttribues(SupermodelSettings.Scaffolding.CRUDListTopDivId, SupermodelSettings.Scaffolding.CRUDListTopDivCssClass) + ">");
                result.AppendLine("<table" + SupermodelSettings.Scaffolding.MakeIdAndClassAttribues(SupermodelSettings.Scaffolding.CRUDListTableId, SupermodelSettings.Scaffolding.CRUDListTableCssClass) + ">");
                result.AppendLine("<thead>");
                result.AppendLine("<tr>");
                result.AppendLine("<th> Name </th>");
                result.AppendLine("<th colspan=2> Actions </th>");
                result.AppendLine("</tr>");
                result.AppendLine("</thead>");
                result.AppendLine("<tbody>");
                foreach (var item in items)
                {
                    result.AppendLine("<tr>");
                    result.AppendLine("<td>" + _html.Encode(item.Label) + "</td>");
                    result.AppendLine("<td>" + _html.ActionLink("Edit", "Edit", controllerName, new { id = item.Id, parentId }, new { @class = SupermodelSettings.Scaffolding.CRUDListEditCssClass }) + "</td>");
                    result.AppendLine("<td>");
                    result.AppendLine(_html.Supermodel().RESTfulActionLink(HttpVerbs.Delete, "Delete", new { @class = SupermodelSettings.Scaffolding.CRUDListDeleteCssClass }, "Edit", new { id = item.Id, parentId }, controllerName, "Are you sure?").ToString());
                    result.AppendLine("</td>");
                    result.AppendLine("</tr>");
                }
                result.AppendLine("</tbody>");
                result.AppendLine("</table>");
                if (!skipAddNew)
                    result.AppendLine("<p>" + _html.ActionLink("Add New", "Edit", controllerName, new { id = 0, ParentId = parentId }, new { @class = SupermodelSettings.Scaffolding.CRUDListAddNewCssClass }) + "</p>");
                result.AppendLine("</div>");
            }
            return MvcHtmlString.Create(result.ToString());
        }

        private MvcHtmlString CRUDListHelperJQMobile<EntityT, MvcModelT>(IEnumerable<MvcModelT> items, Type controllerType, MvcHtmlString pageTitle, int? parentId, bool skipAddNew)
            where MvcModelT : MvcModelForEntity<EntityT>, new()
            where EntityT : class, IEntity, new()
        {
            var controllerName = controllerType.Supermodel().GetControllerName();

            var result = new StringBuilder();
            if (parentId == null || parentId > 0)
            {
                result.AppendLine("<ul data-role='listview' data-inset='true' data-divider-theme='b' class='sm-crudList'>");
                result.AppendLine("<li data-role='list-divider'>");
                result.AppendLine("<div>");
                result.AppendLine("<div class='sm-crudListAddNew'>");
                if (!skipAddNew) result.AppendLine(_html.ActionLink("Plus", "Edit", controllerName, new RouteValueDictionary { { "id", 0 }, { "parentId", parentId }, { "_", DateTime.Now.Ticks } }, new Dictionary<string, object> { { "data-transition", "flip" }, { "data-icon", "plus" }, { "data-role", "button" }, { "data-iconpos", "notext" }, { "data-theme", "b" }, { "data-inline", "true" } }).ToString());
                result.AppendLine("</div>");
                result.AppendLine("<div class='sm-crudListTitle'>");
                result.AppendLine(pageTitle.ToString());
                result.AppendLine("</div>");
                result.AppendLine("</div>");
                result.AppendLine("</li>");
                foreach (var item in items)
                {
                    result.AppendLine("<li>" + _html.ActionLink(_html.Encode(item.Label), "Edit", controllerName, new RouteValueDictionary { { "id", item.Id }, { "parentId", parentId }, { "_", DateTime.Now.Ticks } }, new Dictionary<string, object> { { "data-transition", "slide" } }) + "</li>");
                }
                result.AppendLine("</ul>");
            }
            return MvcHtmlString.Create(result.ToString());
        }
        #endregion

        #region Display Helpers
        //This one handles both display for Radio and Dropdown
        public MvcHtmlString DropDownDisplay()
        {
            if (_html.ViewData.Model == null) throw new NullReferenceException(ReflectionHelper.GetCurrentContext() + " is called for a model that is null");
            if (!(_html.ViewData.Model is DropdownMvcModel)) throw new InvalidCastException(ReflectionHelper.GetCurrentContext() + " is called for a model of type diffrent from DropdownMvcModel.");

            var dropdown = (DropdownMvcModel)_html.ViewData.Model;
            return MvcHtmlString.Create(dropdown.SelectedLabel);
        }
        public MvcHtmlString BinaryFileDisplay()
        {
            if (_html.ViewData.Model is BinaryFileMvcModel) throw new InvalidCastException(ReflectionHelper.GetCurrentContext() + " is called for a model of type diffrent from BinaryFileMvcModel.");
            // ReSharper disable PossibleInvalidCastException
            return MvcHtmlString.Create((((BinaryFileMvcModel)_html.ViewData.Model)).Name);
            // ReSharper restore PossibleInvalidCastException
        }
        #endregion

        #region MvcModel Display & Editor Helpers
        public MvcHtmlString Editor(string expression)
        {
            var modelMetadata = ModelMetadata.FromStringExpression(expression, _html.ViewData);
            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);

            if (modelMetadata.Model as IMvcModelEditorTemplate != null && (modelMetadata.Model as IMvcModelEditorTemplate).GetIEditorTemplateImplemented())
            {
                return (modelMetadata.Model as IMvcModelEditorTemplate).EditorTemplate(MakeNestedHtmlHelper(modelMetadata, htmlFieldName));
            }
            return _html.Editor(expression);
        }
        public MvcHtmlString EditorFor<ValueT>(Expression<Func<ModelT, ValueT>> expression)
        {
            var html = (HtmlHelper<ModelT>) _html;
            
            var modelMetadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);

            if (modelMetadata.Model as IMvcModelEditorTemplate != null && (modelMetadata.Model as IMvcModelEditorTemplate).GetIEditorTemplateImplemented())
            {
                return (modelMetadata.Model as IMvcModelEditorTemplate).EditorTemplate(MakeNestedHtmlHelper(modelMetadata, htmlFieldName));
            }
            return html.EditorFor(expression);
        }
        public MvcHtmlString EditorForModel()
        {
            var modelMetadata = _html.ViewData.ModelMetadata;

            if (modelMetadata.Model as IMvcModelEditorTemplate != null && (modelMetadata.Model as IMvcModelEditorTemplate).GetIEditorTemplateImplemented())
            {
                return (modelMetadata.Model as IMvcModelEditorTemplate).EditorTemplate(_html);
            }
            return _html.EditorForModel();
        }

        public MvcHtmlString Display(string expression)
        {
            var modelMetadata = ModelMetadata.FromStringExpression(expression, _html.ViewData);
            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);

            if (modelMetadata.Model as IMvcModelDisplayTemplate != null && (modelMetadata.Model as IMvcModelDisplayTemplate).GetIDisplayTemplateImplemented())
            {
                return (modelMetadata.Model as IMvcModelDisplayTemplate).DisplayTemplate(MakeNestedHtmlHelper(modelMetadata, htmlFieldName));
            }
            return _html.Display(expression);
        }
        public MvcHtmlString DisplayFor<ValueT>(Expression<Func<ModelT, ValueT>> expression)
        {
            var html = (HtmlHelper<ModelT>)_html;
            
            var modelMetadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);

            if (modelMetadata.Model as IMvcModelDisplayTemplate != null && (modelMetadata.Model as IMvcModelDisplayTemplate).GetIDisplayTemplateImplemented())
            {
                return (modelMetadata.Model as IMvcModelDisplayTemplate).DisplayTemplate(MakeNestedHtmlHelper(modelMetadata, htmlFieldName));
            }
            return html.DisplayFor(expression);
        }
        public MvcHtmlString DisplayForModel()
        {
            var modelMetadata = _html.ViewData.ModelMetadata;

            if (modelMetadata.Model as IMvcModelDisplayTemplate != null && (modelMetadata.Model as IMvcModelDisplayTemplate).GetIDisplayTemplateImplemented())
            {
                return (modelMetadata.Model as IMvcModelDisplayTemplate).DisplayTemplate(_html);
            }
            return _html.DisplayForModel();
        }

        public HtmlHelper MakeNestedHtmlHelper(ModelMetadata metadata, string htmlFieldName)
        {
            var viewData = new ViewDataDictionary(_html.ViewDataContainer.ViewData)
            {
                Model = metadata.Model,
                ModelMetadata = metadata,
                TemplateInfo = new TemplateInfo
                {
                    FormattedModelValue = metadata.Model,
                    HtmlFieldPrefix = _html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName),
                }
            };
            return new HtmlHelper(new ViewContext(_html.ViewContext, _html.ViewContext.View, viewData, _html.ViewContext.TempData, _html.ViewContext.Writer), new ViewDataContainer(viewData));
        }
        #endregion

        #region Static Placeholder helpers
        public MvcHtmlString JQueryMobileHeader(string jqmVersion, bool removeiPhoneBars)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1.0' />"); 
            if (removeiPhoneBars)
            {
                sb.AppendLine("<meta content='yes' name='apple-mobile-web-app-capable'>");
                //sb.AppendLine("<link href='http://supermodel.googlecode.com/svn/SupermodelJQMobileBlankStartup.png' media='(device-width: 320px) and (device-height: 568px) and (-webkit-device-pixel-ratio: 2)' rel='apple-touch-startup-image'>");
                sb.AppendLine("<link href='" + new UrlHelper(_html.ViewContext.RequestContext).Content("~/content/images/blankstartup.png") + "' media='(device-width: 320px) and (device-height: 568px) and (-webkit-device-pixel-ratio: 2)' rel='apple-touch-startup-image'>");
            }
            sb.AppendLine("<meta http-equiv='Pragma' content='no-cache'>");
            sb.AppendLine("<meta http-equiv='Expires' content='-1'>");

            //sb.AppendLine("<link rel='stylesheet' href='http://supermodel.googlecode.com/svn/SupermodelJQMobile.css' type='text/css'/>");
            sb.AppendLine("<link rel='stylesheet' href='" + new UrlHelper(_html.ViewContext.RequestContext).Content("~/content/css/supermodelmobile.css") + "' />");

            sb.AppendLine("<link rel='stylesheet' href='http://code.jquery.com/mobile/" + jqmVersion + "/jquery.mobile-" + jqmVersion + ".min.css' />");
            sb.AppendLine("<link rel='stylesheet' type='text/css' href='http://dev.jtsage.com/cdn/simpledialog/latest/jquery.mobile.simpledialog.min.css' />");

            sb.AppendLine("<script src='http://code.jquery.com/jquery-1.8.3.min.js'></script>");
            sb.AppendLine("<script src='http://code.jquery.com/mobile/" + jqmVersion + "/jquery.mobile-" + jqmVersion + ".min.js'></script>");
            sb.AppendLine("<script src='http://dev.jtsage.com/cdn/simpledialog/latest/jquery.mobile.simpledialog2.min.js' type='text/javascript'></script>");
            return MvcHtmlString.Create(sb.ToString());
        }
        public MvcHtmlString DialogsScript(WebViewPage webPage)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<script type='text/javascript'>");
            sb.AppendLine("$(function () {");
            if (webPage.TempData.Supermodel().NextPageStartupScript != null) sb.AppendLine(webPage.TempData.Supermodel().NextPageStartupScript);
            if (webPage.TempData.Supermodel().NextPageAlertMessage != null) sb.AppendLine("alert('" +  webPage.TempData.Supermodel().NextPageAlertMessage + "');");
            if (webPage.TempData.Supermodel().NextPageModalMessage != null)
            {
                var modelDialogScript = EmbeddedResource.ReadTextFile(typeof(EmbeddedResource).Assembly, "Supermodel.EmbeddedResources.ModalDialogScript.html");
                modelDialogScript = modelDialogScript.Replace("{%Message%}", webPage.TempData.Supermodel().NextPageModalMessage);
                sb.AppendLine(modelDialogScript);
            }
            var confirmationDialogScript = EmbeddedResource.ReadTextFile(typeof(EmbeddedResource).Assembly, "Supermodel.EmbeddedResources.ConfirmationDialogScript.html");
            sb.AppendLine(confirmationDialogScript);
            sb.AppendLine("});");
            sb.AppendLine("</script>");
            return MvcHtmlString.Create(sb.ToString());
        }
        public MvcHtmlString JQueryMobileMasterLayout(WebViewPage webPage, string appName, string jqmVersion, bool removeiPhoneBars)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<title>" + appName + "</title>");
            sb.AppendLine(JQueryMobileHeader(jqmVersion, removeiPhoneBars).ToString());
            sb.AppendLine("</head> ");
            sb.AppendLine("<body>");
            sb.AppendLine("<div data-role='page' data-url='" + webPage.Request.FilePath + "'>");
            sb.AppendLine(DialogsScript(webPage).ToString());
            sb.AppendLine(webPage.RenderBody().ToString());
            sb.AppendLine("</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return MvcHtmlString.Create(sb.ToString());
        }
        #endregion

        #region Private Helpers
        //private IEnumerable<SelectListItem> GetSelectData(string name)
        //{
        //    object o = null;
        //    if (_html.ViewData != null)
        //    {
        //        o = _html.ViewData.Eval(name);
        //    }
        //    if (o == null)
        //    {
        //        throw new InvalidOperationException(
        //            String.Format(
        //                CultureInfo.CurrentCulture,
        //                @"An error occurred when trying to create the IModelBinder '{0}'. Make sure that the binder has a public parameterless constructor.",
        //                name));
        //    }
        //    var selectList = o as IEnumerable<SelectListItem>;
        //    if (selectList == null)
        //    {
        //        throw new InvalidOperationException(
        //            String.Format(
        //                CultureInfo.CurrentCulture,
        //                @"The model item passed into the dictionary is of type '{0}', but this dictionary requires a model item of type '{1}'.",
        //                name,
        //                o.GetType().FullName));
        //    }
        //    return selectList;
        //}
        #endregion

        #region HtmlHelper context
        private readonly HtmlHelper _html;
        #endregion
    }
}


            
