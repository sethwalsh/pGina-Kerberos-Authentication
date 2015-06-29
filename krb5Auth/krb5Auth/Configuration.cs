using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.DirectoryServices;

namespace pGina.Plugin.krb5Auth
{
    public partial class Configuration : Form
    {
        public string realm = "empty";
        public List<string> groups = new List<string>();
        public bool addLocal = false;

        public Configuration()
        {
            InitializeComponent();
            InitializeCheckedListBox();

            Guid m_uuid = new Guid("16E22B15-4116-4FA4-9BB2-57D54BF61A43");
            dynamic m_settings = new pGina.Shared.Settings.pGinaDynamicSettings(m_uuid);
            currentRealmText.Text = m_settings.Realm;
        }

        private void save_Click(object sender, EventArgs e)
        {            
            realm = rText.Text;
            Guid m_uuid = new Guid("16E22B15-4116-4FA4-9BB2-57D54BF61A43");
            dynamic m_settings = new pGina.Shared.Settings.pGinaDynamicSettings(m_uuid);
            if (realm.CompareTo("") == 0)
            {
                //m_settings.Realm = currentRealmText;
            }
            else
            {
                m_settings.Realm = realm;
            }
            
            this.Close();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /**
         * For every Local System Group that is selected, add the currently logging in user to the plugin registry setting array for local
         * groups.  Default is to add the user to Users group, and is necessary for successful login.
         * */
        private void InitializeCheckedListBox()
        {
            try
            {
                DirectoryEntry machine = new DirectoryEntry("WinNT://" + Environment.MachineName + ",Computer");
                                
                foreach (DirectoryEntry child in machine.Children)
                {
                    if (child.SchemaClassName == "Group")
                    {
                        groups.Add(child.Name);                        

                        if (child.Name.CompareTo("Users") == 0)
                        {
                            this.localUserGroups.Items.Add(child.Name, true);
                        }
                        else
                        {
                            this.localUserGroups.Items.Add(child.Name);
                        }
                    }
                }

                Guid m_uuid = new Guid("16E22B15-4116-4FA4-9BB2-57D54BF61A43");
                dynamic m_settings = new pGina.Shared.Settings.pGinaDynamicSettings(m_uuid);
                m_settings.LocalGroups = this.groups.ToArray();
            }
            catch (Exception e)
            {
            }
        }

        private void createLocalAccount_CheckedChanged(object sender, EventArgs e)
        {
            Guid m_uuid = new Guid("16E22B15-4116-4FA4-9BB2-57D54BF61A43");
            dynamic m_settings = new pGina.Shared.Settings.pGinaDynamicSettings(m_uuid);
            m_settings.AddLocal = this.createLocalAccount.Checked;
            //this.addLocal = this.createLocalAccount.Checked;
        }
    }
}
