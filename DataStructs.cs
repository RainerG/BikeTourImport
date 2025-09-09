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
    LAST CHANGE:   01.09.2025
    ***************************************************************************/
    public class Record
    {
        /***************************************************************************
        SPECIFICATION: Accessors
        CREATED:       22.07.2018
        LAST CHANGE:   09.09.2025
        ***************************************************************************/
        public int    iAltitude { get { return Utils.Str2Int(ConvertNeg( Altitude ) ); } }
        public double dAltitude { get { return Utils.Str2Double( Altitude ); } }
        public double dTemper   { get { return Utils.Str2Double( Temperature ); } }
        public double dHrtRate  { get { return Utils.Str2Double( HeartRate ); } }
        public double dSpeed    { get { return Utils.Str2Double( Speed ); } }

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
        public Record()
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

        public Record( List<string> a_Data )
            : this()
        {
            int i=0;
            if (a_Data.Count < 10) return;

            Date            = a_Data[i++];
            Time            = a_Data[i++];
            Distance        = string.Format("{0}",Utils.Str2Double( a_Data[i++] ) / 1000 );
            Speed           = a_Data[i++].Replace(".",",");
            Cadence         = a_Data[i++];
            HeartRate       = a_Data[i++];
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
        LAST CHANGE:   06.09.2025
        ***************************************************************************/
        public List<string> GetLine( bool a_Fit = false )
        {
            List<string> ret = new List<string>();

            //double spd = Utils.Str2Double(Speed);
            //Speed = ( spd * 3.6 ).ToString("0.00");

            ret.Add( Date      );
            ret.Add( Time      );
            ret.Add( Distance  );
            ret.Add( Speed     );
            if (a_Fit)
            {
                ret.Add( Cadence   );
                ret.Add( HeartRate );
                ret.Add( Altitude  );
                ret.Add( Temperature );
            }
           else
            {
                ret.Add( HeartRate );
                ret.Add( ConvertNons( Cadence ) );
                ret.Add(Power     );
                ret.Add( ConvertNeg( Altitude ) );
                ret.Add( ConvertNeg(Temperature) );
            }

            return ret;
        }
    }

    /***************************************************************************
    SPECIFICATION: 
    CREATED:       01.09.2025
    LAST CHANGE:   01.09.2025
    ***************************************************************************/
    public class Lap
    {
        /***************************************************************************
        SPECIFICATION: Accessors
        CREATED:       01.09.2025
        LAST CHANGE:   01.09.2025
        ***************************************************************************/
        public DateTime TimeStart { get { return StartTime; } }
        public DateTime TimeStop  { get { return StartTime + TotTime; } }

        /***************************************************************************
        SPECIFICATION: Members
        CREATED:       01.09.2025
        LAST CHANGE:   03.09.2025
        ***************************************************************************/
        public  ushort   TotAscent   ;
        public  ushort   TotDescent  ;
        public  float    TotDistance ;
        public  byte     AvgHrtRate  ;
        public  byte     MaxHrtRate  ;
        public  byte     AvgCadence  ;
        public  byte     MaxCadence  ;
        public  byte     Sport       ;
        public  sbyte    AvgTemp     ;
        public  sbyte    MaxTemp     ;
        private DateTime StartTime   ;
        public  TimeSpan TotTime     ;

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       01.09.2025
        LAST CHANGE:   03.09.2025
        ***************************************************************************/
        public Lap()
        {
            TotAscent   = 0; 
            TotDescent  = 0;
            TotDistance = 0.0f;
            AvgHrtRate  = 0;
            MaxHrtRate  = 0;
            AvgCadence  = 0;
            MaxCadence  = 0;
            AvgTemp     = 0;
            MaxTemp     = 0;
            Sport       = 0;

            StartTime   = new DateTime();
            TotTime     = new TimeSpan();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       01.09.2025
        LAST CHANGE:   01.09.2025
        ***************************************************************************/
        public void SetStartTime( uint a_Time )
        {
            System.DateTime fitEpoch = new System.DateTime( 1989, 12, 31, 2, 0, 0, DateTimeKind.Unspecified );
            fitEpoch  = fitEpoch.AddSeconds( (double)a_Time );
            StartTime = fitEpoch; //.ToString( "yyyy.mm.dd hh:mm:ss");
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       01.09.2025
        LAST CHANGE:   01.09.2025
        ***************************************************************************/
        public void SetTotTime( float a_Time )
        {
            TimeSpan tottm = new TimeSpan( 0,0, (int)a_Time );
            TotTime = tottm; //.ToString();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       01.09.2025
        LAST CHANGE:   01.09.2025
        ***************************************************************************/
        private DateTime CalcTime( uint a_Secs )
        {
            DateTime fitEpoch = new DateTime( 1989, 12, 31, 0, 0, 0, DateTimeKind.Local );
            fitEpoch = fitEpoch.AddSeconds( (double)a_Secs );
            return fitEpoch;
        }
    }
}
