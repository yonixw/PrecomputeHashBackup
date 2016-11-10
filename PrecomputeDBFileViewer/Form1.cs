using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PrecomputeDBFileViewer.SQLiteDatasetTableAdapters;

namespace PrecomputeDBFileViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dBToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        string dbPath = null;
        FilesTableAdapter adapFiles = null;
        FoldersTableAdapter adapFolders = null;

        private void openSql3DbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dlgOpen.ShowDialog() == DialogResult.OK) {
                dbPath = dlgOpen.FileName;

                adapFiles = new FilesTableAdapter();
                adapFiles.Connection.ConnectionString = "data source=\"" + dbPath + "\"";

                adapFolders = new FoldersTableAdapter();
                adapFolders.Connection.ConnectionString = "data source=\"" + dbPath + "\"";

                addFolders(-1, null);
            }
        }

        string strFrom64(string input)
        {
            // SO? 11743160
            return Encoding.UTF8.GetString(System.Convert.FromBase64String(input));
        }

        void addFolders(int id, TreeNode parent) {
            if (dbPath == null  || adapFolders == null) {
                Debug.Print("Null folder var setected. returning.");
            }

            SQLiteDataset.FoldersDataTable dt = adapFolders.GetDataByParentFolder(id);
            if (dt.Rows.Count == 0) return;

            TreeNodeCollection targetCollection = (parent != null) ? parent.Nodes : treeFolders.Nodes;

            foreach (SQLiteDataset.FoldersRow row in dt)
            {
                TreeNode tn = new TreeNode(strFrom64(row.FolderName) + " (" + row.FolderSize.ToString() + ")");
                tn.Tag = row.FolderId;
                targetCollection.Add(tn);

                // Add dummy for expanding on demand.
                TreeNode dummy = new TreeNode("Loading...");
                tn.Nodes.Add(dummy);
            }
        }

      

        void showFiles(int folderid) {
            lsvFiles.Items.Clear();

            if (dbPath == null || adapFiles == null)
            {
                Debug.Print("Null files var setected. returning.");
            }

            SQLiteDataset.FilesDataTable dt = adapFiles.GetDataByFolderID(folderid);
            if (dt.Rows.Count == 0) return;

            foreach (SQLiteDataset.FilesRow row in dt)
            {
                ListViewItem lvi = new ListViewItem(new[] { strFrom64(row.FileName), row.FileHash, row.FileSize.ToString()});
                lsvFiles.Items.Add(lvi);
            }

            foreach( ColumnHeader col in  lsvFiles.Columns  )
            {
                col.Width = -2; // max item. // -1 is for columnt header text
            }

        }

        private void treeFolders_AfterExpand(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            if (node.Nodes[0].Text == "Loading...") {
                node.Nodes.RemoveAt(0);
                addFolders((int)node.Tag, node);
            }
        }

        private void treeFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            showFiles((int)e.Node.Tag);
        }
    }
}
