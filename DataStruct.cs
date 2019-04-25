using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NS_Utilities;

namespace BikeTourImport
{
    /***************************************************************************
    SPECIFICATION: 
    CREATED:       01.05.2018
    LAST CHANGE:   01.05.2018
    ***************************************************************************/
    public class DataStruct
    {
        /***************************************************************************
        SPECIFICATION: Accessors
        CREATED:       22.07.2018
        LAST CHANGE:   22.07.2018
        ***************************************************************************/
        public int iAltitude { get { return Utils.Str2Int(ConvertNeg(Altitude)); } }

        /***************************************************************************
        SPECIFICATION: Members
        CREATED:       01.05.2018
        LAST CHANGE:   01.05.2018
        ***************************************************************************/
        public string Date           ; 
        public string Time           ; 
        public string Distance       ; 
        public string Speed          ; 
        public string HeartRate      ; 
        public string Cadence        ;
        public string Power          ; 
        public string RightPedalPower;
        public string Altitude       ; 
        public string Temperature    ;

        /***************************************************************************
        SPECIFICATION: C'tors
        CREATED:       01.05.2018
        LAST CHANGE:   20.05.2018
        ***************************************************************************/
        public DataStruct()
        {

        }

        public DataStruct( List<string> a_Data )
        {
            int i=0;
            if (a_Data.Count < 10) return;

            Date            = a_Data[i++];
            Time            = a_Data[i++];
            Distance        = string.Format("{0}",Utils.Str2Double( a_Data[i++] ) / 1000 );
            Speed           = a_Data[i++].Replace(".",",");
            HeartRate       = a_Data[i++];
            Cadence         = a_Data[i++];
            Power           = a_Data[i++];
            RightPedalPower = a_Data[i++];
            Altitude        = a_Data[i++];
            Temperature     = a_Data[i++];
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       22.07.2018
        LAST CHANGE:   17.09.2018
        ***************************************************************************/
        private string ConvertNeg( string a_Val )
        {
            if (a_Val == null) return "null";

            uint val = Utils.Str2UInt( a_Val );
            if ( val > 0x7fff ) val -= 0x10000;
            return string.Format( "{0}", (int)val );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       03.08.2018
        LAST CHANGE:   17.09.2018
        ***************************************************************************/
        private string ConvertNons( string a_Val )
        {
            if (a_Val == null) return "null";

            int val = Utils.Str2Int( a_Val );
            if ( val < 0 ) val = -1;
            return string.Format( "{0}", val );
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       01.05.2018
        LAST CHANGE:   03.08.2018
        ***************************************************************************/
        public List<string> GetLine()
        {
            List<string> ret = new List<string>();

            ret.Add(Date      );
            ret.Add(Time      );
            ret.Add(Distance  );
            ret.Add(Speed     );
            ret.Add(HeartRate );
            ret.Add( ConvertNons( Cadence ) );
            ret.Add(Power     );
            //ret.Add(RightPedalPower);
            ret.Add( ConvertNeg (Altitude) );
            ret.Add( ConvertNeg (Temperature) );

            return ret;
        }
    }
}
