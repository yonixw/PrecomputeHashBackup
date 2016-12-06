using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PrivateData = PrecomputeBackupManager.Properties.Settings;

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

            if (PrivateData.Default.PBAuthCode != "")
            {
                // Only if PB auth is provided
                backgroundWorkerPushBulletDecision.RunWorkerAsync(); // Start listening for responses
            }
        }
        
        public bool SaveDecision = false;
        void decideAction(DialogResult result, bool SaveDecision) {
            this.DialogResult = result;
            this.SaveDecision = SaveDecision;
            backgroundWorkerPushBulletDecision.CancelAsync();
        }

        private void btnTryAgain_Click(object sender, EventArgs e)
        {
            decideAction(DialogResult.OK, cbSave.Checked);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            decideAction(DialogResult.Cancel, cbSave.Checked);
            this.Close();
        }

        private void backgroundWorkerPushBulletDecision_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!e.Cancel) {
                if (question == null)
                {
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
                }
                else
                {

                    PushBulletAPI.PushNoteObject[] lastPushes = PushBulletAPI.Pushes.getLastMessages(10, question.created);

                    if (lastPushes != null && lastPushes.Length > 0)
                    {
                        // First is last. and should be for us.
                        PushBulletAPI.PushNoteObject note = lastPushes[1]; // 0 - Question, 1 - Answer

                        // original note can exist too (search is >= lastTime)
                        if (note.iden != question.iden)
                        {
                            string lower = note.body.ToLower();
                            bool tryFound = (lower.Contains("1") || lower.Contains("try"));
                            bool skipFound = (lower.Contains("2") || lower.Contains("skip"));
                            bool saveFound = lower.Contains("save");

                            if (tryFound && skipFound || (!tryFound && !skipFound))
                            {
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
                            else
                            {
                                // Send response
                                string finalResponse = saveFound ? " and I will remember it." : ".";
                                finalResponse =
                                    "Thank you!\n"
                                    + ((tryFound) ? "You chose to try again" : "You chose to skip the file")
                                    + finalResponse;
                                PushBulletAPI.Pushes.createPushNote("Backup BOT", finalResponse);

                                // Finish this dialog
                                if (tryFound)
                                {
                                    _parent.Log("User chose to try again. Save? " + saveFound);
                                    decideAction(DialogResult.OK, saveFound);
                                    return;
                                }
                                else if (skipFound)
                                {
                                    _parent.Log("User chose to skip file. Save? " + saveFound);
                                    decideAction(DialogResult.Cancel, saveFound);
                                    return;
                                }
                            }

                            
                        }

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
