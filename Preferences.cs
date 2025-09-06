using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NS_AppConfig;

namespace BikeTourImport
{
    /***************************************************************************
    SPECIFICATION: 
    CREATED:       06.09.2025
    LAST CHANGE:   06.09.2025
    ***************************************************************************/
    public partial class Preferences:Form
    {
        /***************************************************************************
        SPECIFICATION: Accessors
        CREATED:       06.09.2025
        LAST CHANGE:   06.09.2025
        ***************************************************************************/
        public int MinSmpleInt { get { return (int)numSmpleInt.Value; } }

        /***************************************************************************
        SPECIFICATION: C'tor
        CREATED:       06.09.2025
        LAST CHANGE:   06.09.2025
        ***************************************************************************/
        public Preferences()
        {
            InitializeComponent();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       06.09.2025
        LAST CHANGE:   06.09.2025
        ***************************************************************************/
        public void Serialize( ref AppSettings a_Conf )
        {
            if( a_Conf.IsReading )
            {
                if (a_Conf.DbVersion < 378) return;

                a_Conf.DeserializeDialog( this );
                numSmpleInt.Value = a_Conf.Deserialize<int>();
            }
            else
            {
                a_Conf.SerializeDialog( this );
                a_Conf.Serialize( (int)numSmpleInt.Value );
            }
        }

    } // class
} // namespace
