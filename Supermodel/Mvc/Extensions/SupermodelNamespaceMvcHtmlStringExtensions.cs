using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Supermodel.Mvc.Extensions
{
    public class SupermodelNamespaceMvcHtmlStringExtensions
    {
        #region Constructros
        public SupermodelNamespaceMvcHtmlStringExtensions(MvcHtmlString mvcStr)
        {
            _mvcStr = mvcStr;
        } 
        #endregion

        #region Methods
        public MvcHtmlString DisableAllControls()
        {
            return MvcHtmlString.Create(ToStringHandleNull().Supermodel().DisableAllControls());
        }
        public MvcHtmlString DisableAllControlsIf(bool condition)
        {
            return MvcHtmlString.Create(ToStringHandleNull().Supermodel().DisableAllControlsIf(condition));
        }
        public MvcHtmlString MakeRequired()
        {
            return MvcHtmlString.Create(ToStringHandleNull().Supermodel().MakeRequired());
        }
        public MvcHtmlString RemoveRowTags()
        {
            var str = ToStringHandleNull();
            str = str.Replace("</tr>", "");
            str = Regex.Replace(str, "<tr.*>", "");
            return MvcHtmlString.Create(str);
        }
        public string ToStringHandleNull()
        {
            return _mvcStr != null ? _mvcStr.ToString() : "";
        }
        #endregion

        #region Private Context
        private readonly MvcHtmlString _mvcStr; 
        #endregion
    
    }
}
