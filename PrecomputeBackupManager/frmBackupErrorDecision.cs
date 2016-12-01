using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrecomputeBackupManager
{
    public partial class frmBackupErrorDecision : Form
    {
        string _filename;
        Exception _ex;

        public frmBackupErrorDecision(string fileName, Exception ex)
        {
            InitializeComponent();
            _filename = fileName;
            _ex = ex;
        }

        private void frmBackupErrorDecision_Load(object sender, EventArgs e)
        {
            
        }

        private void btnTryAgain_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
