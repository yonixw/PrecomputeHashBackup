using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrecomputeDBFileViewer
{
    public partial class Duplicates : Form
    {
        public Duplicates()
        {
            InitializeComponent();
        }

        private void Duplicates_Load(object sender, EventArgs e)
        {
            bwLoadDuplicates.RunWorkerAsync();
        }
    }
}
