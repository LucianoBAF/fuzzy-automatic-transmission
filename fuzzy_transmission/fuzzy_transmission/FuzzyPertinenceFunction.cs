using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fuzzy_transmission
{

    class FuzzyPertinenceFunction
    {
        public string PFname { get; set; }

        //Every MF included in the pertinence function, like C (cold), N (normal), H (hot), etc...
        public List<MembershipFunction> MembershipFunctions { get; set; }

        //Describes the universe of discourse, like VH (very high), M (medium), etc...
        public string[] getMFslinguisticVariables() {
            string[] linguisticVariables = new string[this.MembershipFunctions.Count];

            for(int i = 0; i < this.MembershipFunctions.Count; i++) {
                linguisticVariables[i] = this.MembershipFunctions[i].MFlinguisticVariable;
            }

            return linguisticVariables;
        }

        public string getMFslinguisticVariable(int whichMF)
        {
            return this.MembershipFunctions[whichMF].MFlinguisticVariable;
        }

        public FuzzyPertinenceFunction()
        {
            this.PFname = "";
            this.MembershipFunctions = new List<MembershipFunction>();
        }

        public FuzzyPertinenceFunction(string pFname)
        {
            this.PFname = pFname;
        }

        public FuzzyPertinenceFunction(string pFname, List<MembershipFunction> membershipFunctions)
        {
            this.PFname = pFname;
            this.MembershipFunctions = membershipFunctions;
        }
    }


}
