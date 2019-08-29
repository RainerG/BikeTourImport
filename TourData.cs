using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

using NS_Utilities;
using NS_AppConfig;
using NS_UserOut;
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
        SPECIFICATION: Members
        CREATED:       01.05.2018
        LAST CHANGE:   01.05.2018
        ***************************************************************************/
        private int               m_LineNr;
        private List<DataStruct>  m_Data;
        private List<string>      m_Cols;
        private OutputList        m_OutList;
        private WordExcelExport   m_XlExp;
        private Statistics        m_Statcs;
        private UserRichTextBox   m_RTB;

        /***************************************************************************
        SPECIFICATION: C'tor
        CREATED:       01.05.2018
        LAST CHANGE:   01.04.2019
        ***************************************************************************/
        public TourData( UserRichTextBox a_RTB )
        {
            m_RTB       = a_RTB;
            m_LineNr    = 0;
            m_Data      = new List<DataStruct>();
            m_OutList   = new OutputList();
            m_XlExp     = m_OutList.WdXlExport;
            m_Statcs    = new Statistics();

            List<NumFormat> frmts = m_XlExp.NumFrmts;
            frmts.Add( new NumFormat( 1,"yyyy.mm.dd") );
            frmts.Add( new NumFormat( 2,"hh:mm:ss") );
            frmts.Add( new NumFormat( 3,"@") );
            frmts.Add( new NumFormat( 4,"@") );
            frmts.Add( new NumFormat( 5,"@") );
            frmts.Add( new NumFormat( 6,"@") );
            frmts.Add( new NumFormat( 7,"@") );
            frmts.Add( new NumFormat( 8,"@") );
            frmts.Add( new NumFormat( 9,"@") );

            List<ChartFormat> chrts = m_XlExp.ChrtFrmts;
            chrts.Add( new ChartFormat( 2, 3, Excel.XlRgbColor.rgbGreen ));       // Distance
            chrts.Add( new ChartFormat( 2, 8, Excel.XlRgbColor.rgbBrown ));       // Altitude
            chrts.Add( new ChartFormat( 2, 5, Excel.XlRgbColor.rgbBlueViolet ));  // Heart rate
            chrts.Add( new ChartFormat( 2, 4, Excel.XlRgbColor.rgbViolet ));      // Speed
            chrts.Add( new ChartFormat( 2, 6, Excel.XlRgbColor.rgbBlueViolet ));  // Cadence
            chrts.Add( new ChartFormat( 2, 7, Excel.XlRgbColor.rgbRed ));         // Power 
            chrts.Add( new ChartFormat( 2, 9, Excel.XlRgbColor.rgbOrange ));      // Temp

            chrts.Add( new ChartFormat( 3, 2, Excel.XlRgbColor.rgbGreen ));       // Time
            chrts.Add( new ChartFormat( 3, 8, Excel.XlRgbColor.rgbBrown ));       // Altitude
            chrts.Add( new ChartFormat( 3, 5, Excel.XlRgbColor.rgbBlueViolet ));  // Heart rate
            chrts.Add( new ChartFormat( 3, 4, Excel.XlRgbColor.rgbViolet ));      // Speed
            chrts.Add( new ChartFormat( 3, 6, Excel.XlRgbColor.rgbBlueViolet ));  // Cadence
            chrts.Add( new ChartFormat( 3, 7, Excel.XlRgbColor.rgbRed ));         // Power 
            chrts.Add( new ChartFormat( 3, 9, Excel.XlRgbColor.rgbOrange ));      // Temp

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
        LAST CHANGE:   01.05.2018
        ***************************************************************************/
        public void Import( string a_Fname )
        {
            StreamReader rdr = null;
            m_Data.Clear();
            m_LineNr = 0;

            try
            {
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

                ShowInList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error parsing line " + m_LineNr, "Import error");
            }
            finally
            {
                rdr.Close();
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       01.05.2018
        LAST CHANGE:   24.04.2019
        ***************************************************************************/
        private bool ParseLine( string a_Line )
        {
            List<string> segs = Utils.SplitExt(a_Line,";");

            if (segs.Count != m_Cols.Count + 1) return false;

            m_Data.Add( new DataStruct(segs) );

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
        LAST CHANGE:   01.05.2018
        ***************************************************************************/
        private void ShowInList()
        {
            m_OutList.Clear();

            m_OutList.ShowColumns(m_Cols);

            foreach( DataStruct ds in m_Data )
            {
                m_OutList.AddLine( ds.GetLine() );
            }

            m_OutList.Refresh();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       22.07.2018
        LAST CHANGE:   24.04.2019
        ***************************************************************************/
        public void Calculate()
        {
            int    accp = 0;
            int    accn = 0;
            int    acc  = 0;
            int    amax = -32000;
            int    amin =  32000;
            int    offs = 0;
            double dist = 0.0;

            bool init = true;

            foreach( DataStruct ds in m_Data )
            {
                if( init )
                {
                    init = false;
                    acc  = ds.iAltitude;
                    offs = acc;
                    continue;
                }
                
                int ac = ds.iAltitude;
                if (ac > amax) amax = ac;
                if (ac < amin) amin = ac;

                if (ac > acc+1) accp += ac-acc;
                if (ac < acc-1) accn += acc-ac;
                acc = ac;
            }

            DataStruct td_last = m_Data[m_Data.Count-1];
            DataStruct td_frst = m_Data[0];

            m_Statcs.AltMax    = amax;
            m_Statcs.AltMin    = amin;
            m_Statcs.Ascent    = accp - offs;
            m_Statcs.Descent   = accn - offs;
            dist               = Utils.Str2Double( td_last.Distance );
            m_Statcs.Distnce   = dist / 1000;
            m_Statcs.TimeStart = Convert.ToDateTime( td_frst.Date + " " + td_frst.Time );
            m_Statcs.TimeStop  = Convert.ToDateTime( td_last.Date + " " + td_last.Time );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       25.08.2018
        LAST CHANGE:   25.08.2018
        ***************************************************************************/
        private void Output( string a_Name, int a_Val, string a_Unit )
        {
            OutName( a_Name );
            m_RTB.Output(string.Format("{0,6} {1}\n", a_Val, a_Unit ) , Color.Green, false );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       26.08.2018
        LAST CHANGE:   26.08.2018
        ***************************************************************************/
        private void Output( string a_Name, double a_Val, string a_Unit )
        {
            OutName( a_Name );
            m_RTB.Output(string.Format("{0:0.00,6} {1}\n", a_Val, a_Unit ) , Color.Green, false );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       02.09.2018
        LAST CHANGE:   02.09.2018
        ***************************************************************************/
        private void Output( string a_Name, DateTime a_Val )
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
        LAST CHANGE:   26.08.2018
        ***************************************************************************/
        private void OutName( string a_Name )
        {
            m_RTB.Output(string.Format("{0,-13}: ",a_Name), Color.Blue , true  );    
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       25.08.2018
        LAST CHANGE:   02.09.2018
        ***************************************************************************/
        public void ShowData( )
        {
            m_RTB.Clear();
            Output("Start Time"  , m_Statcs.TimeStart );
            Output("Stop  Time"  , m_Statcs.TimeStop  );
            Output("Duration"    , m_Statcs.TimeStop - m_Statcs.TimeStart );
                                   
            Output("Distance"    , m_Statcs.Distnce, "km");
            Output("Max Altitude", m_Statcs.AltMax , "m");
            Output("Min Altitude", m_Statcs.AltMin , "m");
            Output("Ascent"      , m_Statcs.Ascent , "hm");
            Output("Descent"     , m_Statcs.Descent, "hm");
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       28.08.2018
        LAST CHANGE:   27.06.2019
        ***************************************************************************/
        public void Export2XL( string a_Fname, string a_Title, string a_Odo )
        {
            string sht = "MTB";
            if ( a_Odo.ToLower().EndsWith("c") ) sht = "CB";
            m_XlExp.OpenExcel( a_Fname, sht );

            List<XlCell> lst = new List<XlCell>();
            lst.Add( new XlCell( a_Title, "" ) );
            lst.Add( new XlCell( string.Format( "{0:dd.MMM.yyyy}", m_Statcs.TimeStart.Date ), "dd.MMM.yyyy" ) );
            lst.Add( new XlCell( "" ) );
            lst.Add( new XlCell( string.Format( "{0:00}:{1:00}", m_Statcs.TimeStart.Hour, m_Statcs.TimeStart.Minute ), "HH:mm" ) );
            lst.Add( new XlCell( string.Format( "{0}", a_Odo ) ) );
            lst.Add( new XlCell( "" ) );
            lst.Add( new XlCell( "" ) );
            lst.Add( new XlCell( "" ) );
            lst.Add( new XlCell( "" ) );
            string dist = string.Format( "{0:0.00}", m_Statcs.Distnce );
            dist = dist.Replace(".",",");
            lst.Add( new XlCell( string.Format( dist ), "@" ) );
            lst.Add( new XlCell( string.Format( "{0}", m_Statcs.Ascent ), "@" ) );
            m_XlExp.AppendLine( lst, 0, true );
        }

    } // class
} // namespace