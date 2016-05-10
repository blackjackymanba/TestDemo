namespace GPMGateway.Common.DataStructure
{
    public class Terminal
    {
        private BMember _bMember;

        public Terminal(int bMemberID)
        {
            _bMember = new BMember(bMemberID);
        }

        //TERMINAL_ID,SGCOMPCODE,SGOPERNO,SGSPOTCODE,BMEMBERID,SGRCVORGNO,SGTERMINALNO

        public string TerminalNo;

        public string CompanyCode;

        public string OperatorNo;

        public string SpotCode;

        public string ChargeOrganizationNo;

        public BMember BMember
        {
            get { return _bMember; }
        }
    }
}