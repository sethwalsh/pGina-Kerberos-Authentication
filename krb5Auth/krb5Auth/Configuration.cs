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
        public string realm = "";
        private log4net.ILog m_logger;
        
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
    }
}
