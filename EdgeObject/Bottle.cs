using Edge.EdgeSecurity;
using Edge.EdgeUtility;

namespace Edge.EdgeObject
{
    public class Bottle : ISignableObject
    {
        #region Bottle Serial Number Components

        private string _mBatch;
        private string _mFillCode;
        private string _mPartNumber;
        private string _mRevision;
        private string _mSerialNum;
                                                                                                               
        public string FillCode
        {
            get { return _mFillCode; }
            set { _mFillCode = value; }
        }

        public string Batch
        {
            get { return _mBatch; }
            set { _mBatch = value; }
        }

        public string PartNumber
        {
            get { return _mPartNumber; }
            set { _mPartNumber = value; }
        }

        public string Revision
        {
            get { return _mRevision; }
            set { _mRevision = value; }
        }

        public string SerialNum
        {
            get { return _mSerialNum; }
            set { _mSerialNum = value; }
        }

        #endregion

        public Bottle(string pFillCode, string pBatch, string pPartNumber, string pRevision, string pSerialNum)
        {
            _mFillCode = pFillCode;
            _mBatch = pBatch;
            _mPartNumber = pPartNumber;
            _mRevision = pRevision;
            _mSerialNum = pSerialNum;
        }

        public string BottleIdentifierRawString
        {
            get { return _mFillCode + _mBatch + _mPartNumber + _mRevision + _mSerialNum; }
        }

        #region ISignableObject Members

        public byte[] DataForSigning
        {
            get { return Utility.StringToByteArray(BottleIdentifierRawString); }
        }

        public byte[] DataSignature { get; set; }

        #endregion
    }
}