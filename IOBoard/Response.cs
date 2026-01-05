namespace Edge.IOBoard
{
    public class Response
    {
        #region ParseStatusType enum

        public enum ParseStatusType
        {
            Valid,
            UnknownError,
            ParseError
        }

        #endregion

        public const string ResponseGetAllInputs = "@I";
        public const string ResponseSetAllOutputs = "@S";
        public const string ResponseSetIO = "@A";
        public const string ResponseSetIOManual = "@M";
        public const string ResponseGetAllUic = "@U";
        public const string ResponseGetTagData = "@D";
        public ParseStatusType ParseStatus = ParseStatusType.UnknownError;

        private string mType = string.Empty;

        public string ResponseType
        {
            get { return mType; }
        }

        public static Response ParseResponse(string pResponseString)
        {
            Response r;
            var rType = string.Empty;

            if (pResponseString.Length > 2)
            {
                rType = pResponseString.Substring(0, 2);
                rType = rType.ToUpper();
            }

            switch (rType)
            {
                case ResponseSetIO: //@A
                case ResponseSetIOManual: //@M
                    r = new ResponseSetIO();
                    break;

                case ResponseSetAllOutputs:  //@S
                    r = new Response {ParseStatus = ParseStatusType.Valid};
                    break;

                case ResponseGetAllInputs:  //@I
                    r = new ResponseGetAllInputs();
                    break;

                case ResponseGetAllUic: //@U
                    r = new ResponseGetAllUic();
                    break;

                case ResponseGetTagData: //@D
                    r = new ResponseGetTagData();
                    break;

                default:
                    r = new Response();
                    break;
            }

            r.mType = rType;
            r.UpdateFromString(pResponseString);

            return r;
        }

        public virtual void UpdateFromString(string pResponseString)
        {
        }
    }
}