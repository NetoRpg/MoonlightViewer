using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ImageViewer
{
    public partial class MoonlightViewer : Form
    {
        public MoonlightViewer()
        {
            InitializeComponent();
        }

        int index = 0;
        List<string> files;
        FolderBrowserDialog fbdialog = new FolderBrowserDialog();

        Bitmap LoadBitmap(string path)
        {
            //using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            //using (BinaryReader reader = new BinaryReader(stream))
            //{
            //    MemoryStream memoryStream = new MemoryStream(reader.ReadBytes((int)stream.Length));
            //    return new Bitmap(memoryStream);
            //}

            MemoryStream ms = new MemoryStream(File.ReadAllBytes(path));
            return Bitmap.FromStream(ms) as Bitmap;
        }

        private void btnListar_Click(object sender, EventArgs e)
        {
            DialogResult result = fbdialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.files = new List<string>(Directory.GetFiles(fbdialog.SelectedPath, "*.jpg"));
                pbMain.Image = null;
                lblSize.Text = "0 de 0";
                this.index = 0;
                changePicture();
            } 
        }


        private void changePicture(Keys key = Keys.Escape) {

            if (this.files == null || this.files.Count <= 0)
                return;

            switch(key){
                case Keys.Right:
                    if (this.index < this.files.Count -1)
                        this.index++;
                    break;
                case Keys.Left:
                    if(this.index > 0)
                        this.index--;
                    break;
            }
            

            Bitmap bitmap = LoadBitmap(files[this.index]);

            Size size = bitmap.Size;

            if (size.Width > this.panel.Width)
            {
                int height = (int)((double)(this.panel.Width) / ((double)bitmap.Width / (double)bitmap.Height));
                size = new Size((this.panel.Width), height);
            }

            if (size.Height > this.panel.Height)
            {
                int width = (int)((double)(this.panel.Height) * ((double)bitmap.Width / (double)bitmap.Height));
                size = new Size(width, (this.panel.Height));
            }

            if(size != bitmap.Size)
                bitmap = new Bitmap(bitmap, size);

            pbMain.Size = bitmap.Size;

            pbMain.Image = bitmap;

            pbMain.Location = new Point(
                this.panel.Width / 2 - pbMain.Size.Width / 2,
                this.panel.Height / 2 - pbMain.Size.Height / 2
            );

            this.Text = String.Format("Moonlight Viewer - {0}",Path.GetFileName (files[index]));
            this.lblSize.Text = String.Format("{0} de {1}", this.index + 1, files.Count);
        }


        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Left || keyData == Keys.Right)
            {
                changePicture(keyData);
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            changePicture();
        }

        private void MoonlightViewer_Load(object sender, EventArgs e)
        {
            lblSize.Text = String.Empty;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (this.files == null || this.files.Count <= 0)
                return;

           string path = Path.Combine(fbdialog.SelectedPath, "Removed");

           if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Directory.Move(files[index], Path.Combine(path, Path.GetFileName(files[index])));

            this.files.RemoveAt(this.index);
            changePicture();
        }



    }
}
