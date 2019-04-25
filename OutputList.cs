using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NS_AppConfig;
using NS_Utilities;
using NS_UserOut;

namespace BikeTourImport
{
    public partial class OutputList:UserOutList
    {
        /***************************************************************************
        SPECIFICATION: Constants
        CREATED:       18.04.2015
        LAST CHANGE:   18.04.2015
        ***************************************************************************/

        /***************************************************************************
        SPECIFICATION: Members
        CREATED:       31.03.2015
        LAST CHANGE:   29.05.2015
        ***************************************************************************/
        private string           m_FileName;

        /***************************************************************************
        SPECIFICATION: C'tor
        CREATED:       31.03.2015
        LAST CHANGE:   09.09.2015
        ***************************************************************************/
        public OutputList( )
            :base()
        {
            InitializeComponent();
            
            userListViewOutp.Font = new Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            m_FileName = "";
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       09.09.2015
        LAST CHANGE:   09.09.2015
        ***************************************************************************/
        public void Serialize( ref AppSettings a_Conf )
        {
            if( a_Conf.IsReading )
            {
            }
            else
            {
            }

            base.Serialize( ref a_Conf );
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       16.04.2015
        LAST CHANGE:   17.04.2015
        ***************************************************************************/
        private void userListViewOutp_DoubleClick( object sender, EventArgs e )
        {
            ListViewItem it = userListViewOutp.SelectedItems[0];

            int line = int.Parse(it.Text);

            if (line == 0) return;

            Utils.Edit(m_FileName, line);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       29.05.2015
        LAST CHANGE:   29.05.2015
        ***************************************************************************/
        private void hideFuncNamesStartingWithToolStripMenuItem_Click( object sender, EventArgs e )
        {
            //ShowFuncs();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       29.05.2015
        LAST CHANGE:   29.05.2015
        ***************************************************************************/
        private void hideObjectsWithTooBigSizeToolStripMenuItem_Click( object sender, EventArgs e )
        {
            //ShowFuncs();
        }

    } // class
} // Namespace
