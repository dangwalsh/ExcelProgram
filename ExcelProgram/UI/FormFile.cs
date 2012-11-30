using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

using Autodesk.Revit.UI;

namespace ExcelProgram
{
    public partial class FormFile : Form
    {
        private ProjSet _ProjSet;
        private static string _Filename;
        private static string _NameCol;
        private static string _AreaCol;
        private static string _CountCol;
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

        public static string NameCol
        {
            get { return _NameCol; }
            set { _NameCol = value; }
        }

        public static string AreaCol
        {
            get { return _AreaCol; }
            set { _AreaCol = value; }
        }

        public static string CountCol
        {
            get { return _CountCol; }
            set { _CountCol = value; }
        }

        public FormFile(ProjSet p)
        {
            InitializeComponent();
            this.progressBar.Hide();
            this.numericPadX.Value = 8.0M;
            this.numericPadY.Value = 4.0M;
            _ProjSet = p;
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

        private void UpdateFields()
        {
            this.textBoxName.Text = _NameCol;
            this.textBoxArea.Text = _AreaCol;
            this.textBoxCount.Text = _CountCol;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                if (!FileSelect())
                {
                    throw new Exception("Failed to open file!");
                }
                else
                {
                    DataParser dataParser = new DataParser(_Filename);
                    FormTable formTable = new FormTable(ref dataParser);
                    formTable.ShowDialog();
                    UpdateFields();
                }
            }

            catch (Exception ex)
            {
                TaskDialog.Show("File Error", ex.Message);
            }

            labelPath.Text = _Filename;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            this.btnBrowse.Hide();
            this.btnClose.Hide();
            this.btnImport.Hide();
            this.progressBar.Show();
            this.progressBar.Minimum = 1;
            this.progressBar.Maximum = ShapeMaker.Shapes.Count;
            this.progressBar.Value = 1;
            this.progressBar.Step = 1;

            foreach (Shape s in ShapeMaker.Shapes)
            {
                this.progressBar.PerformStep();
                ModelFactory sf = new ModelFactory(_ProjSet.Doc, s);
                sf.Make();
            }

            this.progressBar.Hide();
            this.btnBrowse.Show();
            this.btnClose.Show();
            this.btnImport.Show();
            this.progressBar.Value = 1;
        }

        private void numericPadX_ValueChanged(object sender, EventArgs e)
        {
            _xPad = Convert.ToDouble(this.numericPadX.Value);
        }

        private void numericPadY_ValueChanged(object sender, EventArgs e)
        {
            _yPad = Convert.ToDouble(this.numericPadY.Value);
        }
    }
}
