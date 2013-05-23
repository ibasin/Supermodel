using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Newtonsoft.Json;
using ReflectionMapper;
using Supermodel.DDD.Models.Domain;

namespace Supermodel.DDD.Models.View.Mvc.UIComponents
{
    public class BinaryFileMvcModel : IComparable, IMvcModelModelBinder, IMvcModelEditorTemplate, ICustomMapper
    {
        public void Empty()
        {
            Name = null;
            BinaryContent = null;
        }
        
        public string Name { get; set; }
        [JsonIgnore, NotRMapped] public string Extension { get { return Path.GetExtension(Name);} }
        [JsonIgnore, NotRMapped] public string FileNameWithoutExtension { get { return Path.GetFileNameWithoutExtension(Name); } }
        public byte[] BinaryContent { get; set; }

        [JsonIgnore]
        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Name) && (BinaryContent == null || BinaryContent.Length == 0); } 
        }

        public int CompareTo(object obj)
        {
            var typedObj = (BinaryFileMvcModel)obj;
            if (Name == null) return 0; //if we are an empty object, we say it equals b/c then we do not override db value
            var result = string.CompareOrdinal(Name, typedObj.Name);
            return result != 0 ? result : BinaryContent.GetHashCode().CompareTo(typedObj.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as BinaryFileMvcModel);
        }

        public bool Equals(BinaryFileMvcModel other)
        {
            return other != null &&
                   (this.Name == null && other.Name == null ||
                    this.Name != null && this.Name.Equals(other.Name)) &&
                   (this.BinaryContent == null && other.BinaryContent == null ||
                    this.BinaryContent != null && this.BinaryContent.SequenceEqual(other.BinaryContent));
        }

        public override int GetHashCode()
        {
            // http://stackoverflow.com/a/263416/39396
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                if (this.Name != null)
                    hash = hash * 23 + this.Name.GetHashCode();
                if (this.BinaryContent != null)
                    hash = hash * 23 + this.BinaryContent.GetHashCode();
                return hash;
            }
        }

        public virtual object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext == null) throw new ArgumentNullException("controllerContext");
            if (bindingContext == null) throw new ArgumentNullException("bindingContext");

            var rawFile = controllerContext.HttpContext.Request.Files[bindingContext.ModelName];

            var file = (BinaryFileMvcModel)ReflectionHelper.CreateType(GetType());
            if (rawFile == null || rawFile.ContentLength == 0 || string.IsNullOrEmpty(rawFile.FileName))
            {
                var originalValue = (BinaryFileMvcModel)bindingContext.Model;
                if (originalValue.IsEmpty)
                {
                    file.Name = null;
                    file.BinaryContent = null;
                    if (bindingContext.ModelMetadata.IsRequired) bindingContext.ModelState.AddModelError(bindingContext.ModelName, string.Format("The field {0} is required", bindingContext.ModelMetadata.DisplayName ?? bindingContext.ModelMetadata.PropertyName));
                }
                else
                {
                    file = originalValue;
                }
            }
            else
            {
                file.Name = Path.GetFileName(rawFile.FileName);
                file.BinaryContent = new byte[rawFile.ContentLength];
                rawFile.InputStream.Read(file.BinaryContent, 0, rawFile.ContentLength);
            }
            return file;
        }

        public virtual bool GetIEditorTemplateImplemented() { return true; }
        public virtual MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
        {
            if (!(html.ViewData.Model is BinaryFileMvcModel)) throw new InvalidCastException(ReflectionHelper.GetCurrentContext() + " is called for a model of type diffrent from BinaryFileFormModel.");
            var model = ((BinaryFileMvcModel)html.ViewData.Model);

            var fileInputName = html.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix;
            var fileInputId = fileInputName.Replace(".", "_");

            var result = new StringBuilder();
            result.AppendLine(string.Format("<input type=\"file\" name=\"{0}\" id=\"{1}\" />", fileInputName, fileInputId));

            if (!string.IsNullOrEmpty(model.Name) && html.ViewDataContainer.ViewData.ModelState.IsValid)
            {
                var route = new { id = html.ViewContext.RouteData.Values["id"], parentId = HttpContext.Current.Request.QueryString["parentId"], pn = html.ViewData.ModelMetadata.PropertyName };
                // ReSharper disable Mvc.ActionNotResolved
                result.AppendLine("<br />" + html.ActionLink(model.Name, "GetBinaryFile", route));
                // ReSharper restore Mvc.ActionNotResolved
                if (!html.ViewData.ModelMetadata.IsRequired)
                {
                    // ReSharper disable Mvc.ActionNotResolved
                    result.AppendLine("<br />" + html.ActionLink("Delete File", "DeleteBinaryFile", route, new Dictionary<string, object> { { "data-sm-ConfirmMsg", "This will permanently delete the file. Are you sure?" } }));
                    // ReSharper restore Mvc.ActionNotResolved
                }
            }
            return MvcHtmlString.Create(result.ToString());
        }

        public virtual object MapFromObjectCustom(object obj, Type objType)
        {
            if (objType != typeof(BinaryFile)) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1}", GetType().Name, objType.Name));

            var binaryFileObj = (BinaryFile)obj;
            Name = (binaryFileObj == null) ? "" : binaryFileObj.Name;
            BinaryContent = (binaryFileObj == null) ? new byte[0] : binaryFileObj.BinaryContent;

            return this;
        }
        public virtual object MapToObjectCustom(object obj, Type objType)
        {
            if (objType != typeof(BinaryFile)) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1}", GetType().Name, objType.Name));

            var binaryFileObject = new BinaryFile { Name = Name, BinaryContent = BinaryContent };
            return binaryFileObject;
        }
    }
}
