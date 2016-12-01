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

        PushBulletAPI.PushNoteObject question;
        private void frmBackupErrorDecision_Load(object sender, EventArgs e)
        {
            this.rtbDetails.Text =
                "File path:\n" +
                _filename + "\n\n" +
                "Error details:\n" +
                _ex.Message + "\n\n" +
                _ex.StackTrace
                ;

            question = PushBulletAPI.Pushes.createPushNote("Hello", "Hello");
            backgroundWorkerPushBulletDecision.RunWorkerAsync(); // Start listening for responses
        }

        void decideAction(DialogResult result) {
            this.DialogResult = result;
            backgroundWorkerPushBulletDecision.CancelAsync();
            this.Close();
        }

        private void btnTryAgain_Click(object sender, EventArgs e)
        {
            decideAction(DialogResult.OK);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            decideAction(DialogResult.Cancel);
        }

        private void backgroundWorkerPushBulletDecision_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!e.Cancel) {
                PushBulletAPI.PushNoteObject[] lastPushes = PushBulletAPI.Pushes.getLastMessages(10, question.created);

                // Sleep 1 minute
                System.Threading.Thread.Sleep((int)TimeSpan.FromMinutes(1).TotalMilliseconds); 
            }
        }

        private void backgroundWorkerPushBulletDecision_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
    }
}
