using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YoutDown
{
    public partial class FormPrincipal : Form
    {
        Chave c = new Chave();

        private string url = "";

        public Label L1;
        public RoundedPanel R1;
        public RoundedPanel R2;
        PictureBox pictureBoxThumb = new PictureBox();
        private int VezesBaixada = 0;
        public FormPrincipal()
        {
            InitializeComponent();
            this.Padding = new Padding(40);
            this.Size = new Size(800, 700);
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.BackColor = c.Netro02;
            start();
        }

        private void start()
        {
            // 🔴 Header Vermelho
            R1 = CriarRoundedPanel(c.Vermelho, DockStyle.Top, 320, 32);

            // 🧾 Seção do Formulário
            R2 = CriarRoundedPanel(c.Netro01, DockStyle.Top, AutoSize: true, radius: 32, padding: new Padding(24));

            // Título com ícone
            L1 = CriarLabel("Link do Vídeo:", c.H1_Font, c.Netro04, DockStyle.Left);
            Panel icone = new Panel
            {
                Dock = DockStyle.Fill,
                BackgroundImage = Properties.Resources.IconLink,
                BackgroundImageLayout = ImageLayout.Stretch,
            };
            Panel containerIcone = new Panel
            {
                Dock = DockStyle.Left,
                Width = 40,
                Padding = new Padding(4),
            };
            containerIcone.Controls.Add(icone);

            Panel ContainerH1 = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
            };
            ContainerH1.Controls.Add(L1);
            ContainerH1.Controls.Add(containerIcone);

            // Subtítulo
            Label L1_sub = CriarLabel(
                "Cole o link do vídeo do YouTube que você deseja baixar:",
                c.H1_Sub_Font, c.Netro03, DockStyle.Top);

            // Texto "URL do YouTube"
            Label L2 = CriarLabel("URL do YouTube:", c.Button_Font, c.Netro03, DockStyle.Top);

            // Campo de entrada
            TextBox T1 = new TextBox
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24),
                Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = c.Netro03,
                BackColor = c.Netro02,
                BorderStyle = BorderStyle.None,
                Height = 40,
            };

            RoundedPanel T1Box = CriarRoundedPanel(c.Netro02, DockStyle.Left, 0, 16, new Padding(16));
            T1Box.Width = this.Width - (24 * 2) - 80 - 120 - 16 - 8;
            T1Box.Controls.Add(T1);

            // Botão de Verificação
            BotaoArredondado B1 = new BotaoArredondado
            {
                Text = "Verificar",
                FlatAppearance = { BorderSize = 0 },
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Right,
                Radius = 16,
                Width = 120,
                BackColor = c.Vermelho,
                ForeColor = c.Netro01,
                Font = c.Button_Font,
            };

            // Container do input + botão
            Panel Container01 = new Panel
            {
                Dock = DockStyle.Top,
                Height = 48
            };
            Container01.Controls.Add(B1);
            Container01.Controls.Add(T1Box);

            // Montagem da hierarquia
            R2.Controls.Add(Container01);
            R2.Controls.Add(RetornaPaddingTop(8));
            R2.Controls.Add(L2);
            R2.Controls.Add(RetornaPaddingTop(24));
            R2.Controls.Add(L1_sub);
            R2.Controls.Add(RetornaPaddingTop(8));
            R2.Controls.Add(ContainerH1);

            // Adição final à tela
            this.Controls.Add(R2);
            this.Controls.Add(RetornaPaddingTop(16));
            this.Controls.Add(R1);


            B1.Click += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(T1.Text))
                {
                    MessageBox.Show("Por favor, insira um link válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                btnBuscas(T1.Text);

            };


        }
        private async void btnBaixarMp4(string url)
        {
            var youtube = new YoutubeService();
            await youtube.BaixarMp4ComDialogAsync(url);
        }

        private async void btnBuscas(string url)
        {
            var youtube = new YoutubeService();
            var (titulo, thumb) = await youtube.BuscarVideoAsync(url);

            if (string.IsNullOrWhiteSpace(titulo))
            {
                MessageBox.Show("Vídeo não encontrado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            L1.Text = titulo;



            R1.Height = 160;
            pictureBoxThumb.Image = thumb;

            pictureBoxThumb.Dock = DockStyle.Fill;
            pictureBoxThumb.SizeMode = PictureBoxSizeMode.CenterImage;
            R1.Controls.Add(pictureBoxThumb);
            this.url = url;
            VezesBaixada++;
            if (VezesBaixada < 2)
                Continua();
        }

        public void Continua()
        {

            Panel padding = RetornaPaddingTop(24);

            // Painel principal de opções
            Panel painelFormatos = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(0)
            };

            // Label de instrução
            Label formatoLabel = CriarLabel("Formato de Download:", c.Button_Font, c.Netro03, DockStyle.Top);

            // RadioButton MP4
            RadioButton rbMp4 = new RadioButton
            {
                Text = "MP4",
                Font = c.Button_Font,
                ForeColor = c.Netro03,
                Checked = true,
                AutoSize = true
            };

            // RadioButton MP3
            RadioButton rbMp3 = new RadioButton
            {
                Text = "MP3",
                Font = c.Button_Font,
                ForeColor = c.Netro03,
                AutoSize = true
            };

            // ComboBox de resoluções
            ComboBox cbResolucao = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                ForeColor = c.Netro03,
                BackColor = c.Netro01,
                Width = 120,
                Visible = true // só visível quando MP4 estiver marcado
            };
            cbResolucao.Items.AddRange(new object[] { "1080p", "720p", "480p", "360p" });
            cbResolucao.SelectedIndex = 1; // padrão: 720p

            // Container para RadioButtons
            FlowLayoutPanel containerRadios = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
            };
            containerRadios.Controls.Add(rbMp4);
            containerRadios.Controls.Add(rbMp3);
            containerRadios.Controls.Add(cbResolucao);

            // Botão de download
            BotaoArredondado btnDownload = new BotaoArredondado
            {
                Text = "Baixar",
                Radius = 16,
                Width = 120,
                Height = 40,
                BackColor = c.Vermelho,
                ForeColor = c.Netro01,
                Font = c.Button_Font,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Dock = DockStyle.Top,
            };

            btnDownload.Click += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    MessageBox.Show("URL inválida", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var youtube = new YoutubeService();

                if (rbMp4.Checked)
                    await youtube.BaixarMp4ComDialogAsync(url, cbResolucao.SelectedItem?.ToString() ?? "720p");
                else
                    await youtube.BaixarMp3ComDialogAsync(url);
            };

            // Atualiza visibilidade do ComboBox com base no formato
            rbMp4.CheckedChanged += (s, e) => cbResolucao.Visible = rbMp4.Checked;

            // Monta o painel de opções
            painelFormatos.Controls.Add(btnDownload);
            painelFormatos.Controls.Add(RetornaPaddingTop(8));
            painelFormatos.Controls.Add(containerRadios);
            painelFormatos.Controls.Add(formatoLabel);
            painelFormatos.Controls.Add(padding);

            // Adiciona ao R2 como bloco único
            R2.Controls.Add(painelFormatos);
            R2.Controls.SetChildIndex(painelFormatos, 0);

            Panel linha = RetornalinhaTop();
            R2.Controls.Add(linha);
            R2.Controls.SetChildIndex(linha, 1);

        }






        // 🔧 Métodos auxiliares

        private Label CriarLabel(string texto, Font fonte, Color cor, DockStyle dock) =>
            new Label
            {
                Text = texto,
                Font = fonte,
                ForeColor = cor,
                Dock = dock,
                AutoSize = true
            };

        private RoundedPanel CriarRoundedPanel(Color cor, DockStyle dock, int height = 0, int radius = 0, Padding? padding = null, bool AutoSize = false)
        {
            return new RoundedPanel
            {
                BackColor = cor,
                Dock = dock,
                Radius = radius,
                Height = height,
                AutoSize = AutoSize,
                Padding = padding ?? new Padding(0)
            };
        }

        private Panel RetornaPaddingTop(int h) =>
            new Panel
            {
                Dock = DockStyle.Top,
                Height = h
            };

        private Panel RetornalinhaTop()
        {
            Panel linha = new Panel
            {
                Dock = DockStyle.Top,
                Height = 2,
                BackColor = c.Netro02

            };

            Panel Panelbox = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
            };

            Panelbox.Controls.Add(RetornaPaddingTop(8));
            Panelbox.Controls.Add(linha);
            Panelbox.Controls.Add(RetornaPaddingTop(24));
            return Panelbox;

        }

    }
}
