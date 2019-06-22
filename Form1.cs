using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using Codeplex.Data;
using TagLib;
using TagLib.Id3v2;

namespace PostTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        protected HttpClient httpClient = new HttpClient();
        string joySound;
        string petitlyric;

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            string requestUri = "https://petitlyrics.com/lyrics/2730388";
            HttpResponseMessage response;
            string responseBody;
            var cancellationTokenSource = new CancellationTokenSource();

            response = await httpClient.GetAsync(requestUri, cancellationTokenSource.Token);
            responseBody = await response.Content.ReadAsStringAsync();

            var cookie = response.Headers.GetValues("Set-Cookie").First();

            Match lm = Regex.Match(responseBody, "<script type=\"text/javascript\" src=\"(/lib/pl-lib.js.+?)\" charset=\"UTF-8\"></script>", RegexOptions.Singleline);

            if (lm.Success == false)
                return;

            requestUri = "https://petitlyrics.com" + lm.Groups[1].Value;

            response = await httpClient.GetAsync(requestUri, cancellationTokenSource.Token);
            responseBody = await response.Content.ReadAsStringAsync();

            lm = Regex.Match(responseBody, "jqxhr.setRequestHeader\\('X-CSRF-Token', '(.+?)'\\)", RegexOptions.Singleline);

            if (lm.Success == false)
                return;

            requestUri = "https://petitlyrics.com/com/get_lyrics.ajax";

            var stringContent = new StringContent("lyrics_id=2730388", Encoding.UTF8, @"application/x-www-form-urlencoded");

            //stringContent.Headers.Add("Cookie", cookie);

            //stringContent.Headers.Add("Host", "petitlyrics.com");
            //stringContent.Headers.Add("Referer", "https://petitlyrics.com/lyrics/2730388");
            stringContent.Headers.Add("X-CSRF-Token", lm.Groups[1].Value);
            stringContent.Headers.Add("X-Requested-With", "XMLHttpRequest");

            //httpClient.DefaultRequestHeaders.Host = "petitlyrics.com";
            //httpClient.DefaultRequestHeaders.Add("Referer", "https://petitlyrics.com/lyrics/2730388");
            //httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            //httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            //httpClient.DefaultRequestHeaders.Add("Accept-Language", "ja,en-US;q=0.7,en;q=0.3");
            //httpClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
            //httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            //httpClient.DefaultRequestHeaders.Add("Host", "petitlyrics.com");
            //httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0");
            //httpClient.DefaultRequestHeaders.Add("Accept", "*/*");

            response = await httpClient.PostAsync(requestUri, stringContent, cancellationTokenSource.Token);
            petitlyric = await response.Content.ReadAsStringAsync();

            int i = 0;
        }

        private async void Button2_Click(object sender, EventArgs e)
        {
            string requestUri = "https://mspxy.joysound.com/Common/Lyric";
            HttpResponseMessage response;
            string responseBody;
            var cancellationTokenSource = new CancellationTokenSource();

            var stringContent = new StringContent("kind=naviGroupId&selSongNo=41743&interactionFlg=0&apiVer=1.0", Encoding.UTF8, @"application/x-www-form-urlencoded");

            //httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            //httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            //httpClient.DefaultRequestHeaders.Add("Accept-Language", "ja,en-US;q=0.7,en;q=0.3");
            //httpClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
            //httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            //httpClient.DefaultRequestHeaders.Add("Host", "mspxy.joysound.com");
            //httpClient.DefaultRequestHeaders.Add("X-JSP-APP-NAME", "0000800");
            //httpClient.DefaultRequestHeaders.Add("Origin", "https://www.joysound.com");
            //httpClient.DefaultRequestHeaders.Add("Referer", "https://www.joysound.com/web/search/song/41743");
            //httpClient.DefaultRequestHeaders.Add("TE", "Trailers");
            //httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0");
            stringContent.Headers.Add("X-JSP-APP-NAME", "0000800");
            stringContent.Headers.Add("Origin", "https://www.joysound.com");

            response = await httpClient.PostAsync(requestUri, stringContent, cancellationTokenSource.Token);
            joySound = await response.Content.ReadAsStringAsync();

            int i = 0;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            var json = DynamicJson.Parse(petitlyric);

            try
            {
                var s = json[0].lyrics;
            }
            catch
            {

            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            var json = DynamicJson.Parse(joySound);

            try
            {
                var s = json.lyricList[0].lyric;
            }
            catch
            {

            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            var tf = TagLib.File.Create(@"e:\tmp\dadata.mp3");

            var id3 = tf.GetTag(TagTypes.Id3v2) as TagLib.Id3v2.Tag;

            ReadOnlyByteVector USLT = "USLT";

            //UnsynchronisedLyricsFrame uslt;
            //foreach (var frame in id3.GetFrames(USLT))
            //{
            //    uslt = frame as UnsynchronisedLyricsFrame;
            //}

            //tf.Tag.Lyrics = "ascii?";

            //List<Frame> frames = new List<Frame>(id3.GetFrames(USLT));

            id3.RemoveFrames(USLT);

            UnsynchronisedLyricsFrame frame;

            frame = UnsynchronisedLyricsFrame.Get(id3, string.Empty, "eng", true);

            frame.Text = "今度こその新しい歌詞";

            tf.Save();
        }
    }
}
