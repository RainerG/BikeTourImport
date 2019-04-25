using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using NS_UserCombo;
using NS_Utilities;
using NS_AppConfig;

namespace BikeTourImport
{
    public partial class BikeImport:Form
    {
        /***************************************************************************
        SPECIFICATION: 
        CREATED:       01.05.2018
        LAST CHANGE:   24.04.2019
        ***************************************************************************/
        private const string APPNAME    = "BikeTourImport.ini";
        private const string EXPORTXL   = "D:\\data\\DOCS\\Bike\\Radtouren\\KMundHM.xlsm";
        private const string ODOMETER   = "CM9.3A";
        private const int    DB_VERSION = 150;

        private string      m_Filename;
        private string      m_ExpFileNm;
        private AppSettings m_Config;
        private TourData    m_Data;

        /***************************************************************************
        SPECIFICATION: C'tor
        CREATED:       30.04.2018
        LAST CHANGE:   01.04.2019
        ***************************************************************************/
        public BikeImport()
        {
            InitializeComponent();

            m_Config     = new AppSettings(APPNAME);
            m_Data       = new TourData( userRichTextBox );
            m_Filename   = "none";
            m_ExpFileNm  = "none";

            fileCmbTour  .DragDir = false;
            fileCmbExport.DragDir = false;

            userCmbOdo.Text = ODOMETER+" M";
            userCmbOdo.Items.Add(ODOMETER + " C");
            userCmbOdo.Items.Add(ODOMETER + " M");

            fileCmbExport.Text = EXPORTXL;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       01.05.2018
        LAST CHANGE:   24.04.2019
        ***************************************************************************/
        public void Serialize( ref AppSettings a_Conf )
        {
            if( a_Conf.IsReading )
            {
                a_Conf.DeserializeDbVersion();
                a_Conf.DeserializeDialog( this );
            }
            else
            {
                a_Conf.Serialize( DB_VERSION );
                a_Conf.CurrDbVersion = DB_VERSION;
                a_Conf.SerializeDialog  ( this );
            }

            fileCmbTour  .Serialize( ref a_Conf );
            fileCmbExport.Serialize( ref a_Conf );
            m_Data       .Serialize( ref a_Conf );  
            userCmbTitle .Serialize( ref a_Conf );
            userCmbOdo   .Serialize( ref a_Conf );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       24.04.2019
        LAST CHANGE:   24.04.2019
        ***************************************************************************/
        private void AddTextEntries()
        {
            fileCmbTour  .AddTextEntry();
            fileCmbExport.AddTextEntry();
            userCmbOdo   .AddTextEntry();
            userCmbTitle .AddTextEntry();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       01.05.2018
        LAST CHANGE:   01.05.2018
        ***************************************************************************/
        private void BikeImport_FormClosing( object sender, FormClosingEventArgs e )
        {
            if (! m_Config.OpenWrite() ) return;

            Serialize( ref m_Config );

            m_Config.Close();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       01.05.2018
        LAST CHANGE:   29.08.2018
        ***************************************************************************/
        private void BikeImport_Load( object sender, EventArgs e )
        {
            if (! m_Config.OpenRead() ) return;

            try
            {
                Serialize( ref m_Config );
                m_Filename  = fileCmbTour  .Text;
                m_ExpFileNm = fileCmbExport.Text;
            }
            catch ( Exception ex )
            {
                return;
            }
            finally
            {
                m_Config.Close();
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.04.2018
        LAST CHANGE:   30.04.2018
        ***************************************************************************/
        private void btnTourBrwse_Click( object sender, EventArgs e )
        {
            DialogResult dlg = fileCmbTour.BrowseFileRead("CSV files (*.csv)|*.csv|All files (*.*)|*.*");

            if (dlg != DialogResult.OK) return;

            m_Filename = fileCmbTour.Text;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.04.2018
        LAST CHANGE:   30.04.2018
        ***************************************************************************/
        private void btnTourEdit_Click( object sender, EventArgs e )
        {
            fileCmbTour.Edit();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       01.05.2018
        LAST CHANGE:   24.04.2019
        ***************************************************************************/
        private void btnImport_Click( object sender, EventArgs e )
        {
            m_Filename = fileCmbTour.Text;
            m_Data.Import( m_Filename );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       22.07.2018
        LAST CHANGE:   01.04.2019
        ***************************************************************************/
        private void btnExport_Click( object sender, EventArgs e )
        {
            m_ExpFileNm = fileCmbExport.Text;

            if ( m_ExpFileNm == "none" )
            {
                MessageBox.Show( "Select a valid EXCEL export file before", "File not found" );
                return;
            }

            if (! File.Exists( m_ExpFileNm ) )
            {
                MessageBox.Show( "File " + m_ExpFileNm + " does not exist", "File not found" );
                return;
            }

            AppendData2XL( m_ExpFileNm );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       28.08.2018
        LAST CHANGE:   24.04.2019
        ***************************************************************************/
        private void AppendData2XL( string a_Fname )
        {
            m_Data.Export2XL( a_Fname, userCmbTitle.Text, userCmbOdo.Text );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       27.08.2018
        LAST CHANGE:   27.08.2018
        ***************************************************************************/
        private void btnExpBrwse_Click( object sender, EventArgs e )
        {
            DialogResult dlg = fileCmbExport.BrowseFileRead( "Text files (*.xls*)|*.xls*|All files (*.*)|*.*" );

            if (dlg != DialogResult.OK) return;

            m_ExpFileNm = fileCmbExport.Text;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       22.07.2018
        LAST CHANGE:   24.04.2019
        ***************************************************************************/
        private void btnCalc_Click( object sender, EventArgs e )
        {
            AddTextEntries();
            m_Data.Calculate();
            m_Data.ShowData ();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       02.09.2018
        LAST CHANGE:   24.04.2019
        ***************************************************************************/
        private void btnEditExport_Click( object sender, EventArgs e )
        {
            AddTextEntries();
            fileCmbExport.Edit();
        }
    }
}
