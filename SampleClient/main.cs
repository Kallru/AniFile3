using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using MonoTorrent.Common;
using MonoTorrent.Client;
using System.Net;
using System.Diagnostics;
using System.Threading;
using MonoTorrent.BEncoding;
using MonoTorrent.Client.Encryption;
using MonoTorrent.Client.Tracker;

#if !DISABLE_DHT
using MonoTorrent.Dht;
using MonoTorrent.Dht.Listeners;
#endif

namespace MonoTorrent
{
    class main
    {
        static string dhtNodeFile;
        static string basePath;
        static string downloadsPath;
        static string fastResumeFile;
        static string torrentsPath;
        static ClientEngine engine;				// The engine used for downloading
        static List<TorrentManager> torrents;	// The list where all the torrentManagers will be stored that the engine gives us
        static Top10Listener listener;			// This is a subclass of TraceListener which remembers the last 20 statements sent to it

        static void Main(string[] args)
        {
            /* Generate the paths to the folder we will save .torrent files to and where we download files to */
            basePath = Environment.CurrentDirectory;						// This is the directory we are currently in
            torrentsPath = Path.Combine(basePath, "Torrents");				// This is the directory we will save .torrents to
            downloadsPath = Path.Combine(basePath, "Downloads");			// This is the directory we will save downloads to
            fastResumeFile = Path.Combine(torrentsPath, "fastresume.data");
            dhtNodeFile = Path.Combine(basePath, "DhtNodes");
            torrents = new List<TorrentManager>();							// This is where we will store the torrentmanagers
            listener = new Top10Listener(10);

            // We need to cleanup correctly when the user closes the window by using ctrl-c
            // or an unhandled exception happens
            Console.CancelKeyPress += delegate { shutdown(); };
            AppDomain.CurrentDomain.ProcessExit += delegate { shutdown(); };
            AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e) { Console.WriteLine(e.ExceptionObject); shutdown(); };
            Thread.GetDomain().UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e) { Console.WriteLine(e.ExceptionObject); shutdown(); };

            StartEngine();
        }

        private static void StartEngine()
        {
            int port;
            Torrent torrent = null;
            // Ask the user what port they want to use for incoming connections
            Console.Write(Environment.NewLine + "Choose a listen port: ");
            while (!Int32.TryParse(Console.ReadLine(), out port)) { }



            // Create the settings which the engine will use
            // downloadsPath - this is the path where we will save all the files to
            // port - this is the port we listen for connections on
            EngineSettings engineSettings = new EngineSettings(downloadsPath, port);
            engineSettings.PreferEncryption = false;
            engineSettings.AllowedEncryption = EncryptionTypes.All;

            //engineSettings.GlobalMaxUploadSpeed = 30 * 1024;
            //engineSettings.GlobalMaxDownloadSpeed = 100 * 1024;
            //engineSettings.MaxReadRate = 1 * 1024 * 1024;


            // Create the default settings which a torrent will have.
            // 4 Upload slots - a good ratio is one slot per 5kB of upload speed
            // 50 open connections - should never really need to be changed
            // Unlimited download speed - valid range from 0 -> int.Max
            // Unlimited upload speed - valid range from 0 -> int.Max
            TorrentSettings torrentDefaults = new TorrentSettings(4, 150, 0, 0);

            // Create an instance of the engine.
            engine = new ClientEngine(engineSettings);
            engine.ChangeListenEndpoint(new IPEndPoint(IPAddress.Any, port));
#if !DISABLE_DHT
            byte[] nodes = null;
            try
            {
                nodes = File.ReadAllBytes(dhtNodeFile);
            }
            catch
            {
                Console.WriteLine("No existing dht nodes could be loaded");
            }

            DhtListener dhtListner = new DhtListener (new IPEndPoint (IPAddress.Any, port));
            DhtEngine dht = new DhtEngine (dhtListner);
            engine.RegisterDht(dht);
            dhtListner.Start();
            engine.DhtEngine.Start(nodes);
#endif
            // If the SavePath does not exist, we want to create it.
            if (!Directory.Exists(engine.Settings.SavePath))
                Directory.CreateDirectory(engine.Settings.SavePath);

            // If the torrentsPath does not exist, we want to create it
            if (!Directory.Exists(torrentsPath))
                Directory.CreateDirectory(torrentsPath);

            BEncodedDictionary fastResume;
            try
            {
                fastResume = BEncodedValue.Decode<BEncodedDictionary>(File.ReadAllBytes(fastResumeFile));
            }
            catch
            {
                fastResume = new BEncodedDictionary();
            }

            // For each file in the torrents path that is a .torrent file, load it into the engine.
            foreach (string file in Directory.GetFiles(torrentsPath))
            {
                if (file.EndsWith(".torrent"))
                {
                    try
                    {
                        // Load the .torrent from the file into a Torrent instance
                        // You can use this to do preprocessing should you need to
                        torrent = Torrent.Load(file);
                        Console.WriteLine(torrent.InfoHash.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.Write("Couldn't decode {0}: ", file);
                        Console.WriteLine(e.Message);
                        continue;
                    }

                    //string magnetLink = @"magnet:?xt=urn:btih:B4D3A91D9CE527AA7C5B6EDACB969E5486B76EF2&dn= tvN  ªÔΩ√ºº≥¢ πŸ¥Ÿ∏Ò¿Â∆Ì.E09.170929.720p-NEXT&tr=http://720pier.ru/tracker/announce.php?passkey=3t3mrdei39bxn7x9114tv9vy1reubs8g&tr=http://mgtracker.org:2710/announce&tr=http://tracker.kamigami.org:2710/announce&tr=http://tracker.mg64.net:6881/announce&tr=http://tracker2.wasabii.com.tw:6969/announce&tr=udp://9.rarbg.me:2720/announce&tr=udp://bt.xxx-tracker.com:2710/announce&tr=udp://opentrackr.org:1337/announce&tr=udp://www.eddie4.nl:6969/announce&tr=udp://zer0day.to:1337/announce&tr=udp://tracker1.wasabii.com.tw:6969/announce&tr=udp://185.50.198.188:1337/announce&tr=udp://tracker.leechers-paradise.org:6969&tr=udp://tracker2.indowebster.com:6969/announce&tr=udp://tracker.coppersurfer.tk:6969&tr=http://bt.ttk.artvid.ru:6969/announce&tr=http://bt.artvid.ru:6969/announce&tr=udp://thetracker.org./announce&tr=udp://tracker4.piratux.com:6969/announce&tr=udp://tracker.zer0day.to:1337/announce&tr=udp://62.212.85.66:2710/announce&tr=udp://eddie4.nl:6969/announce&tr=udp://public.popcorn-tracker.org:6969/announce&tr=udp://tracker.grepler.com:6969/announce&tr=http://tracker.dler.org:6969/announce&tr=http://tracker.tiny-vps.com:6969/announce&tr=http://tracker.filetracker.pl:8089/announce&tr=http://tracker.tvunderground.org.ru:3218/announce&tr=http://tracker.grepler.com:6969/announce&tr=http://tracker.kuroy.me:5944/announce&tr=http://210.244.71.26:6969/announce";
                    //string magnetLink = @"magnet:?xt=urn:btih:B4D3A91D9CE527AA7C5B6EDACB969E5486B76EF2";
                    string magnetLink = @"magnet:?xt=urn:btih:4E4CBCD360FEC29E86E1F65B8C7E2AAB241B947F&dn=%ec%b6%94%eb%b8%94%eb%a6%ac%eb%84%a4%ea%b0%80%20%eb%96%b4%eb%8b%a4.E06.END.170930.720p-NEXT.mp4&tr=%2audp%3a%2f%2fzer0day.to%3a1337%2fannounce&tr=udp%3a%2f%2ftracker1.wasabii.com.tw%3a6969%2fannounce&tr=http%3a%2f%2fmgtracker.org%3a6969%2fannounce&tr=udp%3a%2f%2ftracker.grepler.com%3a6969%2fannounce&tr=http%3a%2f%2ftracker.kamigami.org%3a2710%2fannounce&tr=udp%3a%2f%2f182.176.139.129%3a6969%2fannounce&tr=http%3a%2f%2ftracker.mg64.net%3a6881%2fannounce&tr=udp%3a%2f%2f185.50.198.188%3a1337%2fannounce&tr=udp%3a%2f%2f168.235.67.63%3a6969%2fannounce&tr=udp%3a%2f%2ftracker.leechers-paradise.org%3a6969&tr=udp%3a%2f%2fbt.xxx-tracker.com%3a2710%2fannounce&tr=udp%3a%2f%2fexplodie.org%3a6969%2fannounce&tr=udp%3a%2f%2ftracker.coppersurfer.tk%3a6969%2fannounce&tr=udp%3a%2f%2ftracker.ilibr.org%3a80%2fannounce&tr=udp%3a%2f%2ftracker.coppersurfer.tk%3a6969&tr=http%3a%2f%2fbt.ttk.artvid.ru%3a6969%2fannounce&tr=http%3a%2f%2fbt.artvid.ru%3a6969%2fannounce&tr=http%3a%2f%2ftracker2.wasabii.com.tw%3a6969%2fannounce&tr=udp%3a%2f%2fthetracker.org.%2fannounce&tr=udp%3a%2f%2feddie4.nl%3a6969%2fannounce&tr=udp%3a%2f%2f62.212.85.66%3a2710%2fannounce&tr=udp%3a%2f%2ftracker.ilibr.org%3a6969%2fannounce&tr=udp%3a%2f%2ftracker.zer0day.to%3a1337%2fannounce&tr=http%3a%2f%2fwww.itracker.kr%2fannounce.php";
                    var magnet = new MagnetLink(magnetLink);

                    string[] trackers = magnetLink.Split(new[] { "&tr=" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int j = 2; j < trackers.Length; ++j)
                    {
                        magnet.AnnounceUrls.Add(trackers[j]);
                    }

                    TorrentManager manager = new TorrentManager(new MagnetLink(magnetLink), downloadsPath, torrentDefaults, downloadsPath);

                    ////var hashHex = InfoHash.FromHex("B4D3A91D9CE527AA7C5B6EDACB969E5486B76EF2");
                    //var hashHex = InfoHash.FromMagnetLink(magnetLink);
                    //var tracker = new List<RawTrackerTier>();
                    //var tier = new RawTrackerTier();
                    //for (int j = 1; j < trackers.Length; ++j)
                    //{
                    //    tier.Add(trackers[j]);
                    //}
                    //tracker.Add(tier);

                    //TorrentManager manager = new TorrentManager(hashHex, downloadsPath, torrentDefaults, downloadsPath, tracker);

                    // When any preprocessing has been completed, you create a TorrentManager
                    // which you then register with the engine.
                    //TorrentManager manager = new TorrentManager(torrent, downloadsPath, torrentDefaults);
                    //if (fastResume.ContainsKey(torrent.InfoHash.ToHex ()))
                    //    manager.LoadFastResume(new FastResume ((BEncodedDictionary)fastResume[torrent.infoHash.ToHex ()]));

                    engine.Register(manager);

                    // Store the torrent manager in our list so we can access it later
                    torrents.Add(manager);
                    manager.PeersFound += new EventHandler<PeersAddedEventArgs>(manager_PeersFound);
                }
            }

            // If we loaded no torrents, just exist. The user can put files in the torrents directory and start
            // the client again
            if (torrents.Count == 0)
            {
                Console.WriteLine("No torrents found in the Torrents directory");
                Console.WriteLine("Exiting...");
                engine.Dispose();
                return;
            }

            // For each torrent manager we loaded and stored in our list, hook into the events
            // in the torrent manager and start the engine.
            foreach (TorrentManager manager in torrents)
            {
                // Every time a piece is hashed, this is fired.
                manager.PieceHashed += delegate(object o, PieceHashedEventArgs e) {
                    lock (listener)
                        listener.WriteLine(string.Format("Piece Hashed: {0} - {1}", e.PieceIndex, e.HashPassed ? "Pass" : "Fail"));
                };

                // Every time the state changes (Stopped -> Seeding -> Downloading -> Hashing) this is fired
                manager.TorrentStateChanged += delegate (object o, TorrentStateChangedEventArgs e) {
                    lock (listener)
                        listener.WriteLine("OldState: " + e.OldState.ToString() + " NewState: " + e.NewState.ToString());
                };

                // Every time the tracker's state changes, this is fired
                foreach (TrackerTier tier in manager.TrackerManager)
                {
                    foreach (MonoTorrent.Client.Tracker.Tracker t in tier.Trackers)
                    {
                        t.AnnounceComplete += delegate(object sender, AnnounceResponseEventArgs e) {
                            listener.WriteLine(string.Format("{0}: {1}", e.Successful, e.Tracker.ToString()));
                        };
                    }
                }
                // Start the torrentmanager. The file will then hash (if required) and begin downloading/seeding
                manager.Start();
            }

            // While the torrents are still running, print out some stats to the screen.
            // Details for all the loaded torrent managers are shown.
            int i = 0;
            bool running = true;
            StringBuilder sb = new StringBuilder(1024);
            while (running)
            {
                if ((i++) % 10 == 0)
                {
                    sb.Remove(0, sb.Length);
                    running = torrents.Exists(delegate(TorrentManager m) { return m.State != TorrentState.Stopped; });

                    float totalDownloadSpeed = engine.TotalDownloadSpeed / 1024.0f;
                    if(totalDownloadSpeed > 1024)
                    {
                        AppendFormat(sb, "Total Download Rate: {0:0.00}MB/sec", totalDownloadSpeed / 1024.0f);
                    }
                    else
                    {
                        AppendFormat(sb, "Total Download Rate: {0:0.00}kB/sec", totalDownloadSpeed);
                    }

                    AppendFormat(sb, "Total Upload Rate:   {0:0.00}kB/sec", engine.TotalUploadSpeed / 1024.0);
                    AppendFormat(sb, "Disk Read Rate:      {0:0.00} kB/s", engine.DiskManager.ReadRate / 1024.0);
                    AppendFormat(sb, "Disk Write Rate:     {0:0.00} kB/s", engine.DiskManager.WriteRate / 1024.0);
                    AppendFormat(sb, "Total Read:         {0:0.00} kB", engine.DiskManager.TotalRead / 1024.0);
                    AppendFormat(sb, "Total Written:      {0:0.00} kB", engine.DiskManager.TotalWritten / 1024.0);
                    AppendFormat(sb, "Open Connections:    {0}", engine.ConnectionManager.OpenConnections);
                    
                    foreach (TorrentManager manager in torrents)
                    {
                        AppendSeperator(sb);
                        AppendFormat(sb, "State:           {0}", manager.State);
                        AppendFormat(sb, "Name:            {0}", manager.Torrent == null ? "MetaDataMode" : manager.Torrent.Name);
                        AppendFormat(sb, "Progress:           {0:0.00}", manager.Progress);
                        AppendFormat(sb, "Download Speed:     {0:0.00} kB/s", manager.Monitor.DownloadSpeed / 1024.0);
                        AppendFormat(sb, "Upload Speed:       {0:0.00} kB/s", manager.Monitor.UploadSpeed / 1024.0);
                        AppendFormat(sb, "Total Downloaded:   {0:0.00} MB", manager.Monitor.DataBytesDownloaded / (1024.0 * 1024.0));
                        AppendFormat(sb, "Total Uploaded:     {0:0.00} MB", manager.Monitor.DataBytesUploaded / (1024.0 * 1024.0));
                        MonoTorrent.Client.Tracker.Tracker tracker = manager.TrackerManager.CurrentTracker;
                        //AppendFormat(sb, "Tracker Status:     {0}", tracker == null ? "<no tracker>" : tracker.State.ToString());
                        AppendFormat(sb, "Warning Message:    {0}", tracker == null ? "<no tracker>" : tracker.WarningMessage);
                        AppendFormat(sb, "Failure Message:    {0}", tracker == null ? "<no tracker>" : tracker.FailureMessage);
                        if (manager.PieceManager != null)
                            AppendFormat(sb, "Current Requests:   {0}", manager.PieceManager.CurrentRequestCount());
                        
                        foreach (PeerId p in manager.GetPeers())
                            AppendFormat(sb, "\t{2} - {1:0.00}/{3:0.00}kB/sec - {0}", p.Peer.ConnectionUri,
                                                                                      p.Monitor.DownloadSpeed / 1024.0,
                                                                                      p.AmRequestingPiecesCount,
                                                                                      p.Monitor.UploadSpeed/ 1024.0);
                       
                        AppendFormat(sb, "", null);
                        if (manager.Torrent != null)
                            foreach (TorrentFile file in manager.Torrent.Files)
                                AppendFormat(sb, "{1:0.00}% - {0}", file.Path, file.BitField.PercentComplete);
                    }
                    Console.Clear();
                    Console.WriteLine(sb.ToString());
                    listener.ExportTo(Console.Out);
                }

                System.Threading.Thread.Sleep(500);
            }
        }

        static void manager_PeersFound(object sender, PeersAddedEventArgs e)
        {
            lock (listener)
                listener.WriteLine(string.Format("Found {0} new peers and {1} existing peers", e.NewPeers, e.ExistingPeers ));//throw new Exception("The method or operation is not implemented.");
        }

        private static void AppendSeperator(StringBuilder sb)
        {
            AppendFormat(sb, "", null);
            AppendFormat(sb, "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -", null);
            AppendFormat(sb, "", null);
        }
		private static void AppendFormat(StringBuilder sb, string str, params object[] formatting)
		{
            if (formatting != null)
                sb.AppendFormat(str, formatting);
            else
                sb.Append(str);
			sb.AppendLine();
		}

		private static void shutdown()
		{
            BEncodedDictionary fastResume = new BEncodedDictionary();
            for (int i = 0; i < torrents.Count; i++)
            {
                torrents[i].Stop(); ;
                while (torrents[i].State != TorrentState.Stopped)
                {
                    Console.WriteLine("{0} is {1}", torrents[i].Torrent.Name, torrents[i].State);
                    Thread.Sleep(250);
                }

                fastResume.Add(torrents[i].Torrent.InfoHash.ToHex (), torrents[i].SaveFastResume().Encode());
            }

#if !DISABLE_DHT
            File.WriteAllBytes(dhtNodeFile, engine.DhtEngine.SaveNodes());
#endif
            File.WriteAllBytes(fastResumeFile, fastResume.Encode());
            engine.Dispose();

			foreach (TraceListener lst in Debug.Listeners)
			{
				lst.Flush();
				lst.Close();
			}

            System.Threading.Thread.Sleep(2000);
		}
	}
}
