using System.Configuration;

namespace Supermodel.Mvc
{
    public static class Config
    {
        public static int UseInitializerIdx 
        { 
            get
            {
                var strVal = ConfigurationManager.AppSettings["Supermodel.UseInitializerIdx"];
                return strVal == null ? 0 : int.Parse(strVal);
            } 
        }
    }
}
