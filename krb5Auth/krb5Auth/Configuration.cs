/**
 * Configuration Form - A configuration form for the krb5Auth pGina authentication plugin.  Allows the user to configure various settings
 * relating to the functionality of the krb5Auth plugin such as the default kerberos realm target, creation of a local user account, 
 * adding the user to local system groups after creation.
 * 
 * Written by Seth Walsh
 * seth.walsh@utah.edu
 * University of Utah
 * */

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
        private log4net.ILog m_logger;
        private List<string> grps;

        public Configuration()
        {
            // Initialize form base components
            InitializeComponent();

            m_logger = log4net.LogManager.GetLogger("pGina.Plugin.krb5Plugin");

            // Get the GUID for this plugin and grab the currently stored Kerberos realm
            Guid m_uuid = new Guid("16E22B15-4116-4FA4-9BB2-57D54BF61A43");
            dynamic m_settings = new pGina.Shared.Settings.pGinaDynamicSettings(m_uuid);

            // Currently set krb realm from the registry settings for this plugin
            currentRealmText.Text = m_settings.Realm;

            // Currently set local groups from the registry settings for this plugin
            string[] g = m_settings.LocalGroups;
            grps = new List<string>(g);
            for(int i = 0; i < grps.Count; i++)
            {
                m_logger.InfoFormat("grps: {0}", grps[i]);
            }
            //grps = g.ToList();

            // Initialize the local groups check box list
            InitializeCheckedListBox();

            bool addLocal = m_settings.AddLocal;
            if (addLocal)
                this.createLocalAccount.Checked = true;
        }

        /**
         * Handle the save button click.  Grab any settings that were checked / set and store them accordingly in the registry for
         * this plugin, then close the dialog.
         * 
         * */
        private void save_Click(object sender, EventArgs e)
        {            
            realm = rText.Text;
            Guid m_uuid = new Guid("16E22B15-4116-4FA4-9BB2-57D54BF61A43");
            dynamic m_settings = new pGina.Shared.Settings.pGinaDynamicSettings(m_uuid);

            // If the realm text field is empty, do nothing
            if (realm.CompareTo("") == 0)
            {
                //m_settings.Realm = currentRealmText;
            }
            else
            {
                // Set the realm in the plugin registry as chosen by the user
                m_settings.Realm = realm;
                m_logger.InfoFormat("Configuration setting Kerberos realm to: {0}", realm);
            }

            // Get all the checked groups in the list of local groups available
            for (int i = 0; i < localUserGroups.Items.Count; i++)
            {
                if (localUserGroups.GetItemChecked(i))
                {
                    groups.Add(localUserGroups.Items[i].ToString());

                    m_logger.InfoFormat("Configuration adding Group: {0} to local user groups array.", localUserGroups.Items[i].ToString());
                }
            }

            // Convert to a string[] and store it in the memory for this plugin
            m_settings.LocalGroups = this.groups.ToArray();
            
            // Close the configuration dialog
            this.Close();
        }

        /**
         * Cancel button
         * */
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
                        if( (child.Name.CompareTo("Users") == 0) || grps.Contains(child.Name) )
                        //if(child.Name.CompareTo("Users") == 0)
                        {
                            this.localUserGroups.Items.Add(child.Name, true);
                        }
                        else
                        {
                            this.localUserGroups.Items.Add(child.Name);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                m_logger.InfoFormat("Configuration Error InitializeCheckedListBox(): {0}", e.Message);
            }
        }

        /**
         * Determines if we are, or are not, creating a local account on this machine for the user attempting to login.
         * */
        private void createLocalAccount_CheckedChanged(object sender, EventArgs e)
        {
            Guid m_uuid = new Guid("16E22B15-4116-4FA4-9BB2-57D54BF61A43");
            dynamic m_settings = new pGina.Shared.Settings.pGinaDynamicSettings(m_uuid);
            m_settings.AddLocal = this.createLocalAccount.Checked;
            
            m_logger.InfoFormat("Configuration Create Local Account: {0}", this.createLocalAccount.Checked.ToString());
        }
    }
}
