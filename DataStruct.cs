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
        LAST CHANGE:   20.06.2025
        ***************************************************************************/
        public int    iAltitude { get { return Utils.Str2Int(ConvertNeg(Altitude)); } }
        public double dAltitude { get { return Utils.Str2Double(Altitude); } }
        public double dSpeed    { get { return Utils.Str2Double(Speed); } }

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
        LAST CHANGE:   20.06.2025
        ***************************************************************************/
        public DataStruct()
        {
            Date            = "";
            Time            = "";
            Distance        = "";
            Speed           = "";
            HeartRate       = "";
            Cadence         = "";
            Power           = "";
            RightPedalPower = "";
            Altitude        = "";
            Temperature     = "";
        }

        public DataStruct( List<string> a_Data )
            : this()
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

            int val = Utils.Str2Int( a_Val );
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
        LAST CHANGE:   24.06.2025
        ***************************************************************************/
        public List<string> GetLine( bool a_Fit = false )
        {
            List<string> ret = new List<string>();

            //double spd = Utils.Str2Double(Speed);
            //Speed = ( spd * 3.6 ).ToString("0.00");

            ret.Add(Date      );
            ret.Add(Time      );
            ret.Add(Distance  );
            ret.Add(Speed     );
            ret.Add(HeartRate );
            if (a_Fit)
            {
                ret.Add( Altitude );
                ret.Add( Temperature );
            }
           else
            {
                ret.Add( ConvertNons( Cadence ) );
                ret.Add(Power     );
                ret.Add( ConvertNeg( Altitude ) );
                ret.Add( ConvertNeg(Temperature) );
            }

            return ret;
        }
    }
}
