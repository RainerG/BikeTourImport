using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Dynastream.Fit;

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
        LAST CHANGE:   29.05.2025
        ***************************************************************************/
        private const int    DB_VERSION = 378;
        private const string BASE_NAME  = "BikeTourImport";
        private const string INI_FNAME  = BASE_NAME + ".ini";
        private const string EXPORTXL   = "D:\\data\\DOCS\\Bike\\Radtouren\\KMundHM.xlsm";
        private const string ODOMETER   = "CM9.3A";
        private const string RELEASE    = "Release: 1.00 RC4";


        /***************************************************************************
        SPECIFICATION: Members
        CREATED:       01.05.2018
        LAST CHANGE:   06.09.2025
        ***************************************************************************/
        private string      m_Filename;
        private string      m_ExpFileNm;
        private AppSettings m_Config;
        private TourData    m_Data;
        private bool        m_Fit;
        private string      m_ExportFname;
        private Preferences m_Prefs;
        private Windows     m_Wins;


        /***************************************************************************
        SPECIFICATION: C'tor
        CREATED:       30.04.2018
        LAST CHANGE:   06.09.2025
        ***************************************************************************/
        public BikeImport()
        {
            InitializeComponent();

            string sCurrDir = Directory.GetCurrentDirectory() + "\\";
            m_Config = new AppSettings( sCurrDir + INI_FNAME );
            m_ExportFname = sCurrDir + BASE_NAME + "_1.ini";

            m_Config = new AppSettings(INI_FNAME);
            m_Prefs  = new Preferences();
            m_Data   = new TourData( userRichTextBox, m_Prefs );

            m_Filename   = "none";
            m_ExpFileNm  = "none";
            m_Fit        = false;

            fileCmbTour  .DragDir = false;
            fileCmbExport.DragDir = false;

            userCmbOdo.Text = "Edge 200";
            userCmbOdo.Items.Add( "Edge 200" );
            userCmbOdo.Items.Add( "Sigma Rox" );
            userCmbOdo.Items.Add( ODOMETER + " C" );
            userCmbOdo.Items.Add( ODOMETER + " M");

            fileCmbExport.Text = EXPORTXL;

            m_Wins = new Windows( this );
            m_Wins.Wins.Add( new Window( m_Data.OutList ) );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       01.05.2018
        LAST CHANGE:   06.09.2025
        ***************************************************************************/
        public void Serialize( ref AppSettings a_Conf )
        {
            if( a_Conf.IsReading )
            {
                a_Conf.DeserializeDbVersion();
                a_Conf.DeserializeDialog( this );
                if (a_Conf.DbVersion > 377)
                {
                    m_ExportFname                         = a_Conf.Deserialize<string>();
                    stickWindowsToolStripMenuItem.Checked = a_Conf.Deserialize<bool>();
                }
            }
            else
            {
                a_Conf.Serialize( DB_VERSION );
                a_Conf.CurrDbVersion = DB_VERSION;
                a_Conf.SerializeDialog  ( this );
                a_Conf.Serialize( m_ExportFname );
                a_Conf.Serialize( stickWindowsToolStripMenuItem.Checked );
            }

            fileCmbTour  .Serialize( ref a_Conf );
            fileCmbExport.Serialize( ref a_Conf );
            m_Data       .Serialize( ref a_Conf );  
            userCmbTitle .Serialize( ref a_Conf );
            userCmbOdo   .Serialize( ref a_Conf );
            m_Prefs      .Serialize( ref a_Conf );
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
        LAST CHANGE:   27.06.2019
        ***************************************************************************/
        private void BikeImport_FormClosing( object sender, FormClosingEventArgs e )
        {
            AddTextEntries();

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
            DialogResult dlg = fileCmbTour.BrowseFileRead( "FIT files (*.fit)|*.fit|CSV files (*.csv)|*.csv|All files (*.*)|*.*" );

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
        LAST CHANGE:   26.06.2019
        ***************************************************************************/
        private void btnImport_Click( object sender, EventArgs e )
        {
            m_Filename = fileCmbTour.Text;
            string ext = Utils.GetExtension( m_Filename ).ToLower();
            m_Fit = ( ext == "fit" );
            m_Data.Import( m_Filename, m_Fit );
            m_Data.Calculate( m_Fit );
            m_Data.ShowData ( m_Fit );

            ShowTitle( m_Filename );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       06.05.2019
        LAST CHANGE:   03.09.2025
        ***************************************************************************/
        private void btnRenFile_Click( object sender, EventArgs e )
        {
            try
            {
                m_Filename = fileCmbTour.Text;

                if ( ! System.IO.File.Exists(m_Filename) )
                {
                    MessageBox.Show(m_Filename + " does not exist","File error");
                    return;
                }

                string fnbody = Utils.GetFilenameBody(m_Filename);
                List<string> segs = Utils.SplitExt(fnbody,"_");
                fnbody = segs[0];
                string newfn  = userCmbTitle.Text.Trim().Replace(" ","_");

                if( m_Filename.Contains( newfn ) )
                {
                    MessageBox.Show( m_Filename + " already renamed", "File renaming error" );
                    return;
                }

                if( fnbody.Length > 19 )
                {
                    MessageBox.Show( m_Filename + " already has a postfix", "File renaming error" );
                    return;
                }

                newfn = fnbody + "_" + newfn;

                string ext = "." + Utils.GetExtension( m_Filename );
                m_Filename = m_Filename = Utils.GetPath( m_Filename );
                m_Filename = Utils.ConcatPaths( m_Filename, newfn );
                m_Filename = m_Filename + ext;
                fileCmbTour.Text = m_Filename;

                string dir    = Utils.GetPath( m_Filename );
                string[] fls  = Directory.GetFiles(dir);

                foreach( string fl in fls )
                {
                    if ( fl.Contains( fnbody ) )
                    {
                        FileInfo fi = new FileInfo(fl);
                        if (fi.Exists)
                        {
                            ext = "." + Utils.GetExtension(fl);
                            fi.MoveTo( Utils.ConcatPaths(dir,newfn) + ext );
                        }
                    }
                }
            }
            catch( Exception ex )
            {
                MessageBox.Show( ex.Message, "Renaming exception");
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       22.07.2018
        LAST CHANGE:   27.06.2019
        ***************************************************************************/
        private void btnExport_Click( object sender, EventArgs e )
        {
            AddTextEntries();

            m_ExpFileNm = fileCmbExport.Text;

            if ( m_ExpFileNm == "none" )
            {
                MessageBox.Show( "Select a valid EXCEL export file before", "File not found" );
                return;
            }

            if (! System.IO.File.Exists( m_ExpFileNm ) )
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
            AddTextEntries();
            DialogResult dlg = fileCmbExport.BrowseFileRead( "Text files (*.xls*)|*.xls*|All files (*.*)|*.*" );

            if (dlg != DialogResult.OK) return;

            m_ExpFileNm = fileCmbExport.Text;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       22.07.2018
        LAST CHANGE:   20.06.2025
        ***************************************************************************/
        private void btnCalc_Click( object sender, EventArgs e )
        {
            AddTextEntries();
            m_Data.Calculate( m_Fit );
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

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       29.08.2019
        LAST CHANGE:   29.08.2019
        ***************************************************************************/
        private void aboutToolStripMenuItem_Click( object sender, EventArgs e )
        {
            About dlg = new About( RELEASE );
            dlg.ShowDialog();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       14.10.2019
        LAST CHANGE:   14.10.2019
        ***************************************************************************/
        private void ShowTitle( string a_Filename )
        {
            string fname = Utils.GetFilenameBody(a_Filename);
            List<string> segs = Utils.SplitExt( fname, "_-" );
            if ( segs.Count <= 6 ) return;
            segs.RemoveRange(0,6);

            List<string> ttls = new List<string>();
            foreach( string ttl in userCmbTitle.Items ) ttls.Add( ttl );

            foreach( string ts in ttls )
            {
                bool found = true;

                List<string> tsegs = Utils.SplitExt( ts, " " );

                if (tsegs.Count != segs.Count ) continue;

                foreach( string tsg in tsegs )
                {
                    string f = segs.Find( s => s == tsg );
                    if ( f == null )
                    {
                        found = false;
                        break;
                    }
                }

                if ( found )
                {
                    userCmbTitle.Text = ts;
                    return;
                }
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       06.09.2025
        LAST CHANGE:   06.09.2025
        ***************************************************************************/
        private void preferencesToolStripMenuItem_Click( object sender, EventArgs e )
        {
            m_Prefs.ShowDialog();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       06.09.2025
        LAST CHANGE:   06.09.2025
        ***************************************************************************/
        private void exportSettingsToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if( m_Config.Export( ref m_ExportFname ) )
            {
                Serialize( ref m_Config );
                m_Config.Close();
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       06.09.2025
        LAST CHANGE:   06.09.2025
        ***************************************************************************/
        private void importSettingsToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if( m_Config.Import( ref m_ExportFname ) )
            {
                Serialize( ref m_Config );
                m_Config.Close();
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       06.09.2025
        LAST CHANGE:   06.09.2025
        ***************************************************************************/
        private void allWindowsToFrontToolStripMenuItem_Click( object sender, EventArgs e )
        {
            m_Wins.AllWinsToFront();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       06.09.2025
        LAST CHANGE:   06.09.2025
        ***************************************************************************/
        private void rearrangeWindowsToolStripMenuItem_Click( object sender, EventArgs e )
        {
            m_Wins.Rearrange();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       06.09.2025
        LAST CHANGE:   06.09.2025
        ***************************************************************************/
        private void BikeImport_LocationChanged( object sender, EventArgs e )
        {
            if( m_Wins == null ) return;
            m_Wins.LocationChanged( stickWindowsToolStripMenuItem.Checked );
        }
    }
}
