using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace pGina.Plugin.krb5Auth
{
    public partial class Configuration : Form
    {
        public string realm = "empty";

        public Configuration()
        {
            InitializeComponent();
        }

        private void save_Click(object sender, EventArgs e)
        {
            realm = rText.Text;
            Guid m_uuid = new Guid("16E22B15-4116-4FA4-9BB2-57D54BF61A43");
            dynamic m_settings = new pGina.Shared.Settings.pGinaDynamicSettings(m_uuid);
            m_settings.Realm = realm;
            this.Close();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
