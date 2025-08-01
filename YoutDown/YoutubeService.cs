using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;

public class YoutubeService
{
    private readonly YoutubeClient youtube;
    private readonly string ffmpegPath;

    public YoutubeService()
    {
        youtube = new YoutubeClient();
        ffmpegPath = ObterOuExtrairFfmpeg();
    }

    public async Task<(string titulo, Image thumb)> BuscarVideoAsync(string url)
    {
        try
        {
            var video = await youtube.Videos.GetAsync(url);
            var thumbUrl = video.Thumbnails.GetWithHighestResolution()?.Url;

            if (string.IsNullOrEmpty(video.Title) || string.IsNullOrEmpty(thumbUrl))
                return (null, null);

            using (var wc = new WebClient())
            using (var stream = new MemoryStream(wc.DownloadData(thumbUrl)))
            {
                var imagem = Image.FromStream(stream);
                return (video.Title, imagem);
            }
        }
        catch
        {
            return (null, null);
        }
    }

    public async Task BaixarMp4ComDialogAsync(string url, string resolucao = "720p")
    {
        var video = await youtube.Videos.GetAsync(url);
        var manifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);

        var videoStream = manifest.GetVideoOnlyStreams()
            .FirstOrDefault(s => s.VideoQuality.Label == resolucao) ??
            manifest.GetVideoOnlyStreams().GetWithHighestVideoQuality();

        var audioStream = manifest.GetAudioOnlyStreams().GetWithHighestBitrate();

        if (videoStream == null || audioStream == null)
            throw new Exception("Streams não encontrados.");

        using (var dialog = new SaveFileDialog
        {
            Title = "Salvar vídeo MP4",
            Filter = "Vídeo MP4 (*.mp4)|*.mp4",
            FileName = $"{SanitizeFileName(video.Title)}.mp4"
        })
        {
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            string outputPath = dialog.FileName;
            string videoPath = Path.GetTempFileName() + $".{videoStream.Container.Name}";
            string audioPath = Path.GetTempFileName() + $".{audioStream.Container.Name}";

            await youtube.Videos.Streams.DownloadAsync(videoStream, videoPath);
            await youtube.Videos.Streams.DownloadAsync(audioStream, audioPath);

            string args = $"-i \"{videoPath}\" -i \"{audioPath}\" -c:v copy -c:a aac -strict experimental \"{outputPath}\"";
            await ExecutarFfmpegAsync(args);

            File.Delete(videoPath);
            File.Delete(audioPath);

            MessageBox.Show("Vídeo baixado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    public async Task BaixarMp3ComDialogAsync(string url)
    {
        var video = await youtube.Videos.GetAsync(url);
        var manifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
        var audioStream = manifest.GetAudioOnlyStreams().GetWithHighestBitrate();

        if (audioStream == null)
            throw new Exception("Áudio não encontrado.");

        using (var dialog = new SaveFileDialog
        {
            Title = "Salvar áudio MP3",
            Filter = "Áudio MP3 (*.mp3)|*.mp3",
            FileName = $"{SanitizeFileName(video.Title)}.mp3"
        })
        {
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            string audioPath = Path.GetTempFileName() + $".{audioStream.Container.Name}";
            string outputPath = dialog.FileName;

            await youtube.Videos.Streams.DownloadAsync(audioStream, audioPath);

            string args = $"-i \"{audioPath}\" -vn -ab 192k -ar 44100 -y \"{outputPath}\"";
            await ExecutarFfmpegAsync(args);

            File.Delete(audioPath);

            MessageBox.Show("Áudio MP3 baixado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private async Task ExecutarFfmpegAsync(string args)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        process.OutputDataReceived += (s, e) => Console.WriteLine(e.Data);
        process.ErrorDataReceived += (s, e) => Console.WriteLine(e.Data);

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await Task.Run(() => process.WaitForExit());
    }

    private string SanitizeFileName(string title)
    {
        return string.Join("_", title.Split(Path.GetInvalidFileNameChars()));
    }

    private string ObterOuExtrairFfmpeg()
    {
        string pastaDocumentos = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string pastaAlvo = Path.Combine(pastaDocumentos, "YoutDown");

        if (!Directory.Exists(pastaAlvo))
            Directory.CreateDirectory(pastaAlvo);

        string caminhoFfmpeg = Path.Combine(pastaAlvo, "ffmpeg.exe");

        if (!File.Exists(caminhoFfmpeg))
        {
            string nomeRecurso = "YoutubeProjeto.Resources.ffmpeg.exe";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(nomeRecurso))
            {
                if (stream == null)
                    throw new Exception("ffmpeg.exe não encontrado nos recursos.");

                using (var file = new FileStream(caminhoFfmpeg, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(file);
                }
            }
        }

        return caminhoFfmpeg;
    }
}