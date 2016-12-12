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

        PushBulletAPI.PushNoteObject questionPushNoteID;
        private void frmBackupErrorDecision_Load(object sender, EventArgs e)
        {
            this.rtbDetails.Text =
                "File path:\n" +
                _filename + "\n\n" +
                "Error details:\n" +
                _ex.Message + "\n\n" +
                _ex.StackTrace
                ;

            if (frmMain.pbAuth != "")
            {
                // Only if PB auth is provided
                backgroundWorkerPushBulletDecision.RunWorkerAsync(); // Start listening for responses
            }
        }

        public bool SaveDecision = false;
        void decideAction(DialogResult result, bool SaveDecision)
        {
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

        const string myFormPushNoteTitle = "Backup BOT Update";

        private PushBulletAPI.PushNoteObject myCustomNote(string questionText) {
            // Add general response information.
            questionText += questionText
                + "\n\n"
                + "===================\n"
                + "Please respond with text:\n"
                + "\t* 1 or try to retry uploading\n"
                + "\t* 2 or skip to skip the file (but saved in log)\n"
                + "\t* add `save` to your response to save this action for all future errors"
            ;
            return PushBulletAPI.Pushes.createPushNote(myFormPushNoteTitle, questionText);
        }

        private void backgroundWorkerPushBulletDecision_DoWork(object sender, DoWorkEventArgs e)
        {
            string currentQuestion =
                    "Error uploading file:\n"
                    + _filename + "\n\n"
                    + "Error message:\n"
                    + _ex.Message;

            while (!e.Cancel)
            {
                if (questionPushNoteID == null) // Error or Calrification message could not sent.
                {
                    questionPushNoteID = myCustomNote( currentQuestion);
                }
                else
                {

                    PushBulletAPI.PushNoteObject[] lastPushes = PushBulletAPI.Pushes.getLastMessages(10, questionPushNoteID.created);

                    if (lastPushes == null) continue; // Wait 1 second before trying again.

                    List<PushBulletAPI.PushNoteObject> userResponseNotes = new List<PushBulletAPI.PushNoteObject>();

                    // Ignore and filter bots updates:
                    foreach (PushBulletAPI.PushNoteObject note in lastPushes)
                    {
                        if (
                        // When to Ignore:
                        /* 1) If iden is of the question*/ note.iden == questionPushNoteID.iden ||
                        /* 2) Has title (not null or empty) */ 
                            (note.title != null && note.title != "")
                        )
                        {
                            // Ignore
                        }
                        else {
                            userResponseNotes.Add(note);
                        }
                    }


                    if (userResponseNotes.Count > 1)
                    {
                        // Cant know. Ask again.
                        currentQuestion =
                            "Found multiple responses, Please send a single response again.";

                        // if null than will be asked again on next while iteration.
                        questionPushNoteID = myCustomNote( currentQuestion);
                    }
                    else if (userResponseNotes.Count == 1)
                    {
                        string lowerNoteBody = userResponseNotes[0].body.ToLower();
                        bool tryFound = (lowerNoteBody.Contains("1") || lowerNoteBody.Contains("try"));
                        bool skipFound = (lowerNoteBody.Contains("2") || lowerNoteBody.Contains("skip"));
                        bool saveFound = lowerNoteBody.Contains("save");

                        if (tryFound && skipFound || (!tryFound && !skipFound))
                        {
                            // Ask again:
                            currentQuestion =
                                "Can't understand your response. Please try again.";

                            // if null than will be asked again on next while iteration.
                            questionPushNoteID = myCustomNote( currentQuestion);
                        }
                        else
                        {
                            // Log the decisive note for bug inspecting.
                            _parent.Log("Deciding dialog found answer! \n\n" + userResponseNotes[0].ToString());

                            // Send response
                            string finalResponse = saveFound ? " and I will remember it." : ".";
                            finalResponse =
                                "Thank you!\n"
                                + ((tryFound) ? "You chose to try again" : "You chose to skip the file")
                                + finalResponse;

                            // We use the main form auto-retry for final note because
                            //      we already got our answer.
                            _parent.AddPushBulletNoteToQueue(myFormPushNoteTitle, finalResponse);

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
                            else
                            {
                                _parent.Log("Error, skip not found and try not found! ");
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
