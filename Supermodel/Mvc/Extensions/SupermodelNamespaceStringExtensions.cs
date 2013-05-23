namespace Supermodel.Mvc.Extensions
{
    public class SupermodelNamespaceStringExtensions
    {
        #region Constructros
        public SupermodelNamespaceStringExtensions(string str)
        {
            _str = str;
        } 
        #endregion

        #region Methods
        public string DisableAllControls()
        {
            return _str.Replace("<input ", "<input disabled='disabled' ").Replace("<textarea ", "<textarea disabled='disabled' ").Replace("<select ", "<select disabled='disabled' ");
        }

        public string DisableAllControlsIf(bool condition)
        {
            return condition ? DisableAllControls() : _str;
        }

        public string MakeRequired()
        {
            return _str.Replace("class=\"", "class=\"required ");
        }

        public string ToStringHandleNull()
        {
            return _str ?? "";
        }
        #endregion

        #region Private Context
        private readonly string _str; 
        #endregion
    }
}
