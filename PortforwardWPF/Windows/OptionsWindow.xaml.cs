using CraftaServ.Classes;
using PortforwardWPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CraftaServ.Windows
{

    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    /// 


    /// <summary>
    /// ==== Things to add ====
    /// Server options
    /// - Read/Write the server.properties textfile & read it and append it to a listview
    /// - Read/Write the OP users
    /// - Read/Write Banned Users
    /// - Read/Write Banned IP's
    /// - Read/Write WhiteList
    /// - Install Plugin Feature
    /// - Play sound on player join / leave
    /// 
    /// </summary>
    public partial class OptionsWindow : Window
    {
        List<ServerProperties> serverProperties = new List<ServerProperties>();

        PerformanceCounter pCounterTotalCpu;
        PerformanceCounter pCounterServerUsageCpu;

        public OptionsWindow()
        {
            InitializeComponent();
            cbConnectionSound.IsChecked = Properties.Settings.Default.PlayLoginSound;
            cbDisconnectSound.IsChecked = Properties.Settings.Default.PlayerDisconnectSound;
            cbErrorMessage.IsChecked = Properties.Settings.Default.PlayErrorSound;

            RetrievePropertyData();
            InitializePerformanceCounters();
        }

        private void InitializePerformanceCounters()
        {
            try
            {
                pCounterTotalCpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                pCounterServerUsageCpu = new PerformanceCounter("Process", "% Processor Time", MainWindow.instance.process.ProcessName);
                
                DispatcherTimer dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                dispatcherTimer.Start();
            }
            catch (InvalidOperationException)
            {
                Debug.Print("Could not find the process!");
            }
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            pgCpuPerformance.Value = (int)(pCounterTotalCpu).NextValue();
            pgRamPerformance.Value = pCounterServerUsageCpu.NextValue() / Environment.ProcessorCount;
        }

        private void cbDisconnectSound_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.PlayerDisconnectSound = cbDisconnectSound.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void cbConnectionSound_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.PlayLoginSound = cbConnectionSound.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void cbErrorMessage_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.PlayErrorSound = cbErrorMessage.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void RetrievePropertyData()
        {
            try
            {
                string line;
                using (StreamReader sr = new StreamReader("server.properties"))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!line.StartsWith("#"))
                        {
                            string[] LineSplitter = line.Split('=');
                            string theproperty = LineSplitter[0].ToString();
                            string value = LineSplitter[1].ToString();
                            serverProperties.Add(new ServerProperties { Property = theproperty, Value = value });
                        }
                    }
                }
                dgItems.ItemsSource = serverProperties;
            }
            catch
            {

            }

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            List<string> newValuesList = new List<string>();
            foreach (var item in serverProperties)
            {
                newValuesList.Add($"{item.Property}={item.Value}");
            }
            File.WriteAllLines("server.properties", newValuesList);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region PluginDownload

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            WebClient wc = new WebClient();
            wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
            wc.DownloadFileAsync(new Uri("https://dev.bukkit.org/projects/vault/files/latest"), "Vault.jar");
        }

        private void btnWorldEditDownload_Click(object sender, RoutedEventArgs e)
        {
            WebClient wc = new WebClient();
            wc.DownloadProgressChanged += WordEditDownloadProgress;
            wc.DownloadFileAsync(new Uri("https://dev.bukkit.org/projects/worldedit/files/latest"), "WorldEdit.jar");
        }

        private void ClearLaggDownload_Click(object sender, RoutedEventArgs e)
        {
            WebClient wc = new WebClient();
            wc.DownloadProgressChanged += clearLagDownloadProgress;
            wc.DownloadFileAsync(new Uri("https://dev.bukkit.org/projects/clearlagg/files/latest"), "ClearLagg.jar");
        }

        private void btnPexDownload_Click(object sender, RoutedEventArgs e)
        {
            WebClient wc = new WebClient();
            wc.DownloadProgressChanged += pexDownloadProgress;
            wc.DownloadFileAsync(new Uri("https://dev.bukkit.org/projects/permissionsex/files/latest"), "Permissionsex.jar");
        }

        private void btnWorldGuardDownload_Click(object sender, RoutedEventArgs e)
        {
            WebClient wc = new WebClient();
            wc.DownloadProgressChanged += worldGuardDownloadPRogress;
            wc.DownloadFileAsync(new Uri("https://dev.bukkit.org/projects/worldguard/files/latest"), "WorldGuard.jar");
        }

        private void btnEssentialsDownload_Click(object sender, RoutedEventArgs e)
        {
            WebClient wc = new WebClient();
            wc.DownloadProgressChanged += EssentialsProgressDownload;
            wc.DownloadFileAsync(new Uri("https://dev.bukkit.org/projects/essentials/files/latest"), "Essentials.jar");
        }

        private void btnMultiVerse_Click(object sender, RoutedEventArgs e)
        {
            WebClient wc = new WebClient();
            wc.DownloadProgressChanged += MultiVerseDownloadProgress;
            wc.DownloadFileAsync(new Uri("https://dev.bukkit.org/projects/multiverse-core/files/latest"), "MultiVerse.jar");
        }

        private void btnFactions_Click(object sender, RoutedEventArgs e)
        {
            WebClient wc = new WebClient();
            wc.DownloadProgressChanged += factionsProgressDownload;
            wc.DownloadFileAsync(new Uri("https://dev.bukkit.org/projects/factions/files/latest"), "Factions.jar");
        }

        private void btnAuthMe_Click(object sender, RoutedEventArgs e)
        {
            WebClient wc = new WebClient();
            wc.DownloadProgressChanged += AuthMeProgressDownload;
            wc.DownloadFileAsync(new Uri("https://dev.bukkit.org/projects/authme-reloaded/files/latest"), "AuthMe.jar");
        }

        private void AuthMeProgressDownload(object sender, DownloadProgressChangedEventArgs e)
        {
            AuthMeProgress.Value = e.ProgressPercentage;
        }

        private void factionsProgressDownload(object sender, DownloadProgressChangedEventArgs e)
        {
            FactionsProgress.Value = e.ProgressPercentage;
        }

        private void MultiVerseDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            MultiVerseProgress.Value = e.ProgressPercentage;
        }

        private void EssentialsProgressDownload(object sender, DownloadProgressChangedEventArgs e)
        {
            EssentialsProgress.Value = e.ProgressPercentage;
        }

        private void worldGuardDownloadPRogress(object sender, DownloadProgressChangedEventArgs e)
        {
            WorldGuardProgress.Value = e.ProgressPercentage;
        }

        private void pexDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            pexProgressBar.Value = e.ProgressPercentage;
        }

        private void clearLagDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            ClearLaggProgress.Value = e.ProgressPercentage;
        }

        private void WordEditDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            WorldEditProgress.Value = e.ProgressPercentage;
        }

        public void wc_DownloadProgressChanged(Object sender, DownloadProgressChangedEventArgs e)
        {
            vaultDownloadStatus.Value = e.ProgressPercentage;
        }
        #endregion
        
    }
}
