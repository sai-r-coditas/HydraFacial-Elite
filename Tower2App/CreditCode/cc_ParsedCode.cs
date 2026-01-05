using System;


namespace Edge.Tower2.UI
{
    //Helper class for storing parsed code's information
    class ParsedCode
    {   
        public enum CodeStatus
        {
            InvalidCode,
            IncorrectCodeLength,
            ValidCodeMasterUnlock,
            ValidCodeDays,
            ValidCodeContinuousMode
        };
        
        private CodeStatus m_codeStatus = CodeStatus.InvalidCode;
        private Int32 m_daysUnlocked = 0;
        private String m_code = "";
        private string m_sequence = "";

        public ParsedCode(CodeStatus codeStatus, Int32 days, String code, String sequence)
        {
            m_codeStatus = codeStatus;
            m_daysUnlocked = days;
            m_code = code;
            m_sequence = sequence;
        }

        public CodeStatus getParsedCodeStatus()
        {
            return m_codeStatus;
        }

        public Int32 getDaysUnlocked()
        {
            return m_daysUnlocked;
        }

        public String getCode()
        {
            return m_code;
        }

        public string getSequence()
        {
            return m_sequence;
        }
    }
}
