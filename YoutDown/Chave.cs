using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YoutDown
{
    internal class Chave
    {
        public Color Vermelho = Color.FromArgb(239, 74, 54);

        public Color Netro01 = Color.FromArgb(253, 253, 253);
        public Color Netro02 = Color.FromArgb(224, 229, 235);
        public Color Netro03 = Color.FromArgb(115, 123, 140);
        public Color Netro04 = Color.FromArgb(22, 24, 29);


        public Font H1_Font { get { return new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point, 0); } }
        public Font H1_Sub_Font { get { return new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point, 0); } }
        public Font Button_Font { get { return new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0); } }
    }

    public class RoundedPanel : Panel
    {
        private int radius = 1;
        public RoundedPanel() { DoubleBufferedPanel(); }



        public int Radius { get { return radius; } set { radius = value; } }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(Width - radius, Height - radius, radius, radius, 0, 90);
            path.AddArc(0, Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            this.Region = new Region(path);
        }

        public void DoubleBufferedPanel()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.UpdateStyles();
        }

    }
    public class BotaoArredondado : Button
    {
        public int _radius = 1;

        public int Radius { get { return _radius; } set { _radius = value; } }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, _radius, _radius, 180, 90);
            path.AddArc(Width - _radius, 0, _radius, _radius, 270, 90);
            path.AddArc(Width - _radius, Height - _radius, _radius, _radius, 0, 90);
            path.AddArc(0, Height - _radius, _radius, _radius, 90, 90);
            path.CloseFigure();
            this.Region = new Region(path);
        }
    }
    public partial class FormArredondado : Form
    {
        public FormArredondado()
        {
            this.Paint += new PaintEventHandler(DesenharBordasArredondadas);
        }

        private void DesenharBordasArredondadas(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Rectangle bounds = this.ClientRectangle;
            int radius = 18;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(bounds.X, bounds.Y, radius, radius, 180, 90); // Canto superior esquerdo
                path.AddArc(bounds.X + bounds.Width - radius, bounds.Y, radius, radius, 270, 90); // Canto superior direito
                path.AddArc(bounds.X + bounds.Width - radius, bounds.Y + bounds.Height - radius, radius, radius, 0, 90); // Canto inferior direito
                path.AddArc(bounds.X, bounds.Y + bounds.Height - radius, radius, radius, 90, 90); // Canto inferior esquerdo
                path.CloseFigure();

                // Preenche o formulário com a cor de fundo
                graphics.FillPath(new SolidBrush(this.BackColor), path);
                this.Region = new Region(path);
            }
        }
    }
}
