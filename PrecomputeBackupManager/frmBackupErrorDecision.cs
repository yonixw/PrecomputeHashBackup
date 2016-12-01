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
        frmMain _parent;

        public frmBackupErrorDecision(string fileName, Exception ex, frmMain parent)
        {
            InitializeComponent();
            _filename = fileName;
            _ex = ex;
            _parent = parent;
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

            string questionText =
            "Error uploading file:\n"
            + _filename + "\n\n"
            + "Error message:\n"
            + _ex.Message + "\n\n"
            + "===================\n"
            + "Please respond with text:\n"
            + "\t* 1 or try to retry uploading\n" 
            + "\t* 2 or skip to skip the file (but saved in log)\n"
            + "\t* add `save` to your response to save this action for all future errors" 
            ;

            try
            {
                question = PushBulletAPI.Pushes.createPushNote("Backup BOT", questionText);
            }
            catch (Exception ex)
            {
                _parent.Log(ex);
            }

            backgroundWorkerPushBulletDecision.RunWorkerAsync(); // Start listening for responses
        }

        void decideAction(DialogResult result) {
            this.DialogResult = result;
            backgroundWorkerPushBulletDecision.CancelAsync();
        }

        private void btnTryAgain_Click(object sender, EventArgs e)
        {
            decideAction(DialogResult.OK);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            decideAction(DialogResult.Cancel);
            this.Close();
        }

        private void backgroundWorkerPushBulletDecision_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!e.Cancel) {
                PushBulletAPI.PushNoteObject[] lastPushes = PushBulletAPI.Pushes.getLastMessages(10, question.created);

                foreach (PushBulletAPI.PushNoteObject note in lastPushes) {
                    // original note can exist too (search is >= lastTime)
                    if (note.iden != question.iden ) 
                    {
                        string lower = note.body.ToLower();
                        bool tryFound = (lower.Contains("1") || lower.Contains("try"));
                        bool skipFound = (lower.Contains("2") || lower.Contains("skip"));
                        bool saveFound = lower.Contains("save");

                        if (tryFound && skipFound || (!tryFound && !skipFound) ) {
                            // Ask again:
                            string questionText =
                            "Can't understand your response. Please try again.\n\n"
                            + "===================\n"
                            + "Please respond with text:\n"
                            + "\t* 1 or try to retry uploading\n"
                            + "\t* 2 or skip to skip the file (but saved in log)\n"
                            + "\t* add `save` to your response to save this action for all future errors"
                            ;

                            try
                            {
                                question = PushBulletAPI.Pushes.createPushNote("Backup BOT", questionText);
                            }
                            catch (Exception ex)
                            {
                                _parent.Log(ex);
                            }
                        }
                        else {
                            // Send response
                            string finalResponse = saveFound ? " and I will remember it." : ".";
                            finalResponse =
                                "Thank you!\n"
                                + ((tryFound) ? "You chose to try again" : "You chose to skip the file")
                                + finalResponse;
                            PushBulletAPI.Pushes.createPushNote("Backup BOT", finalResponse);

                            // Finish this dialog
                            cbSave.Invoke(new Action(() => { cbSave.Checked = saveFound; }));
                            if (tryFound) {
                                _parent.Log("User chose to try again. Save? " + saveFound);
                                decideAction(DialogResult.OK);
                                return;
                            }else if (skipFound) {
                                _parent.Log("User chose to skip file. Save? " + saveFound);
                                decideAction(DialogResult.Cancel);
                                return;
                            }
                        }

                        break; // first response should be for us :)
                    }
                }


                // Sleep 1 minute
                System.Threading.Thread.Sleep((int)TimeSpan.FromSeconds(30).TotalMilliseconds); 
            }
        }

        private void backgroundWorkerPushBulletDecision_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }
    }
}
