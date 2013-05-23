using System.Web.Mvc;

namespace Supermodel.Mvc.Extensions
{
    public class SupermodelNamespaceTempDataDictionaryExtensions
    {
       public SupermodelNamespaceTempDataDictionaryExtensions(TempDataDictionary tempData)
       {
           _tempData = tempData;
       }

       public string NextPageStartupScript
       {
           get { return _tempData["sm-startupScript"] == null ? null : _tempData["sm-startupScript"].ToString(); }
           set { _tempData["sm-startupScript"] = value; }
       }
       public string NextPageAlertMessage
       {
           get { return _tempData["sm-alertMessage"] == null ? null : _tempData["sm-alertMessage"].ToString(); }
           set { _tempData["sm-alertMessage"] = value; }
       }
       public string NextPageModalMessage
       {
           get { return _tempData["sm-modalMessage"] == null ? null : _tempData["sm-modalMessage"].ToString(); }
           set { _tempData["sm-modalMessage"] = value; }
       }

       private readonly TempDataDictionary _tempData;
    }
}
