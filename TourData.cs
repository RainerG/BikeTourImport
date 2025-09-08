using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dynastream.Fit;
using NS_AppConfig;
using NS_UserOut;
using NS_Utilities;
using NS_WordExcel;
using Excel = Microsoft.Office.Interop.Excel; 

namespace BikeTourImport
{
    /***************************************************************************
    SPECIFICATION: 
    CREATED:       01.05.2018
    LAST CHANGE:   01.05.2018
    ***************************************************************************/
    class TourData
    {
        /***************************************************************************
        SPECIFICATION: Accessors
        CREATED:       06.09.2025
        LAST CHANGE:   08.09.2025
        ***************************************************************************/
        public OutputList OutList  { get { return m_OutList; } }
        public string     ExpFilNm { set { m_ExpFilNm = value; } }

        /***************************************************************************
        SPECIFICATION: Members
        CREATED:       01.05.2018
        LAST CHANGE:   06.09.2025
        ***************************************************************************/
        private int               m_LineNr;
        private List<Record>      m_Recs;
        private Lap               m_Lap;
        private List<string>      m_Cols;
        private OutputList        m_OutList;
        private WordExcelExport   m_XlExp;
        private Statistics        m_Statcs;
        private UserRichTextBox   m_RTB;
        private Preferences       m_Prefs;
        private string            m_ExpFilNm;

        /***************************************************************************
        SPECIFICATION: C'tor
        CREATED:       01.05.2018
        LAST CHANGE:   08.09.2025
        ***************************************************************************/
        public TourData( UserRichTextBox a_RTB, Preferences a_Prefs )
        {
            m_RTB      = a_RTB;
            m_LineNr   = 0;
            m_Lap      = null;
            m_Recs     = new List<Record>();
            m_OutList  = new OutputList();
            m_XlExp    = m_OutList.WdXlExport;
            m_Statcs   = new Statistics();
            m_Prefs    = a_Prefs;
            m_ExpFilNm = "Tour";

            List<NumFormat> frmts = m_XlExp.NumFrmts;
            frmts.Add( new NumFormat( 1,"yyyy.mm.dd") );
            frmts.Add( new NumFormat( 2,"hh:mm:ss") );
            frmts.Add( new NumFormat( 3,"@") );
            frmts.Add( new NumFormat( 4,"@") );
            frmts.Add( new NumFormat( 5,"@") );
            frmts.Add( new NumFormat( 6,"@") );
            frmts.Add( new NumFormat( 7,"@") );
            frmts.Add( new NumFormat( 8,"@") );

            List<ChartFormat> chrts = m_XlExp.ChrtFrmts;
            chrts.Add( new ChartFormat( 2, 3, Excel.XlRgbColor.rgbGreen ));       // Distance
            chrts.Add( new ChartFormat( 2, 7, Excel.XlRgbColor.rgbBrown ));       // Altitude
            chrts.Add( new ChartFormat( 2, 6, Excel.XlRgbColor.rgbBlueViolet ));  // Heart rate
            chrts.Add( new ChartFormat( 2, 4, Excel.XlRgbColor.rgbViolet ));      // Speed
            chrts.Add( new ChartFormat( 2, 5, Excel.XlRgbColor.rgbBlueViolet ));  // Cadence
            chrts.Add( new ChartFormat( 2, 8, Excel.XlRgbColor.rgbOrange ));      // Temp

            //chrts.Add( new ChartFormat( 3, 2, Excel.XlRgbColor.rgbGreen ));       // Time
            //chrts.Add( new ChartFormat( 3, 7, Excel.XlRgbColor.rgbBrown ));       // Altitude
            //chrts.Add( new ChartFormat( 3, 6, Excel.XlRgbColor.rgbBlueViolet ));  // Heart rate
            //chrts.Add( new ChartFormat( 3, 4, Excel.XlRgbColor.rgbViolet ));      // Speed
            //chrts.Add( new ChartFormat( 3, 5, Excel.XlRgbColor.rgbBlueViolet ));  // Cadence
            //chrts.Add( new ChartFormat( 3, 8, Excel.XlRgbColor.rgbOrange ));      // Temp

            //chrts.Add( new ChartFormat( "Time", "Dist"  , Excel.XlRgbColor.rgbGreen ));      
            //chrts.Add( new ChartFormat( "Time", "Alti"  , Excel.XlRgbColor.rgbBrown ));      
            //chrts.Add( new ChartFormat( "Time", "Heart" , Excel.XlRgbColor.rgbBlueViolet )); 
            //chrts.Add( new ChartFormat( "Time", "Speed" , Excel.XlRgbColor.rgbViolet ));     
            //chrts.Add( new ChartFormat( "Time", "Caden" , Excel.XlRgbColor.rgbBlueViolet )); 
            //chrts.Add( new ChartFormat( "Time", "Temp"  , Excel.XlRgbColor.rgbOrange ));     

            //chrts.Add( new ChartFormat( "Dist", "Time"  , Excel.XlRgbColor.rgbGreen ));      
            //chrts.Add( new ChartFormat( "Dist", "Alti"  , Excel.XlRgbColor.rgbBrown ));      
            //chrts.Add( new ChartFormat( "Dist", "Heart" , Excel.XlRgbColor.rgbBlueViolet )); 
            //chrts.Add( new ChartFormat( "Dist", "Speed" , Excel.XlRgbColor.rgbViolet ));     
            //chrts.Add( new ChartFormat( "Dist", "Caden" , Excel.XlRgbColor.rgbBlueViolet )); 
            //chrts.Add( new ChartFormat( "Dist", "Temp"  , Excel.XlRgbColor.rgbOrange ));     

        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       14.05.2018
        LAST CHANGE:   14.05.2018
        ***************************************************************************/
        public void Serialize( ref AppSettings a_Conf )
        {
            if( a_Conf.IsReading )
            {
            }
            else
            {
            }

            m_OutList.Serialize( ref a_Conf );
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       01.05.2018
        LAST CHANGE:   07.09.2025
        ***************************************************************************/
        public void Import( string a_Fname, bool a_Fit = false )
        {
            StreamReader rdr = null;
            string sectname  = "";
            m_Recs.Clear();
            m_LineNr = 0;
            System.DateTime lasttime = new System.DateTime();

            try
            {
                if ( ! System.IO.File.Exists( a_Fname ) ) return;

                string ext = Utils.GetExtension( a_Fname ).ToLower();

                switch( ext )
                {
                    case "fit":
                        using( FileStream fitFile = new FileStream( a_Fname, FileMode.Open ) )
                        {
                            Decode decode = new Decode();
                            MesgBroadcaster broadcaster = new MesgBroadcaster();

                            m_Cols = new List<string>();
                            m_Cols.Add( "Date" );
                            m_Cols.Add( "Time" );
                            m_Cols.Add( "Distance" );
                            m_Cols.Add( "Speed" );
                            m_Cols.Add( "Cadence" );
                            m_Cols.Add( "HeartRate" );
                            m_Cols.Add( "Altitude" );
                            m_Cols.Add( "Temperature" );

                            broadcaster.MesgEvent += ( sender, e ) =>
                            {
                                #if false
                                List<Field> flds = (List<Field>)e.mesg.Fields;
                                List<string> fds = new List<string>();
                                foreach( Field f in flds ) fds.Add( f.Name );
                                System.IO.File.WriteAllLines( "d:\\tmp\\test.txt", fds );
                                #endif

                                sectname = e.mesg.Name;

                                switch( sectname.ToLower() )
                                {
                                    case "unknown":
                                        break;

                                    case "split":
                                        var totelpsdtm  = e.mesg.GetFieldValue ( "TotalElapsedTime" );
                                        var tottmrtm    = e.mesg.GetFieldValue ( "TotalTimerTime" );
                                        var totdist     = e.mesg.GetFieldValue ( "TotalDistance" );
                                        var avgspeed    = e.mesg.GetFieldValue ( "AvgSpeed" );
                                        var starttm     = e.mesg.GetFieldValue ( "StartTime" );
                                        var strtposlat  = e.mesg.GetFieldValue ( "StartPositionLat" );
                                        var strtposlon  = e.mesg.GetFieldValue ( "StartPositionLong" );
                                        var maxspeed    = e.mesg.GetFieldValue ( "MaxSpeed" );
                                        var avgvertspd  = e.mesg.GetFieldValue ( "AvgVertSpeed" );
                                        var endtime     = e.mesg.GetFieldValue ( "EndTime" );
                                        var totcalories = e.mesg.GetFieldValue ( "TotalCalories" );
                                        var startelev   = e.mesg.GetFieldValue ( "StartElevation" );
                                        var msgidx      = e.mesg.GetFieldValue ( "MessageIndex" );
                                        var totascnt    = e.mesg.GetFieldValue ( "TotalAscent" );
                                        var totdescnt   = e.mesg.GetFieldValue ( "TotalDescent" );
                                        var splittype   = e.mesg.GetFieldValue ( "SplitType" );
                                        break;

                                    case "splitsummary":
                                            tottmrtm    = e.mesg.GetFieldValue ( "TotalTimerTime" );
                                            msgidx      = e.mesg.GetFieldValue ( "MessageIndex" );
                                        var numsplts    = e.mesg.GetFieldValue ( "NumSplits" );
                                        var splttype    = e.mesg.GetFieldValue ( "SplitType" );
                                        break;

                                    case "timeinzone":
                                        var timestmp       = e.mesg.GetFieldValue ( "Timestamp" );
                                        var timeinhrzn     = e.mesg.GetFieldValue ( "TimeInHrZone" );
                                        var timeinpwrzn    = e.mesg.GetFieldValue ( "TimeInPowerZone" );
                                        var powznhgbound   = e.mesg.GetFieldValue ( "PowerZoneHighBoundary" );
                                        var refmsg         = e.mesg.GetFieldValue ( "ReferenceMesg" );
                                        var refidx         = e.mesg.GetFieldValue ( "ReferenceIndex" );
                                        var hrunhghbound   = e.mesg.GetFieldValue ( "HrZoneHighBoundary" );
                                        var hrcalctp       = e.mesg.GetFieldValue ( "HrCalcType" );
                                        var maxheartrate   = e.mesg.GetFieldValue ( "MaxHeartRate" );
                                        var restnghrtrate  = e.mesg.GetFieldValue ( "RestingHeartRate" );
                                        break;

                                    case "timestampcorrelation":
                                            timestmp       = e.mesg.GetFieldValue ( "Timestamp" );
                                        var systimestmp    = e.mesg.GetFieldValue ( "SystemTimestamp" );
                                        var loctimestmp    = e.mesg.GetFieldValue ( "LocalTimestamp" );
                                        break;

                                    case "fileid":
                                        var serial    = e.mesg.GetFieldValue ( "SerialNumber" );
                                        var time      = e.mesg.GetFieldValue ( "TimeCreated" );
                                        var manufact  = e.mesg.GetFieldValue ( "Manufacturer" );
                                        var product   = e.mesg.GetFieldValue ( "Product" );
                                        var type      = e.mesg.GetFieldValue ( "Type" );
                                        break;

                                    case "filecreator":
                                        var swvers    = e.mesg.GetFieldValue ( "SoftwareVersion" );
                                        break;

                                    case "activity":
                                        var acttype   = e.mesg.GetFieldValue( "Type" );

                                            serial    = e.mesg.GetFieldValue ( "SerialNumber" );
                                            time      = e.mesg.GetFieldValue ( "TimeCreated" );
                                        var distance  = e.mesg.GetFieldValue ( "distance" );
                                        var speed     = e.mesg.GetFieldValue ( "speed" );
                                        var altitude  = e.mesg.GetFieldValue ( "altitude" );
                                        var heartRate = e.mesg.GetFieldValue ( "heart_rate" );
                                        break;

                                    case "session":  // a lot more are available
                                            timestmp    = e.mesg.GetFieldValue ( "Timestamp" );
                                            starttm     = e.mesg.GetFieldValue ( "StartTime" );
                                        var totdistance = e.mesg.GetFieldValue ( "TotalDistance" );
                                        var totascent   = e.mesg.GetFieldValue ( "TotalAscent" );
                                        var totdescent  = e.mesg.GetFieldValue ( "TotalDescent" );
                                        var mintemp     = e.mesg.GetFieldValue ( "MinTemperature" );
                                        var avgtemp     = e.mesg.GetFieldValue ( "AvgTemperature" );
                                        var maxtemp     = e.mesg.GetFieldValue ( "MaxTemperature" );
                                        break;

                                    case "lap":
                                            timestmp    = e.mesg.GetFieldValue ( "Timestamp" );
                                            starttm     = e.mesg.GetFieldValue ( "StartTime" );
                                            strtposlat  = e.mesg.GetFieldValue ( "StartPositionLat" );
                                            strtposlon  = e.mesg.GetFieldValue ( "StartPositionLong" );
                                        var endposlat   = e.mesg.GetFieldValue ( "EndPositionLat" );
                                        var endposlon   = e.mesg.GetFieldValue ( "EndPositionLong" );
                                            totelpsdtm  = e.mesg.GetFieldValue ( "TotalElapsedTime" );
                                            tottmrtm    = e.mesg.GetFieldValue ( "TotalTimerTime" );
                                            totdist     = e.mesg.GetFieldValue ( "TotalDistance" );
                                        var totcycles   = e.mesg.GetFieldValue ( "TotalCycles" );
                                        var avglftpowph = e.mesg.GetFieldValue ( "AvgLeftPowerPhase" );
                                        var avglftpowpp = e.mesg.GetFieldValue ( "AvgLeftPowerPhasePeak" );
                                        var avgrhtpowph = e.mesg.GetFieldValue ( "AvgRightPowerPhase" );
                                        var avgrhtpowpp = e.mesg.GetFieldValue ( "AvgRightPowerPhasePeak" );
                                        var avgpowpos   = e.mesg.GetFieldValue ( "AvgPowerPosition" );
                                        var maxpowpos   = e.mesg.GetFieldValue ( "MaxPowerPosition" );
                                        var enhavgspd   = e.mesg.GetFieldValue ( "EnhancedAvgSpeed" );
                                        var enhmaxspd   = e.mesg.GetFieldValue ( "EnhancedMaxSpeed" );
                                            msgidx      = e.mesg.GetFieldValue ( "MessageIndex" );
                                        var totcals     = e.mesg.GetFieldValue ( "TotalCalories" );
                                            totascnt    = e.mesg.GetFieldValue ( "TotalAscent" );
                                            totdescnt   = e.mesg.GetFieldValue ( "TotalDescent" );
                                        var evnt        = e.mesg.GetFieldValue ( "Event" );
                                        var evttype     = e.mesg.GetFieldValue ( "EventType" );
                                        var avghrtrate  = e.mesg.GetFieldValue ( "AvgHeartRate" );
                                        var maxhrtrate  = e.mesg.GetFieldValue ( "MaxHeartRate" );
                                        var avgcadence  = e.mesg.GetFieldValue ( "AvgCadence" );
                                        var maxcadence  = e.mesg.GetFieldValue ( "MaxCadence" );
                                        var laptrigger  = e.mesg.GetFieldValue ( "LapTrigger" );
                                        var sport       = e.mesg.GetFieldValue ( "Sport" );
                                        var subsport    = e.mesg.GetFieldValue ( "SubSport" );
                                            avgtemp     = e.mesg.GetFieldValue ( "AvgTemperature" );
                                            maxtemp     = e.mesg.GetFieldValue ( "MaxTemperature" );
                                        var avgfrctcad  = e.mesg.GetFieldValue ( "AvgFractionalCadence" );
                                        var maxfrctcad  = e.mesg.GetFieldValue ( "MaxFractionalCadence" );
                                        var avgcadpos   = e.mesg.GetFieldValue ( "AvgCadencePosition" );
                                        var maxcadpos   = e.mesg.GetFieldValue ( "MaxCadencePosition" );
                                            mintemp     = e.mesg.GetFieldValue ( "MinTemperature" );

                                        m_Lap = new Lap();
                                        m_Lap.TotAscent   = (ushort)totascnt  ;
                                        m_Lap.TotDescent  = (ushort)totdescnt ;
                                        m_Lap.TotDistance = (float)totdist / 1000.0f;
                                        m_Lap.MaxHrtRate  = (byte)RetVal( maxhrtrate, "b" );
                                        m_Lap.AvgHrtRate  = (byte)RetVal( avghrtrate, "b" );
                                        m_Lap.MaxCadence  = (byte)RetVal( maxcadence, "b" ); 
                                        m_Lap.AvgCadence  = (byte)RetVal( avgcadence, "b" );
                                        m_Lap.MaxTemp     = (sbyte)RetVal( maxtemp  , "sb");
                                        m_Lap.AvgTemp     = (sbyte)RetVal( avgtemp  , "sb");
                                        m_Lap.Sport       = (byte) RetVal( sport    , "b" );
                                        m_Lap.SetStartTime( (uint) starttm );
                                        m_Lap.SetTotTime  ( (float)totelpsdtm );

                                        lasttime = m_Lap.TimeStart;
                                        break;

                                    case "event":
                                            timestmp  = e.mesg.GetFieldValue ( "Timestamp" );
                                        var dat       = e.mesg.GetFieldValue ( "Data" );
                                            evnt      = e.mesg.GetFieldValue ( "Event" );
                                        var evnttype  = e.mesg.GetFieldValue ( "EventType" );
                                        var evntgrp   = e.mesg.GetFieldValue ( "EventGroup" );
                                        break;

                                    case "record":
                                            timestmp    = e.mesg.GetFieldValue ( "Timestamp" );
                                            distance    = e.mesg.GetFieldValue ( "Distance" );       
                                        var poslat      = e.mesg.GetFieldValue ( "PositionLat" );       
                                        var poslong     = e.mesg.GetFieldValue ( "PositionLong" );       
                                        var enhaltitude = e.mesg.GetFieldValue ( "EnhancedAltitude" );
                                        var hrtrate     = e.mesg.GetFieldValue ( "HeartRate" );
                                        var cadence     = e.mesg.GetFieldValue ( "Cadence" );
                                        var frctCdnce   = e.mesg.GetFieldValue ( "FractionalCadence" );
                                        var enhspeed    = e.mesg.GetFieldValue ( "EnhancedSpeed" );       
                                        var temperature = e.mesg.GetFieldValue ( "Temperature" );

                                        Record ds = new Record();

                                        System.DateTime dt = new Dynastream.Fit.DateTime((uint)timestmp).GetDateTime();  dt = dt.AddHours(2);
                                        TimeSpan        ts = dt - lasttime;
                                        if ( ts.Seconds < m_Prefs.MinSmpleInt && dt != lasttime ) break;

                                        lasttime        = dt;
                                        ds.Date         = string.Format( "{0}", dt.ToString( "yyyy-MM-dd" ) );
                                        ds.Time         = string.Format( "{0}", dt.ToString( "HH:mm:ss" ) );
                                        ds.Distance     = string.Format( "{0}", distance );
                                        ds.Speed        = string.Format( "{0}", enhspeed );
                                        ds.Cadence      = string.Format( "{0}", cadence  );
                                        ds.HeartRate    = string.Format( "{0}", hrtrate  );
                                        ds.Altitude     = string.Format( "{0}", enhaltitude );
                                        ds.Temperature  = string.Format( "{0}", temperature );

                                        m_Recs.Add( ds );
                                        break;

                                    case "deviceinfo":
                                            timestmp  = e.mesg.GetFieldValue ( "Timestamp" );       
                                        var cumopertm = e.mesg.GetFieldValue ( "CumOperatingTime" );       
                                        var serialnr  = e.mesg.GetFieldValue ( "SerialNumber" );       
                                        var manufct   = e.mesg.GetFieldValue ( "Manufacturer" );       
                                        var poduct    = e.mesg.GetFieldValue ( "Product" );       
                                            swvers    = e.mesg.GetFieldValue ( "SoftwareVersion" );       
                                        var devidx    = e.mesg.GetFieldValue ( "DeviceIndex" );       
                                        var antntwk   = e.mesg.GetFieldValue ( "AntNetwork" );       
                                        var srcetyp   = e.mesg.GetFieldValue ( "SourceType" );       
                                        break;

                                    case "devicesettings":
                                        var utcoffs       = e.mesg.GetFieldValue ( "UtcOffset" );
                                        var timeoffs      = e.mesg.GetFieldValue ( "TimeOffset" );
                                        var atosynmnstps  = e.mesg.GetFieldValue ( "AutosyncMinSteps" );
                                        var atosyntmmintm = e.mesg.GetFieldValue ( "AutosyncMinTime" );
                                        var acttmzone     = e.mesg.GetFieldValue ( "ActiveTimeZone" );
                                        var timemode      = e.mesg.GetFieldValue ( "TimeMode" );
                                        var timzoneoffs   = e.mesg.GetFieldValue ( "TimeZoneOffset" );
                                        var bcklghtmde    = e.mesg.GetFieldValue ( "BacklightMode" );
                                        var acttrackenab  = e.mesg.GetFieldValue ( "ActivityTrackerEnabled" );
                                        var datemode      = e.mesg.GetFieldValue ( "DateMode" );
                                        break;

                                    case "userprofile":
                                        var fndlynme      = e.mesg.GetFieldValue ( "FriendlyName" ); 
                                        var waketime      = e.mesg.GetFieldValue ( "WakeTime" );
                                        var sleeptime     = e.mesg.GetFieldValue ( "SleepTime" );
                                        var weight        = e.mesg.GetFieldValue ( "Weight" );
                                        var gender        = e.mesg.GetFieldValue ( "Gender" );
                                        var age           = e.mesg.GetFieldValue ( "Age" );
                                        var height        = e.mesg.GetFieldValue ( "Height" );
                                        var language      = e.mesg.GetFieldValue ( "Language" );
                                        var elevsettngs   = e.mesg.GetFieldValue ( "ElevSetting" );
                                        var weightsttngs  = e.mesg.GetFieldValue ( "WeightSetting" );
                                        var restheartrte  = e.mesg.GetFieldValue ( "RestingHeartRate" );
                                        var defmxbikehr   = e.mesg.GetFieldValue ( "DefaultMaxBikingHeartRate" );
                                        var defmxhr       = e.mesg.GetFieldValue ( "DefaultMaxHeartRate" );
                                        var hrsettngs     = e.mesg.GetFieldValue ( "HrSetting" );
                                        var speedsttngs   = e.mesg.GetFieldValue ( "SpeedSetting" );
                                        var diststtngs    = e.mesg.GetFieldValue ( "DistSetting" );
                                        var powsttngs     = e.mesg.GetFieldValue ( "PowerSetting" );
                                        var activityclss  = e.mesg.GetFieldValue ( "ActivityClass" );
                                        var possttngs     = e.mesg.GetFieldValue ( "PositionSetting" );
                                        var tempsttngs    = e.mesg.GetFieldValue ( "TemperatureSetting" );
                                        var heightsttngs  = e.mesg.GetFieldValue ( "HeightSetting" );
                                        break;

                                    case "sport":
                                        var name      = e.mesg.GetFieldValue ( "Name" );
                                            sport     = e.mesg.GetFieldValue ( "Sport" );
                                            subsport  = e.mesg.GetFieldValue ( "SubSport" );
                                        break;

                                    case "trainingsettings":
                                        break;

                                    case "zonestarget":
                                        var fntthrshpow   = e.mesg.GetFieldValue ( "FunctionalThresholdPower" );
                                        var hrcalctype    = e.mesg.GetFieldValue ( "HrCalcType" );
                                        var pwrcalctype   = e.mesg.GetFieldValue ( "PwrCalcType" );
                                        break;

                                    case "hrv":
                                        time  = e.mesg.GetFieldValue ( "Time" );
                                        break;

                                    default:
                                        break;
                                }
                            };

                            m_LineNr++;
                            decode.MesgEvent += broadcaster.OnMesg;
                            decode.Read( fitFile );
                        }

                        break;

                    default:
                        rdr = new StreamReader( a_Fname );
                        string line = "";

                        while( !rdr.EndOfStream )
                        {
                            line = rdr.ReadLine();

                            if (m_LineNr == 0)
                            {
                                ParseCols( line );
                                m_LineNr++;
                                continue;
                            }

                            ParseLine( line );

                            m_LineNr++;
                        }
                        break;
                }

                ShowInList( m_ExpFilNm, a_Fit );
            }
            catch ( Exception ex )
            {
                MessageBox.Show("Error parsing section " + sectname + " loop nr: " +  m_LineNr + "\n\n" + ex.Message, "Import error");
            }
            finally
            {
                if (rdr != null) rdr.Close();
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       07.09.2025
        LAST CHANGE:   07.09.2025
        ***************************************************************************/
        private object RetVal( object a_Val, string a_Tp )
        {
            if ( a_Val == null )
            {
                switch( a_Tp )
                {
                    case "b" : return (byte)0;
                    case "sb": return (sbyte)0;
                    case "i" : return (int)0;
                    case "ui": return (uint)0;
                }
            }

            return a_Val;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       01.05.2018
        LAST CHANGE:   14.10.2020
        ***************************************************************************/
        private bool ParseLine( string a_Line )
        {
            List<string> segs = Utils.SplitExt(a_Line,";");

            if (segs.Count != m_Cols.Count + 1) return false;

            m_Recs.Add( new Record(segs) );

            return true;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       14.05.2018
        LAST CHANGE:   14.05.2018
        ***************************************************************************/
        private void ReplCols( string a_Key, string a_Replcmnt )
        {
            int idx = m_Cols.FindIndex( c => c.Contains(a_Key) );
            if (idx == -1) return;
            if (a_Replcmnt != "") m_Cols[idx] = a_Replcmnt;
            else                  m_Cols.RemoveAt(idx);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       01.05.2018
        LAST CHANGE:   01.05.2018
        ***************************************************************************/
        private void ParseCols( string a_Line )
        {
            m_Cols = Utils.SplitExt(a_Line,";");

            ReplCols("mm"   ,"Dist. (m)");
            ReplCols("Temp" ,"Temp. (°C)");
            ReplCols("Watts","Power (W)");

            ReplCols("Right","");
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       01.05.2018
        LAST CHANGE:   08.09.2025
        ***************************************************************************/
        private void ShowInList( string a_ExpFilNm, bool a_Fit = false )
        {
            m_OutList.Clear();
            m_OutList.XlExpFilNm = a_ExpFilNm;

            m_OutList.ShowColumns(m_Cols);

            foreach( Record ds in m_Recs )
            {
                m_OutList.AddLine( ds.GetLine( a_Fit ) );
            }

            m_OutList.Refresh();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       22.07.2018
        LAST CHANGE:   20.06.2025
        ***************************************************************************/
        public void Calculate( bool a_Fit = false )
        {
            double accp = 0;
            double accn = 0;
            double amax = -32000;
            double amin =  32000;
            double dist = 0.0;
            double ac   = 0;
            double pv   = 0;
            bool   init = true;

            Record prv = null;

            try
            {
                foreach( Record ds in m_Recs )
                {
                    if( init )
                    {
                        init = false;
                        prv  = ds;
                        continue;
                    }
                
                    if ( a_Fit )
                    {
                        ac = ds .dAltitude;
                        pv = prv.dAltitude;
                    }
                    else
                    {
                        ac = (double)ds .iAltitude;
                        pv = (double)prv.iAltitude;
                    }

                    if( ds.dSpeed == 0.0 ) continue;

                    if (ac > amax) amax = ac;
                    if (ac < amin) amin = ac;

                    if ( pv > ac  ) accn += ( pv - ac );
                    if ( pv < ac  ) accp += ( ac - pv );

                    prv = ds;
                }

                if ( m_Recs.Count < 1 ) return;

                Record td_last = m_Recs[m_Recs.Count-1];
                Record td_frst = m_Recs[0];

                m_Statcs.AltMax    = amax;
                m_Statcs.AltMin    = amin;
                m_Statcs.Ascent    = accp;
                m_Statcs.Descent   = accn;
                dist               = Utils.Str2Double( td_last.Distance );
                m_Statcs.Distnce   = dist / 1000;
                m_Statcs.TimeStart = Convert.ToDateTime( td_frst.Date + " " + td_frst.Time );
                m_Statcs.TimeStop  = Convert.ToDateTime( td_last.Date + " " + td_last.Time );
            }
            catch( Exception ex )
            {
                MessageBox.Show( ex.Message + "\nDataSet: " + prv.GetLine( a_Fit ), "Calculate exception in data set: " );
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       25.08.2018
        LAST CHANGE:   25.08.2018
        ***************************************************************************/
        private void Output( string a_Name, int a_Val, string a_Unit )
        {
            OutName( a_Name );
            m_RTB.Output(string.Format("{0,7} {1}\n", a_Val, a_Unit ) , Color.Green, false );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       26.08.2018
        LAST CHANGE:   03.09.2025
        ***************************************************************************/
        private void Output( string a_Name, double a_Val, string a_Unit )
        {
            OutName( a_Name );
            m_RTB.Output(string.Format("{0,7:0.00} {1}\n", a_Val, a_Unit ) , Color.Green, false );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       02.09.2018
        LAST CHANGE:   02.09.2018
        ***************************************************************************/
        private void Output( string a_Name, System.DateTime a_Val )
        {
            OutName( a_Name );
            m_RTB.Output(string.Format("{0} {1}\n", a_Val, a_Val.DayOfWeek ) , Color.Green, false );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       02.09.2018
        LAST CHANGE:   02.09.2018
        ***************************************************************************/
        private void Output( string a_Name, TimeSpan a_Val )
        {
            OutName( a_Name );
            m_RTB.Output(string.Format("{0}\n", a_Val ) , Color.Green, false );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       26.08.2018
        LAST CHANGE:   03.09.2025
        ***************************************************************************/
        private void OutName( string a_Name )
        {
            m_RTB.Output(string.Format("{0,-14}: ",a_Name), Color.Blue , true  );    
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       25.08.2018
        LAST CHANGE:   01.09.2025
        ***************************************************************************/
        public void ShowData( bool a_Fit = false )
        {
            m_RTB.Clear();
            
            if ( a_Fit )
            {
                Output( "Start Time", m_Lap.TimeStart );
                Output( "Stop  Time", m_Lap.TimeStop  );
                Output( "Duration"  , m_Lap.TotTime   );

                Output( "Distance"      , m_Lap.TotDistance, "km"  );
                Output( "Max Altitude"  , m_Statcs.AltMax  , "m"   );
                Output( "Min Altitude"  , m_Statcs.AltMin  , "m"   );
                Output( "Ascent"        , m_Lap.TotAscent  , "hm"  );
                Output( "Descent"       , m_Lap.TotDescent , "hm"  );
                Output( "Max Cadence"   , m_Lap.MaxCadence , "rpm" );
                Output( "Avg Cadence"   , m_Lap.AvgCadence , "rpm" );
                Output( "Max Heart Rate", m_Lap.MaxHrtRate, "bpm" );
                Output( "Avg Heart Rate", m_Lap.AvgHrtRate, "bpm" );
                Output( "Max Temper."   , m_Lap.MaxTemp   , "°C" );
                Output( "Avg Temper."   , m_Lap.AvgTemp   , "°C" );
            }
            else
            {
                Output( "Start Time", m_Statcs.TimeStart );
                Output( "Stop  Time", m_Statcs.TimeStop );
                Output( "Duration", m_Statcs.TimeStop - m_Statcs.TimeStart );

                Output( "Distance", m_Statcs.Distnce, "km" );
                Output( "Max Altitude", m_Statcs.AltMax, "m" );
                Output( "Min Altitude", m_Statcs.AltMin, "m" );
                Output( "Ascent", m_Statcs.Ascent, "hm" );
                Output( "Descent", m_Statcs.Descent, "hm" );
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       28.08.2018
        LAST CHANGE:   03.09.2025
        ***************************************************************************/
        public void Export2XL( string a_Fname, string a_Title, string a_Odo )
        {
            string sht = "MTB";
            if ( a_Odo.ToLower().EndsWith("c") ) sht = "CB";
            m_XlExp.OpenExcel( a_Fname, sht );

            List<XlCell> lst = new List<XlCell>();
            if ( m_Lap == null )
            {
                lst.Add( new XlCell( a_Title, "" ) );
                lst.Add( new XlCell( string.Format( "{0:dd. MMM. yyyy}", m_Statcs.TimeStart.Date ), "dd. MMM. yyyy" ) );
                lst.Add( new XlCell( "" ) );
                lst.Add( new XlCell( string.Format( "{0:00}:{1:00}", m_Statcs.TimeStart.Hour, m_Statcs.TimeStart.Minute ), "HH:mm" ) );
                lst.Add( new XlCell( string.Format( "{0:00}:{1:00}", m_Statcs.TimeStop.Hour, m_Statcs.TimeStop.Minute ), "HH:mm" ) );
                lst.Add( new XlCell( "" ) );
                lst.Add( new XlCell( string.Format( "{0}", a_Odo ) ) );
                lst.Add( new XlCell( "" ) );
                lst.Add( new XlCell( "" ) );
                lst.Add( new XlCell( "" ) );
                lst.Add( new XlCell( "" ) );
                string dist = string.Format( "{0:0.00}", m_Statcs.Distnce );
                dist = dist.Replace( ".", "," );
                lst.Add( new XlCell( string.Format( dist ), "@" ) );
                lst.Add( new XlCell( string.Format( "{0}", m_Statcs.Ascent ), "@" ) );
            }
            else
            {
                lst.Add( new XlCell( a_Title, "" ) );
                lst.Add( new XlCell( string.Format( "{0:dd. MMM. yyyy}", m_Statcs.TimeStart.Date ), "dd. MMM. yyyy" ) );
                lst.Add( new XlCell( "" ) );
                lst.Add( new XlCell( string.Format( "{0:00}:{1:00}", m_Statcs.TimeStart.Hour, m_Statcs.TimeStart.Minute ), "HH:mm" ) );
                lst.Add( new XlCell( string.Format( "{0:00}:{1:00}", m_Statcs.TimeStop.Hour, m_Statcs.TimeStop.Minute ), "HH:mm" ) );
                lst.Add( new XlCell( "" ) );
                lst.Add( new XlCell( string.Format( "{0}", a_Odo ) ) );
                lst.Add( new XlCell( "" ) );
                lst.Add( new XlCell( "" ) );
                lst.Add( new XlCell( "" ) );
                lst.Add( new XlCell( "" ) );
                string dist = string.Format( "{0:0.00}", m_Lap.TotDistance );
                dist = dist.Replace( ".", "," );
                lst.Add( new XlCell( string.Format( dist ), "@" ) );
                lst.Add( new XlCell( string.Format( "{0}", m_Lap.TotAscent ), "@" ) );
            }
            m_XlExp.AppendLine( lst, 0, true );
            m_XlExp.SaveExcel( );
        }
    } // class
} // namespace