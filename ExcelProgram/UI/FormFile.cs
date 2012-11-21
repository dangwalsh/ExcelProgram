using System;
using System.Windows.Forms;

using Autodesk.Revit.UI;

namespace ExcelProgram
{
    public partial class FormFile : Form
    {
        private ProjSet _ProjSet;
        private static string _Filename;
        private static double _xPad;
        private static double _yPad;

        public static double xPad
        {
            get { return _xPad; }
        }

        public static double yPad
        {
            get { return _yPad; }
        }

        public FormFile(ProjSet p)
        {
            InitializeComponent();
            this.progressBar.Hide();
            _ProjSet = p;     
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                if (!FileSelect())
                {
                    throw new Exception("Failed to open file!");
                }
            }

            catch (Exception ex)
            {
                TaskDialog.Show("File Error", ex.Message);
            }

            labelPath.Text = _Filename;
        }

        static bool FileSelect()
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Title = "Select Program Data File";
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.Filter = ".txt Files (*.txt)|*.txt";
            bool result = (DialogResult.OK == dlg.ShowDialog());
            _Filename = dlg.FileName;

            return result;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            _xPad = Convert.ToDouble(this.numericPadX.Value);
            _yPad = Convert.ToDouble(this.numericPadY.Value);

            Parser parser = new Parser(_Filename, 
                                       (int)this.numericName.Value,
                                       (int)this.numericArea.Value,
                                       (int)this.numericCount.Value);

            this.btnBrowse.Hide();
            this.btnClose.Hide();
            this.btnImport.Hide();
            this.progressBar.Show();
            this.progressBar.Minimum = 1;
            this.progressBar.Maximum = parser.ProgramShapes.Count;
            this.progressBar.Value = 1;
            this.progressBar.Step = 1;

            double spacing = 0.0;
            foreach (Shape s in parser.ProgramShapes)
            {
                this.progressBar.PerformStep();
                MassFactory sf = new MassFactory(_ProjSet.Doc, s, spacing);
                spacing = sf.Make();
            }
            this.progressBar.Hide();
            this.btnBrowse.Show();
            this.btnClose.Show();
            this.btnImport.Show();
            this.progressBar.Value = 1;
        }
    }
}
