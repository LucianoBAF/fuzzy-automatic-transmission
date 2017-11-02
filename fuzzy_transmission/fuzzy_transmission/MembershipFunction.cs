using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fuzzy_transmission
{
    class MembershipFunction
    {
        //Describes the universe of discourse, like VH (very high), M (medium), etc...
        public string MFlinguisticVariable { get; set; }

        //Three points for triangle or four for trapezium
        public float[] MFpoints { get; set; }

        //The outcome resulted from applying an input to a MF
        public float MFoutput { get; set; }


        public float Centorid()
        {
            float a = this.MFpoints[2] - this.MFpoints[1];
            float b = this.MFpoints[3] - this.MFpoints[0];
            float c = this.MFpoints[1] - this.MFpoints[0];

            return ((2 * a * c) + (a * a) + (c * b) + (a * b) + (b * b)) / (3 * (a + b)) + this.MFpoints[0];
        }

        public float Area()
        {
            float a = this.Centorid() - this.MFpoints[0];
            float b = this.MFpoints[3] - this.MFpoints[0];

            return (this.MFoutput * (b + (b - (a * this.MFoutput)))) / 2;
        }


        public MembershipFunction()
        {
            this.MFlinguisticVariable = "";
            this.MFpoints = new float[] { };
            this.MFoutput = 0;
        }

        public MembershipFunction(string mFlinguisticVariable)
        {
            this.MFlinguisticVariable = mFlinguisticVariable;
        }

        public MembershipFunction(string mFlinguisticVariable, float[] mFpoints)
        {
            this.MFlinguisticVariable = mFlinguisticVariable;
            this.MFpoints = mFpoints;
        }

        public MembershipFunction(string mFlinguisticVariable, float[] mFpoints, float mFoutput)
        {
            this.MFlinguisticVariable = mFlinguisticVariable;
            this.MFpoints = mFpoints;
            this.MFoutput = mFoutput;
        }
    }
}
